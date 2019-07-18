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
			ClientSize = new Size(300, -1);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			var item = new Button {Text = "Close"};
			item.Click += (sender, args) => Close();
			PositiveButtons.Add(item);

			bool cacheLoaded = managerForm.LogCache != null;

			var deleteButton = new Button
			{
				Text = "Delete the cache",
				Enabled = cacheLoaded
			};

			var pruneButton = new Button
			{
				Text = "Prune missing logs",
				Enabled = cacheLoaded
			};

			var countLabel = new Label {Text = "Not loaded"};
			var unloadedCountLabel = new Label {Text = "Not loaded"};
			var sizeLabel = new Label {Text = "No file"};

			formLayout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				formLayout.AddRow(new Label
				{
					Text = "The parsed contents of logs are saved in a cache file to save time.\n" +
					       "You can delete the cached results here to parse the logs again or prune\n" +
					       "results for logs that are not in the scanned directory anymore.",
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
					    $"Prune the cache? {unloadedLogs} results of currently unloaded will be forgotten. " +
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

			void OnCacheLoaded(object sender, EventArgs _)
			{
				deleteButton.Enabled = true;
				pruneButton.Enabled = true;
				UpdateLabelTexts(managerForm, countLabel, unloadedCountLabel, sizeLabel);
			}

			managerForm.CacheLoaded += OnCacheLoaded;

			Closed += (sender, args) => managerForm.CacheLoaded -= OnCacheLoaded;
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