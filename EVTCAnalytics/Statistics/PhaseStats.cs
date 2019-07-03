using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Statistics
{
	public class PhaseStats
	{
		public long StartTime { get; }
		public long EndTime { get; }
		public long PhaseDuration => EndTime - StartTime;

		public string PhaseName { get; }

		public SquadDamageData TotalDamageData { get; }
		public IEnumerable<TargetSquadDamageData> TargetDamageData { get; }

		public PhaseStats(string phaseName, long startTime, long endTime, IEnumerable<TargetSquadDamageData> targetDamageData, SquadDamageData totalDamageData)
		{
			TargetDamageData = targetDamageData;
			TotalDamageData = totalDamageData;
			StartTime = startTime;
			EndTime = endTime;
			PhaseName = phaseName;
		}
	}
}