using System.Drawing;

namespace DanTup.DaC64.Emulation
{
	class Vic2
	{
		Memory Ram;
		Bitmap Buffer;

		public Vic2(Memory ram, Bitmap buffer)
		{
			Ram = ram;
			Buffer = buffer;

			// Initialise with a nice rainbow screen.
			for (var x = 0; x < 320; x++)
			{
				for (var y = 0; y < 200; y++)
				{
					Buffer.SetPixel(x, y, Color.FromArgb(x % 255, y % 255, 128));
				}
			}
		}

		internal bool Process(int cpuCycle)
		{
			// <--      504      -->
			// <-- 92 | 320 | 92 -->

			//  ^    ^
			//  |    |
			//  |    56
			//  |   ---
			// 312  200
			//  |   ---
			//  |    56
			//  |    |
			//  V    V

			var x = cpuCycle % 504;
			var y = (cpuCycle - x) / 504;

			if (x >= 92 && x < 412 && y >= 56 && y < 256)
			{
				var realX = x - 92;
				var realY = y - 56;

				Buffer.SetPixel(realX, realY, Color.FromArgb(realX % 255, realY % 255, 128));
			}

			return false;
		}
	}
}
