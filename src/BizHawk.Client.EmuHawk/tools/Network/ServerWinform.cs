using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BizHawk.Bizware.BizwareGL;
using BizHawk.Client.EmuHawk.tools.Network;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class NetServerWinform : Form
	{

		public static NetServerWinform netServerWinform = null;
		private IVideoProvider _currentVideoProvider;
		private UdpServer udpServer = new UdpServer();
		private ISoundProvider _currentSoundProvider;
		private Func<byte[]> videoPngTask;

		private NetServerWinform(IVideoProvider videoProvider, ISoundProvider soundProvider)
		{
			InitializeComponent();
			_currentVideoProvider = videoProvider;
			_currentSoundProvider = soundProvider;
			byte[] NetworkingTakeScreenshot() => (byte[])new ImageConverter().ConvertTo(netServerWinform.MakeScreenshotImage().ToSysdrawingBitmap(), typeof(byte[]));
			videoPngTask = NetworkingTakeScreenshot;
		}

		public static void start(ISoundProvider soundProvider, IVideoProvider videoProvider)
		{
			if (NetCliendWinform.netCliendWinform != null)
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
			ret.ToSysdrawingBitmap();
			return ret;
		}

		public static void runLoop()
		{
			if (netServerWinform == null)
			{
				return;
			}
			byte[] video = netServerWinform.videoPngTask();
/*			using (MemoryStream memoryStream = new MemoryStream(video))
			{
				Bitmap bitmap = new Bitmap(memoryStream);
				byte[] b = ConvertBitmapToByteArray(bitmap);

			}*/

		}

		// 将 Bitmap 转换为像素数组
		private static byte[] ConvertBitmapToByteArray(Bitmap bitmap)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;
			byte[] byteArray = new byte[width * height * 4];

			int index = 0;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Color color = bitmap.GetPixel(x, y);
					byteArray[index++] = color.A;
					byteArray[index++] = color.R;
					byteArray[index++] = color.G;
					byteArray[index++] = color.B;
				}
			}

			return byteArray;
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
	}
}
