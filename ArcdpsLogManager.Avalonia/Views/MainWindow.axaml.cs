using System;
using System.Diagnostics;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;
using GW2Scratch.ArcdpsLogManager.Configuration;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			RestoreWindowState();
			Opened += OnWindowOpened;
		}

		// Checks for a new program version once the window is shown (Avalonia counterpart of the
		// Eto ManagerForm's Shown += CheckUpdates), gated by Settings.CheckForUpdates.
		private async void OnWindowOpened(object? sender, EventArgs e)
		{
			if (DataContext is MainWindowViewModel shell)
			{
				var release = await shell.CheckForProgramUpdateAsync();
				if (release != null)
				{
					var window = new ProgramUpdateWindow
					{
						DataContext = new ProgramUpdateWindowViewModel(release),
					};
					await window.ShowDialog(this);
				}
			}
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void RestoreWindowState()
		{
			var state = Settings.WindowState;
			if (state.Width is > 100)
			{
				Width = state.Width.Value;
			}

			if (state.Height is > 100)
			{
				Height = state.Height.Value;
			}

			if (state.X.HasValue && state.Y.HasValue)
			{
				WindowStartupLocation = WindowStartupLocation.Manual;
				Position = new PixelPoint(state.X.Value, state.Y.Value);
			}

			if (state.Maximized)
			{
				WindowState = global::Avalonia.Controls.WindowState.Maximized;
			}
		}

		private void SaveWindowState()
		{
			bool maximized = WindowState == global::Avalonia.Controls.WindowState.Maximized;
			var info = new WindowStateInfo
			{
				// When maximized, keep the current (normal) size fields as-is by only recording them
				// when not maximized, so the restore size is sensible.
				Width = maximized ? Settings.WindowState.Width : Width,
				Height = maximized ? Settings.WindowState.Height : Height,
				X = maximized ? Settings.WindowState.X : Position.X,
				Y = maximized ? Settings.WindowState.Y : Position.Y,
				Maximized = maximized,
			};
			Settings.WindowState = info;
		}

		// Custom title-bar behaviour, needed because ExtendClientAreaChromeHints="NoChrome" hides the
		// OS-drawn title bar (drag handle) and caption buttons entirely.
		private void OnHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			{
				BeginMoveDrag(e);
			}
		}

		private void OnHeaderDoubleTapped(object? sender, RoutedEventArgs e)
		{
			WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
		}

		private void OnMinimizeClick(object? sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void OnMaximizeRestoreClick(object? sender, RoutedEventArgs e)
		{
			WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
		}

		private void OnCloseClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnOpenSettingsClick(object? sender, RoutedEventArgs e)
		{
			var window = new SettingsWindow
			{
				DataContext = new SettingsWindowViewModel(new SettingsService()),
			};
			window.ShowDialog(this);
		}

		private async void OnOpenAboutClick(object? sender, RoutedEventArgs e)
		{
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			string message = "arcdps Log Manager\n" +
			                 $"Version {version}\n\n" +
			                 "A manager for Guild Wars 2 arcdps encounter logs.\n" +
			                 "Developed by Sejsel.\n\n" +
			                 "https://github.com/gw2scratch/evtc";
			await Dialogs.ShowInfoAsync(this, "About", message);
		}

		private static void OnOpenDonateClick(object? sender, RoutedEventArgs e) =>
			OpenUrl("https://ko-fi.com/sejsel");

		private static void OnOpenChangelogClick(object? sender, RoutedEventArgs e) =>
			OpenUrl("https://github.com/gw2scratch/evtc/releases");

		private static void OnOpenWebsiteClick(object? sender, RoutedEventArgs e) =>
			OpenUrl("https://gw2scratch.com/");

		private static void OpenUrl(string url)
		{
			try
			{
				Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
			}
			catch
			{
				// Ignore failures to launch the browser.
			}
		}

		private void OnOpenCacheClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is MainWindowViewModel shell)
			{
				var window = new CacheWindow
				{
					DataContext = new CacheWindowViewModel(
						shell.CacheService.Cache,
						() => shell.LoadedLogs,
						shell.ReloadAsync),
				};
				window.ShowDialog(this);
			}
		}

		private void OnOpenApiClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is MainWindowViewModel shell)
			{
				var window = new ApiWindow
				{
					DataContext = new ApiWindowViewModel(shell.ApiData, shell.SettingsService),
				};
				window.ShowDialog(this);
			}
		}

		private void OnOpenProcessingUpdateClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is not MainWindowViewModel shell)
			{
				return;
			}

			if (shell.Processing == null)
			{
				_ = Dialogs.ShowInfoAsync(this, "Update logs with outdated data",
					"Reprocessing requires an active log directory scan. The cache is currently " +
					"read-only (in use by another instance) or no log directories are configured.");
				return;
			}

			var updates = shell.GetLogUpdates();
			if (updates.Count == 0)
			{
				_ = Dialogs.ShowInfoAsync(this, "Update logs with outdated data",
					"No logs currently require reprocessing.");
				return;
			}

			var window = new ProcessingUpdateWindow
			{
				DataContext = new ProcessingUpdateWindowViewModel(updates, shell.Processing),
			};
			window.ShowDialog(this);
		}

		private void OnOpenAdvancedFiltersClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is MainWindowViewModel shell)
			{
				var window = new AdvancedFiltersWindow
				{
					DataContext = new AdvancedFiltersWindowViewModel(shell.Filters, shell.Images, shell.LoadedLogs),
				};
				window.ShowDialog(this);
			}
		}

		private void OnOpenCompressClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is not MainWindowViewModel shell)
			{
				return;
			}

			var processor = shell.CompressionProcessor;
			if (processor == null)
			{
				_ = Dialogs.ShowInfoAsync(this, "Compress logs",
					"Compression is unavailable because the log cache is read-only (in use by " +
					"another instance) or has not finished loading yet.");
				return;
			}

			var window = new CompressWindow();
			window.DataContext = new CompressWindowViewModel(window, shell.LoadedLogs, processor);
			window.Closed += (_, _) => (window.DataContext as IDisposable)?.Dispose();
			window.ShowDialog(this);
		}

		protected override void OnClosed(EventArgs e)
		{
			// Persist window placement, pending cache changes, and release the log cache mutex.
			SaveWindowState();
			(DataContext as IDisposable)?.Dispose();
			base.OnClosed(e);
		}
	}
}
