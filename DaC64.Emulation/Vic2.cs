using System.Drawing;

namespace DanTup.DaC64.Emulation
{
	class Vic2
	{
		Memory Ram;
		Bitmap Buffer;

		Color[] palette = new[]
		{
			Color.FromArgb(0, 0, 0),
			Color.FromArgb(255, 255, 255),
			Color.FromArgb(136, 0, 0),
			Color.FromArgb(170, 255, 238),
			Color.FromArgb(204, 68, 204),
			Color.FromArgb(0, 204, 85),
			Color.FromArgb(0, 0, 170),
			Color.FromArgb(238, 238, 119),

			Color.FromArgb(221, 136, 85),
			Color.FromArgb(102, 68, 0),
			Color.FromArgb( 255, 119, 119),
			Color.FromArgb(51, 51, 51),
			Color.FromArgb( 119, 119, 119),
			Color.FromArgb( 170, 255, 102),
			Color.FromArgb(0, 136, 255),
			Color.FromArgb(187, 187 ,187)
		};

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

		internal bool Process(int pos)
		{
			// <--      63      -->
			// <-- 11 | 40 | 11 -->

			//  ^   ^
			//  |   |
			//  |   7
			//  |  --
			// 39  25
			//  |  --
			//  |   7
			//  |   |
			//  V   V

			var xChar = pos % 63;
			var yChar = pos / 63;

			if (xChar >= 11 && xChar < 51 && yChar >= 7 && yChar < 32)
			{
				var xPos = xChar - 11;
				var yPos = yChar - 7;

				// TODO: Handle other modes (This is basic characters).

				var memoryOffset = yPos * 40 + xPos;
				var data = Ram.Read((ushort)(0x400 + memoryOffset));
				var backgroundColour = Ram.Read((ushort)(0xD021));
				var colour = Ram.Read((ushort)(0xD800 + memoryOffset));


				var xOffset = xPos * 8;
				var yOffset = yPos * 8;
				for (var x = 0; x < 8; x++)
				{
					for (var y = 0; y < 8; y++)
					{
						Buffer.SetPixel(xOffset + x, yOffset + y, palette[backgroundColour & 0xF]);
					}
				}
			}

			return false;
		}
	}
}
