using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>
	/// The Avalonia counterpart of the Eto <c>LoadingForm</c>: a small, lightweight window shown
	/// while the log cache and API data load, before the (much heavier) <see cref="MainWindow"/> is
	/// constructed and shown.
	/// </summary>
	public partial class LoadingWindow : Window
	{
		public LoadingWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
