using System;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager;

namespace GW2Scratch.ArcdpsLogManager.Wpf
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			// optional - enables GDI text display mode
			/*
			Style.Add<Eto.Wpf.Forms.FormHandler>(null, handler => TextOptions.SetTextFormattingMode(handler.Control, TextFormattingMode.Display));
			Style.Add<Eto.Wpf.Forms.DialogHandler>(null, handler => TextOptions.SetTextFormattingMode(handler.Control, TextFormattingMode.Display));
			*/

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

