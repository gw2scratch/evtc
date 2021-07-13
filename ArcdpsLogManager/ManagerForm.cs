using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Analytics;
using GW2Scratch.ArcdpsLogManager.Collections;
using GW2Scratch.ArcdpsLogManager.Commands;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Controls.Filters;
using GW2Scratch.ArcdpsLogManager.Dialogs;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Caching;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Logs.Updates;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Sections;
using GW2Scratch.ArcdpsLogManager.Timing;
using GW2Scratch.ArcdpsLogManager.Updates;
using GW2Scratch.ArcdpsLogManager.Uploads;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Processing;
using Gw2Sharp;

namespace GW2Scratch.ArcdpsLogManager
{

	public sealed class ManagerForm : Form
	{
		private static readonly TimeSpan LogCacheAutoSavePeriod = TimeSpan.FromSeconds(60);
		private readonly Cooldown gridRefreshCooldown = new Cooldown(TimeSpan.FromSeconds(5));
		private readonly Cooldown filterRefreshCooldown = new Cooldown(TimeSpan.FromSeconds(5));

		private ProgramUpdateChecker ProgramUpdateChecker { get; } = new ProgramUpdateChecker("http://gw2scratch.com/releases/manager.json");
		private ImageProvider ImageProvider { get; } = new ImageProvider();
		private LogFinder LogFinder { get; } = new LogFinder();

		private LogAnalytics LogAnalytics { get; } =
			new LogAnalytics(new EVTCParser(), new LogProcessor(), log => new LogAnalyzer(log, null));

		private ApiProcessor ApiProcessor { get; }
		private UploadProcessor UploadProcessor { get; }
		private LogDataProcessor LogDataProcessor { get; }
		private ILogNameProvider LogNameProvider { get; }
		private LogDataUpdater LogDataUpdater { get; } = new LogDataUpdater();
		private LogCacheAutoSaver LogCacheAutoSaver { get; }

		private readonly BulkObservableCollection<LogData> logs = new BulkObservableCollection<LogData>();
		private readonly FilterCollection<LogData> logsFiltered;

		private CancellationTokenSource logLoadTaskTokenSource = null;

		public IEnumerable<LogData> LoadedLogs => logs;
		public LogCache LogCache { get; }
		private ApiData ApiData { get; }
		private LogFilters Filters { get; }

		private readonly List<FileSystemWatcher> fileSystemWatchers = new List<FileSystemWatcher>();

