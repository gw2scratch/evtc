using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class PlayersSectionView : UserControl
	{
		public PlayersSectionView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
