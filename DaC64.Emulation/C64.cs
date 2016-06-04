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
		internal Bitmap Screen { get; } = new Bitmap(256, 240);

		const ushort ResetVector = 0xFFFC;
		const ushort InitialStackPointer = 0x01FF;

		public C64()
		{
			Ram = new Memory(0xFFFF);
			Cpu = new Cpu6502(Ram, resetVector: ResetVector, stackPointer: InitialStackPointer);

			var cpuSpeed = 21.477272 / 12;
			CpuCycleDurationMilliseconds = 1.0f / cpuSpeed;
		}

		public void Run() => Run(null);

		public void Run(Action<Bitmap> drawFrame)
		{
			var sw = new Stopwatch();
			while (true)
			{
				sw.Start();
				var cpuCyclesSpent = Cpu.Step();

				// If we get null back, the program has ended/hit unknown opcode.
				if (cpuCyclesSpent == null)
					return;

				// TODO: ...?
				//if (timeToRender)
				//	drawFrame?.Invoke(Screen);

				// Sleep until it's time for the next cycle.
				var timeToSleep = cpuCyclesSpent.Value * (CpuCycleDurationMilliseconds - sw.ElapsedMilliseconds);
				if (timeToSleep > 0)
					Thread.Sleep((int)timeToSleep);
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
