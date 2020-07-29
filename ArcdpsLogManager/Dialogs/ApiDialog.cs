using System;
using System.IO;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Processing;

namespace GW2Scratch.ArcdpsLogManager.Dialogs
{
	public class ApiDialog : Dialog
	{
		public ApiDialog(ApiProcessor apiProcessor)
		{
			Title = "API data - arcdps Log Manager";
			ClientSize = new Size(500, -1);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			var item = new Button {Text = "Close"};
			item.Click += (sender, args) => Close();
			PositiveButtons.Add(item);

			var deleteButton = new Button
			{
				Text = "Delete the cache",
			};

			var enableCheckbox = new CheckBox {Text = "Use the GW2 API", Checked = Settings.UseGW2Api};
			enableCheckbox.CheckedChanged += (sender, args) => Settings.UseGW2Api = enableCheckbox.Checked ?? false;

			var guildCountLabel = new Label {Text = "Not loaded"};
			var sizeLabel = new Label {Text = "No file"};

			UpdateLabelTexts(apiProcessor.ApiData, guildCountLabel, sizeLabel);

			formLayout.BeginVertical(new Padding(10), new Size(0, 0));
			{
				formLayout.AddRow(new Label
				{
					Text = "Guild names and tags have to be loaded from the official GW2 API as the EVTC logs only contain GUID values.",
					Wrap = WrapMode.Word
				});
			}
			formLayout.EndVertical();
			formLayout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				formLayout.AddRow(new Label {Text = "Guilds:"}, guildCountLabel);
				formLayout.AddRow(new Label {Text = "Cache file size:"}, sizeLabel);
			}
			formLayout.EndVertical();
			formLayout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				formLayout.AddRow(enableCheckbox);
				formLayout.AddRow(deleteButton);
			}
			formLayout.EndVertical();

			void OnGuildAdded(object sender, EventArgs _)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					UpdateLabelTexts(apiProcessor.ApiData, guildCountLabel, sizeLabel);
				});
			}

			deleteButton.Click += (sender, args) =>
			{
				if (MessageBox.Show(
					    $"Delete the API cache? The API data of all {apiProcessor.ApiData.CachedGuildCount} guilds " +
					    "will be forgotten. Renamed guilds will have their names/tags updated, but data of " +
					    "disbanded guilds won't be retrievable anymore. You may have to restart the program for " +
					    "everything to update.",
					    MessageBoxButtons.OKCancel) == DialogResult.Ok)
				{
					apiProcessor.ApiData.Clear();
					apiProcessor.ApiData.SaveDataToFile();
					MessageBox.Show("API Cache deleted.");
					UpdateLabelTexts(apiProcessor.ApiData, guildCountLabel, sizeLabel);
				}
			};

			apiProcessor.Processed += OnGuildAdded;

			Closed += (sender, args) => apiProcessor.Processed -= OnGuildAdded;
		}

		private void UpdateLabelTexts(ApiData apiData, Label countLabel, Label cacheSizeLabel)
		{
			UpdateGuildCountText(apiData, countLabel);
			UpdateCacheSizeText(apiData, cacheSizeLabel);
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

		private void UpdateCacheSizeText(ApiData apiData, Label label)
		{
			string text = "Not loaded";

			if (apiData != null)
			{
				FileInfo fileInfo = apiData.GetCacheFileInfo();
				text = fileInfo.Exists ? $"{fileInfo.Length / 1000.0 / 1000.0:0.00} MB" : "No file";
			}

			label.Text = text;
		}
	}
}