using System.Diagnostics;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Updates;

namespace GW2Scratch.ArcdpsLogManager.Dialogs
{
	public class ProgramUpdateDialog : Dialog
	{
		public ProgramUpdateDialog(Release release)
		{
			Title = $"Update available";
			ClientSize = new Size(-1, -1);
			var layout = new DynamicLayout();
			Content = layout;

			var changelog = new Button {Text = "View changes"};
			var later = new Button {Text = "Later"};
			var ignore = new Button {Text = "Ignore"};
			var download = new Button {Text = "Download"};

			layout.BeginVertical(new Padding(10), new Size(10, 10));
			{
				layout.AddRow(new Label
				{
					Text = $"Log Manager {release.Version} is available for download."
				});
				layout.AddRow(changelog);
			}
			layout.EndVertical();

			later.Click += (sender, args) => Close();
			ignore.Click += (sender, args) =>
			{
				Settings.IgnoredUpdateVersions = Settings.IgnoredUpdateVersions.Append(release.Version).Distinct().ToList();
				Close();
			};
			changelog.Click += (sender, args) =>
			{
				var processInfo = new ProcessStartInfo()
				{
					FileName = release.ChangelogUrl,
					UseShellExecute = true
				};
				Process.Start(processInfo);
			};
			download.Click += (sender, args) =>
			{
				var processInfo = new ProcessStartInfo()
				{
					FileName = release.ToolSiteUrl,
					UseShellExecute = true
				};
				Process.Start(processInfo);
				Close();
			};

			AbortButton = later;
			DefaultButton = download;
			PositiveButtons.Add(download);
			NegativeButtons.Add(later);
			NegativeButtons.Add(ignore);
		}
	}
}