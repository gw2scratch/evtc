using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that all raid encounters logs belong to. Has subcategories for individual encounter types.
	/// </summary>
	public class RaidEncounterLogGroup : LogGroup
	{
		private readonly IReadOnlyList<LogGroup> subgroups;

		public override string Name { get; } = "Raid Encounters";
		public override IEnumerable<LogGroup> Subgroups => subgroups;

		public RaidEncounterLogGroup()
		{
			var encounterCategories = ((EncounterCategory[]) Enum.GetValues(typeof(EncounterCategory)))
				.Where(x => x.IsRaidEncounter());

			var encounterGroups = encounterCategories
				.Select(category => new CategoryLogGroup(category))
				.ToList<LogGroup>();

			subgroups = encounterGroups;
		}

		public override bool IsInGroup(LogData log)
		{
			return log.Encounter.GetEncounterCategory().IsRaidEncounter();
		}
	}
}