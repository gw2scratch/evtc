using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class PlayerDetailPanelView : UserControl
	{
		public PlayerDetailPanelView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
