using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that all strike mission logs belong to. Has subcategories for individual strike types.
	/// </summary>
	public class StrikeMissionLogGroup : LogGroup
	{
		private readonly IReadOnlyList<LogGroup> subgroups;

		public override string Name { get; } = "Strike Missions";
		public override IEnumerable<LogGroup> Subgroups => subgroups;

		public StrikeMissionLogGroup()
		{
			var strikeCategories = ((EncounterCategory[]) Enum.GetValues(typeof(EncounterCategory)))
				.Where(x => x.IsStrikeMission());

			var strikeGroups = strikeCategories
				.Select(category => new CategoryLogGroup(category))
				.ToList<LogGroup>();

			subgroups = strikeGroups;
		}

		public override bool IsInGroup(LogData log)
		{
			return log.Encounter.GetEncounterCategory().IsStrikeMission();
		}
	}
}