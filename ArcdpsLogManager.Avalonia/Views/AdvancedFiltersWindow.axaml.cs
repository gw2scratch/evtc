using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class AdvancedFiltersWindow : Window
	{
		public AdvancedFiltersWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void OnCloseClick(object? sender, RoutedEventArgs e)
		{
			Close();
		}

		// Opens the player-picker dialog (Avalonia counterpart of the Eto PlayerFilterPanel's
		// "Add player" button constructing a PlayerSelectDialog) instead of typing a raw account
		// name; the window itself is constructed here (not in the view model) matching this
		// project's existing convention (e.g. LogsSectionView's UnreliableLogsWindow, GameDataSectionView's
		// LogListWindow).
		private async void OnAddPlayerClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is not AdvancedFiltersWindowViewModel vm)
			{
				return;
			}

			var dialog = new PlayerSelectWindow
			{
				DataContext = new PlayerSelectWindowViewModel(vm.LoadedLogs, vm.Images),
			};
			var selectedAccountName = await dialog.ShowDialog<string?>(this);
			if (selectedAccountName != null)
			{
				vm.AddPlayerByAccountName(selectedAccountName);
			}
		}
	}
}
