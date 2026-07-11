using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;
using GW2Scratch.ArcdpsLogManager.Avalonia.Views;
using GW2Scratch.ArcdpsLogManager.Configuration;

namespace GW2Scratch.ArcdpsLogManager.Avalonia
{
	public class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			// Apply the persisted theme preference before any window is shown.
			ThemeManager.Apply(Settings.Theme);

			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				// The main window's view model kicks off cache loading, directory discovery, and
				// log processing scheduling, which take a while for large log collections. Showing
				// the fully-built MainWindow (a big, richly-bound window) immediately made it look
				// frozen for the whole duration, since it had nothing to render but its static
				// chrome. Instead, show a small, lightweight loading window first — the Avalonia
				// counterpart of the Eto LoadingForm/ManagerForm handoff — and only construct and
				// show MainWindow once there is data ready to display.
				desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

				var loadingWindow = new LoadingWindow();
				desktop.MainWindow = loadingWindow;
				loadingWindow.Show();

				_ = LoadAndShowMainWindowAsync(desktop, loadingWindow);
			}

			base.OnFrameworkInitializationCompleted();
		}

		private static async System.Threading.Tasks.Task LoadAndShowMainWindowAsync(
			IClassicDesktopStyleApplicationLifetime desktop, Window loadingWindow)
		{
			var viewModel = new MainWindowViewModel(new SettingsService());
			await viewModel.InitialLoadTask;

			var mainWindow = new MainWindow { DataContext = viewModel };
			mainWindow.Closed += (_, _) => desktop.Shutdown();

			desktop.MainWindow = mainWindow;
			mainWindow.Show();
			loadingWindow.Close();
		}
	}
}
