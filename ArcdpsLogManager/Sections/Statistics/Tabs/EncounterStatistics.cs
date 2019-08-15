using System;
using System.Collections.Generic;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Sections.Statistics.Tabs
{
	public class EncounterStatistics : DynamicLayout
	{
		private readonly GridView<EncounterStats> statsGridView;
		private GridViewSorter<EncounterStats> sorter;

		private FilterCollection<EncounterStats> stats;

		private FilterCollection<EncounterStats> DataStore
		{
			get => stats;
			set
			{
				if (value == null)
				{
					value = new FilterCollection<EncounterStats>();
				}

				stats = value;
				if (statsGridView != null)
				{
					statsGridView.DataStore = stats;
					sorter.UpdateDataStore();
				}
			}
		}

		public EncounterStatistics()
		{
			statsGridView = ConstructGridView();
			DataStore = new FilterCollection<EncounterStats>();

			Add(statsGridView);
		}

		private GridView<EncounterStats> ConstructGridView()
		{
			var gridView = new GridView<EncounterStats>();
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Encounter name",
				DataCell = new TextBoxCell {Binding = new DelegateBinding<EncounterStats, string>(x => x.Name)}
			});

			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Logs",
				DataCell = new TextBoxCell {Binding = new DelegateBinding<EncounterStats, string>(x => $"{x.LogCount}")}
			});

			var shownResults = new (EncounterResult Result, string CountHeaderText, string TimeHeaderText)[]
			{
				(EncounterResult.Success, "Successes", "Time in successes"),
				(EncounterResult.Failure, "Failures", "Time in failures")
			};

			var customSorts = new Dictionary<GridColumn, Comparison<EncounterStats>>();

			foreach ((var result, string countHeaderText, string timeHeaderText) in shownResults)
			{
				gridView.Columns.Add(new GridColumn
				{
					HeaderText = countHeaderText,
					DataCell = new TextBoxCell
					{
						Binding = new DelegateBinding<EncounterStats, string>(x =>
							x.LogCountsByResult.TryGetValue(result, out int count) ? $"{count}" : "0")
					}
				});

				TimeSpan GetTime(EncounterStats x) =>
					x.TimeSpentByResult.TryGetValue(result, out TimeSpan time) ? time : TimeSpan.Zero;

				var totalTimeColumn = new GridColumn
				{
					HeaderText = timeHeaderText,
					DataCell = new TextBoxCell
					{
						Binding = new DelegateBinding<EncounterStats, string>(x =>
						{
							var time = GetTime(x);
							var str = $@"{time:hh\h\ mm\m\ ss\s}";
							if (time.Days > 0)
							{
								str = $@"{time:%d\d} " + str;
							}

							return str;
						})
					}
				};
				gridView.Columns.Add(totalTimeColumn);
				customSorts[totalTimeColumn] = (left, right) => GetTime(left).CompareTo(GetTime(right));
			}

			sorter = new GridViewSorter<EncounterStats>(gridView, customSorts);
			sorter.EnableSorting();

			return gridView;
		}

		public void UpdateDataFromLogs(IEnumerable<LogData> logs)
		{
			var statsByEncounter = new Dictionary<string, EncounterStats>();

			foreach (var log in logs)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed) continue;

				if (!statsByEncounter.ContainsKey(log.EncounterName))
				{
					statsByEncounter[log.EncounterName] = new EncounterStats(log.EncounterName);
				}

				var stats = statsByEncounter[log.EncounterName];

				if (!stats.LogCountsByResult.ContainsKey(log.EncounterResult))
				{
					stats.LogCountsByResult[log.EncounterResult] = 0;
				}

				if (!stats.TimeSpentByResult.ContainsKey(log.EncounterResult))
				{
					stats.TimeSpentByResult[log.EncounterResult] = TimeSpan.Zero;
				}

				stats.LogCount++;
				stats.LogCountsByResult[log.EncounterResult]++;
				stats.TimeSpentByResult[log.EncounterResult] += log.EncounterDuration;
			}

			DataStore = new FilterCollection<EncounterStats>(statsByEncounter.Values);
		}
	}
}