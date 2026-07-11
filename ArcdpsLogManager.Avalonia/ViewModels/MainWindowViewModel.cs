using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Logs.Updates;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Updates;
using GW2Scratch.EVTCAnalytics.GameData;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// Shell view model (Avalonia counterpart of the Eto <c>ManagerForm</c>). Owns the shared
	/// services (image provider, log name provider, settings, log cache, API data), loads the logs
	/// once and distributes them to the individual sections.
	/// </summary>
	public partial class MainWindowViewModel : ObservableObject, IDisposable
	{
		private readonly ISettingsService settings;
		private readonly ImageProvider images = new ImageProvider();
		private readonly ILogNameProvider nameProvider = new TranslatedLogNameProvider(GameLanguage.English);
		private readonly LogCacheService cacheService = new LogCacheService();
		private readonly ApiData apiData = ApiData.LoadFromFile();
		private readonly TextSearchFilter textFilter;

		private List<LogRow> allRows = new List<LogRow>();
		private readonly Dictionary<LogData, LogRow> rowByLog = new Dictionary<LogData, LogRow>();
		private readonly ConcurrentQueue<LogData> processedQueue = new ConcurrentQueue<LogData>();
		private LogProcessingService? processingService;
		private DispatcherTimer? processingRefreshTimer;
		private bool loaded;

		// Live discovery of newly-created log files while the app is running (Avalonia counterpart
		// of the Eto ManagerForm's fileSystemWatchers/SetupFileSystemWatchers/AddNewLog).
		private readonly LogFinder logFinder = new LogFinder();
		private readonly List<FileSystemWatcher> fileSystemWatchers = new List<FileSystemWatcher>();

		private readonly LogDataUpdater logDataUpdater = new LogDataUpdater();
		private readonly ProgramUpdateChecker programUpdateChecker =
			new ProgramUpdateChecker("http://gw2scratch.com/releases/manager.json");
		private LogCompressionProcessor? compressionProcessor;

		[ObservableProperty] private string searchQuery = "";

		[ObservableProperty] private string tagSearchText = "";

		[ObservableProperty] private bool showFilterSidebar;

		/// <summary>Gates the "Game data" and "Services" debug tabs, exactly like the Eto
		/// <c>ManagerForm</c> gates its equivalent tabs on this same setting.</summary>
		[ObservableProperty] private bool showDebugData;

		[ObservableProperty] private bool showGuildTagsInLogDetail;
		[ObservableProperty] private bool showFailurePercentagesInLogList;

		[ObservableProperty] private DateTimeOffset? minDate;
		[ObservableProperty] private DateTimeOffset? maxDate;

		[ObservableProperty] private string advancedFiltersButtonText = "Advanced filters";

		[ObservableProperty] private Models.LogGroupNode? selectedGroup;

		private Logs.Filters.Groups.RootLogGroup? rootGroup;

		/// <summary>The encounter-group filter tree roots (a single "All logs" root with subgroups).</summary>
		public ObservableCollection<Models.LogGroupNode> EncounterGroups { get; } = new();

		/// <summary>Distinct tags found across the logs, selectable to require them in the filter.</summary>
		public ObservableCollection<Models.TagFilterItem> AvailableTags { get; } = new();

		/// <summary>The shared filter model (bound directly by the filter sidebar).</summary>
		public LogFilters Filters { get; }

		public LogsSectionViewModel LogsSection { get; }
		public PlayersSectionViewModel PlayersSection { get; }
		public GuildsSectionViewModel GuildsSection { get; }
		public StatisticsSectionViewModel StatisticsSection { get; }
		public WeeklyClearsSectionViewModel WeeklyClearsSection { get; }
		public GameDataSectionViewModel GameDataSection { get; }
		public ServicesSectionViewModel ServicesSection { get; }

		public MainWindowViewModel() : this(new SettingsService())
		{
		}

		public MainWindowViewModel(ISettingsService settings)
		{
			this.settings = settings;

			showFilterSidebar = settings.ShowFilterSidebar;
			showDebugData = settings.ShowDebugData;
			showGuildTagsInLogDetail = settings.ShowGuildTagsInLogDetail;
			showFailurePercentagesInLogList = settings.ShowFailurePercentagesInLogList;
			settings.PropertyChanged += (_, e) =>
			{
				if (e.PropertyName == nameof(ISettingsService.ShowFilterSidebar))
				{
					ShowFilterSidebar = settings.ShowFilterSidebar;
				}
				else if (e.PropertyName == nameof(ISettingsService.ShowDebugData))
				{
					ShowDebugData = settings.ShowDebugData;
				}
				else if (e.PropertyName == nameof(ISettingsService.ShowGuildTagsInLogDetail))
				{
					ShowGuildTagsInLogDetail = settings.ShowGuildTagsInLogDetail;
				}
				else if (e.PropertyName == nameof(ISettingsService.ShowFailurePercentagesInLogList))
				{
					ShowFailurePercentagesInLogList = settings.ShowFailurePercentagesInLogList;
				}
			};

			textFilter = new TextSearchFilter(nameProvider);
			Filters = new LogFilters(nameProvider, textFilter);
			Filters.PropertyChanged += OnFiltersChanged;

			LogsSection = new LogsSectionViewModel(images, nameProvider, cacheService);
			PlayersSection = new PlayersSectionViewModel(images, nameProvider);
			GuildsSection = new GuildsSectionViewModel(apiData, images, nameProvider);
			StatisticsSection = new StatisticsSectionViewModel(nameProvider, images);
			WeeklyClearsSection = new WeeklyClearsSectionViewModel(images);
			GameDataSection = new GameDataSectionViewModel(images, nameProvider, cacheService);
			ServicesSection = new ServicesSectionViewModel();

			// Matches the Eto ManagerForm's constructor-time subscription: re-point the watchers at
			// the new directories whenever the configured log root paths change.
			Settings.LogRootPathChanged += (_, _) => SetupFileSystemWatchers();

			InitialLoadTask = LoadAsync();
		}

		/// <summary>
		/// Completes once the initial cache load (and, if configured, directory discovery) has
		/// finished. Awaited by the startup sequence so the heavy <see cref="MainWindow"/> is only
		/// shown once there is data to show, mirroring the Eto <c>LoadingForm</c> -&gt;
		/// <c>ManagerForm</c> handoff instead of showing a big window that sits unresponsive while
		/// it loads.
		/// </summary>
		public Task InitialLoadTask { get; }

		private void OnFiltersChanged(object? sender, PropertyChangedEventArgs e)
		{
			ApplyFilters();
			UpdateAdvancedFiltersButtonText();
		}

		/// <summary>Counts non-default advanced filters (composition/instability/players/processing
		/// status), mirroring the Eto <c>AdvancedFilterPanel.CountNonDefaultAdvancedFilters</c>
		/// exactly — including that panel's choice to count the whole processing-status tab as one
		/// and to not count the dps.report-upload-status tab at all.</summary>
		private void UpdateAdvancedFiltersButtonText()
		{
			int count = 0;
			if (!Filters.CompositionFilters.IsDefault) count++;
			if (!Filters.InstabilityFilters.IsDefault) count++;
			if (!Filters.PlayerFilters.IsDefault) count++;
			if (!AreProcessingFiltersDefault()) count++;
			AdvancedFiltersButtonText = count > 0 ? $"Advanced filters ({count} set)" : "Advanced filters";
		}

		private bool AreProcessingFiltersDefault()
		{
			return Filters.ShowParseUnparsedLogs && Filters.ShowParseParsingLogs &&
			       Filters.ShowParseParsedLogs && Filters.ShowParseFailedLogs;
		}

		partial void OnSearchQueryChanged(string value)
		{
			textFilter.Query = value;
			ApplyFilters();
		}

		private async Task LoadAsync()
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();
				var rootPaths = settings.LogRootPaths;

				// Load the cache, then (if writable and directories are configured) discover logs on
				// disk and schedule unparsed ones for background processing — all off the UI thread.
				List<LogRow> rows = await Task.Run(() =>
				{
					IReadOnlyList<LogData> logs = cacheService.Load();

					if (cacheService.Cache is { } cache && rootPaths.Count > 0)
					{
						processingService = new LogProcessingService(cache, apiData);
						logs = processingService.DiscoverAndSchedule(rootPaths);
					}

					return logs
						.OrderByDescending(log => log.EncounterStartTime)
						.Select(log => new LogRow(log, images, nameProvider))
						.ToList();
				});

				ApplyLoadedLogs(rows);

				// Enables the multi-log panel's "Reprocess all" (needs the background processor,
				// which only exists once the cache is writable and directories are configured).
				LogsSection.MultipleDetail.ProcessingService = processingService;

				// Enables the Upload/Cancel buttons in the single- and multi-log detail panels
				// (same precondition as ProcessingService above).
				LogsSection.Detail.UploadProcessor = processingService?.UploadProcessor;
				LogsSection.MultipleDetail.UploadProcessor = processingService?.UploadProcessor;

				// Feed the Services debug tab's live status boxes. Mirrors the Eto ManagerForm
				// wiring its BackgroundProcessorDetail controls once the processors exist.
				if (processingService != null)
				{
					ServicesSection.AttachUploadProcessor(processingService.UploadProcessor);
					ServicesSection.AttachApiProcessor(processingService.ApiProcessor);
					ServicesSection.AttachLogDataProcessor(processingService.LogDataProcessor);
				}

				if (CompressionProcessor is { } compressor)
				{
					ServicesSection.AttachCompressionProcessor(compressor);
				}

				stopwatch.Stop();
				string note = cacheService.StateNote is { } s ? $" {s}" : "";
				LogsSection.StatusText =
					$"Loaded {rows.Count:N0} logs in {stopwatch.ElapsedMilliseconds:N0} ms.{note}";

				// Reflect background processing results incrementally.
				if (processingService != null)
				{
					processingService.LogProcessed += OnLogProcessed;
					processingService.GuildDataUpdated += OnGuildDataUpdated;
					processingRefreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(750) };
					processingRefreshTimer.Tick += (_, _) => DrainProcessed();
					processingRefreshTimer.Start();
				}

				SetupFileSystemWatchers();
			}
			catch (Exception e)
			{
				LogsSection.StatusText = $"Failed to load cache: {e.Message}";
			}
		}

		/// <summary>
		/// Watches the configured log root directories for newly-created files so they show up
		/// immediately, without requiring an app restart — the Avalonia counterpart of the Eto
		/// ManagerForm's <c>SetupFileSystemWatchers</c>. A no-op when there's no writable/processing
		/// cache to add discovered logs to.
		/// </summary>
		private void SetupFileSystemWatchers()
		{
			foreach (var watcher in fileSystemWatchers)
			{
				watcher.Dispose();
			}
			fileSystemWatchers.Clear();

			if (processingService == null)
			{
				return;
			}

			// We do not want to process logs before they are fully written, mid-compression and the
			// like, so we add a delay after detecting one.
			var delay = TimeSpan.FromSeconds(5);
			var temporaryFileDelay = TimeSpan.FromSeconds(15);

			foreach (var directory in settings.LogRootPaths)
			{
				try
				{
					var watcher = new FileSystemWatcher(directory);
					watcher.IncludeSubdirectories = true;
					watcher.Filter = "*";
					watcher.Created += (_, args) =>
					{
						Task.Run(async () =>
						{
							await Task.Delay(logFinder.IsLikelyTemporary(args.FullPath) ? temporaryFileDelay : delay);

							if (File.Exists(args.FullPath) && logFinder.IsLikelyEvtcLog(args.FullPath))
							{
								Dispatcher.UIThread.Post(() => AddNewLog(args.FullPath));
							}
						});
					};
					watcher.Renamed += (_, args) =>
					{
						Task.Run(async () =>
						{
							await Task.Delay(logFinder.IsLikelyTemporary(args.FullPath) ? temporaryFileDelay : delay);

							if (File.Exists(args.FullPath) && logFinder.IsLikelyEvtcLog(args.FullPath))
							{
								Dispatcher.UIThread.Post(() => AddNewLog(args.FullPath));
							}
						});
					};
					watcher.EnableRaisingEvents = true;

					fileSystemWatchers.Add(watcher);
				}
				catch (Exception e)
				{
					Debug.WriteLine($"Failed to set up FileSystemWatcher: {e.Message}");
				}
			}
		}

		private void AddNewLog(string fullName)
		{
			if (processingService == null || allRows.Any(r => r.Log.FileName == fullName))
			{
				return;
			}

			var log = processingService.RegisterDiscoveredLog(fullName);
			var row = new LogRow(log, images, nameProvider);
			InsertRowSorted(row);
			rowByLog[log] = row;
			ApplyFilters();
		}

		/// <summary>
		/// Inserts a row into <see cref="allRows"/> at the position matching its sorted order
		/// (by <see cref="LogData.EncounterStartTime"/>, descending — the same order the initial
		/// load and <see cref="ReloadAsync"/> produce), rather than appending it. The Eto ManagerForm
		/// got this for free from its <c>FilterCollection</c> maintaining a live sort as items were
		/// added; <see cref="allRows"/> is a plain list, so newly-discovered/reprocessed logs need to
		/// be placed explicitly or they'd show up at the very end of the grid instead of the top.
		/// </summary>
		private void InsertRowSorted(LogRow row)
		{
			int index = allRows.FindIndex(r => r.Log.EncounterStartTime < row.Log.EncounterStartTime);
			if (index < 0)
			{
				allRows.Add(row);
			}
			else
			{
				allRows.Insert(index, row);
			}
		}

		/// <summary>
		/// Re-discovers logs from the configured directories (reusing the already-held cache) and
		/// rebuilds the display. Used after cache operations (prune/delete) or a settings change.
		/// </summary>
		public async Task ReloadAsync()
		{
			try
			{
				var rootPaths = settings.LogRootPaths;
				List<LogRow> rows = await Task.Run(() =>
				{
					IReadOnlyList<LogData> logs;
					if (processingService != null && rootPaths.Count > 0)
					{
						logs = processingService.DiscoverAndSchedule(rootPaths);
					}
					else
					{
						logs = cacheService.Cache?.GetAllLogs() ?? (IReadOnlyList<LogData>) Array.Empty<LogData>();
					}

					return logs
						.OrderByDescending(log => log.EncounterStartTime)
						.Select(log => new LogRow(log, images, nameProvider))
						.ToList();
				});

				ApplyLoadedLogs(rows);
				LogsSection.StatusText = $"Reloaded {rows.Count:N0} logs.";
			}
			catch (Exception e)
			{
				LogsSection.StatusText = $"Failed to reload: {e.Message}";
			}
		}

		// Populates the display (rows, encounter tree, tags) from the given projected rows.
		private void ApplyLoadedLogs(List<LogRow> rows)
		{
			allRows = rows;
			rowByLog.Clear();
			foreach (var row in rows)
			{
				rowByLog[row.Log] = row;
			}

			var allLogs = rows.Select(r => r.Log).ToList();
			rootGroup = new Logs.Filters.Groups.RootLogGroup(allLogs, nameProvider);
			Filters.LogGroups = new[] { (Logs.Filters.Groups.LogGroup) rootGroup };
			EncounterGroups.Clear();
			EncounterGroups.Add(new Models.LogGroupNode(rootGroup, images, allLogs));

			AvailableTags.Clear();
			foreach (var tag in rows.SelectMany(r => r.Log.Tags.Select(t => t.Name)).Distinct().OrderBy(t => t))
			{
				var item = new Models.TagFilterItem(tag);
				item.PropertyChanged += (_, args) =>
				{
					if (args.PropertyName == nameof(Models.TagFilterItem.IsSelected))
					{
						UpdateRequiredTags();
					}
				};
				AvailableTags.Add(item);
			}
			ApplyTagSearch();

			loaded = true;
			ApplyFilters();
		}

		/// <summary>Exposes the log cache service (for the Cache dialog).</summary>
		public LogCacheService CacheService => cacheService;

		/// <summary>Exposes the processing service (for the API dialog). Null before load.</summary>
		public LogProcessingService? Processing => processingService;

		/// <summary>The currently loaded logs (for dialogs that operate on them).</summary>
		public IReadOnlyList<LogData> LoadedLogs => allRows.Select(r => r.Log).ToList();

		/// <summary>The shared API (guild) data cache (for the API dialog).</summary>
		public ApiData ApiData => apiData;

		/// <summary>The settings service (for dialogs that bind settings).</summary>
		public ISettingsService SettingsService => settings;

		/// <summary>Exposes the log-name provider (for dialogs that need to display encounter names).</summary>
		public ILogNameProvider NameProvider => nameProvider;

		/// <summary>Exposes the shared image provider (for dialogs that need game icons, e.g. advanced filters).</summary>
		public ImageProvider Images => images;

		/// <summary>
		/// The log-compression processor, created lazily once the cache is writable; null if the
		/// cache is currently read-only (in use by another instance) or has not loaded yet.
		/// </summary>
		public LogCompressionProcessor? CompressionProcessor =>
			compressionProcessor ??= cacheService.Cache is { } cache ? new LogCompressionProcessor(cache) : null;

		/// <summary>
		/// Computes the currently loaded logs that have outdated processed data and would benefit
		/// from reprocessing (for the "logs have outdated data" dialog).
		/// </summary>
		public IReadOnlyList<LogUpdateList> GetLogUpdates() => logDataUpdater.GetUpdates(LoadedLogs).ToList();

		/// <summary>
		/// Checks (once) whether a new program version is available, respecting the
		/// check-for-updates setting and any versions the user chose to ignore.
		/// </summary>
		public async Task<Release?> CheckForProgramUpdateAsync()
		{
			if (!settings.CheckForUpdates)
			{
				return null;
			}

			return await programUpdateChecker.CheckUpdates();
		}

		// Raised on a background thread; just enqueue and let the UI-thread timer coalesce updates.
		private void OnLogProcessed(object? sender, BackgroundProcessorEventArgs<LogData> e)
		{
			processedQueue.Enqueue(e.Item);
		}

		// Raised on a background thread when guild data is fetched; refresh guild names on the UI thread.
		private void OnGuildDataUpdated(object? sender, EventArgs e)
		{
			Dispatcher.UIThread.Post(() =>
			{
				if (loaded)
				{
					var filteredLogs = allRows.Where(row => Filters.FilterLog(row.Log)).Select(r => r.Log).ToList();
					GuildsSection.UpdateFromLogs(filteredLogs);
				}
			});
		}

		private void DrainProcessed()
		{
			bool changed = false;
			while (processedQueue.TryDequeue(out var log))
			{
				var newRow = new LogRow(log, images, nameProvider);
				if (rowByLog.TryGetValue(log, out var oldRow))
				{
					int index = allRows.IndexOf(oldRow);
					if (index >= 0)
					{
						allRows.RemoveAt(index);
					}
				}

				// Re-insert at the sorted position rather than replacing in place: parsing can
				// refine EncounterStartTime from the file's creation-time estimate to the log's
				// actual (precise) encounter start, which can shift where it belongs.
				InsertRowSorted(newRow);

				rowByLog[log] = newRow;
				changed = true;
			}

			if (changed)
			{
				ApplyFilters();
			}

			// Note: ScheduledCount only reflects items still waiting in the queue, not the one
			// currently being processed (it moves out of the queue as soon as it's dequeued) — so it
			// can read 0 while a log is still being worked on. We used to stop this timer once it hit
			// 0, which raced with exactly that case: a single newly-discovered log would get dequeued
			// (ScheduledCount -> 0) before it finished processing, the timer would stop, and the
			// eventual Processed event would enqueue into processedQueue with nothing left to drain
			// it — the row stayed stuck on "Processing..." until the next app restart. The timer now
			// just keeps polling for the lifetime of the session (matching the Eto ManagerForm, which
			// keeps its equivalent LogDataProcessor.Processed subscription for the whole app run);
			// draining an empty queue every 750ms is cheap.
			int remaining = processingService?.ScheduledCount ?? 0;
			if (remaining > 0)
			{
				LogsSection.StatusText = $"Processing logs… {remaining:N0} remaining.";
			}
		}

		/// <summary>Applies the current filters and re-distributes the filtered logs to every section.</summary>
		private void ApplyFilters()
		{
			if (!loaded)
			{
				return;
			}

			var filteredRows = allRows.Where(row => Filters.FilterLog(row.Log)).ToList();
			var filteredLogs = filteredRows.Select(r => r.Log).ToList();

			LogsSection.SetLogs(filteredRows);
			PlayersSection.UpdateFromLogs(filteredLogs);
			GuildsSection.UpdateFromLogs(filteredLogs);
			StatisticsSection.UpdateFromLogs(filteredLogs);
			// Mirrors the Eto GameDataCollecting reading the (filtered) main Logs tab's DataStore.
			GameDataSection.SetLogs(filteredLogs);
			// Deliberately ignores the active filters (matches the Eto ManagerForm's
			// "we want to ignore filters for this tab" comment) — clears would otherwise appear to
			// randomly go missing depending on unrelated filter state, which is more confusing than
			// filters simply not applying to this one tab.
			WeeklyClearsSection.UpdateFromLogs(allRows.Select(r => r.Log));

			if (allRows.Count > 0)
			{
				LogsSection.StatusText = $"Showing {filteredRows.Count:N0} of {allRows.Count:N0} logs.";
			}
		}

		/// <summary>
		/// Permanently deletes the given logs' files (Delete-key flow, Avalonia counterpart of the
		/// Eto <c>LogDeleteDialog</c>), after a confirmation dialog. Prunes them from the cache and
		/// the display.
		/// </summary>
		/// <param name="confirmUnreliableLogs">
		/// Invoked (with the logs to delete) only if some of them have unreliable success
		/// detection (Avalonia counterpart of the Eto <c>UnreliableLogsFoundDialog</c> check);
		/// deletion is aborted unless it returns <see langword="true"/>. The view layer supplies
		/// this so the view model does not need to construct a window itself.
		/// </param>
		public async System.Threading.Tasks.Task DeleteLogsAsync(IReadOnlyList<LogRow> rows, global::Avalonia.Controls.Window owner,
			Func<IReadOnlyList<LogData>, System.Threading.Tasks.Task<bool>>? confirmUnreliableLogs = null)
		{
			if (rows.Count == 0)
			{
				return;
			}

			bool confirmed = await Services.Dialogs.ShowConfirmAsync(owner, "Confirm deletion",
				$"Permanently delete {rows.Count} log file(s)?\n\n" +
				"This cannot be undone and statistics from these logs will be lost.");
			if (!confirmed)
			{
				return;
			}

			var logsToDelete = rows.Select(r => r.Log).ToList();
			if (confirmUnreliableLogs != null && UnreliableLogsWindowViewModel.IsApplicable(logsToDelete))
			{
				bool proceedAnyway = await confirmUnreliableLogs(logsToDelete);
				if (!proceedAnyway)
				{
					return;
				}
			}

			int deleted = 0;
			foreach (var row in rows)
			{
				try
				{
					if (row.Log.FileName is { } fileName && System.IO.File.Exists(fileName))
					{
						System.IO.File.Delete(fileName);
					}

					allRows.Remove(row);
					rowByLog.Remove(row.Log);
					deleted++;
				}
				catch
				{
					// Skip files that cannot be deleted (in use, permissions); keep going.
				}
			}

			// Prune the deleted entries from the writable cache so they do not linger.
			cacheService.Cache?.Prune(allRows.Select(r => r.Log).ToList());

			ApplyFilters();
			LogsSection.StatusText = $"Deleted {deleted:N0} log file(s).";
		}

		/// <summary>Stops background processing, persists pending cache changes and releases the mutex.</summary>
		public void Dispose()
		{
			foreach (var watcher in fileSystemWatchers)
			{
				watcher.Dispose();
			}
			fileSystemWatchers.Clear();

			processingRefreshTimer?.Stop();
			processingService?.Dispose();
			cacheService.Dispose();
			WeeklyClearsSection.Dispose();
			GameDataSection.Dispose();
		}

		partial void OnShowFilterSidebarChanged(bool value)
		{
			settings.ShowFilterSidebar = value;
		}

		partial void OnShowDebugDataChanged(bool value)
		{
			settings.ShowDebugData = value;
		}

		partial void OnShowGuildTagsInLogDetailChanged(bool value)
		{
			settings.ShowGuildTagsInLogDetail = value;
		}

		partial void OnShowFailurePercentagesInLogListChanged(bool value)
		{
			settings.ShowFailurePercentagesInLogList = value;
		}

		partial void OnMinDateChanged(DateTimeOffset? value)
		{
			Filters.MinDateTime = value?.LocalDateTime.Date;
		}

		partial void OnMaxDateChanged(DateTimeOffset? value)
		{
			// Include the whole selected day.
			Filters.MaxDateTime = value?.LocalDateTime.Date.AddDays(1).AddSeconds(-1);
		}

		[RelayCommand]
		private void ClearDates()
		{
			MinDate = null;
			MaxDate = null;
		}

		partial void OnSelectedGroupChanged(Models.LogGroupNode? value)
		{
			// Selecting a node filters to that group's subtree; no selection = all logs.
			var group = value?.Group ?? (Logs.Filters.Groups.LogGroup?) rootGroup;
			if (group != null)
			{
				Filters.LogGroups = new[] { group };
			}
		}

		private void UpdateRequiredTags()
		{
			Filters.RequiredTags = AvailableTags.Where(t => t.IsSelected).Select(t => t.Name).ToList();
		}

		partial void OnTagSearchTextChanged(string value)
		{
			ApplyTagSearch();
		}

		private void ApplyTagSearch()
		{
			foreach (var tag in AvailableTags)
			{
				tag.IsVisible = string.IsNullOrWhiteSpace(TagSearchText) ||
				                tag.Name.Contains(TagSearchText, StringComparison.OrdinalIgnoreCase);
			}
		}
	}
}
