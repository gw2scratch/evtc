using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Services
{
	/// <summary>
	/// Minimal message-box helpers.
	/// </summary>
	/// <remarks>
	/// The migration plan allowed either the MIT <c>MsBox.Avalonia</c> package or a small custom
	/// dialog. MsBox.Avalonia's only version resolvable in the build environment here was a
	/// prerelease targeting an older Avalonia 11 RC (risking a runtime mismatch with 11.2.1), so we
	/// use this tiny self-contained dialog instead. It is intentionally API-shaped like a message
	/// box (<see cref="ShowInfoAsync"/> / <see cref="ShowConfirmAsync"/>) so it can be swapped for
	/// MsBox.Avalonia on a machine with full nuget.org access with minimal changes.
	/// </remarks>
	public static class Dialogs
	{
		public static Task ShowInfoAsync(Window owner, string title, string message)
			=> ShowAsync(owner, title, message, confirm: false);

		public static Task<bool> ShowConfirmAsync(Window owner, string title, string message)
			=> ShowAsync(owner, title, message, confirm: true);

		private static async Task<bool> ShowAsync(Window owner, string title, string message, bool confirm)
		{
			bool result = false;

			var dialog = new Window
			{
				Title = title,
				SizeToContent = SizeToContent.WidthAndHeight,
				CanResize = false,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				MinWidth = 320,
				Background = owner.Background,
			};

			var buttons = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right,
				Spacing = 8,
			};

			// The plain info variant (no Cancel button) still closes on Escape (IsCancel doesn't
			// require a click handler beyond the default Close() below, since result already
			// defaults to false and info dialogs have no "confirmed" meaning anyway).
			var okButton = new Button
			{
				Content = confirm ? "Yes" : "OK", IsDefault = true, IsCancel = !confirm, MinWidth = 80
			};
			okButton.Click += (_, _) =>
			{
				result = true;
				dialog.Close();
			};
			buttons.Children.Add(okButton);

			if (confirm)
			{
				var cancelButton = new Button { Content = "No", IsCancel = true, MinWidth = 80 };
				cancelButton.Click += (_, _) => dialog.Close();
				buttons.Children.Add(cancelButton);
			}

			dialog.Content = new StackPanel
			{
				Margin = new global::Avalonia.Thickness(20),
				Spacing = 16,
				Children =
				{
					new TextBlock
					{
						Text = message,
						TextWrapping = TextWrapping.Wrap,
						MaxWidth = 480,
					},
					buttons,
				},
			};

			await dialog.ShowDialog(owner);
			return result;
		}
	}
}
