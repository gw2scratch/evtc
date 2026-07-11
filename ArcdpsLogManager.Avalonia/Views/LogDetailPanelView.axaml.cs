using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class LogDetailPanelView : UserControl
	{
		public LogDetailPanelView()
		{
			InitializeComponent();

			var copyButton = this.FindControl<Button>("CopyUrlButton");
			var openButton = this.FindControl<Button>("OpenUrlButton");

			if (copyButton != null)
			{
				copyButton.Click += OnCopyUrlClick;
			}

			if (openButton != null)
			{
				openButton.Click += OnOpenUrlClick;
			}
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private async void OnCopyUrlClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (DataContext is not LogDetailPanelViewModel vm || vm.UploadUrl is not { } url)
			{
				return;
			}

			var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
			if (clipboard != null)
			{
				await clipboard.SetTextAsync(url);
			}
		}

		private async void OnOpenUrlClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (DataContext is not LogDetailPanelViewModel vm || vm.UploadUrl is not { } url)
			{
				return;
			}

			try
			{
				Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
			}
			catch (System.Exception ex)
			{
				if (TopLevel.GetTopLevel(this) is Window owner)
				{
					await Dialogs.ShowInfoAsync(owner, "Error", $"Failed to open the URL: {ex.Message}. Please try again.");
				}
			}
		}

		// Opens the containing folder with the log file selected, exactly like the Eto
		// LogDetailPanel's fileNameButton (Windows Explorer /select, with a dbus-based fallback
		// for Linux file managers, falling back further to just opening the directory).
		private void OnFileNameClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (DataContext is not LogDetailPanelViewModel vm || string.IsNullOrEmpty(vm.FilePath))
			{
				return;
			}

			string path = vm.FilePath;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Process.Start("explorer.exe", $"/select,\"{path}\"");
				return;
			}

			var dbusArgs = "--session --dest=org.freedesktop.FileManager1 " +
			               "--type=method_call /org/freedesktop/FileManager1 org.freedesktop.FileManager1.ShowItems " +
			               $"array:string:\"file://{path}\" string:\"\"";
			var dbusProcessInfo = new ProcessStartInfo
			{
				FileName = "dbus-send",
				Arguments = dbusArgs,
				UseShellExecute = true
			};

			bool success;
			try
			{
				var dbusProcess = Process.Start(dbusProcessInfo);
				dbusProcess?.WaitForExit();
				success = (dbusProcess?.ExitCode ?? 1) == 0;
			}
			catch
			{
				success = false;
			}

			if (!success)
			{
				try
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = Path.GetDirectoryName(path),
						UseShellExecute = true
					});
				}
				catch
				{
					// Best-effort; nothing more we can do if this fails too.
				}
			}
		}
	}
}
