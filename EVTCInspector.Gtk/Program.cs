using System;
using Eto.Forms;
using GW2Scratch.EVTCInspector;

namespace EVTCInspector.Gtk
{
	class Program
	{
		[STAThread]
		static void Main()
		{
			new Application().Run(new BrowserForm());
		}
	}
}