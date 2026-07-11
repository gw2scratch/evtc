using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for a standalone window showing a fixed set of logs (Avalonia counterpart of the
	/// Eto <c>GameDataCollecting</c>'s "click a species/skill row's Logs cell" popup, which opens a
	/// <c>Form</c> containing a <c>LogList</c> with <c>DataStore</c> set to that row's log list).
	/// Reuses the same <see cref="LogsSectionViewModel"/>/<see cref="LogsSectionView"/> the main Logs
	/// tab uses, so favoriting/detail/etc. behave identically.
	/// </summary>
	public partial class LogListWindowViewModel : ObservableObject
	{
		[ObservableProperty] private string title;

		public LogsSectionViewModel Logs { get; }

		public LogListWindowViewModel(string title, IReadOnlyList<LogData> logs, ImageProvider images,
			ILogNameProvider nameProvider, LogCacheService cacheService, ApiData apiData)
		{
			this.title = title;
			Logs = new LogsSectionViewModel(images, nameProvider, cacheService, apiData);
			Logs.SetLogs(logs.Select(log => new LogRow(log, images, nameProvider)).ToList());
		}
	}
}
