using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that all raid logs belong to. Has subcategories for individual wings.
	/// </summary>
	public class RaidLogGroup : LogGroup
	{
		private readonly IReadOnlyList<LogGroup> subgroups;

		public override string Name { get; } = "Raids";
		public override IEnumerable<LogGroup> Subgroups => subgroups;

		public RaidLogGroup()
		{
			var raidCategories = ((EncounterCategory[]) Enum.GetValues(typeof(EncounterCategory)))
				.Where(x => x.IsRaid());

			var raidGroups = raidCategories
				.Select(category => new CategoryLogGroup(category))
				.ToList<LogGroup>();

			subgroups = raidGroups;
		}

		public override bool IsInGroup(LogData log)
		{
			return log.Encounter.GetEncounterCategory().IsRaid();
		}
	}
}