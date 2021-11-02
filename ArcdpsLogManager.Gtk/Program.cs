using Eto.Forms;
using Eto.GtkSharp.Forms.Controls;
using Gtk;
using System;
using Application = Eto.Forms.Application;
using Style = Eto.Style;

namespace GW2Scratch.ArcdpsLogManager.Gtk
{
	class Program
	{
		[STAThread]
		static void Main()
		{
			// TODO: For some reason, this gets reset unlike on the WPF platform.
			// As far as I can tell, SortIndicator gets set properly, and then it miraculously
			// resets back to false - and the sort-hidden callback is not called when that happens,
			// which means it is likely some external code. However, there doesn't seem to be anything
			// in Eto and GtkSharp that would manipulate SortIndicator.
			// Setting the value manually using a debugger works.
			Style.Add<GridColumnHandler>("sort-hidden", handler =>
			{
				handler.Control.SortIndicator = false;
			});
			Style.Add<GridColumnHandler>("sort-ascending", handler =>
			{
				handler.Control.SortOrder = SortType.Ascending;
				handler.Control.SortIndicator = true;
			});
			Style.Add<GridColumnHandler>("sort-descending", handler =>
			{
				handler.Control.SortOrder = SortType.Descending;
				handler.Control.SortIndicator = true;
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