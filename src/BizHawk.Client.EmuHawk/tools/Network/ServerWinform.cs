using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using BizHawk.Bizware.BizwareGL;
using BizHawk.Client.EmuHawk.tools.Network;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class NetServerWinform : Form, UdpHandel
	{

		public static NetServerWinform netServerWinform = null;
		private IVideoProvider _currentVideoProvider;
		private UdpServer udpServer ;
		private ISoundProvider _currentSoundProvider;
		private Func<byte[]> videoPngTask;


		private UdpUser udpUser;

		private NetServerWinform(IVideoProvider videoProvider, ISoundProvider soundProvider)
		{
			udpServer = new UdpServer(this);
			InitializeComponent();
			_currentVideoProvider = videoProvider;
			_currentSoundProvider = soundProvider;
			byte[] NetworkingTakeScreenshot() => (byte[])new ImageConverter().ConvertTo(netServerWinform.MakeScreenshotImage().ToSysdrawingBitmap(), typeof(byte[]));
			videoPngTask = NetworkingTakeScreenshot;
		}

		public static void start(ISoundProvider soundProvider, IVideoProvider videoProvider)
		{
			if (NetClienWinform.netCliendWinform != null)
			{

				string message = "Net Server/Clinet  Cannot be opened at the same time";
				string caption = "Message";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBoxIcon icon = MessageBoxIcon.Information;
				MessageBox.Show(message, caption, buttons, icon);
				return;
			}
			if (netServerWinform != null)
			{
				netServerWinform.TopMost = true;
				netServerWinform.TopMost = false;
			}
			else
			{
				netServerWinform = new NetServerWinform(videoProvider, soundProvider);
				netServerWinform.Show();
			}
		}

		private BitmapBuffer MakeScreenshotImage()
		{
			var ret = new BitmapBuffer(_currentVideoProvider.BufferWidth, _currentVideoProvider.BufferHeight, _currentVideoProvider.GetVideoBuffer().ToArray());
			ret.DiscardAlpha();
			return ret;
		}

		public static void runLoop()
		{
		//	if (netServerWinform == null&& !netServerWinform.udpServer.isUserLink())
			if (netServerWinform == null)
			{
				return;
			}
			byte[] video = netServerWinform.videoPngTask();
			netServerWinform._currentSoundProvider.GetSamplesSync(out var samples, out var count);
			//Console.WriteLine("声音：" + count);
		}


		private void startButton_Click(object sender, EventArgs e)
		{
			if (udpServer.isStart())
			{
				udpServer.stop();
				netInfo.Text = "service is not start";
				startButton.Text = "start";
				return;
			}
			try
			{
				int port = int.Parse(portInput.Text);
				udpServer.start(port);
				netInfo.Text = "service is stating";
				portInput.Enabled = false;
				startButton.Text = "stop";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine(ex);
			}
		}

		private void NetServerWinform_FormClosed(object sender, FormClosedEventArgs e)
		{
			udpServer.stop();
			netServerWinform = null;
		}

		public void Handel(byte cmd, byte[] data, IPEndPoint clientEP)
		{
			switch (cmd) {
				case Cmd.c_login:
					login(data, clientEP);
					break;
				default:
					Console.WriteLine("server 收到未知消息：" + cmd);
					break;

			}
		}



		private void login(byte[] data, IPEndPoint clientEP) {
			udpUser = new UdpUser(clientEP);
			Console.WriteLine("clientEP:" + clientEP);
			Console.WriteLine("clientEP:" + clientEP.Address);
			Console.WriteLine("clientEP:" + clientEP.Port);
			udpServer.sendData(Cmd.s_login_ok, new byte[]{1 }, udpUser);
		}

		public void SendError(Exception e, IPEndPoint clientEP)
		{
			Console.WriteLine(e);
		}
	}
}
