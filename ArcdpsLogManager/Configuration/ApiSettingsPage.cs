using Eto.Drawing;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Configuration
{
	public class ApiSettingsPage : SettingsPage
	{
		private readonly CheckBox apiDataCheckbox;

		public ApiSettingsPage()
		{
			Text = "Guild Wars 2 API";
			apiDataCheckbox = new CheckBox {Text = "Use the Guild Wars 2 API", Checked = Settings.UseGW2Api};

			var layout = new DynamicLayout();
			layout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				layout.BeginGroup("API Data", new Padding(5), new Size(5, 5));
				{
					layout.AddRow(new Label
					{
						Text = "The program can use the official Guild Wars 2 API to retrieve guild data. " +
						       "No API key is required. If this is not enabled, guild names and " +
						       "tags will not be available.",
						Wrap = WrapMode.Word,
						Height = 50
					});
					layout.AddRow(apiDataCheckbox);
				}
				layout.EndGroup();
				layout.AddRow(null);
			}
			layout.EndVertical();


			Content = layout;
		}

		public override void SaveSettings()
		{
			Settings.UseGW2Api = apiDataCheckbox.Checked ?? false;
		}
	}
}