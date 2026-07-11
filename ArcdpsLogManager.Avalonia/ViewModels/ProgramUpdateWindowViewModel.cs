using System.Diagnostics;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Updates;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the "a new version is available" dialog (Avalonia counterpart of the Eto
	/// <c>ProgramUpdateDialog</c>).
	/// </summary>
	public class ProgramUpdateWindowViewModel
	{
		private readonly Release release;

		public string Message => $"Log Manager {release.Version} is available for download.";

		public ProgramUpdateWindowViewModel(Release release)
		{
			this.release = release;
		}

		public void OpenChangelog() => OpenUrl(release.ChangelogUrl);

		public void OpenDownloadPage() => OpenUrl(release.ToolSiteUrl);

		public void IgnoreThisVersion()
		{
			Settings.IgnoredUpdateVersions = Settings.IgnoredUpdateVersions.Append(release.Version).Distinct().ToList();
		}

		private static void OpenUrl(string url)
		{
			var processInfo = new ProcessStartInfo
			{
				FileName = url,
				UseShellExecute = true
			};
			Process.Start(processInfo);
		}
	}
}
