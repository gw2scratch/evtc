using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private async void OnAddDirectoryClick(object? sender, RoutedEventArgs e)
		{
			var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
			{
				Title = "Select a log directory",
				AllowMultiple = false,
			});

			var folder = folders.FirstOrDefault();
			if (folder != null && DataContext is SettingsWindowViewModel vm)
			{
				string? path = folder.TryGetLocalPath();
				if (path != null)
				{
					vm.AddDirectory(path);
				}
			}
		}

		private void OnCloseClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
