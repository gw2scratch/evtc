using System;
using Eto.Forms;

namespace GW2Scratch.EVTCInspector.Gtk
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