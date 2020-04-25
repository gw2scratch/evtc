using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that all logs belong to.
	/// </summary>
	public class RootLogGroup : LogGroup
	{
		private readonly IReadOnlyList<LogGroup> subgroups;

		public override string Name { get; } = "All logs";
		public override IEnumerable<LogGroup> Subgroups => subgroups;

		public RootLogGroup(IEnumerable<LogData> logs)
		{
			// All logs
			// - Raids
			// -- Wing 1
			// -- Wing 2
			// -- ...
			// -- Wing 7
			// - Strike Missions
			// - Fractals
			// - Festivals
			// - Special Forces Training Area (Golems)
			// - Any other non-raid EncounterCategory
			// - Others
			// -- Encounter 1 (by target name)
			// -- Encounter 2 (by target name)
			// -- Encounter 3 (by target name)
			// - WvW

			var manuallyAddedCategories = new[]
			{
				EncounterCategory.StrikeMission,
				EncounterCategory.Fractal,
				EncounterCategory.Festival,
				EncounterCategory.SpecialForcesTrainingArea,
				EncounterCategory.Other,
				EncounterCategory.WorldVersusWorld
			};

			var leftoverCategories = ((EncounterCategory[]) Enum.GetValues(typeof(EncounterCategory)))
				.Where(x => !x.IsRaid()
				            && !manuallyAddedCategories.Contains(x));

			var groups = new List<LogGroup>();

			groups.Add(new RaidLogGroup());
			groups.Add(new CategoryLogGroup(EncounterCategory.StrikeMission));
			groups.Add(new CategoryLogGroup(EncounterCategory.Fractal));
			groups.Add(new CategoryLogGroup(EncounterCategory.Festival));
			groups.Add(new CategoryLogGroup(EncounterCategory.SpecialForcesTrainingArea));
			foreach (var category in leftoverCategories)
			{
				groups.Add(new CategoryLogGroup(category));
			}
			groups.Add(CategoryLogGroup.Other(logs.Where(x => x.Encounter == Encounter.Other).Select(x => x.MainTargetName).Distinct()));
			groups.Add(new CategoryLogGroup(EncounterCategory.WorldVersusWorld));

			this.subgroups = groups;
		}

		public override bool IsInGroup(LogData log)
		{
			return true;
		}
	}
}