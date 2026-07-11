using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Gw2Api;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the API-data dialog (Avalonia counterpart of the Eto <c>ApiDialog</c>):
	/// shows guild-cache stats, toggles <c>UseGW2Api</c>, and clears the API cache.
	/// </summary>
	public partial class ApiWindowViewModel : ObservableObject
	{
		private readonly ApiData apiData;

		public ISettingsService Settings { get; }

		[ObservableProperty] private string guildCountText = "0";
		[ObservableProperty] private string sizeText = "No file";

		public ApiWindowViewModel(ApiData apiData, ISettingsService settings)
		{
			this.apiData = apiData;
			Settings = settings;
			RefreshStats();
		}

		private void RefreshStats()
		{
			GuildCountText = $"{apiData.CachedGuildCount:N0}";
			FileInfo info = apiData.GetCacheFileInfo();
			SizeText = info.Exists ? $"{info.Length / 1000.0 / 1000.0:0.00} MB" : "No file";
		}

		public async Task DeleteAsync(Window owner)
		{
			bool confirmed = await Dialogs.ShowConfirmAsync(owner, "Delete API cache",
				$"Delete the API cache? The API data of all {apiData.CachedGuildCount} guilds will be forgotten. " +
				"Renamed guilds will have their names/tags updated, but data of disbanded guilds won't be " +
				"retrievable anymore.");
			if (!confirmed)
			{
				return;
			}

			apiData.Clear();
			apiData.SaveDataToFile();
			RefreshStats();
			await Dialogs.ShowInfoAsync(owner, "Delete API cache", "API cache deleted.");
		}
	}
}
