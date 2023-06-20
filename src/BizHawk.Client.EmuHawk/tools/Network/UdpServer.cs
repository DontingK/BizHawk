using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BizHawk.Client.EmuHawk.tools.Network
{
	internal class UdpServer
	{
		private UdpClient udpServer;
		private Thread thread;
		private bool isRun;
		private UdpHandel udpHandel;
		public UdpServer(UdpHandel udpHandel)
		{
			this.udpHandel = udpHandel;
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
					udpHandel.Handel(cmd, receivedData, clientEP);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			Console.WriteLine("监听结束");

		}


		public void sendData(byte cmd, byte[] data, IPEndPoint clientEP)
		{
			byte[] cmds = new byte[] { cmd};
			byte[] rData = cmds.Concat(data).ToArray();
			udpServer.Send(rData, rData.Length, clientEP);
		}
		public void sendData(byte[] data, IPEndPoint clientEP)
		{
			udpServer.Send(rData, rData.Length, clientEP);
		}
		public void sendData(byte cmd, byte[] data, UdpUser udpUser)
		{
			byte[] cmds = new byte[] { cmd};
			byte[] rData = cmds.Concat(data).ToArray();
			try
			{
				udpServer.Send(rData, rData.Length, udpUser.clientEP);
			}
			catch (Exception e)
			{
				//user 退出
				udpHandel.SendError(e,udpUser.clientEP);
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


	public interface UdpHandel {

		void Handel(byte cmd, byte[] data, IPEndPoint clientEP);
		void SendError(Exception e,IPEndPoint clientEP);
	}

	internal class UdpUser
	{

		public IPEndPoint clientEP { get; }

		public UdpUser(IPEndPoint clientEP)
		{
			this.clientEP = clientEP;
		}
	}
}
