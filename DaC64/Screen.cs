using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using DanTup.DaC64.Emulation;

namespace DanTup.DaC64
{
	public partial class Screen : Form
	{
		const string RomFile = @"..\..\..\ROMs\64c.251913-01.bin";
		C64 c64 = new C64();

		public Screen()
		{
			InitializeComponent();

			var program = File.ReadAllBytes(RomFile);
			c64.LoadProgram(program);

			Task.Run(() => c64.Run(UpdateScreen));
		}

		void UpdateScreen(Bitmap img)
		{
			try
			{
				Invoke((Action)(() => pbScreen.Image = (Bitmap)img.Clone()));
			}
			catch
			{
				// TODO: This happens when form gets closed/disposed (expected).
				// However may also masked real errors here :(
			}
		}
	}
}
