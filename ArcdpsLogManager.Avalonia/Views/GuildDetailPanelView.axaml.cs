using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class GuildDetailPanelView : UserControl
	{
		public GuildDetailPanelView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
