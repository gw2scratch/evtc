using Plugin.Settings;
using Eto.Drawing;
using Eto.Forms;

namespace ArcdpsLogManager
{
	public class LogSettingsDialog : Dialog
	{
		private string LogDirectoryPath { get; set; } = Settings.LogRootPath;

		public LogSettingsDialog()
		{
			Title = "Settings";
			ClientSize = new Size(500, -1);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			var dialog = new SelectFolderDialog();

			var item = new Button {Text = "Save"};
			item.Click += (sender, args) => Close();

			PositiveButtons.Add(item);

			var locationTextBox = new TextBox
			{
				ReadOnly = true,
				PlaceholderText = "Log Location",
				Text = LogDirectoryPath,
			};

			var locationDialogButton = new Button {Text = "Select Log Directory"};
			locationDialogButton.Click += (sender, args) =>
			{
				if (dialog.ShowDialog(this) == DialogResult.Ok)
				{
					LogDirectoryPath = dialog.Directory;
					locationTextBox.Text = LogDirectoryPath;
				}
			};

			formLayout.BeginVertical(new Padding(10), new Size(5, 5));
			formLayout.BeginHorizontal();
			formLayout.Add(locationTextBox, true);
			formLayout.Add(locationDialogButton);
			formLayout.EndHorizontal();
			formLayout.EndVertical();
			formLayout.AddSeparateRow(null);

			Closed += (sender, args) =>
			{
				Settings.LogRootPath = LogDirectoryPath;
			};
		}
	}
}