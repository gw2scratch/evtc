using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class MultipleLogPanelView : UserControl
	{
		public MultipleLogPanelView()
		{
			InitializeComponent();

			var copyButton = this.FindControl<Button>("CopyLinksButton");
			var openButton = this.FindControl<Button>("OpenLinksButton");

			if (copyButton != null)
			{
				copyButton.Click += OnCopyLinksClick;
			}

			if (openButton != null)
			{
				openButton.Click += OnOpenLinksClick;
			}
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private async void OnCopyLinksClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (DataContext is not MultipleLogPanelViewModel vm)
			{
				return;
			}

			var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
			if (clipboard != null)
			{
				await clipboard.SetTextAsync(vm.UploadedUrlsText);
			}
		}

		private async void OnOpenLinksClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (DataContext is not MultipleLogPanelViewModel vm)
			{
				return;
			}

			var urls = vm.UploadedUrls;
			if (urls.Count == 0)
			{
				return;
			}

			if (urls.Count > 5 && TopLevel.GetTopLevel(this) is Window owner)
			{
				bool confirmed = await Dialogs.ShowConfirmAsync(owner, "Open uploaded logs in a browser",
					$"Are you sure you want to open {urls.Count} logs at once in a browser?");
				if (!confirmed)
				{
					return;
				}
			}

			foreach (var url in urls)
			{
				try
				{
					Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
				}
				catch
				{
					// Ignore failures to launch the browser for an individual URL and keep going.
				}
			}
		}
	}
}
