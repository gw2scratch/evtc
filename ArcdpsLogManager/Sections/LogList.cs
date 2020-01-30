using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Data;
using GW2Scratch.ArcdpsLogManager.Logs;
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
		private readonly GridView<LogData> logGridView;

		private const int PlayerIconSize = 20;
		private const int PlayerIconSpacing = 5;

		private GridViewSorter<LogData> sorter;
		private FilterCollection<LogData> dataStore;

        private bool mouseDown = false;

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
			ImageProvider imageProvider)
		{
			this.apiData = apiData;
			this.logProcessor = logProcessor;
			this.uploadProcessor = uploadProcessor;
			this.imageProvider = imageProvider;

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
			return new LogDetailPanel(apiData, logProcessor, uploadProcessor, imageProvider);
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
				HeaderText = "CM",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x =>
					{
						switch (x.EncounterMode)
						{
							case EncounterMode.Challenge:
								return "Yes";
							case EncounterMode.Normal:
								return "No";
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

			compositionColumn.Visible = Settings.ShowSquadCompositions;
			Settings.ShowSquadCompositionsChanged += (sender, args) =>
			{
				compositionColumn.Visible = Settings.ShowSquadCompositions;
			};

			// Some extra height is needed on the WPF platform to properly fit the icons
			gridView.RowHeight = Math.Max(gridView.RowHeight, PlayerIconSize + 2);

			gridView.SelectionChanged += (sender, args) =>
			{
				detailPanel.LogData = gridView.SelectedItem;
				multipleLogPanel.LogData = gridView.SelectedItems;
			};

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

            gridView.MouseDown += OnGridViewOnMouseDown;
            gridView.MouseUp += GridViewOnMouseUp;
            gridView.MouseLeave += GridViewOnMouseLeave;

			return gridView;
		}

        private void GridViewOnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!mouseDown) return;

            var files = logGridView.SelectedItems.Select(x => new Uri(x.FileInfo.ToString())).ToArray();
            var dob = new DataObject {Uris = files};
            DoDragDrop(dob, DragEffects.Copy | DragEffects.Move);
        }

        private void GridViewOnMouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void OnGridViewOnMouseDown(object sender, MouseEventArgs args)
        {
            mouseDown = true;
        }
    }
}