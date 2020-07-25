using System;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager;

namespace ArcdpsLogManager.Gtk
{
	class Program
	{
		[STAThread]
		static void Main()
		{
            new Application().Run(new LoadingForm());
		}
	}
}