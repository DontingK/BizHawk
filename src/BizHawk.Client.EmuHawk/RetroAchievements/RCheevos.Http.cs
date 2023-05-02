using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BizHawk.Client.EmuHawk
{
	public partial class RCheevos
	{
		private static readonly HttpClient _http = new() { DefaultRequestHeaders = { ConnectionClose = true } };

		/// <summary>
		/// A concurrent stack containing all pending HTTP requests
		/// The main thread will push new requests onto this stack
		/// The HTTP thread will pop requests and start them
		/// </summary>
		private readonly ConcurrentStack<RCheevoHttpRequest> _inactiveHttpRequests = new();

		/// <summary>
		/// A list containing all currently active HTTP requests
		/// Completed requests might be restarted if ShouldRetry is true
		/// Otherwise, the completed request is disposed and removed
		/// Only the HTTP thread is allowed to use this list, no other thread may use it
		/// </summary>
		private readonly List<RCheevoHttpRequest> _activeHttpRequests = new();

		private volatile bool _isActive;
		private readonly Thread _httpThread;

		/// <summary>
		/// Base class for all HTTP requests to rcheevos servers
		/// </summary>
		public abstract class RCheevoHttpRequest : IDisposable
		{
			private readonly object _syncObject = new();
			private readonly ManualResetEventSlim _completionEvent = new();
			private bool _isDisposed;

			public virtual bool ShouldRetry { get; protected set; }

			public bool IsCompleted
			{
				get
				{
					lock (_syncObject)
					{
						return _isDisposed || _completionEvent.IsSet;
					}
				}
			}

			public abstract void DoRequest();
			protected abstract void ResponseCallback(byte[] serv_resp);

			public void Wait()
			{
				lock (_syncObject)
				{
					if (_isDisposed) return;
					_completionEvent.Wait();
				}
			}

			public virtual void Dispose()
			{
				if (_isDisposed) return;

				lock (_syncObject)
				{
					_completionEvent.Wait();
					_completionEvent.Dispose();
					_isDisposed = true;
				}
			}

			/// <summary>
			/// Don't use, for FailedRCheevosRequest use only
			/// </summary>
			protected void DisposeWithoutWait()
			{
#pragma warning disable BHI1101 // yeah, complain I guess, but this is a hack so meh
				if (GetType() != typeof(FailedRCheevosRequest)) throw new InvalidOperationException();
#pragma warning restore BHI1101
				_completionEvent.Dispose();
				_isDisposed = true;
			}

			public void Reset()
			{
				ShouldRetry = false;
				_completionEvent.Reset();
			}

			protected void InternalDoRequest(LibRCheevos.rc_error_t apiParamsResult, ref LibRCheevos.rc_api_request_t request)
			{
				if (apiParamsResult != LibRCheevos.rc_error_t.RC_OK)
				{
					// api params were bad, so we can't send a request
					// therefore any retry will fail
					ShouldRetry = false;
					_completionEvent.Set();
					_lib.rc_api_destroy_request(ref request);
					return;
				}

				var apiTask = request.post_data != IntPtr.Zero
					? HttpPost(request.URL, request.PostData)
					: HttpGet(request.URL);
				apiTask.ConfigureAwait(false);

				_lib.rc_api_destroy_request(ref request);
				var result = apiTask.Result; // FIXME: THIS IS BAD (but kind of needed?)

				if (result is null) // likely a timeout
				{
					ShouldRetry = true;
					_completionEvent.Set();
					return;
				}

				ResponseCallback(result);

				ShouldRetry = false; // this is a bit naive, but if the response callback "fails," retrying will just result in the same thing
				_completionEvent.Set();
			}
		}

		/// <summary>
		/// Represents a generic failed rcheevos request
		/// </summary>
		public sealed class FailedRCheevosRequest : RCheevoHttpRequest
		{
			public static readonly FailedRCheevosRequest Singleton = new();

			public override bool ShouldRetry => false;

			protected override void ResponseCallback(byte[] serv_resp)
			{
			}

			public override void DoRequest()
			{
			}

			private FailedRCheevosRequest()
			{
				DisposeWithoutWait();
			}
		}

		private void HttpRequestThreadProc()
		{
			while (_isActive)
			{
				if (_inactiveHttpRequests.TryPop(out var request))
				{
					Task.Run(request.DoRequest);
					_activeHttpRequests.Add(request);
				}

				foreach (var activeRequest in _activeHttpRequests.Where(activeRequest => activeRequest.IsCompleted && activeRequest.ShouldRetry).ToArray())
				{
					activeRequest.Reset();
					Task.Run(activeRequest.DoRequest);
				}

				_activeHttpRequests.RemoveAll(activeRequest =>
				{
					var shouldRemove = activeRequest.IsCompleted && !activeRequest.ShouldRetry;
					if (shouldRemove)
					{
						activeRequest.Dispose();
					}

					return shouldRemove;
				});
			}
		}

		private static async Task<byte[]> HttpGet(string url)
		{
			try
			{
				var response = await _http.GetAsync(url).ConfigureAwait(false);
				return response.IsSuccessStatusCode
					? await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false)
					: null;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}

		private static async Task<byte[]> HttpPost(string url, string post)
		{
			try
			{
				using var content = new StringContent(post, Encoding.UTF8, "application/x-www-form-urlencoded");
				using var response = await _http.PostAsync(url, content).ConfigureAwait(false);
				return response.IsSuccessStatusCode
					? await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false)
					: null;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}
	}
}