		public ManagerForm(LogCache logCache, ApiData apiData)
		{
			LogCache = logCache ?? throw new ArgumentNullException(nameof(logCache));
			ApiData = apiData ?? throw new ArgumentNullException(nameof(apiData));

			// Background processors
			var dpsReportUploader = new DpsReportUploader();
			UploadProcessor = new UploadProcessor(dpsReportUploader, LogCache);
			ApiProcessor = new ApiProcessor(ApiData, new Gw2Client());
			LogDataProcessor = new LogDataProcessor(LogCache, ApiProcessor, LogAnalytics);
			LogNameProvider = new TranslatedLogNameProvider(GameLanguage.English);
			LogCacheAutoSaver = LogCacheAutoSaver.StartNew(logCache, LogCacheAutoSavePeriod);

			LogDataProcessor.StoppingWithError += (sender, args) =>
			{
				Application.Instance.InvokeAsync(() => MessageBox.Show(this,
					$"The background processor for logs has failed critically. " +
					$"Please report the following error:\n\nException: {args.Exception}", "Error", MessageBoxType.Error));
			};
			
			ApiProcessor.StoppingWithError += (sender, args) =>
			{
				Application.Instance.InvokeAsync(() => MessageBox.Show(this,
					$"The background processor for API requests has failed critically. " +
					$"Please report the following error:\n\nException: {args.Exception}", "Error", MessageBoxType.Error));
			};
			
			UploadProcessor.StoppingWithError += (sender, args) =>
			{
				Application.Instance.InvokeAsync(() => MessageBox.Show(this,
					$"The background processor for log uploads has failed critically. " +
					$"Please report the following error:\n\nException: {args.Exception}", "Error", MessageBoxType.Error));
			};

			Filters = new LogFilters(new SettingsFilters());
			Filters.PropertyChanged += (sender, args) => logsFiltered.Refresh();

			if (Settings.UseGW2Api)
			{
				ApiProcessor.StartBackgroundTask();
			}

			Settings.UseGW2ApiChanged += (sender, args) =>
			{
				if (Settings.UseGW2Api)
				{
					ApiProcessor.StartBackgroundTask();
				}
				else
				{
					ApiProcessor.StopBackgroundTask();
				}
			};

			Settings.DpsReportDomainChanged += (sender, args) => { dpsReportUploader.Domain = Settings.DpsReportDomain; };

			// Form layout
			Icon = Resources.GetProgramIcon();
			Title = "arcdps Log Manager";
			ClientSize = new Size(1300, 768);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			Menu = ConstructMenuBar();

			formLayout.BeginVertical(new Padding(5), yscale: false);
			{
				formLayout.Add(ConstructMainSplitter(), yscale: true);
				formLayout.Add(ConstructStatusPanel());
			}
			formLayout.EndVertical();

			// Event handlers
			ApiProcessor.Processed += (sender, args) =>
			{
				bool last = args.CurrentScheduledItems == 0;

				if (last)
				{
					ApiData.SaveDataToFile();
				}
			};

			Settings.LogRootPathChanged += (sender, args) => Application.Instance.Invoke(ReloadLogs);

			Closing += (sender, args) =>
			{
				if (LogCache?.ChangedSinceLastSave ?? false)
				{
					LogCache?.SaveToFile();
				}

				ApiData?.SaveDataToFile();
			};
			LogSearchFinished += (sender, args) =>
			{
				var updates = LogDataUpdater.GetUpdates(logs).ToList();
				if (updates.Count > 0)
				{
					new ProcessingUpdateDialog(LogDataProcessor, updates).ShowModal(this);
				}
			};

			// Collection initialization
			logsFiltered = new FilterCollection<LogData>(logs);
			logsFiltered.CollectionChanged += (sender, args) => FilteredLogsUpdated?.Invoke(this, EventArgs.Empty);
			logsFiltered.Filter = Filters.FilterLog;
			LogCollectionsInitialized?.Invoke(this, EventArgs.Empty);
			LogDataProcessor.Processed += (sender, args) =>
			{
				bool last = args.CurrentScheduledItems == 0;
				if (last)
				{
					Application.Instance.AsyncInvoke(logsFiltered.Refresh);
				}
			};

			Shown += (sender, args) => ReloadLogs();
			Shown += (sender, args) => CheckUpdates();
		}

		private void CheckUpdates()
		{
			if (!Settings.CheckForUpdates)
			{
				return;
			}

			Task.Run(ProgramUpdateChecker.CheckUpdates).ContinueWith(t =>
			{
				var release = t.Result;
				if (release != null)
				{
					Application.Instance.Invoke(() => new ProgramUpdateDialog(release).ShowModal(this));
				}
			});
		}

