using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class ApiWindow : Window
	{
		public ApiWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private async void OnDeleteClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is ApiWindowViewModel vm)
			{
				await vm.DeleteAsync(this);
			}
		}

		private void OnCloseClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
