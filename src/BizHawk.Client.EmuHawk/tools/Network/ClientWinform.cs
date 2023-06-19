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

		private NetClienWinform(MainForm mainForm)
		{
			this.mainForm = mainForm;
			this.udpServer = new UdpServer(this);
			InitializeComponent();
		}


		public static void start(MainForm mainForm) {
			if (NetServerWinform.netServerWinform != null)
			{
				string message = "Net Server/Clinet  Cannot be opened at the same time";
				string caption = "Message";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBoxIcon icon = MessageBoxIcon.Information;
				MessageBox.Show(message, caption, buttons, icon);
				return;
			}
			else {
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
			}

		}

		private void loginOk(byte[] data, IPEndPoint clientEP)
		{
			Console.WriteLine("loginOk");
			udpUser = new UdpUser(clientEP);
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
			else {
				IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10010);
				udpServer.start(10012);
				udpServer.sendData(Cmd.c_login, new byte[] { 1 }, clientEP);
				linkButton.Text = "stop link"; 
			}
		}
	}
}
