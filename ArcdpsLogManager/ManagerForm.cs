using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Analytics;
using GW2Scratch.ArcdpsLogManager.Commands;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Data;
using GW2Scratch.ArcdpsLogManager.Dialogs;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Properties;
using GW2Scratch.ArcdpsLogManager.Sections;
using GW2Scratch.ArcdpsLogManager.Timing;
using GW2Scratch.ArcdpsLogManager.Uploaders;
using GW2Scratch.EVTCAnalytics;
using Gw2Sharp;

namespace GW2Scratch.ArcdpsLogManager
{
	public sealed class ManagerForm : Form, INotifyPropertyChanged
	{
		private readonly Cooldown gridRefreshCooldown = new Cooldown(TimeSpan.FromSeconds(2));

		private ImageProvider ImageProvider { get; } = new ImageProvider();
		private LogFinder LogFinder { get; } = new LogFinder();

		private LogAnalytics LogAnalytics { get; } =
			new LogAnalytics(new EVTCParser(), new LogProcessor(), new LogAnalyser());

		private ApiData ApiData { get; } = new ApiData(new Gw2Client());
		private UploadProcessor UploadProcessor { get; set; }
		private LogDataProcessor LogDataProcessor { get; set; }

		private ObservableCollection<LogData> logs = new ObservableCollection<LogData>();
		private FilterCollection<LogData> logsFiltered;

		private CancellationTokenSource logLoadTaskTokenSource = null;

		private string status = "";

