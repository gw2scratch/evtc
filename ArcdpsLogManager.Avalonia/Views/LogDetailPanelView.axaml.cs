using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class LogDetailPanelView : UserControl
	{
		public LogDetailPanelView()
		{
			InitializeComponent();

			var copyButton = this.FindControl<Button>("CopyUrlButton");
			var openButton = this.FindControl<Button>("OpenUrlButton");

			if (copyButton != null)
			{
				copyButton.Click += OnCopyUrlClick;
			}

			if (openButton != null)
			{
				openButton.Click += OnOpenUrlClick;
			}
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private async void OnCopyUrlClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (DataContext is not LogDetailPanelViewModel vm || vm.UploadUrl is not { } url)
			{
				return;
			}

			var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
			if (clipboard != null)
			{
				await clipboard.SetTextAsync(url);
			}
		}

		private async void OnOpenUrlClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (DataContext is not LogDetailPanelViewModel vm || vm.UploadUrl is not { } url)
			{
				return;
			}

			try
			{
				Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
			}
			catch (System.Exception ex)
			{
				if (TopLevel.GetTopLevel(this) is Window owner)
				{
					await Dialogs.ShowInfoAsync(owner, "Error", $"Failed to open the URL: {ex.Message}. Please try again.");
				}
			}
		}
	}
}
