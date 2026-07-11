using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class CompressWindow : Window
	{
		public CompressWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void OnCompressClick(object? sender, RoutedEventArgs e)
		{
			(DataContext as CompressWindowViewModel)?.Compress();
		}

		private void OnCancelClick(object? sender, RoutedEventArgs e)
		{
			(DataContext as CompressWindowViewModel)?.CancelCompression();
		}

		private void OnCloseClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
