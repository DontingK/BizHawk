using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using BizHawk.Bizware.BizwareGL;
using BizHawk.Client.EmuHawk.tools.Network;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class NetClienWinform : Form, UdpHandel
	{
		public static NetClienWinform netCliendWinform;


		private MainForm mainForm;

		private UdpServer udpServer;
		private UdpUser udpUser;

		private NetSoundProvider netSoundProvider=new NetSoundProvider();
		private ISoundProvider mainFormOriginSoundProvider;

		private NetClienWinform(MainForm mainForm)
		{
			this.mainForm = mainForm;
			this.udpServer = new UdpServer(this);
			InitializeComponent();
		}


		public static void start(MainForm mainForm)
		{
			if (NetServerWinform.netServerWinform != null)
			{
				string message = "Net Server/Clinet  Cannot be opened at the same time";
				string caption = "Message";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBoxIcon icon = MessageBoxIcon.Information;
				MessageBox.Show(message, caption, buttons, icon);
				return;
			}
			else
			{
				netCliendWinform = new NetClienWinform(mainForm);
				netCliendWinform.Show();
			}
		}

		public void Handel(byte cmd, byte[] data, IPEndPoint clientEP)
		{
			switch (cmd)
			{
				case Cmd.s_login_ok:
					loginOk(data, clientEP);
					break;
				case Cmd.s_video_sound:
					videoSound(data, clientEP);
					break;
				default:
					Console.WriteLine("未知cmd" + cmd);
					break;
			}

		}

		private void loginOk(byte[] data, IPEndPoint clientEP)
		{
			Console.WriteLine("loginOk");
			udpUser = new UdpUser(clientEP);
			mainFormOriginSoundProvider=mainForm.GetCurrentSoundProvider();
			//
			mainForm.SetCurrentSoundProvider(netSoundProvider);
		}
		private void videoSound(byte[] data, IPEndPoint clientEP)
		{
			int videoLenght = BitConverter.ToInt32(data, 1);
			int soundLenght = data.Length-1-4-videoLenght;
			byte[] video=new byte[videoLenght];
			short[] sound=new short[soundLenght/2];
			//复制sound
			Buffer.BlockCopy(data, 1+4+videoLenght, sound, 0, soundLenght);
			//复制视频
			Buffer.BlockCopy(data, 1+4, video, 0, videoLenght);
			Console.WriteLine("客户端获取到音频：" + sound.Length);
			netSoundProvider.SetSamples(sound);
			
		}
		public void SendError(Exception e, IPEndPoint clientEP)
		{
		}

		private void linkButton_Click(object sender, EventArgs e)
		{
			if (udpServer.isStart())
			{
				udpServer.stop();
				linkButton.Text = "start";
				return;
			}
			else
			{
				IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10010);
				udpServer.start(10012);
				udpServer.sendData(Cmd.c_login, new byte[] { 1 }, clientEP);
				linkButton.Text = "stop link";
			}
		}
	}
}
