using System;
using GW2Scratch.ArcdpsLogManager.Sections.Statistics;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// A display-ready projection of <see cref="EncounterStats"/> for the Statistics → Encounters grid.
	/// </summary>
	public sealed class EncounterStatRow
	{
		public string Name { get; }
		public int LogCount { get; }
		public string TotalTime { get; }
		public int Successes { get; }
		public string TimeInSuccesses { get; }
		public string AverageSuccessTime { get; }
		public int Failures { get; }
		public string TimeInFailures { get; }
		public string AverageFailureTime { get; }
		public string SuccessRate { get; }

		// Numeric backing values for correct sorting.
		public TimeSpan TotalTimeValue { get; }
		public double SuccessRateValue { get; }

		public EncounterStatRow(EncounterStats stats)
		{
			Name = stats.Name;
			LogCount = stats.LogCount;

			TotalTimeValue = stats.GetTotalTimeSpent();
			TotalTime = Format(TotalTimeValue);

			Successes = stats.LogCountsByResult.TryGetValue(EncounterResult.Success, out int s) ? s : 0;
			Failures = stats.LogCountsByResult.TryGetValue(EncounterResult.Failure, out int f) ? f : 0;

			TimeInSuccesses = Format(TimeByResult(stats, EncounterResult.Success));
			TimeInFailures = Format(TimeByResult(stats, EncounterResult.Failure));
			AverageSuccessTime = Format(stats.GetAverageTimeByResult(EncounterResult.Success));
			AverageFailureTime = Format(stats.GetAverageTimeByResult(EncounterResult.Failure));

			SuccessRateValue = stats.GetSuccessRate();
			SuccessRate = $"{SuccessRateValue * 100:0.0}%";
		}

		private static TimeSpan TimeByResult(EncounterStats stats, EncounterResult result)
			=> stats.TimeSpentByResult.TryGetValue(result, out var t) ? t : TimeSpan.Zero;

		private static string Format(TimeSpan time)
		{
			var str = $@"{time:hh\h\ mm\m\ ss\s}";
			if (time.Days > 0)
			{
				str = $@"{time:%d\d} " + str;
			}

			return str;
		}
	}
}
