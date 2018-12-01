using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArcdpsLogManager.Controls;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using Eto.Forms;
using Newtonsoft.Json;
using ScratchEVTCParser.Model.Encounters;

namespace ArcdpsLogManager
{
	public class ManagerForm : Form
	{
		private const string AppDataDirectoryName = "ArcdpsLogManager";
		private const string CacheFilename = "LogDataCache.json";

		private ImageProvider ImageProvider { get; } = new ImageProvider();
		private LogFinder LogFinder { get; } = new LogFinder();

		private ObservableCollection<LogData> logs = new ObservableCollection<LogData>();
		private SelectableFilterCollection<LogData> logsFiltered;

		private GridView<LogData> logGridView;
		private GridView<PlayerData> playerGridView;

		public ManagerForm()
		{
			Title = "arcdps Log Manager";
			ClientSize = new Size(900, 700);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			Closing += (sender, args) => { SerializeLogsToCache().Wait(); };

			var logLocationMenuItem = new ButtonMenuItem {Text = "&Log location"};
			logLocationMenuItem.Click += (sender, args) => { new LogSettingsDialog().ShowModal(); };
			logLocationMenuItem.Shortcut = Application.Instance.CommonModifier | Keys.L;

			var debugDataMenuItem = new CheckMenuItem {Text = "Show &Debug data"};
			debugDataMenuItem.Checked = Settings.ShowDebugData;
			debugDataMenuItem.CheckedChanged += (sender, args) =>
			{
				Settings.ShowDebugData = debugDataMenuItem.Checked;
			};

			var settingsMenuItem = new ButtonMenuItem {Text = "&Settings"};
			settingsMenuItem.Items.Add(logLocationMenuItem);
			settingsMenuItem.Items.Add(debugDataMenuItem);

			Menu = new MenuBar(settingsMenuItem);


			formLayout.BeginVertical(Padding = new Padding(5));

			formLayout.BeginGroup("Filters", new Padding(5));
			formLayout.BeginHorizontal();
			formLayout.Add("By encounter result:");
			formLayout.Add(new CheckBox {Text = "Success", Checked = true, Enabled = false}); // TODO: IMPLEMENT
			formLayout.Add(new CheckBox {Text = "Failure", Checked = true, Enabled = false}); // TODO: IMPLEMENT
			formLayout.Add(new CheckBox {Text = "Unknown", Checked = true, Enabled = false}); // TODO: IMPLEMENT
			formLayout.EndHorizontal();
			formLayout.EndGroup();

			var tabs = new TabControl();

			// Log tab
			var logDetailPanel = ConstructLogDetailPanel();
			logGridView = ConstructLogGridView(logDetailPanel);

			var logLayout = new DynamicLayout();
			logLayout.BeginVertical();
			logLayout.BeginHorizontal();
			logLayout.Add(logGridView, true);
			logLayout.Add(logDetailPanel);
			logLayout.EndHorizontal();
			logLayout.EndVertical();
			tabs.Pages.Add(new TabPage {Text = "Logs", Content = logLayout});

			// Player tab
			var playerDetailPanel = ConstructPlayerDetailPanel();
			playerGridView = ConstructPlayerGridView(playerDetailPanel);

			var playerLayout = new DynamicLayout();
			playerLayout.BeginVertical();
			playerLayout.BeginHorizontal();
			playerLayout.Add(playerGridView, true);
			playerLayout.Add(playerDetailPanel);
			playerLayout.EndHorizontal();
			playerLayout.EndVertical();
			tabs.Pages.Add(new TabPage {Text = "Players", Content = playerLayout});

			formLayout.Add(tabs, true);

			formLayout.EndVertical();

			RecreateLogCollections(new ObservableCollection<LogData>(logs));

			Task.Run(() => LoadLogs(logGridView));
		}

		public async Task LoadLogs(GridView<LogData> logGridView)
		{
			var deserializeTask = DeserializeLogCache();

			await FindLogs();
			var cache = await deserializeTask;

			// Copying the logs into a new collection is required to improve performance on platforms
			// where each modification results in a full refresh of all data in the grid view.
			var newLogs = new ObservableCollection<LogData>(logs);

			if (cache != null)
			{
				Application.Instance.Invoke(() =>
				{
					for (var i = 0; i < logs.Count; i++)
					{
						var log = logs[i];
						if (cache.TryGetValue(log.FileInfo.FullName, out var cachedLog))
						{
							newLogs[i] = cachedLog;
						}
					}
				});
			}

			Application.Instance.Invoke(() => { RecreateLogCollections(newLogs); });

			await ParseLogs(logGridView);

			await SerializeLogsToCache();
		}

