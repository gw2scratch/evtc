using System;
using System.ComponentModel;
using Eto;
using Eto.Forms;

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
			Style.Add<Eto.Wpf.Forms.Controls.GridColumnHandler>("sort-hidden", handler =>
			{
				handler.Control.SortDirection = null;
			});
			Style.Add<Eto.Wpf.Forms.Controls.GridColumnHandler>("sort-ascending", handler =>
			{
				handler.Control.SortDirection = ListSortDirection.Ascending;
			});
			Style.Add<Eto.Wpf.Forms.Controls.GridColumnHandler>("sort-descending", handler =>
			{
				handler.Control.SortDirection = ListSortDirection.Descending;
			});

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

