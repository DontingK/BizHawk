using System;
using System.Linq;
using System.Windows.Forms;
using BizHawk.Bizware.BizwareGL;
using BizHawk.Client.EmuHawk.tools.Network;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class NetCliendWinform : Form
	{
		public static NetCliendWinform netCliendWinform;

		private NetCliendWinform()
		{
		}


		public static void start() {
			if (NetServerWinform.netServerWinform != null)
			{
				string message = "Net Server/Clinet  Cannot be opened at the same time";
				string caption = "Message";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBoxIcon icon = MessageBoxIcon.Information;
				MessageBox.Show(message, caption, buttons, icon);
				return;
			}
		}
	
	}
}
