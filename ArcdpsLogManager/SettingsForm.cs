using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager
{
	public class SettingsForm : Form
	{
		private static readonly string[] DefaultLogLocation = {"Guild Wars 2", "addons", "arcdps", "arcdps.cbtlogs"};

		public event EventHandler SettingsSaved;

		public SettingsForm()
		{
			Title = "Settings - arcdps Log Manager";
			ClientSize = new Size(400, -1);
			MinimumSize = new Size(400, 300);

			var apiDataCheckbox = new CheckBox {Text = "Use the Guild Wars 2 API", Checked = Settings.UseGW2Api};
			var dialog = new SelectFolderDialog();

			var locationTextBox = new TextBox
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

			var saveButton = new Button {Text = "Save"};
			saveButton.Click += (sender, args) =>
			{
				Settings.UseGW2Api = apiDataCheckbox.Checked ?? false;
				if (locationTextBox.Text.Trim() != Settings.LogRootPaths.FirstOrDefault())
				{
					Settings.LogRootPaths = new [] {locationTextBox.Text};
				}

				SettingsSaved?.Invoke(this, EventArgs.Empty);

				Close();
			};

			var layout = new DynamicLayout();
			layout.BeginVertical(spacing: new Size(5, 5), padding: new Padding(10));
			{
				layout.BeginGroup("Logs", new Padding(5), new Size(5, 5));
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
				layout.BeginGroup("Data", new Padding(5), new Size(5, 5));
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
			}
			layout.EndVertical();
			layout.Add(null);
			layout.BeginVertical(padding: new Padding(10));
			{
				layout.BeginHorizontal();
				{
					layout.Add(null, xscale: true);
					layout.Add(saveButton, xscale: false);
				}
				layout.EndHorizontal();
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