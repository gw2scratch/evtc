using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Sections.Statistics
{
	public class EncounterStats
	{
		public string Name { get; }
		public int LogCount { get; set; }
		public Dictionary<EncounterResult, int> LogCountsByResult { get; } = new Dictionary<EncounterResult, int>();

		public Dictionary<EncounterResult, TimeSpan> TimeSpentByResult { get; } =
			new Dictionary<EncounterResult, TimeSpan>();

		public EncounterStats(string name)
		{
			Name = name;
		}

		public TimeSpan GetTotalTimeSpent()
		{
			return TimeSpentByResult.Aggregate(TimeSpan.Zero, (x, y) => x + y.Value);
		}

		public double GetSuccessRate()
		{
			if (!LogCountsByResult.TryGetValue(EncounterResult.Success, out int successes))
			{
				successes = 0;
			}

			if (!LogCountsByResult.TryGetValue(EncounterResult.Failure, out int failures))
			{
				failures = 0;
			}

			if (successes + failures == 0)
			{
				// Only unknown results are present, instead of returning NaN we return a success rate of 0.
				return 0;
			}

			return (double) successes / (successes + failures);
		}
	}
}