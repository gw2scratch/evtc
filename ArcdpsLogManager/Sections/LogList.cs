using System;
using System.Collections.Generic;
using System.Globalization;
using ArcdpsLogManager.Controls;
using ArcdpsLogManager.Logs;
using Eto.Forms;
using ScratchEVTCParser.Model.Encounters;

namespace ArcdpsLogManager.Sections
{
	public class LogList : Panel
	{
		private readonly ImageProvider imageProvider;
		private readonly GridView<LogData> logGridView;

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
	}
}