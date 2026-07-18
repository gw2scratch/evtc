using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;
using GW2Scratch.ArcdpsLogManager.Updates;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class AboutWindow : Window
	{
		private const string UpdateFeedUrl = "http://gw2scratch.com/releases/manager.json";

		private AboutWindowViewModel ViewModel => (AboutWindowViewModel)DataContext!;

		public AboutWindow()
		{
			DataContext = new AboutWindowViewModel();
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private async void OnCheckUpdatesClick(object? sender, RoutedEventArgs e)
		{
			ViewModel.IsCheckingForUpdates = true;
			ViewModel.CheckUpdatesButtonText = "Checking…";
			try
			{
				var checker = new ProgramUpdateChecker(UpdateFeedUrl);
				var release = await checker.CheckUpdates();
				if (release != null)
				{
					var window = new ProgramUpdateWindow
					{
						DataContext = new ProgramUpdateWindowViewModel(release),
					};
					await window.ShowDialog(this);
				}
				else
				{
					await Dialogs.ShowInfoAsync(this, "Check for updates", "You're using the latest version.");
				}
			}
			finally
			{
				ViewModel.IsCheckingForUpdates = false;
				ViewModel.CheckUpdatesButtonText = "Check for updates";
			}
		}

		private static void OnOpenWebsiteClick(object? sender, RoutedEventArgs e) =>
			OpenUrl("https://gw2scratch.com/");

		private static void OnOpenDiscordClick(object? sender, RoutedEventArgs e) =>
			OpenUrl("https://discord.gg/rNXRS6ZkYe");

		private static void OnOpenGitHubClick(object? sender, RoutedEventArgs e) =>
			OpenUrl("https://github.com/gw2scratch/evtc");

		private static void OnOpenDonateClick(object? sender, RoutedEventArgs e) =>
			OpenUrl("https://ko-fi.com/sejsel");

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

		private void OnCloseClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
