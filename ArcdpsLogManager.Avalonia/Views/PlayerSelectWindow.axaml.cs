using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>
	/// Player-picker dialog (Avalonia counterpart of the Eto <c>Dialogs/PlayerSelectDialog.cs</c>).
	/// Show via <c>ShowDialog&lt;string?&gt;</c>: returns the selected account name (with its
	/// leading ':', matching the convention used everywhere else in this app) or
	/// <see langword="null"/> if the user cancelled or nothing was selected.
	/// </summary>
	public partial class PlayerSelectWindow : Window
	{
		public PlayerSelectWindow()
		{
			InitializeComponent();

			// Initial focus: let the user start typing to filter immediately (Avalonia does not
			// auto-focus any control on window open).
			Opened += (_, _) => FilterTextBox.Focus();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void OnSelectClick(object? sender, RoutedEventArgs e)
		{
			Close(SelectedAccountName);
		}

		private void OnCancelClick(object? sender, RoutedEventArgs e)
		{
			Close(null);
		}

		private void OnRowDoubleTapped(object? sender, TappedEventArgs e)
		{
			if (SelectedAccountName != null)
			{
				Close(SelectedAccountName);
			}
		}

		private string? SelectedAccountName =>
			(DataContext as PlayerSelectWindowViewModel)?.SelectedPlayer?.Data.AccountName;
	}
}
