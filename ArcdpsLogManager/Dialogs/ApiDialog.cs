using System;
using Eto.Drawing;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Dialogs
{
	public class ApiDialog : Dialog
	{
		public ApiDialog(ApiData apiData)
		{
			Title = "API data - arcdps Log Manager";
			ClientSize = new Size(300, -1);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			var item = new Button {Text = "Close"};
			item.Click += (sender, args) => Close();
			PositiveButtons.Add(item);

			var enableCheckbox = new CheckBox {Text = "Use the GW2 API", Checked = Settings.UseGW2Api};
			enableCheckbox.CheckedChanged += (sender, args) => Settings.UseGW2Api = enableCheckbox.Checked ?? false;

			var guildCountLabel = new Label {Text = "Not loaded"};

			UpdateLabelTexts(apiData, guildCountLabel);

			formLayout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				formLayout.AddRow(new Label
				{
					Text = "Guild names and tags can be loaded the official GW2 API as\n" +
					       "the EVTC logs only contain GUID values."
				});
			}
			formLayout.EndVertical();
			formLayout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				formLayout.AddRow(new Label {Text = "Guilds:"}, guildCountLabel);
			}
			formLayout.EndVertical();
			formLayout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				formLayout.AddRow(enableCheckbox);
			}
			formLayout.EndVertical();

			void OnGuildAdded(object sender, EventArgs _)
			{
				Application.Instance.AsyncInvoke(() => { UpdateLabelTexts(apiData, guildCountLabel); });
			}

			apiData.GuildAdded += OnGuildAdded;

			Closed += (sender, args) => apiData.GuildAdded -= OnGuildAdded;
		}

		private void UpdateLabelTexts(ApiData apiData, Label countLabel)
		{
			UpdateGuildCountText(apiData, countLabel);
		}

		private void UpdateGuildCountText(ApiData apiData, Label label)
		{
			string text = "Not loaded";

			if (apiData != null)
			{
				text = $"{apiData.CachedGuildCount}";
			}

			label.Text = text;
		}
	}
}