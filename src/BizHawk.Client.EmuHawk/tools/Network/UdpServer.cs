using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BizHawk.Emulation.Cores.Waterbox.NymaCore.NymaSettingsInfo;

namespace BizHawk.Client.EmuHawk.tools.Network
{
	internal class UdpServer
	{
		private UdpClient udpServer;
		private Thread thread;
		private bool isRun;
		private ClintUser clintUser;
		private byte userUid = 0;
		public UdpServer()
		{

		}

		public void start(int port)
		{

			// 创建一个UDP监听器
			udpServer = new UdpClient(port);
			Console.WriteLine("UDP服务器已启动，正在监听端口 {0}...", port);
			thread = new Thread(run);
			isRun = true;
			thread.Start();

		}

		public void stop()
		{
			if (isStart())
			{
				Console.WriteLine("UDP关闭");
				isRun = false;
				clintUser = null;
				udpServer.Close();
				thread = null;
				Console.WriteLine("UDP以关闭");
			}

		}

		private void run()
		{
			Console.WriteLine("开始监听......");
			while (isRun)
			{
				try
				{
					// 接收客户端发送的消息
					IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
					byte[] receivedData = udpServer.Receive(ref clientEP);
					byte cmd = receivedData[0];

					switch (cmd)
					{
						case Cmd.c_login:
							login(receivedData, clientEP);
							break;

					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			Console.WriteLine("监听结束");

		}

		private void login(byte[] receivedData, IPEndPoint clientEP)
		{
			string name = Encoding.UTF8.GetString(receivedData, 0, receivedData.Length - 1);
			if (clintUser!=null)
			{
				string msg = "2p is linked";
				byte[] data = Encoding.UTF8.GetBytes(msg);
				sendData(Cmd.s_login_fail, data, clientEP);
			}
			else
			{
				byte userId = this.userUid++;
				sendData(Cmd.s_login_ok, new byte[] { userId }, clientEP);
			}
		}


		public void sendData(byte cmd, byte[] data, IPEndPoint clientEP)
		{
			byte[] cmds = new byte[1];
			byte[] rData = cmds.Concat(data).ToArray();
			udpServer.Send(rData, rData.Length, clientEP);
		}
		public void sendData(byte cmd, byte[] data, ClintUser clintUser)
		{
			byte[] cmds = new byte[1];
			byte[] rData = cmds.Concat(data).ToArray();
			try
			{
				udpServer.Send(rData, rData.Length, clintUser.clientEP);
			}
			catch (Exception e)
			{
				//user 退出
				clintUser = null;
			}
		}

		internal bool isStart()
		{
			if (thread != null)
			{

				return true;
			}
			return false;
		}
	}

	internal class ClintUser
	{

		private string name;
		public IPEndPoint clientEP { get; }
		private byte id;

		public ClintUser(string name, IPEndPoint clientEP, byte id)
		{
			this.name = name;
			this.clientEP = clientEP;
			this.id = id;
		}
	}
}