		public async Task FindLogs()
		{
			try
			{
				// Invoking for every single added file is a lot of added overhead, instead add multiple files at a time.
				const int flushAmount = 100;
				List<LogData> foundLogs = new List<LogData>();

				//foreach (var file in LogFinder.GetTesting())
				foreach (var log in LogFinder.GetFromDirectory(Settings.LogRootPath))
				{
					foundLogs.Add(log);

					if (foundLogs.Count == flushAmount)
					{
						Application.Instance.Invoke(() =>
						{
							foreach (var flushedLog in foundLogs)
							{
								logs.Add(flushedLog);
							}
						});
						foundLogs.Clear();
					}
				}

				// Add the remaining logs
				Application.Instance.Invoke(() =>
				{
					foreach (var flushedLog in foundLogs)
					{
						logs.Add(flushedLog);
					}
				});
			}
			catch
			{
				// TODO: Handle exceptions properly
			}
		}

		public async Task ParseLogs(GridView<LogData> logGridView, bool reparse = false)
		{
			foreach (var log in logs)
			{
				// Skip already parsed logs
				if (log.ParsingStatus == ParsingStatus.Parsed && !reparse)
				{
					continue;
				}

				bool failedBefore = log.ParsingStatus == ParsingStatus.Failed;

				log.ParseData();
				var index = logsFiltered.IndexOf(log);

				// There is no point in reloading data if it was failed before and failed again
				// because no data visible in the view has changed
				if (!(failedBefore && log.ParsingStatus == ParsingStatus.Failed))
				{
					Application.Instance.Invoke(() => { logGridView.ReloadData(index); });
				}
			}
		}

		public async Task SerializeLogsToCache()
		{
			var logDataByFilename = logs.ToDictionary(x => x.FileInfo.FullName);

			var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				AppDataDirectoryName);

			var cacheFilePath = Path.Combine(directory, CacheFilename);

			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			using (var writer = new StreamWriter(cacheFilePath))
			{
				var serializer = new JsonSerializer();
				serializer.Serialize(writer, logDataByFilename);
			}
		}

		public async Task<Dictionary<string, LogData>> DeserializeLogCache()
		{
			var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				AppDataDirectoryName);

			var cacheFilePath = Path.Combine(directory, CacheFilename);
			if (File.Exists(cacheFilePath))
			{
				try
				{
					using (var reader = File.OpenText(cacheFilePath))
					{
						var serializer = new JsonSerializer();
						var dictionary =
							(Dictionary<string, LogData>) serializer.Deserialize(reader,
								typeof(Dictionary<string, LogData>));

						return dictionary;
					}
				}
				catch
				{
					// Ignored
					return null;
				}
			}

			return null;
		}

		public LogDetailPanel ConstructLogDetailPanel()
		{
			return new LogDetailPanel(ImageProvider);
		}

		public GridView<LogData> ConstructLogGridView(LogDetailPanel detailPanel)
		{
			var gridView = new GridView<LogData>();
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Encounter",
				DataCell = new TextBoxCell {Binding = new DelegateBinding<LogData, string>(x => x.EncounterName)}
			});

			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Result",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x =>
					{
						switch (x.EncounterResult)
						{
							case EncounterResult.Success:
								return "Success";
							case EncounterResult.Failure:
								return "Failure";
							case EncounterResult.Unknown:
								return "Unknown";
							default:
								throw new ArgumentOutOfRangeException();
						}
					})
				}
			});

			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Date",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x =>
					{
						if (x.EncounterStartTime == default)
						{
							return "Unknown";
						}

						return x.EncounterStartTime.ToLocalTime().DateTime.ToString(CultureInfo.CurrentCulture);
					})
				}
			});

			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Duration",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x =>
					{
						var seconds = x.EncounterDuration.TotalSeconds;
						return $"{seconds / 60:0}m {seconds % 60:00.0}s";
					})
				}
			});

			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Players",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x => "Composition will be here")
				}
			});

			gridView.SelectionChanged += (sender, args) => { detailPanel.LogData = gridView.SelectedItem; };

			return gridView;
		}

		private PlayerDetailPanel ConstructPlayerDetailPanel()
		{
			return new PlayerDetailPanel(ImageProvider);
		}

		private GridView<PlayerData> ConstructPlayerGridView(PlayerDetailPanel playerDetailPanel)
		{
			var gridView = new GridView<PlayerData>();
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Account name",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<PlayerData, string>(x => x.AccountName.Substring(1))}
			});
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Log count",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<PlayerData, string>(x => x.Logs.Count.ToString())}
			});

			gridView.SelectionChanged += (sender, args) =>
			{
				if (gridView.SelectedItem != null)
				{
					playerDetailPanel.PlayerData = gridView.SelectedItem;
				}
			};

			return gridView;
		}

		private void RecreateLogCollections(ObservableCollection<LogData> newLogCollection)
		{
			logs = newLogCollection;

			logsFiltered = new SelectableFilterCollection<LogData>(playerGridView, logs);

			logsFiltered.CollectionChanged += (sender, args) =>
			{
				var logsByAccountName = new Dictionary<string, List<LogData>>();
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
					}
				}

				var playerData = logsByAccountName.Select(x => new PlayerData(x.Key, x.Value)).OrderByDescending(x => x.Logs.Count).ToArray();
				playerGridView.DataStore = playerData;
			};

			logGridView.DataStore = logsFiltered;

			logsFiltered.Refresh();
		}
	}
}