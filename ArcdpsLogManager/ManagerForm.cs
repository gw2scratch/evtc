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
using GW2Scratch.ArcdpsLogManager.Data;
using GW2Scratch.ArcdpsLogManager.GW2Api.V2;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Properties;
using GW2Scratch.ArcdpsLogManager.Sections;
using GW2Scratch.ArcdpsLogManager.Sections.Guilds;
using GW2Scratch.ArcdpsLogManager.Sections.Players;
using GW2Scratch.ArcdpsLogManager.Timing;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager
{
	public sealed class ManagerForm : Form, INotifyPropertyChanged
	{
		private static readonly DateTime GuildWars2ReleaseDate = new DateTime(2012, 8, 28);

		private readonly Cooldown gridRefreshCooldown = new Cooldown(TimeSpan.FromSeconds(2));

		private ImageProvider ImageProvider { get; } = new ImageProvider();
		private LogFinder LogFinder { get; } = new LogFinder();
		private EVTCParser EVTCParser { get; } = new EVTCParser();
		private LogProcessor LogProcessor { get; } = new LogProcessor();
		private StatisticsCalculator StatisticsCalculator { get; } = new StatisticsCalculator();

		private ApiData ApiData { get; } = new ApiData(new GuildEndpoint());

		private ObservableCollection<LogData> logs = new ObservableCollection<LogData>();
		private FilterCollection<LogData> logsFiltered;

		private readonly DropDown encounterFilterDropDown;
		private readonly LogList mainLogList;
		private readonly PlayerList playerList;
		private readonly GuildList guildList;

		private const string EncounterFilterAll = "All";
		private string EncounterFilter { get; set; } = EncounterFilterAll;

		private bool ShowSuccessfulLogs { get; set; } = true;
		private bool ShowFailedLogs { get; set; } = true;
		private bool ShowUnknownLogs { get; set; } = true;

		internal bool ShowParseUnparsedLogs { get; set; } = true;
		internal bool ShowParseParsingLogs { get; set; } = true;
		internal bool ShowParseParsedLogs { get; set; } = true;
		internal bool ShowParseFailedLogs { get; set; } = true;

		private DateTime? MinDateTimeFilter { get; set; } = GuildWars2ReleaseDate;
		private DateTime? MaxDateTimeFilter { get; set; } = DateTime.Now.Date.AddDays(1);

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

		public event EventHandler CacheLoaded;
		public event PropertyChangedEventHandler PropertyChanged;

		public IEnumerable<LogData> LoadedLogs => logs;
		public LogCache LogCache { get; private set; }

		public ManagerForm()
		{
			Title = "arcdps Log Manager";
			ClientSize = new Size(900, 700);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			Closing += (sender, args) =>
			{
				if (LogCache?.ChangedSinceLastSave ?? false)
				{
					LogCache?.SaveToFile();
				}
			};

			var logCacheMenuItem = new ButtonMenuItem {Text = "Log &cache"};
			logCacheMenuItem.Click += (sender, args) => { new CacheDialog(this).ShowModal(this); };

			var logLocationMenuItem = new ButtonMenuItem {Text = "&Log location"};
			logLocationMenuItem.Click += (sender, args) => { new LogSettingsDialog(this).ShowModal(this); };
			logLocationMenuItem.Shortcut = Application.Instance.CommonModifier | Keys.L;

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

			// TODO: Implement
			var buildTemplateMenuItem = new ButtonMenuItem {Text = "&Build templates", Enabled = false};
			// TODO: Implement
			var arcdpsSettingsMenuItem = new ButtonMenuItem {Text = "&arcdps settings", Enabled = false};

			var arcdpsMenuItem = new ButtonMenuItem {Text = "&arcdps"};
			arcdpsMenuItem.Items.Add(arcdpsSettingsMenuItem);
			arcdpsMenuItem.Items.Add(buildTemplateMenuItem);

			var dataMenuItem = new ButtonMenuItem {Text = "&Data"};
			dataMenuItem.Items.Add(logCacheMenuItem);

			var settingsMenuItem = new ButtonMenuItem {Text = "&Settings"};
			settingsMenuItem.Items.Add(logLocationMenuItem);
			settingsMenuItem.Items.Add(showCompositionsMenuItem);
			settingsMenuItem.Items.Add(new SeparatorMenuItem());
			settingsMenuItem.Items.Add(debugDataMenuItem);

			Menu = new MenuBar(arcdpsMenuItem, dataMenuItem, settingsMenuItem);

			formLayout.BeginVertical(new Padding(5), yscale: true);

			encounterFilterDropDown = new DropDown();
			UpdateFilterDropdown();
			encounterFilterDropDown.SelectedKey = EncounterFilterAll;
			encounterFilterDropDown.SelectedKeyBinding.Bind(this, x => x.EncounterFilter);

			var successCheckBox = new CheckBox {Text = "Success"};
			successCheckBox.CheckedBinding.Bind(this, x => x.ShowSuccessfulLogs);
			var failureCheckBox = new CheckBox {Text = "Failure"};
			failureCheckBox.CheckedBinding.Bind(this, x => x.ShowFailedLogs);
			var unknownCheckBox = new CheckBox {Text = "Unknown"};
			unknownCheckBox.CheckedBinding.Bind(this, x => x.ShowUnknownLogs);

			var startDateTimePicker = new DateTimePicker {Mode = DateTimePickerMode.DateTime};
			startDateTimePicker.ValueBinding.Bind(this, x => x.MinDateTimeFilter);
			var endDateTimePicker = new DateTimePicker {Mode = DateTimePickerMode.DateTime};
			endDateTimePicker.ValueBinding.Bind(this, x => x.MaxDateTimeFilter);

			var lastDayButton = new Button {Text = "Last day"};
			lastDayButton.Click += (sender, args) =>
			{
				startDateTimePicker.Value = DateTime.Now - TimeSpan.FromDays(1);
				endDateTimePicker.Value = DateTime.Now;
			};

			var allTimeButton = new Button {Text = "All time"};
			allTimeButton.Click += (sender, args) =>
			{
				startDateTimePicker.Value = GuildWars2ReleaseDate;
				endDateTimePicker.Value = DateTime.Now;
			};

			var advancedFiltersButton = new Button {Text = "Advanced"};
			advancedFiltersButton.Click += (sender, args) =>
			{
				var form = new Form
				{
					Title = "Advanced filters - arcdps Log Manager",
					Content = new AdvancedFilters(this, ImageProvider)
				};
				form.Show();
			};

			var applyFilterButton = new Button {Text = "Apply"};
			applyFilterButton.Click += (sender, args) => { logsFiltered.Refresh(); };

			formLayout.BeginGroup("Filters", new Padding(5));
			formLayout.BeginHorizontal();

			formLayout.BeginVertical();
			{
				formLayout.BeginVertical(new Padding(5), new Size(4, 0));
				{
					formLayout.BeginHorizontal();
					{
						formLayout.Add(new Label {Text = "Encounter", VerticalAlignment = VerticalAlignment.Center});
						formLayout.Add(encounterFilterDropDown);
						formLayout.Add(new Label {Text = "Result", VerticalAlignment = VerticalAlignment.Center});
						formLayout.Add(successCheckBox);
						formLayout.Add(failureCheckBox);
						formLayout.Add(unknownCheckBox);
					}
					formLayout.EndHorizontal();
				}
				formLayout.EndBeginVertical(new Padding(5), new Size(4, 0));
				{
					formLayout.BeginHorizontal();
					{
						formLayout.Add(
							new Label {Text = "Encounter time between", VerticalAlignment = VerticalAlignment.Center});
						formLayout.Add(startDateTimePicker);
						formLayout.Add(new Label {Text = "and", VerticalAlignment = VerticalAlignment.Center});
						formLayout.Add(endDateTimePicker);
						formLayout.Add(lastDayButton);
						formLayout.Add(allTimeButton);
					}
					formLayout.EndHorizontal();
				}
				formLayout.EndVertical();
			}
			formLayout.EndVertical();

			formLayout.Add(null, true);

			formLayout.BeginVertical(new Padding(5), new Size(0, 5));
			{
				formLayout.Add(advancedFiltersButton);
				formLayout.Add(null, true);
				formLayout.Add(applyFilterButton);
			}
			formLayout.EndVertical();

			formLayout.EndHorizontal();
			formLayout.EndGroup();

			var tabs = new TabControl();

			// Log tab
			mainLogList = new LogList(ImageProvider);
			tabs.Pages.Add(new TabPage {Text = "Logs", Content = mainLogList});

			// Player tab
			playerList = new PlayerList(ImageProvider);
			tabs.Pages.Add(new TabPage {Text = "Players", Content = playerList});

			// Guild tab
			guildList = new GuildList(ImageProvider, ApiData);
			tabs.Pages.Add(new TabPage {Text = "Guilds", Content = guildList});

			// Statistics tab
			var statistics = new Statistics(mainLogList, ImageProvider);
			tabs.Pages.Add(new TabPage {Text = "Statistics", Content = statistics});

			// Game data collecting tab
			var gameDataCollecting = new GameDataCollecting(mainLogList);
			var gameDataPage = new TabPage
			{
				Text = "Game data", Content = gameDataCollecting, Visible = Settings.ShowDebugData
			};
			Settings.ShowDebugDataChanged += (sender, args) => gameDataPage.Visible = Settings.ShowDebugData;
			tabs.Pages.Add(gameDataPage);

			formLayout.Add(tabs, true);

			// This is needed to avoid a Gtk3 platform issue where the tab is changed to the game data one.
			Shown += (sender, args) => tabs.SelectedIndex = 0;

			formLayout.EndVertical();

			var statusLabel = new Label();
			statusLabel.TextBinding.Bind(this, x => x.Status);

			formLayout.BeginVertical(new Padding(5), yscale: false);
			{
				formLayout.Add(statusLabel);
			}
			formLayout.EndVertical();

			RecreateLogCollections(new ObservableCollection<LogData>(logs), mainLogList);

			if (string.IsNullOrEmpty(Settings.LogRootPath))
			{
				Shown += (sender, args) => new LogSettingsDialog(this).ShowModal(this);
			}
			else
			{
				ReloadLogs();
			}
		}

		public void ReloadLogs()
		{
			logLoadTaskTokenSource?.Cancel();
			logLoadTaskTokenSource = new CancellationTokenSource();

			logs.Clear();
			Task.Run(() => LoadLogs(mainLogList, logLoadTaskTokenSource.Token));
		}

		private async Task LoadLogs(LogList logList, CancellationToken cancellationToken)
		{
			Application.Instance.Invoke(() => { Status = "Finding logs..."; });
			cancellationToken.ThrowIfCancellationRequested();

			Task<LogCache> deserializeTask = null;
			if (LogCache == null)
			{
				deserializeTask = Task.Run(LogCache.LoadFromFile, cancellationToken);
			}

			FindLogs(logList, cancellationToken);

			Application.Instance.Invoke(() => { Status = "Loading log cache..."; });
			cancellationToken.ThrowIfCancellationRequested();

			if (deserializeTask != null)
			{
				LogCache = await deserializeTask;
				Application.Instance.AsyncInvoke(OnCacheLoaded);
			}

			cancellationToken.ThrowIfCancellationRequested();

			// Copying the logs into a new collection is required to improve performance on platforms
			// where each modification results in a full refresh of all data in the grid view.
			var newLogs = new ObservableCollection<LogData>(logs);

			for (var i = 0; i < logs.Count; i++)
			{
				var log = logs[i];
				if (LogCache.TryGetLogData(log.FileInfo, out var cachedLog))
				{
					newLogs[i] = cachedLog;
				}
			}

			Application.Instance.Invoke(() => { RecreateLogCollections(newLogs, logList); });

			Application.Instance.Invoke(() => { Status = "Parsing logs..."; });
			cancellationToken.ThrowIfCancellationRequested();

			ParseLogs(logList, cancellationToken);

			Application.Instance.Invoke(() => { Status = "Saving cache..."; });
			cancellationToken.ThrowIfCancellationRequested();

			if (LogCache.ChangedSinceLastSave)
			{
				LogCache.SaveToFile();
			}

			Application.Instance.AsyncInvoke(() => { Status = $"{logs.Count} logs found."; });
		}

		private void FindLogs(LogList logList, CancellationToken cancellationToken)
		{
			try
			{
				// Because GridView updates are costly, adding thousands of logs creates significant performance issues.
				// For this reason we add to a new copy of the ObservableCollection and update the GridView once in
				// a few seconds. This is not the cleanest solution, but works for now. None of the invokes can be async
				// or things will break.
				var refreshCooldown = new Cooldown(TimeSpan.FromSeconds(3));
				refreshCooldown.Reset(DateTime.Now);

				var newLogs = new ObservableCollection<LogData>(logs);

				//foreach (var log in LogFinder.GetTesting())
				foreach (var log in LogFinder.GetFromDirectory(Settings.LogRootPath))
				{
					newLogs.Add(log);

					if (refreshCooldown.TryUse(DateTime.Now))
					{
						Application.Instance.Invoke(() => { RecreateLogCollections(newLogs, logList); });
						// newLogs becomes logs, we recreate it now to avoid updating the GridView
						newLogs = new ObservableCollection<LogData>(logs);
					}

					cancellationToken.ThrowIfCancellationRequested();
				}

				// Add the remaining logs
				Application.Instance.Invoke(() => { RecreateLogCollections(newLogs, logList); });
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

		private void ParseLogs(LogList logList, CancellationToken cancellationToken, bool reparse = false)
		{
			IEnumerable<LogData> filteredLogs = logs;

			if (!reparse)
			{
				// Skip already parsed logs
				filteredLogs = filteredLogs.Where(x => x.ParsingStatus != ParsingStatus.Parsed);
			}

			var logsToParse = filteredLogs.ToArray();

			for (var i = 0; i < logsToParse.Length; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}

				var log = logsToParse[i];

				bool failedBefore = log.ParsingStatus == ParsingStatus.Failed;

				log.ParseData(EVTCParser, LogProcessor, StatisticsCalculator);
				LogCache?.CacheLogData(log);

				int logNumber = i + 1;
				Application.Instance.AsyncInvoke(() =>
				{
					Status = $"Parsing logs ({logNumber}/{logsToParse.Length})...";
				});

				// There is no point in reloading data if it was failed before and failed again
				// because no data visible in the view has changed
				if (!(failedBefore && log.ParsingStatus == ParsingStatus.Failed))
				{
					Application.Instance.AsyncInvoke(() =>
					{
						if (gridRefreshCooldown.TryUse(DateTime.Now))
						{
							var index = logsFiltered.IndexOf(log);
							logList.ReloadData(index);
							UpdateFilterDropdown();
						}
					});
				}
			}

			Application.Instance.AsyncInvoke(() =>
			{
				logList.ReloadData();
				UpdateFilterDropdown();
			});

			// We have already broken out of the loop because of it,
			// now we are just setting the task state to cancelled.
			cancellationToken.ThrowIfCancellationRequested();
		}


		private bool FilterLog(LogData log)
		{
			if (EncounterFilter != EncounterFilterAll)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed)
				{
					return false;
				}

				if (log.EncounterName != EncounterFilter)
				{
					return false;
				}
			}

			if (!ShowFailedLogs && log.EncounterResult == EncounterResult.Failure ||
			    !ShowUnknownLogs && log.EncounterResult == EncounterResult.Unknown ||
			    !ShowSuccessfulLogs && log.EncounterResult == EncounterResult.Success)
			{
				return false;
			}

			if (!ShowParseUnparsedLogs && log.ParsingStatus == ParsingStatus.Unparsed ||
			    !ShowParseParsedLogs && log.ParsingStatus == ParsingStatus.Parsed ||
			    !ShowParseParsingLogs && log.ParsingStatus == ParsingStatus.Parsing ||
			    !ShowParseFailedLogs && log.ParsingStatus == ParsingStatus.Failed)
			{
				return false;
			}

			if (log.EncounterStartTime.LocalDateTime < MinDateTimeFilter ||
			    log.EncounterStartTime.LocalDateTime > MaxDateTimeFilter)
			{
				return false;
			}

			return true;
		}

		private void UpdateFilterDropdown()
		{
			var previousKey = encounterFilterDropDown.SelectedKey;

			encounterFilterDropDown.DataStore = new[] {EncounterFilterAll}.Concat(logs
				.Where(x => x.ParsingStatus == ParsingStatus.Parsed)
				.Select(x => x.EncounterName).Distinct().OrderBy(x => x).ToArray());

			encounterFilterDropDown.SelectedKey = previousKey;
		}

		private void RecreateLogCollections(ObservableCollection<LogData> newLogCollection, LogList logList)
		{
			logs = newLogCollection;

			UpdateFilterDropdown();

			logs.CollectionChanged += (sender, args) => { UpdateFilterDropdown(); };

			logsFiltered = new FilterCollection<LogData>(logs);

			logsFiltered.CollectionChanged += (sender, args) =>
			{
				var logsByAccountName = new Dictionary<string, List<LogData>>();
				var dataByGuild = new Dictionary<string, (List<LogData> Logs, List<LogPlayer> Players)>();
				foreach (var log in logsFiltered)
				{
					if (log.ParsingStatus != ParsingStatus.Parsed) continue;

					foreach (var player in log.Players)
					{
						if (!logsByAccountName.ContainsKey(player.AccountName))
						{
							logsByAccountName[player.AccountName] = new List<LogData>();
						}

						logsByAccountName[player.AccountName].Add(log);

						if (player.GuildGuid != null)
						{
							if (!dataByGuild.ContainsKey(player.GuildGuid))
							{
								dataByGuild[player.GuildGuid] = (new List<LogData>(), new List<LogPlayer>());
							}

							var (guildLogs, guildPlayers) = dataByGuild[player.GuildGuid];

							guildLogs.Add(log);
							guildPlayers.Add(player);
						}
					}
				}

				playerList.DataStore.Clear();
				foreach (var data in logsByAccountName.Select(x => new PlayerData(x.Key, x.Value))
					.OrderByDescending(x => x.Logs.Count))
				{
					playerList.DataStore.Add(data);
				}

				playerList.Refresh();

				guildList.DataStore.Clear();
				foreach (var data in dataByGuild
					.Select(x => new GuildData(x.Key, x.Value.Logs.Distinct(), x.Value.Players))
					.OrderByDescending(x => x.Logs.Count))
				{
					guildList.DataStore.Add(data);
				}

				guildList.Refresh();
			};

			logList.DataStore = logsFiltered;

			logsFiltered.Filter = FilterLog;
			logsFiltered.Refresh();
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnCacheLoaded()
		{
			CacheLoaded?.Invoke(this, EventArgs.Empty);
		}
	}
}