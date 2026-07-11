using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>
	/// Warns before permanently deleting logs whose success detection is known to be occasionally
	/// unreliable. Show via <c>ShowDialog&lt;bool&gt;</c>: returns <see langword="true"/> only if
	/// the user chose to delete anyway.
	/// </summary>
	public partial class UnreliableLogsWindow : Window
	{
		public UnreliableLogsWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void OnConfirmClick(object? sender, RoutedEventArgs e)
		{
			Close(true);
		}

		private void OnCancelClick(object? sender, RoutedEventArgs e)
		{
			Close(false);
		}
	}
}
