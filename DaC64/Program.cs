﻿using System;
using System.Windows.Forms;

namespace DanTup.DaC64
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Screen());
		}
	}
}
