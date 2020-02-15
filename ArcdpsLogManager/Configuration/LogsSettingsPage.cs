using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Configuration
{
	public class LogsSettingsPage : SettingsPage
	{
		private static readonly string[] DefaultLogLocation = {"Guild Wars 2", "addons", "arcdps", "arcdps.cbtlogs"};

		private readonly TextBox locationTextBox;
		private readonly CheckBox minDurationCheckBox;
		private readonly NumericMaskedTextBox<int> minDurationTextBox;

		public LogsSettingsPage()
		{
			Text = "Logs";

			var dialog = new SelectFolderDialog();

			locationTextBox = new TextBox
			{
				ReadOnly = true,
				PlaceholderText = "Log Location",
			};

			var locationDialogButton = new Button {Text = "Select Log Directory"};
			locationDialogButton.Click += (sender, args) =>
			{
				if (dialog.ShowDialog(this) == DialogResult.Ok)
				{
					locationTextBox.Text = dialog.Directory;
				}
			};

			minDurationCheckBox = new CheckBox
			{
				Text = "Exclude short logs",
				Checked = Settings.MinimumLogDurationSeconds.HasValue,
				ThreeState = false
			};

			minDurationTextBox = new NumericMaskedTextBox<int>
			{
				Value = Settings.MinimumLogDurationSeconds ?? 5,
				Enabled = minDurationCheckBox.Checked ?? false,
				Width = 50
			};

			minDurationCheckBox.CheckedChanged += (sender, args) =>
				minDurationTextBox.Enabled = minDurationCheckBox.Checked ?? false;

			var durationLabel = new Label
			{
				Text = "Minimum duration in seconds:", VerticalAlignment = VerticalAlignment.Center
			};

			var layout = new DynamicLayout();
			layout.BeginVertical(spacing: new Size(5, 5), padding: new Padding(10));
			{
				layout.BeginGroup("Log directory", new Padding(5), new Size(5, 5));
				{
					layout.AddRow(new Label
					{
						Text = "The directory in which your arcdps logs are stored. Subdirectories " +
						       "are also searched, do not choose a parent directory containing more " +
						       "irrelevant files unless you like extra waiting.",
						Wrap = WrapMode.Word,
						Height = 50
					});
					layout.AddRow(locationTextBox);
					layout.AddRow(locationDialogButton);
				}
				layout.EndGroup();
				layout.BeginGroup("Log filters", new Padding(5), new Size(5, 5));
				{
					layout.AddRow(minDurationCheckBox);
					layout.AddRow(durationLabel, minDurationTextBox, null);
				}
				layout.EndGroup();
			}
			layout.EndVertical();

			Content = layout;

			if (Settings.LogRootPaths.Any())
			{
				if (Settings.LogRootPaths.Count > 1)
				{
					// There is currently no interface for adding more than one log directory, so this would end up
					// losing some quietly when that is implemented.
					throw new NotImplementedException();
				}

				string logRootPath = Settings.LogRootPaths.Single();
				if (Directory.Exists(logRootPath))
				{
					dialog.Directory = logRootPath;
				}

				locationTextBox.Text = logRootPath;
			}
			else
			{
				string defaultDirectory = GetDefaultLogDirectory();
				if (Directory.Exists(defaultDirectory))
				{
					dialog.Directory = defaultDirectory;
					locationTextBox.Text = defaultDirectory;
				}
			}
		}

		public override void SaveSettings()
		{
			if (locationTextBox.Text.Trim() != Settings.LogRootPaths.FirstOrDefault())
			{
				Settings.LogRootPaths = new[] {locationTextBox.Text};
			}

			bool minDurationChecked = minDurationCheckBox.Checked ?? false;
			Settings.MinimumLogDurationSeconds = minDurationChecked ? (int?) minDurationTextBox.Value : null;
		}

		private static string GetDefaultLogDirectory()
		{
			// We need to do this to get the correct separators on all platforms
			var pathParts = new[] {Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}
				.Concat(DefaultLogLocation)
				.ToArray();

			return Path.Combine(pathParts);
		}
	}
}