using Eto.Drawing;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Configuration
{
	public class UpdateSettingsPage : SettingsPage
	{
		private readonly CheckBox updateCheckbox;

		public UpdateSettingsPage()
		{
			Text = "Updates";
			updateCheckbox = new CheckBox {Text = "Check for updates on launch", Checked = Settings.CheckForUpdates};

			var layout = new DynamicLayout();
			layout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				layout.BeginGroup("Log Manager updates", new Padding(5), new Size(5, 5));
				{
					layout.AddRow(new Label
					{
						Text = "The program can automatically look for updates on launch and inform you"  +
						       "when there is a new release available.",
						Wrap = WrapMode.Word,
						Height = 50
					});
					layout.AddRow(updateCheckbox);
				}
				layout.EndGroup();
				layout.AddRow(null);
			}
			layout.EndVertical();


			Content = layout;
		}

		public override void SaveSettings()
		{
			Settings.CheckForUpdates = updateCheckbox.Checked ?? false;
		}
	}
}