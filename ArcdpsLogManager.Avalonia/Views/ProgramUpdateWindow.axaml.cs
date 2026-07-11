using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class ProgramUpdateWindow : Window
	{
		public ProgramUpdateWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void OnChangelogClick(object? sender, RoutedEventArgs e)
		{
			(DataContext as ProgramUpdateWindowViewModel)?.OpenChangelog();
		}

		private void OnDownloadClick(object? sender, RoutedEventArgs e)
		{
			(DataContext as ProgramUpdateWindowViewModel)?.OpenDownloadPage();
			Close();
		}

		private void OnIgnoreClick(object? sender, RoutedEventArgs e)
		{
			(DataContext as ProgramUpdateWindowViewModel)?.IgnoreThisVersion();
			Close();
		}

		private void OnLaterClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