		private Splitter ConstructMainSplitter()
		{
			var filters = ConstructLogFilters();
			var tabs = ConstructMainTabControl();

			var sidebar = new Panel {Content = filters, Padding = new Padding(0, 0, 4, 2)};
			var filterPage = new TabPage {Text = "Filters", Visible = false};
			tabs.Pages.Insert(0, filterPage);

			var mainSplitter = new Splitter
			{
				Orientation = Orientation.Horizontal,
				Panel1 = sidebar,
				Panel2 = tabs,
				Position = 320
			};

			// This is a workaround for an Eto 2.4 bug.
			// See comment below when value is checked.
			bool updatingSidebar = true;
			void UpdateSidebarFromSetting()
			{
				updatingSidebar = true;
				// The filters from the sidebar are moved into
				// their own tab when the sidebar is collapsed
				if (Settings.ShowFilterSidebar)
				{
					filterPage.Content = null;
					filterPage.Visible = false;
					sidebar.Content = filters;
					// This may be called when enlarging the sidebar, we don't want to change the size in that case.
					if (mainSplitter.Position == 0)
					{
						mainSplitter.Position = 300;
					}
				}
				else
				{
					sidebar.Content = null;
					mainSplitter.Position = 0;
					filterPage.Content = filters;
					filterPage.Visible = true;
				}

				updatingSidebar = false;
			}

			UpdateSidebarFromSetting();
			Settings.ShowFilterSidebarChanged += (sender, args) => UpdateSidebarFromSetting();
			Settings.LogRootPathChanged += (sender, args) => SetupFileSystemWatchers();

			mainSplitter.PositionChanged += (sender, args) =>
			{
				if (updatingSidebar)
				{
					// This is a workaround for an Eto 2.4 bug on WPF where setting mainSplitter.Position = 300
					// invokes this event while Position is still set to 0. This results in an infinite loop and
					// ultimately a stack-overflow exception.
					// TODO: Remove on Eto 2.5 if fixed.
					return;
				}

				if (mainSplitter.Position <= 10)
				{
					Settings.ShowFilterSidebar = false;
				}
				else
				{
					Settings.ShowFilterSidebar = true;
				}
			};

			return mainSplitter;
		}

