using System.Collections.Generic;
using System.Linq;

namespace ScratchEVTCParser.Statistics
{
	public class PhaseStats
	{
		public long StartTime { get; }
		public long EndTime { get; }
		public long PhaseDuration => EndTime - StartTime;

		public string PhaseName { get; }

		public IEnumerable<TargetDamageData> TargetDamageData { get; }

		public PhaseStats(string phaseName, long startTime, long endTime, IEnumerable<TargetDamageData> targetDamageData)
		{
			TargetDamageData = targetDamageData;
			StartTime = startTime;
			EndTime = endTime;
			PhaseName = phaseName;
		}
	}
}