using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class GuildsSectionView : UserControl
	{
		public GuildsSectionView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
