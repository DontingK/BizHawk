﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.EmuHawk.tools.Network
{
	//public class NetVideoProvider : IVideoProvider
	public class NetVideoProvider 
	{




	}


	public class NetSoundProvider : ISoundProvider
	{

		public bool CanProvideAsync => true;

		public SyncSoundMode SyncMode => SyncSoundMode.Async;

		private short[] samples = Array.Empty<short>();

		public void SetSamples(short[] samples)
		{
			this.samples = samples;
		}
		public void SetSyncMode(SyncSoundMode mode)
		{
			//SyncMode = mode;
		}

		public void GetSamplesSync(out short[] samples, out int nsamp)
		{
			samples = this.samples;
			nsamp = samples.Length;
		}

		public void GetSamplesAsync(short[] samples)
		{
			samples = this.samples;
		}
		public void DiscardSamples()
		{

		}
	}
}
