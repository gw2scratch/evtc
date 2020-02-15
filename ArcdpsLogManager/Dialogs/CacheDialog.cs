using System;
using Eto.Drawing;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Dialogs
{
	public class CacheDialog : Dialog
	{
		public CacheDialog(ManagerForm managerForm)
		{
			Title = "Log cache - arcdps Log Manager";
			var formLayout = new DynamicLayout();

			var item = new Button {Text = "Close"};
			item.Click += (sender, args) => Close();
			PositiveButtons.Add(item);

			var deleteButton = new Button
			{
				Text = "Delete the cache",
			};

			var pruneButton = new Button
			{
				Text = "Prune missing logs",
			};

			var countLabel = new Label {Text = "Not loaded"};
			var unloadedCountLabel = new Label {Text = "Not loaded"};
			var sizeLabel = new Label {Text = "No file"};

			formLayout.BeginVertical(new Padding(10), new Size(0, 0));
			{
				// This is a very hacky solution for WrapMode.Word not working properly on the Gtk platform
				formLayout.AddRow(new Label
				{
					Text = "The processed contents of logs are saved in a cache file to save time. You can",
					Wrap = WrapMode.None
				});
				formLayout.AddRow(new Label
				{
					Text = "delete the cached results here to process the logs again or prune results for",
					Wrap = WrapMode.None
				});
				formLayout.AddRow(new Label
				{
					Text = "logs that are not in the scanned directory anymore.",
					Wrap = WrapMode.None
				});
			}
			formLayout.EndVertical();
			formLayout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				UpdateLabelTexts(managerForm, countLabel, unloadedCountLabel, sizeLabel);
				formLayout.AddRow(new Label {Text = "Total cached logs:"}, countLabel);
				formLayout.AddRow(new Label {Text = "Unloaded cached logs:"}, unloadedCountLabel);
				formLayout.AddRow(new Label {Text = "Cache file size:"}, sizeLabel);
			}
			formLayout.EndVertical();
			formLayout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				formLayout.AddRow(pruneButton);
				formLayout.AddRow(deleteButton);
			}
			formLayout.EndVertical();

			pruneButton.Click += (sender, args) =>
			{
				int unloadedLogs = managerForm.LogCache?.GetUnloadedLogCount(managerForm.LoadedLogs) ?? 0;
				if (MessageBox.Show(
					    $"Prune the cache? {unloadedLogs} results of currently unloaded logs will be forgotten. " +
					    "If the logs are added back in the future, they will have to be parsed again.",
					    MessageBoxButtons.OKCancel) == DialogResult.Ok)
				{
					int pruned = managerForm.LogCache?.Prune(managerForm.LoadedLogs) ?? 0;
					MessageBox.Show($"Cache pruned, {pruned} results forgotten.");
					managerForm.LogCache?.SaveToFile();
					UpdateLabelTexts(managerForm, countLabel, unloadedCountLabel, sizeLabel);
					managerForm.ReloadLogs();
				}
			};

			deleteButton.Click += (sender, args) =>
			{
				int logCount = managerForm.LogCache?.LogCount ?? 0;
				if (MessageBox.Show(
					    $"Delete the cache? The results of all {logCount} cached logs will be forgotten. " +
					    "All logs will have to be parsed again.",
					    MessageBoxButtons.OKCancel) == DialogResult.Ok)
				{
					managerForm.LogCache?.Clear();
					managerForm.LogCache?.SaveToFile();
					MessageBox.Show($"Cache deleted, {logCount} results forgotten.");
					UpdateLabelTexts(managerForm, countLabel, unloadedCountLabel, sizeLabel);
					managerForm.ReloadLogs();
				}
			};

			UpdateLabelTexts(managerForm, countLabel, unloadedCountLabel, sizeLabel);

			Content = formLayout;
		}

		private void UpdateLabelTexts(ManagerForm managerForm, Label countLabel, Label unloadedCountLabel,
			Label cacheSizeLabel)
		{
			UpdateCacheCountText(managerForm, countLabel);
			UpdateUnloadedCacheCountLabel(managerForm, unloadedCountLabel);
			UpdateCacheSizeText(managerForm, cacheSizeLabel);
		}

		private void UpdateCacheCountText(ManagerForm managerForm, Label label)
		{
			string text = "Not loaded";

			if (managerForm.LogCache != null)
			{
				text = $"{managerForm.LogCache?.LogCount}";
			}

			label.Text = text;
		}

		private void UpdateUnloadedCacheCountLabel(ManagerForm managerForm, Label label)
		{
			string text = "Not loaded";

			if (managerForm.LogCache != null)
			{
				text = $"{managerForm.LogCache?.GetUnloadedLogCount(managerForm.LoadedLogs)}";
			}

			label.Text = text;
		}

		private void UpdateCacheSizeText(ManagerForm managerForm, Label label)
		{
			string text = "Not loaded";

			if (managerForm.LogCache != null)
			{
				var fileInfo = managerForm.LogCache.GetCacheFileInfo();
				text = fileInfo.Exists ? $"{fileInfo.Length / 1000.0 / 1000.0:0.00} MB" : "No file";
			}

			label.Text = text;
		}
	}
}