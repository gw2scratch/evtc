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
			var application = new Application();
			application.UnhandledException += (sender, args) =>
			{
				MessageBox.Show(
					"The application has encountered an unexpected error. Please report this so we can fix it." +
					$"\n\n{args.ExceptionObject}",
					"Critical Error",
					MessageBoxButtons.OK,
					MessageBoxType.Error);
				application.Quit();
			};
			application.Run(new LoadingForm());
		}
	}
}