using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Configuration
{
	public abstract class SettingsPage : TabPage
	{
		public abstract void SaveSettings();
	}
}