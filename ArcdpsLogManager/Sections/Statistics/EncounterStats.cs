using System;
using System.Collections.Generic;
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
	}
}