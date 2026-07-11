using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Caching;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the log-cache dialog (Avalonia counterpart of the Eto <c>CacheDialog</c>):
	/// shows cache stats and offers prune / delete, then reloads.
	/// </summary>
	public partial class CacheWindowViewModel : ObservableObject
	{
		private readonly LogCache? cache;
		private readonly Func<IReadOnlyList<LogData>> getLoadedLogs;
		private readonly Func<Task> reload;

		[ObservableProperty] private string countText = "Not loaded";
		[ObservableProperty] private string unloadedText = "Not loaded";
		[ObservableProperty] private string sizeText = "No file";
		[ObservableProperty] private bool canModify;

		public CacheWindowViewModel(LogCache? cache, Func<IReadOnlyList<LogData>> getLoadedLogs, Func<Task> reload)
		{
			this.cache = cache;
			this.getLoadedLogs = getLoadedLogs;
			this.reload = reload;
			canModify = cache != null;
			RefreshStats();
		}

		private void RefreshStats()
		{
			if (cache == null)
			{
				CountText = "Cache is read-only (in use by another instance)";
				UnloadedText = "-";
				SizeText = "-";
				return;
			}

			CountText = $"{cache.LogCount:N0}";
			UnloadedText = $"{cache.GetUnloadedLogCount(getLoadedLogs()):N0}";
			var info = cache.GetCacheFileInfo();
			SizeText = info.Exists ? $"{info.Length / 1000.0 / 1000.0:0.00} MB" : "No file";
		}

		public async Task PruneAsync(Window owner)
		{
			if (cache == null)
			{
				return;
			}

			int unloaded = cache.GetUnloadedLogCount(getLoadedLogs());
			bool confirmed = await Dialogs.ShowConfirmAsync(owner, "Prune cache",
				$"Prune the cache? {unloaded} results of currently unloaded logs will be forgotten. " +
				"If the logs are added back in the future, they will have to be processed again.");
			if (!confirmed)
			{
				return;
			}

			int pruned = cache.Prune(getLoadedLogs());
			cache.SaveToFile();
			await reload();
			RefreshStats();
			await Dialogs.ShowInfoAsync(owner, "Prune cache", $"Cache pruned, {pruned} results forgotten.");
		}

		public async Task DeleteAsync(Window owner)
		{
			if (cache == null)
			{
				return;
			}

			int logCount = cache.LogCount;
			bool confirmed = await Dialogs.ShowConfirmAsync(owner, "Delete cache",
				$"Delete the cache? The results of all {logCount} cached logs will be forgotten. " +
				"All logs will have to be processed again.");
			if (!confirmed)
			{
				return;
			}

			cache.Clear();
			cache.SaveToFile();
			await reload();
			RefreshStats();
			await Dialogs.ShowInfoAsync(owner, "Delete cache", $"Cache deleted, {logCount} results forgotten.");
		}
	}
}
