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
			for (var x = 0; x < 256; x++)
			{
				for (var y = 0; y < 240; y++)
				{
					Buffer.SetPixel(x, y, Color.FromArgb(x, y, 128));
				}
			}
		}

		internal bool Process(int i)
		{
			return false;
		}
	}
}
