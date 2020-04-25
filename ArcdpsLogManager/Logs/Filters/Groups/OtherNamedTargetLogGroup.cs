using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group for logs with <see cref="Encounter.Other"/> encounter, filters by main target name.
	/// </summary>
	public class OtherNamedTargetLogGroup : LogGroup
	{
		public string MainTargetName { get; }

		public override string Name => MainTargetName;
		public override IEnumerable<LogGroup> Subgroups => Enumerable.Empty<LogGroup>();

		public OtherNamedTargetLogGroup(string mainTargetName)
		{
			MainTargetName = mainTargetName;
		}

		public override bool IsInGroup(LogData log)
		{
			return log.Encounter == Encounter.Other && log.MainTargetName == MainTargetName;
		}
	}
}