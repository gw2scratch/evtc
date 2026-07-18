using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Sections.GameData;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the "Game data" debug section (Avalonia counterpart of the Eto
	/// <c>Sections/GameDataGathering.cs</c>'s <c>GameDataCollecting</c>). Only shown when
	/// <see cref="Configuration.Settings.ShowDebugData"/> is enabled (gated by the shell, exactly
	/// like the Eto <c>ManagerForm</c> gates its "Game data" tab).
	/// </summary>
	/// <remarks>
	/// The actual parsing/aggregation logic lives in Core's <see cref="GameDataGatherer"/> (moved
	/// there from the Eto class so both UIs share one implementation); this view model only owns
	/// UI-only concerns: progress display, cancellation, CSV export and opening a log-list window
	/// for a clicked species/skill row.
	/// </remarks>
	public partial class GameDataSectionViewModel : ObservableObject, IDisposable
	{
		private readonly ImageProvider images;
		private readonly ILogNameProvider nameProvider;
		private readonly LogCacheService cacheService;
		private readonly ApiData apiData;
		private readonly GameDataGatherer gatherer = new GameDataGatherer();
		private CancellationTokenSource? cancellationTokenSource;

		/// <summary>The logs currently shown in the Logs tab, captured by the shell whenever the
		/// active filters change (mirrors the Eto panel reading <c>logList.DataStore</c> at the
		/// moment "Collect data" is clicked).</summary>
		private IReadOnlyList<LogData> currentLogs = Array.Empty<LogData>();

		[ObservableProperty] private int threadCount = Math.Min(Environment.ProcessorCount, 4);
		[ObservableProperty] private bool isRunning;
		[ObservableProperty] private double progressMax = 1;
		[ObservableProperty] private double progressValue;
		[ObservableProperty] private string progressText = "";

		public ObservableCollection<SpeciesDataRow> SpeciesRows { get; } = new();
		public ObservableCollection<SkillDataRow> SkillRows { get; } = new();

		/// <summary>Exposed so the view's code-behind can build a standalone log-list window for a
		/// clicked species/skill row's logs, reusing the same services as the main Logs tab.</summary>
		public ImageProvider Images => images;
		public ILogNameProvider NameProvider => nameProvider;
		public LogCacheService CacheService => cacheService;
		public ApiData ApiData => apiData;

		public GameDataSectionViewModel(ImageProvider images, ILogNameProvider nameProvider, LogCacheService cacheService,
			ApiData apiData)
		{
			this.images = images;
			this.nameProvider = nameProvider;
			this.cacheService = cacheService;
			this.apiData = apiData;
		}

		/// <summary>Called by the shell whenever the filtered log set changes.</summary>
		public void SetLogs(IReadOnlyList<LogData> logs)
		{
			currentLogs = logs;
		}

		private bool CanCollectData() => !IsRunning;

		[RelayCommand(CanExecute = nameof(CanCollectData))]
		private async Task CollectDataAsync()
		{
			cancellationTokenSource?.Cancel();
			var localCts = new CancellationTokenSource();
			cancellationTokenSource = localCts;

			IsRunning = true;
			CollectDataCommand.NotifyCanExecuteChanged();
			CancelCommand.NotifyCanExecuteChanged();

			var logs = currentLogs;
			var progress = new UiThreadProgress<(int done, int totalLogs, int failed)>(p =>
			{
				ProgressMax = p.totalLogs;
				ProgressValue = p.done;
				ProgressText = $"{p.done}/{p.totalLogs} ({p.failed} failed)";
			});

			try
			{
				var (species, skills) = await gatherer.GatherAsync(logs, localCts.Token, ThreadCount, progress);

				SpeciesRows.Clear();
				foreach (var row in species.Select(s => new SpeciesDataRow(s)))
				{
					SpeciesRows.Add(row);
				}

				SkillRows.Clear();
				foreach (var row in skills.Select(s => new SkillDataRow(s)))
				{
					SkillRows.Add(row);
				}
			}
			catch (OperationCanceledException)
			{
				// Cancelled; keep whatever was gathered before (matches the Eto panel, which only
				// updates the grids when the task ran to completion).
			}
			finally
			{
				IsRunning = false;
				CollectDataCommand.NotifyCanExecuteChanged();
				CancelCommand.NotifyCanExecuteChanged();
			}
		}

		private bool CanCancel() => IsRunning;

		[RelayCommand(CanExecute = nameof(CanCancel))]
		private void Cancel()
		{
			cancellationTokenSource?.Cancel();
		}

		public void Dispose()
		{
			cancellationTokenSource?.Cancel();
		}

		/// <summary>
		/// A minimal <see cref="IProgress{T}"/> that always marshals its callback to the UI thread
		/// via <see cref="Dispatcher.UIThread"/> explicitly, rather than relying on
		/// <see cref="System.Progress{T}"/>'s captured-<see cref="SynchronizationContext"/> behaviour
		/// (matching the explicit-dispatch convention used elsewhere in this port, e.g.
		/// <c>CompressWindowViewModel</c>).
		/// </summary>
		private sealed class UiThreadProgress<T> : IProgress<T>
		{
			private readonly Action<T> callback;

			public UiThreadProgress(Action<T> callback)
			{
				this.callback = callback;
			}

			public void Report(T value) => Dispatcher.UIThread.Post(() => callback(value));
		}
	}
}