		private string Status
		{
			get => status;
			set
			{
				if (value == status) return;
				status = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public IEnumerable<LogData> LoadedLogs => logs;
		public LogCache LogCache { get; }

		public ManagerForm(LogCache logCache)
		{
			LogCache = logCache ?? throw new ArgumentNullException(nameof(logCache));

			UploadProcessor = new UploadProcessor(new DpsReportUploader(), LogCache);
			LogDataProcessor = new LogDataProcessor(LogCache, ApiData, LogAnalytics);

			Icon = Resources.GetProgramIcon();
			Title = "arcdps Log Manager";
			ClientSize = new Size(1024, 768);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			Menu = ConstructMenuBar();

			formLayout.BeginVertical(new Padding(5), yscale: true);
			{
				formLayout.Add(ConstructLogFilters());
				formLayout.Add(ConstructMainTabControl(), true);
			}
			formLayout.EndVertical();

			formLayout.BeginVertical(new Padding(5), yscale: false);
			{
				formLayout.Add(ConstructStatusLabel());
			}
			formLayout.EndVertical();

			ApiData.Processed += (sender, args) =>
			{
				bool last = args.CurrentScheduledItems == 0;

				if (last)
				{
					ApiData.SaveDataToFile();
				}
			};

			Shown += (sender, args) => InitializeManager();
			Closing += (sender, args) =>
			{
				if (LogCache?.ChangedSinceLastSave ?? false)
				{
					LogCache?.SaveToFile();
				}

				ApiData?.SaveDataToFile();
			};

			RecreateLogCollections(new ObservableCollection<LogData>(logs));
		}

		private Label ConstructStatusLabel()
		{
			var label = new Label();
			label.TextBinding.Bind(this, x => x.Status);

			return label;
		}

		private LogFilterPanel ConstructLogFilters()
		{
			var filterPanel = new LogFilterPanel(ImageProvider);
			filterPanel.FiltersUpdated += (sender, args) => logsFiltered.Refresh();
			LogCollectionsRecreated += (sender, args) =>
			{
				filterPanel.UpdateLogs(logs);
				logs.CollectionChanged += (s, a) => { filterPanel.UpdateLogs(logs); };

				logsFiltered.Filter = filterPanel.FilterLog;
				logsFiltered.Refresh();
			};
			LogDataProcessor.Processed += (sender, args) =>
			{
				Application.Instance.AsyncInvoke(() => filterPanel.UpdateLogs(logs));
			};

			return filterPanel;
		}

		private MenuBar ConstructMenuBar()
		{
			var logCacheMenuItem = new ButtonMenuItem {Text = "Log &cache"};
			logCacheMenuItem.Click += (sender, args) => { new CacheDialog(this).ShowModal(this); };

			var apiDataMenuItem = new ButtonMenuItem {Text = "&API data"};
			apiDataMenuItem.Click += (sender, args) => { new ApiDialog(ApiData).ShowModal(this); };

			var settingsFormMenuItem = new ButtonMenuItem {Text = "&Settings"};
			settingsFormMenuItem.Click += (sender, args) => { new SettingsForm(this).Show(); };

			var debugDataMenuItem = new CheckMenuItem {Text = "Show &debug data"};
			debugDataMenuItem.Checked = Settings.ShowDebugData;
			debugDataMenuItem.CheckedChanged += (sender, args) =>
			{
				Settings.ShowDebugData = debugDataMenuItem.Checked;
			};

			var showCompositionsMenuItem = new CheckMenuItem {Text = "Show &squad compositions in log list"};
			showCompositionsMenuItem.Checked = Settings.ShowSquadCompositions;
			showCompositionsMenuItem.CheckedChanged += (sender, args) =>
			{
				Settings.ShowSquadCompositions = showCompositionsMenuItem.Checked;
			};

			var showGuildTagsMenuItem = new CheckMenuItem {Text = "Show &guild tags in log details"};
			showGuildTagsMenuItem.Checked = Settings.ShowGuildTagsInLogDetail;
			showGuildTagsMenuItem.CheckedChanged += (sender, args) =>
			{
				Settings.ShowGuildTagsInLogDetail = showGuildTagsMenuItem.Checked;
			};

			// TODO: Implement
			var buildTemplateMenuItem = new ButtonMenuItem {Text = "&Build templates", Enabled = false};
			// TODO: Implement
			var arcdpsSettingsMenuItem = new ButtonMenuItem {Text = "&arcdps settings", Enabled = false};

			var arcdpsMenuItem = new ButtonMenuItem {Text = "&arcdps"};
			arcdpsMenuItem.Items.Add(arcdpsSettingsMenuItem);
			arcdpsMenuItem.Items.Add(buildTemplateMenuItem);

			var dataMenuItem = new ButtonMenuItem {Text = "&Data"};
			dataMenuItem.Items.Add(logCacheMenuItem);
			dataMenuItem.Items.Add(apiDataMenuItem);

			var settingsMenuItem = new ButtonMenuItem {Text = "&Settings"};
			settingsMenuItem.Items.Add(settingsFormMenuItem);
			settingsMenuItem.Items.Add(showCompositionsMenuItem);
			settingsMenuItem.Items.Add(showGuildTagsMenuItem);
			settingsMenuItem.Items.Add(new SeparatorMenuItem());
			settingsMenuItem.Items.Add(debugDataMenuItem);

			var helpMenuItem = new ButtonMenuItem {Text = "Help"};
			helpMenuItem.Items.Add(new About());

			return new MenuBar(arcdpsMenuItem, dataMenuItem, settingsMenuItem, helpMenuItem);
		}

		private TabControl ConstructMainTabControl()
		{
			// Main log list
			var logList = new LogList(LogCache, ApiData, LogAnalytics, UploadProcessor, ImageProvider);
			LogCollectionsRecreated += (sender, args) => logList.DataStore = logsFiltered;

			LogDataProcessor.Processed += (sender, args) =>
			{
				bool last = args.CurrentScheduledItems == 0;

				if (last || gridRefreshCooldown.TryUse(DateTime.Now))
				{
					Application.Instance.AsyncInvoke(() => { logList.ReloadData(); });
				}
			};
			// Player list
			var playerList = new PlayerList(LogCache, ApiData, LogAnalytics, UploadProcessor, ImageProvider);
			FilteredLogsUpdated += (sender, args) => playerList.UpdateDataFromLogs(logsFiltered);

			// Guild list
			var guildList = new GuildList(LogCache, ApiData, LogAnalytics, UploadProcessor, ImageProvider);
			FilteredLogsUpdated += (sender, args) => guildList.UpdateDataFromLogs(logsFiltered);

			// Statistics
			var statistics = new Statistics(logList, ImageProvider);

			// Game data collecting
			var gameDataCollecting = new GameDataCollecting(logList);
			var gameDataPage = new TabPage
			{
				Text = "Game data", Content = gameDataCollecting, Visible = Settings.ShowDebugData
			};
			Settings.ShowDebugDataChanged += (sender, args) => gameDataPage.Visible = Settings.ShowDebugData;

			// Service status
			var serviceStatus = new DynamicLayout {Spacing = new Size(10, 10), Padding = new Padding(5)};
			serviceStatus.AddRow(
				new GroupBox
				{
					Text = "Uploads",
					Content = new BackgroundProcessorDetail {BackgroundProcessor = UploadProcessor}
				},
				new GroupBox
				{
					Text = "Guild Wars 2 API",
					Content = new BackgroundProcessorDetail {BackgroundProcessor = ApiData}
				},
				new GroupBox
				{
					Text = "Log parsing",
					Content = new BackgroundProcessorDetail {BackgroundProcessor = LogDataProcessor}
				},
				null);
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
			Shown += (sender, args) => tabs.SelectedIndex = 0;

			return tabs;
		}

		private void InitializeManager()
		{
			ReloadLogs();

			SetupApiWorker();
		}

		public void ReloadLogs()
		{
			logLoadTaskTokenSource?.Cancel();
			logLoadTaskTokenSource = new CancellationTokenSource();

			logs.Clear();
			Task.Run(() => LoadLogs(logLoadTaskTokenSource.Token));
		}

		private void SetupApiWorker()
		{
			Task.Run(ApiData.LoadDataFromFile).ContinueWith(_ =>
			{
				if (Settings.UseGW2Api)
				{
					ApiData.StartBackgroundTask();
				}

				Settings.UseGW2ApiChanged += (sender, args) =>
				{
					if (Settings.UseGW2Api)
					{
						ApiData.StartBackgroundTask();
					}
					else
					{
						ApiData.StopBackgroundTask();
					}
				};
			});
		}

		// TODO: Do away with this method
		private void LoadLogs(CancellationToken cancellationToken)
		{
			Application.Instance.Invoke(() => { Status = "Finding logs..."; });
			cancellationToken.ThrowIfCancellationRequested();

			FindLogs(cancellationToken);

			Application.Instance.Invoke(() => { Status = "Saving cache..."; });
			cancellationToken.ThrowIfCancellationRequested();

			if (LogCache.ChangedSinceLastSave)
			{
				LogCache.SaveToFile();
			}

			Application.Instance.AsyncInvoke(() => { Status = $"{logs.Count} logs found."; });
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
				var newLogs = new ObservableCollection<LogData>(logs);

				//foreach (var log in LogFinder.GetTesting())
				foreach (var log in Settings.LogRootPaths.SelectMany(x => LogFinder.GetFromDirectory(x, LogCache)))
				{
					newLogs.Add(log);

					if (log.ParsingStatus == ParsingStatus.Parsed)
					{
						ApiData.RegisterLog(log);
					}
					else
					{
						LogDataProcessor.Schedule(log);
					}

					cancellationToken.ThrowIfCancellationRequested();
				}

				Application.Instance.Invoke(() => { RecreateLogCollections(newLogs); });
			}
			catch (Exception e) when (!(e is OperationCanceledException))
			{
				Application.Instance.Invoke(() =>
				{
					MessageBox.Show(this, $"Logs could not be found.\nReason: {e.Message}", "Log Discovery Error",
						MessageBoxType.Error);
				});
			}
		}

		/// <summary>
		/// Recreate log collections, this is significantly faster than updating the old one.
		/// </summary>
		private void RecreateLogCollections(ObservableCollection<LogData> newLogCollection)
		{
			logs = newLogCollection;
			logsFiltered = new FilterCollection<LogData>(logs);
			logsFiltered.CollectionChanged += (sender, args) => FilteredLogsUpdated?.Invoke(this, EventArgs.Empty);
			LogCollectionsRecreated?.Invoke(this, EventArgs.Empty);
		}

		private event EventHandler LogCollectionsRecreated;
		private event EventHandler FilteredLogsUpdated;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}