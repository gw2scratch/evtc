using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class LogList : Panel
	{
		private readonly ApiData apiData;
		private readonly LogDataProcessor logProcessor;
		private readonly UploadProcessor uploadProcessor;
		private readonly ImageProvider imageProvider;
		private readonly ILogNameProvider nameProvider;
		private readonly GridView<LogData> logGridView;

		private const int PlayerIconSize = 20;
		private const int PlayerIconSpacing = 5;

		private GridViewSorter<LogData> sorter;
		private FilterCollection<LogData> dataStore;

		private static readonly Dictionary<string, string> Abbreviations = new Dictionary<string, string>
		{
			{"CM", "Challenge Mode"}
		};

		public FilterCollection<LogData> DataStore
		{
			get => dataStore;
			set
			{
				dataStore = value;
				logGridView.DataStore = dataStore;
				sorter.UpdateDataStore();
			}
		}

		public LogList(ApiData apiData, LogDataProcessor logProcessor, UploadProcessor uploadProcessor,
			ImageProvider imageProvider, ILogNameProvider nameProvider)
		{
			this.apiData = apiData;
			this.logProcessor = logProcessor;
			this.uploadProcessor = uploadProcessor;
			this.imageProvider = imageProvider;
			this.nameProvider = nameProvider;

			var logDetailPanel = ConstructLogDetailPanel();
			var multipleLogPanel = ConstructMultipleLogPanel();
			logGridView = ConstructLogGridView(logDetailPanel, multipleLogPanel);

			var logLayout = new DynamicLayout();
			logLayout.BeginVertical();
			{
				logLayout.BeginHorizontal();
				{
					logLayout.Add(logGridView, true);
					logLayout.Add(logDetailPanel);
					logLayout.Add(multipleLogPanel);
				}
				logLayout.EndHorizontal();
			}
			logLayout.EndVertical();

			Content = logLayout;
		}

		public void ReloadData()
		{
			logGridView.ReloadData(new Range<int>(0, DataStore.Count - 1));
		}

		public void ReloadData(int row)
		{
			logGridView.ReloadData(row);
		}

		private LogDetailPanel ConstructLogDetailPanel()
		{
			return new LogDetailPanel(apiData, logProcessor, uploadProcessor, imageProvider, nameProvider);
		}

		private MultipleLogPanel ConstructMultipleLogPanel()
		{
			return new MultipleLogPanel(logProcessor, uploadProcessor);
		}

		private GridView<LogData> ConstructLogGridView(LogDetailPanel detailPanel, MultipleLogPanel multipleLogPanel)
		{
			var gridView = new GridView<LogData>
			{
				AllowMultipleSelection = true
			};

			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Encounter",
				DataCell = new TextBoxCell {Binding = new DelegateBinding<LogData, string>(x => nameProvider.GetName(x))}
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
				HeaderText = "CM",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x =>
					{
						switch (x.EncounterMode)
						{
							case EncounterMode.Challenge:
								return "CM";
							case EncounterMode.Normal:
								return "";
							case EncounterMode.Unknown:
								return "?";
							default:
								throw new ArgumentOutOfRangeException();
						}
					})
				}
			});

			var dateColumn = new GridColumn()
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

						string prefix = x.ParsingStatus != ParsingStatus.Parsed ? "~" : "";
						string encounterTime = x.EncounterStartTime.ToLocalTime().DateTime
							.ToString(CultureInfo.CurrentCulture);
						return $"{prefix}{encounterTime}";
					})
				}
			};
			gridView.Columns.Add(dateColumn);

			var durationColumn = new GridColumn()
			{
				HeaderText = "Duration",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x =>
					{
						var seconds = x.EncounterDuration.TotalSeconds;
						return $"{(int) seconds / 60:0}m {seconds % 60:00.0}s";
					})
				}
			};
			gridView.Columns.Add(durationColumn);

			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Character",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x => x.PointOfView?.CharacterName ?? "Unknown")
				}
			});


			var compositionCell = new DrawableCell();
			compositionCell.Paint += (sender, args) =>
			{
				if (!(args.Item is LogData log)) return;
				if (log.ParsingStatus != ParsingStatus.Parsed) return;


				var players = log.Players.OrderBy(player => player.Profession)
					.ThenBy(player => player.EliteSpecialization).ToArray();
				var origin = args.ClipRectangle.Location;
				for (int i = 0; i < players.Length; i++)
				{
					var icon = imageProvider.GetTinyProfessionIcon(players[i]);
					var point = origin + new PointF(i * (PlayerIconSize + PlayerIconSpacing), 0);
					args.Graphics.DrawImage(icon, point);
				}
			};

			var compositionColumn = new GridColumn()
			{
				HeaderText = "Players",
				DataCell = compositionCell,
				Width = 11 * (PlayerIconSize + PlayerIconSpacing) // There are logs with 11 players here and there
			};

			gridView.Columns.Add(compositionColumn);

			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Map ID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x => x.MapId?.ToString() ?? "Unknown")
				}
			});

			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Game Version",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x => x.GameBuild?.ToString() ?? "Unknown")
				}
			});

			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "arcdps Version",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x => x.EvtcVersion)
				}
			});

			foreach (var column in gridView.Columns)
			{
				column.Visible = !Settings.HiddenLogListColumns.Contains(column.HeaderText);
			}

			Settings.HiddenLogListColumnsChanged += (sender, args) =>
			{
				foreach (var column in gridView.Columns)
				{
					column.Visible = !Settings.HiddenLogListColumns.Contains(column.HeaderText);
				}
			};

			// Some extra height is needed on the WPF platform to properly fit the icons
			gridView.RowHeight = Math.Max(gridView.RowHeight, PlayerIconSize + 2);

			gridView.SelectionChanged += (sender, args) =>
			{
				detailPanel.LogData = gridView.SelectedItem;

				// This is not ordered by default on some platforms (such as WPF), so we
				// explicitly order the items according to the current sort of the grid.
				multipleLogPanel.LogData = gridView.SelectedItems.OrderBy(x => x, Comparer<LogData>.Create(sorter.GetSort()));
			};

			gridView.ContextMenu = ConstructLogGridViewContextMenu(gridView);

			sorter = new GridViewSorter<LogData>(gridView, new Dictionary<GridColumn, Comparison<LogData>>
			{
				{
					compositionColumn, (x, y) =>
					{
						int xPlayerCount = x.Players?.Count() ?? -1;
						int yPlayerCount = y.Players?.Count() ?? -1;
						return xPlayerCount.CompareTo(yPlayerCount);
					}
				},
				{
					durationColumn, (x, y) => x.EncounterDuration.CompareTo(y.EncounterDuration)
				},
				{
					dateColumn, (x, y) => x.EncounterStartTime.CompareTo(y.EncounterStartTime)
				}
			});

			sorter.EnableSorting();
			sorter.SortByDescending(dateColumn);

			return gridView;
		}

		private ContextMenu ConstructLogGridViewContextMenu(GridView<LogData> gridView)
		{
			var contextMenu = new ContextMenu();
			foreach (var column in gridView.Columns)
			{
				var menuItem = new CheckMenuItem
				{
					Checked = !Settings.HiddenLogListColumns.Contains(column.HeaderText),
					Text = $"{column.HeaderText}"
				};

				if (Abbreviations.TryGetValue(menuItem.Text, out string fullName))
				{
					menuItem.Text = $"{menuItem.Text} ({fullName})";
				}

				menuItem.CheckedChanged += (item, args) =>
				{
					bool shown = menuItem.Checked;
					if (!shown)
					{
						Settings.HiddenLogListColumns = Settings.HiddenLogListColumns.Append(column.HeaderText).Distinct().ToList();
					}
					else
					{
						Settings.HiddenLogListColumns = Settings.HiddenLogListColumns.Where(x => x != column.HeaderText).ToList();
					}
				};
				Settings.HiddenLogListColumnsChanged += (sender, args) =>
				{
					bool shown = !Settings.HiddenLogListColumns.Contains(column.HeaderText);
					menuItem.Checked = shown;
				};
				contextMenu.Items.Add(menuItem);
			}

			return contextMenu;
		}
	}
}