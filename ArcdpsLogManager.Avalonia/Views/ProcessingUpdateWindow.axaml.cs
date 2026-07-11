using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class ProcessingUpdateWindow : Window
	{
		public ProcessingUpdateWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void OnYesClick(object? sender, RoutedEventArgs e)
		{
			(DataContext as ProcessingUpdateWindowViewModel)?.ScheduleReprocessing();
			Close();
		}

		private void OnLaterClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