		private DynamicLayout ConstructStatusPanel()
		{
			// Processing label
			var processingLabel = new Label();

			void UpdatingProcessingLabel(object sender, BackgroundProcessorEventArgs args)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					bool finished = args.CurrentScheduledItems == 0;
					int logCount = logs.Count;
					// The log count check is to prevent a log count of 0 being shown when logs are being loaded and
					// log processing catches up with searching for logs.
					processingLabel.Text = finished && logCount > 0
						? $"{logCount} logs found."
						: $"Processing logs {args.TotalProcessedItems}/{args.TotalScheduledItems}...";
				});
			}

			LogDataProcessor.Processed += UpdatingProcessingLabel;
			LogDataProcessor.Scheduled += UpdatingProcessingLabel;
			LogDataProcessor.Unscheduled += UpdatingProcessingLabel;
			LogSearchStarted += (sender, args) => processingLabel.Text = "Finding logs...";
			LogSearchFinished += (sender, args) => processingLabel.Text = $"{logs.Count} logs found.";

			// Upload state label
			var uploadLabel = new Label();

			void UpdateUploadLabel(object sender, BackgroundProcessorEventArgs args)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					bool finished = args.CurrentScheduledItems == 0;
					uploadLabel.Text = finished
						? ""
						: $"Uploading {args.TotalProcessedItems}/{args.TotalScheduledItems}...";
				});
			}

			UploadProcessor.Processed += UpdateUploadLabel;
			UploadProcessor.Scheduled += UpdateUploadLabel;
			UploadProcessor.Unscheduled += UpdateUploadLabel;

			// API state label
			var apiLabel = new Label();

			void UpdateApiLabel(object sender, BackgroundProcessorEventArgs args)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					bool finished = args.CurrentScheduledItems == 0;
					apiLabel.Text = finished ? "" : $"Guilds (GW2 API) {args.TotalProcessedItems}/{args.TotalScheduledItems}...";
				});
			}

			ApiProcessor.Processed += UpdateApiLabel;
			ApiProcessor.Scheduled += UpdateApiLabel;
			ApiProcessor.Unscheduled += UpdateApiLabel;

			// Layout of the status bar
			var layout = new DynamicLayout();
			layout.BeginHorizontal();
			{
				layout.Add(processingLabel, xscale: true);
				layout.Add(uploadLabel, xscale: true);
				layout.Add(apiLabel, xscale: true);
			}
			layout.EndHorizontal();

			return layout;
		}

		private LogFilterPanel ConstructLogFilters()
		{
			var filterPanel = new LogFilterPanel(ImageProvider, Filters);
			LogCollectionsInitialized += (sender, args) => logs.CollectionChanged += (s, a) => { filterPanel.UpdateLogs(logs); };
			LogDataProcessor.Processed += (sender, args) =>
			{
				bool last = args.CurrentScheduledItems == 0;

				if (last || filterRefreshCooldown.TryUse(DateTime.Now))
				{
					Application.Instance.AsyncInvoke(() => filterPanel.UpdateLogs(logs));
				}
			};

			return filterPanel;
		}

		private MenuBar ConstructMenuBar()
		{
			var updateMenuItem = new ButtonMenuItem {Text = "&Update logs with outdated data…"};
			updateMenuItem.Click += (sender, args) =>
			{
				new ProcessingUpdateDialog(LogDataProcessor, LogDataUpdater.GetUpdates(logs).ToList()).ShowModal(this);
			};
			LogSearchFinished += (sender, args) => { updateMenuItem.Enabled = LogDataUpdater.GetUpdates(logs).Any(); };
			LogDataProcessor.Processed += (sender, args) =>
			{
				if (args.CurrentScheduledItems == 0)
				{
					bool updatesFound = LogDataUpdater.GetUpdates(logs).Any();
					Application.Instance.AsyncInvoke(() => updateMenuItem.Enabled = updatesFound);
				}
			};

			var logCacheMenuItem = new ButtonMenuItem {Text = "&Log cache"};
			logCacheMenuItem.Click += (sender, args) => { new CacheDialog(this).ShowModal(this); };

			var apiDataMenuItem = new ButtonMenuItem {Text = "&API cache"};
			apiDataMenuItem.Click += (sender, args) => { new ApiDialog(ApiProcessor).ShowModal(this); };

			var settingsFormMenuItem = new ButtonMenuItem {Text = "&Settings"};
			settingsFormMenuItem.Click += (sender, args) => { new SettingsForm().Show(); };

			var debugDataMenuItem = new CheckMenuItem {Text = "Show &debug data"};
			debugDataMenuItem.Checked = Settings.ShowDebugData;
			debugDataMenuItem.CheckedChanged += (sender, args) => { Settings.ShowDebugData = debugDataMenuItem.Checked; };

			var showGuildTagsMenuItem = new CheckMenuItem {Text = "Show &guild tags in log details"};
			showGuildTagsMenuItem.Checked = Settings.ShowGuildTagsInLogDetail;
			showGuildTagsMenuItem.CheckedChanged += (sender, args) => { Settings.ShowGuildTagsInLogDetail = showGuildTagsMenuItem.Checked; };

			var showFailurePercentagesMenuItem = new CheckMenuItem {Text = "Show failure health &percentages in log list"};
			showFailurePercentagesMenuItem.Checked = Settings.ShowFailurePercentagesInLogList;
			showFailurePercentagesMenuItem.CheckedChanged += (sender, args) =>
			{
				Settings.ShowFailurePercentagesInLogList = showFailurePercentagesMenuItem.Checked;
			};

			var showSidebarMenuItem = new CheckMenuItem {Text = "Show &filters in a sidebar"};
			showSidebarMenuItem.Checked = Settings.ShowFilterSidebar;
			Settings.ShowFilterSidebarChanged += (sender, args) => showSidebarMenuItem.Checked = Settings.ShowFilterSidebar;
			showSidebarMenuItem.CheckedChanged += (sender, args) => { Settings.ShowFilterSidebar = showSidebarMenuItem.Checked; };

			// TODO: Implement
			/*
			var arcdpsSettingsMenuItem = new ButtonMenuItem {Text = "&arcdps settings", Enabled = false};

			var arcdpsMenuItem = new ButtonMenuItem {Text = "&arcdps", Enabled = false};
			arcdpsMenuItem.Items.Add(arcdpsSettingsMenuItem);
			*/

			var dataMenuItem = new ButtonMenuItem {Text = "&Data"};
			dataMenuItem.Items.Add(updateMenuItem);
			dataMenuItem.Items.Add(new SeparatorMenuItem());
			dataMenuItem.Items.Add(logCacheMenuItem);
			dataMenuItem.Items.Add(apiDataMenuItem);

			var viewMenuItem = new ButtonMenuItem {Text = "&View"};
			viewMenuItem.Items.Add(showSidebarMenuItem);
			viewMenuItem.Items.Add(showGuildTagsMenuItem);
			viewMenuItem.Items.Add(showFailurePercentagesMenuItem);
			viewMenuItem.Items.Add(new SeparatorMenuItem());
			viewMenuItem.Items.Add(debugDataMenuItem);

			var settingsMenuItem = new ButtonMenuItem {Text = "&Settings"};
			settingsMenuItem.Items.Add(settingsFormMenuItem);

			var helpMenuItem = new ButtonMenuItem {Text = "Help"};
			helpMenuItem.Items.Add(new About());

			return new MenuBar(dataMenuItem, viewMenuItem, settingsMenuItem, helpMenuItem);
		}

		private TabControl ConstructMainTabControl()
		{
			// Main log list
			var logList = new LogList(LogCache, ApiData, LogDataProcessor, UploadProcessor, ImageProvider, LogNameProvider);
			LogCollectionsInitialized += (sender, args) => logList.DataStore = logsFiltered;

			LogDataProcessor.Processed += (sender, args) =>
			{
				bool last = args.CurrentScheduledItems == 0;

				if (last || gridRefreshCooldown.TryUse(DateTime.Now))
				{
					Application.Instance.AsyncInvoke(() => { logList.ReloadData(); });
				}
			};
			// Player list
			var playerList = new PlayerList(LogCache, ApiData, LogDataProcessor, UploadProcessor, ImageProvider, LogNameProvider);
			FilteredLogsUpdated += (sender, args) => playerList.UpdateDataFromLogs(logsFiltered);

			// Guild list
			var guildList = new GuildList(LogCache, ApiData, LogDataProcessor, UploadProcessor, ImageProvider, LogNameProvider);
			FilteredLogsUpdated += (sender, args) => guildList.UpdateDataFromLogs(logsFiltered);

			// Statistics
			var statistics = new StatisticsSection(ImageProvider, LogNameProvider);
			FilteredLogsUpdated += (sender, args) => statistics.UpdateDataFromLogs(logsFiltered.ToList());

			// Game data collecting
			var gameDataCollecting = new GameDataCollecting(logList, LogCache, ApiData, LogDataProcessor, UploadProcessor, ImageProvider, LogNameProvider);
			var gameDataPage = new TabPage
			{
				Text = "Game data", Content = gameDataCollecting, Visible = Settings.ShowDebugData
			};
			Settings.ShowDebugDataChanged += (sender, args) => gameDataPage.Visible = Settings.ShowDebugData;

			// Service status
			var serviceStatus = new DynamicLayout {Spacing = new Size(10, 10), Padding = new Padding(5)};
			serviceStatus.BeginHorizontal();
			{
				serviceStatus.Add(new GroupBox
				{
					Text = "Uploads",
					Content = new BackgroundProcessorDetail {BackgroundProcessor = UploadProcessor}
				}, xscale: true);
				serviceStatus.Add(new GroupBox
				{
					Text = "Guild Wars 2 API",
					Content = new BackgroundProcessorDetail {BackgroundProcessor = ApiProcessor}
				}, xscale: true);
				serviceStatus.Add(new GroupBox
				{
					Text = "Log parsing",
					Content = new BackgroundProcessorDetail {BackgroundProcessor = LogDataProcessor}
				}, xscale: true);
			}
			serviceStatus.AddRow(null);
			var servicePage = new TabPage
			{
				Text = "Services", Content = serviceStatus, Visible = Settings.ShowDebugData
			};
			Settings.ShowDebugDataChanged += (sender, args) => servicePage.Visible = Settings.ShowDebugData;

			var tabs = new TabControl();
			tabs.Pages.Add(new TabPage {Text = "Logs", Content = logList});
			tabs.Pages.Add(new TabPage {Text = "Players", Content = playerList});
			tabs.Pages.Add(new TabPage {Text = "Guilds", Content = guildList});
			tabs.Pages.Add(new TabPage {Text = "Statistics", Content = statistics});
			tabs.Pages.Add(gameDataPage);
			tabs.Pages.Add(servicePage);

			// This is needed to avoid a Gtk platform issue where the tab is changed to the last one.
			Shown += (sender, args) => tabs.SelectedIndex = 1;

			return tabs;
		}

		public void ReloadLogs()
		{
			logLoadTaskTokenSource?.Cancel();
			logLoadTaskTokenSource = new CancellationTokenSource();

			logs.Clear();
			LogSearchStarted?.Invoke(this, EventArgs.Empty);
			Task.Run(() => FindLogs(logLoadTaskTokenSource.Token))
				.ContinueWith(t => Application.Instance.Invoke(() => LogSearchFinished?.Invoke(null, EventArgs.Empty)));

			SetupFileSystemWatchers();
		}


		private void SetupFileSystemWatchers()
		{
			foreach (var watcher in fileSystemWatchers)
			{
				watcher.Dispose();
			}

			// We do not want to process logs before they are fully written,
			// mid-compression and the like, so we add a delay after detecting one.
			var delay = TimeSpan.FromSeconds(5);

			fileSystemWatchers.Clear();
			foreach (var directory in Settings.LogRootPaths)
			{
				try
				{
					var watcher = new FileSystemWatcher(directory);
					watcher.IncludeSubdirectories = true;
					watcher.Filter = "*";
					watcher.Created += (sender, args) =>
					{
						Task.Run(async () =>
						{
							await Task.Delay(delay);
							if (LogFinder.IsLikelyEvtcLog(args.FullPath))
							{
								Application.Instance.AsyncInvoke(() => AddNewLog(args.FullPath));
							}
						});
					};
					watcher.Renamed += (sender, args) =>
					{
						Task.Run(async () =>
						{
							await Task.Delay(delay);
							if (LogFinder.IsLikelyEvtcLog(args.FullPath))
							{
								Application.Instance.AsyncInvoke(() => AddNewLog(args.FullPath));
							}
						});
					};
					watcher.EnableRaisingEvents = true;

					fileSystemWatchers.Add(watcher);
				}
				catch (Exception e)
				{
					// TODO: Replace with proper logging
					Console.Error.WriteLine($"Failed to set up FileSystemWatcher: {e.Message}");
				}
			}
		}

		private void AddNewLog(string fullName)
		{
			if (logs.Any(x => x.FileInfo.FullName == fullName))
			{
				return;
			}

			if (!LogCache.TryGetLogData(fullName, out var log))
			{
				log = new LogData(new FileInfo(fullName));
			}

			if (log.ParsingStatus != ParsingStatus.Parsed)
			{
				LogDataProcessor.Schedule(log);
			}

			logs.Add(log);
		}

		/// <summary>
		/// Discover logs and process them.
		/// </summary>
		private void FindLogs(CancellationToken cancellationToken)
		{
			LogDataProcessor.UnscheduleAll();

			// TODO: Fix the counters being off if a log is currently being processed
			LogDataProcessor.ResetTotalCounters();
			try
			{
				var newLogs = new List<LogData>();

				//foreach (var log in LogFinder.GetTesting())
				foreach (var log in Settings.LogRootPaths.SelectMany(x => LogFinder.GetFromDirectory(x, LogCache)))
				{
					newLogs.Add(log);

					if (log.ParsingStatus == ParsingStatus.Parsed)
					{
						ApiProcessor.RegisterLog(log);
					}
					else
					{
						LogDataProcessor.Schedule(log);
					}

					cancellationToken.ThrowIfCancellationRequested();
				}

				Application.Instance.Invoke(() => { logs.AddRange(newLogs); });
			}
			catch (Exception e) when (!(e is OperationCanceledException))
			{
				Application.Instance.Invoke(() =>
				{
					MessageBox.Show(this, $"Logs could not be found.\nReason: {e.Message}", "Log Discovery Error",
						MessageBoxType.Error);
				});
			}

			if (LogCache.ChangedSinceLastSave)
			{
				LogCache.SaveToFile();
			}
		}

		private event EventHandler LogSearchStarted;
		private event EventHandler LogSearchFinished;
		private event EventHandler FilteredLogsUpdated;
		private event EventHandler LogCollectionsInitialized;
	}
}