using System;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager;

namespace GW2Scratch.ArcdpsLogManager.Gtk
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