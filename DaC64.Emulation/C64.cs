using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace DanTup.DaC64.Emulation
{
	public class C64
	{
		internal double CpuCycleDurationMilliseconds { get; set; }
		internal Memory Ram { get; set; }
		internal Cpu6502 Cpu { get; set; }
		internal Bitmap Screen { get; } = new Bitmap(320, 200);
		Vic2 Vic2;

		const ushort ResetVector = 0xFFFC;
		const ushort InitialStackPointer = 0x01FF;

		public C64()
		{
			Ram = new Memory(0x10000);
			Cpu = new Cpu6502(Ram, resetVector: ResetVector, stackPointer: InitialStackPointer);
			Vic2 = new Vic2(Ram, Screen);

			CpuCycleDurationMilliseconds = 1.0f / 1000000; /* 1Mhz... PAL slightly slower, NTSC slightly faster. Let's see if we can ignore that for now... */
		}

		public void Run() => Run(null);

		public void Run(Action<Bitmap> drawFrame)
		{
			var sw = new Stopwatch();
			int? outstandingCpuCycles = 0;
			var badline = false; // When VIC2 returns badline, we have to skip CPU rendering because it's using the cycles
			while (true)
			{
				// Loop for a full screen frame
				for (var i = 0; i < 504 * 312; i++)
				{
					sw.Reset();
					sw.Start();

					// If we're not still "processing" the previous instruction, then do next one.
					if (outstandingCpuCycles == 0)
						outstandingCpuCycles = Cpu.Step();

					// If we get null back, the program has ended/hit unknown opcode.
					if (outstandingCpuCycles == null)
						return;

					// http://dustlayer.com/vic-ii/2013/4/25/vic-ii-for-beginners-beyond-the-screen-rasters-cycle
					badline = Vic2.Process(i);

					// Sleep until it's time for the next cycle.
					var timeToSleep = (CpuCycleDurationMilliseconds - sw.ElapsedMilliseconds);
					if (timeToSleep > 0)
						Thread.Sleep((int)timeToSleep);
				}

				// Now render
				drawFrame?.Invoke(Screen);
			}
		}

		/// <summary>
		/// Loads a program for the CPU to execute.
		/// </summary>
		public void LoadKernelRom(params byte[] program)
		{
			Ram.Write(0xA000, new ArraySegment<byte>(program, 0, 0x2000).ToArray());
			Ram.Write(0xE000, new ArraySegment<byte>(program, 0x2000, 0x2000).ToArray());
			Cpu.Reset();
		}
	}
}
