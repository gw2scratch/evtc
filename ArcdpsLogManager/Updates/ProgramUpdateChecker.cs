using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Updates
{
	public class ProgramUpdateChecker
	{
		private readonly HttpClient httpClient = new HttpClient();
		private readonly string versionEndpointUrl;

		public ProgramUpdateChecker(string versionEndpointUrl)
		{
			this.versionEndpointUrl = versionEndpointUrl;
		}

		/// <summary>
		/// Check for new updates of the program.
		/// </summary>
		/// <returns>The newest release or <see langword="null"/> in case none were found or if all releases are ignored.</returns>
		public async Task<Release> CheckUpdates()
		{
			var feed = await GetReleaseFeed();
			if (feed == null)
			{
				return null;
			}

			foreach (var release in feed.Releases)
			{
				if (!IsIgnored(release))
				{
					return release;
				}
			}

			return null;
		}

		private bool IsIgnored(Release release)
		{
			if (Settings.IgnoredUpdateVersions.Contains(release.Version))
			{
				return true;
			}

			if (!Version.TryParse(release.Version, out var version))
			{
				return true;
			}

			var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
			if (version <= currentVersion)
			{
				return true;
			}

			return false;
		}

		private async Task<ReleaseFeed> GetReleaseFeed()
		{
			try
			{
				var response = await httpClient.GetAsync(versionEndpointUrl);
				string json = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<ReleaseFeed>(json);
			}
			catch
			{
				return null;
			}
		}
	}
}