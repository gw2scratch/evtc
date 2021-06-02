using System;
using System.Collections.Generic;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Sections.Statistics.Tabs
{
	public class EncounterStatistics : DynamicLayout
	{
		private readonly ILogNameProvider nameProvider;
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

		public EncounterStatistics(ILogNameProvider nameProvider)
		{
			this.nameProvider = nameProvider;
			statsGridView = ConstructGridView();
			DataStore = new FilterCollection<EncounterStats>();

			Add(statsGridView);
		}

		private GridView<EncounterStats> ConstructGridView()
		{
			var customSorts = new Dictionary<GridColumn, Comparison<EncounterStats>>();

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

			var totalTimeColumn = new GridColumn
			{
				HeaderText = "Total time",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<EncounterStats, string>(x => FormatTimeSpan(x.GetTotalTimeSpent()))
				}
			};
			gridView.Columns.Add(totalTimeColumn);
			customSorts[totalTimeColumn] =
				(left, right) => left.GetTotalTimeSpent().CompareTo(right.GetTotalTimeSpent());

			var shownResults = new (EncounterResult Result, string CountHeaderText, string TimeHeaderText)[]
			{
				(EncounterResult.Success, "Successes", "Time in successes"),
				(EncounterResult.Failure, "Failures", "Time in failures")
			};

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

				var timeColumn = new GridColumn
				{
					HeaderText = timeHeaderText,
					DataCell = new TextBoxCell
					{
						Binding = new DelegateBinding<EncounterStats, string>(x => FormatTimeSpan(GetTime(x)))
					}
				};
				gridView.Columns.Add(timeColumn);
				customSorts[timeColumn] = (left, right) => GetTime(left).CompareTo(GetTime(right));

				var averageTimeColumn = new GridColumn
				{
					HeaderText = $"Average {result.ToString()} time",
					DataCell = new TextBoxCell
					{
						Binding = new DelegateBinding<EncounterStats, string>(x => FormatTimeSpan(x.GetAverageTimeByResult(result)))
					}
				};
				gridView.Columns.Add(averageTimeColumn);
			}

			var successRateColumn = new GridColumn
			{
				HeaderText = "Success rate",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<EncounterStats, string>(x => $"{x.GetSuccessRate() * 100:0.0}%")
				}
			};
			gridView.Columns.Add(successRateColumn);
			customSorts[successRateColumn] = (left, right) => left.GetSuccessRate().CompareTo(right.GetSuccessRate());

			sorter = new GridViewSorter<EncounterStats>(gridView, customSorts);
			sorter.EnableSorting();

			return gridView;
		}

		private string FormatTimeSpan(TimeSpan time)
		{
			var str = $@"{time:hh\h\ mm\m\ ss\s}";
			if (time.Days > 0)
			{
				str = $@"{time:%d\d} " + str;
			}

			return str;
		}


		public void UpdateDataFromLogs(IEnumerable<LogData> logs)
		{
			var statsByEncounter = new Dictionary<string, EncounterStats>();

			foreach (var log in logs)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed) continue;

				var encounterName = nameProvider.GetName(log);
				if (!statsByEncounter.ContainsKey(encounterName))
				{
					statsByEncounter[encounterName] = new EncounterStats(encounterName);
				}

				var stats = statsByEncounter[encounterName];

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