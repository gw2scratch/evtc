using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ArcdpsLogManager.Controls;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using Eto.Forms;
using ScratchEVTCParser.Model.Encounters;

namespace ArcdpsLogManager.Sections
{
	public class LogList : Panel
	{
		private readonly ImageProvider imageProvider;
		private readonly GridView<LogData> logGridView;

        private const int PlayerIconSize = 20;
        private const int PlayerIconSpacing = 5;

		private ICollection<LogData> dataStore;

		public ICollection<LogData> DataStore
		{
			get => dataStore;
			set
			{
				dataStore = value;
				logGridView.DataStore = dataStore;
			}
		}

		public LogList(ImageProvider imageProvider)
		{
			this.imageProvider = imageProvider;
			var logDetailPanel = ConstructLogDetailPanel();
			logGridView = ConstructLogGridView(logDetailPanel);

			var logLayout = new DynamicLayout();
			logLayout.BeginVertical();
			logLayout.BeginHorizontal();
			logLayout.Add(logGridView, true);
			logLayout.Add(logDetailPanel);
			logLayout.EndHorizontal();
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

		public LogDetailPanel ConstructLogDetailPanel()
		{
			return new LogDetailPanel(imageProvider);
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
						return $"{(int)seconds / 60:0}m {seconds % 60:00.0}s";
					})
				}
			});

			var compositionCell = new DrawableCell();
			compositionCell.Paint += (sender, args) =>
			{
				if (!(args.Item is LogData log)) return;
				if (log.ParsingStatus != ParsingStatus.Parsed) return;


				var players = log.Players.OrderBy(player => player.Profession).ThenBy(player => player.EliteSpecialization).ToArray();
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

			gridView.RowHeight = Math.Max(gridView.RowHeight, PlayerIconSize + 2);

			gridView.SelectionChanged += (sender, args) => { detailPanel.LogData = gridView.SelectedItem; };

			return gridView;
		}
	}
}