using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that a logs with a given <see cref="EncounterCategory"/> belong to.
	/// </summary>
	public class CategoryLogGroup : LogGroup
	{
		private static readonly Dictionary<EncounterCategory, string> CategoryNames = new Dictionary<EncounterCategory, string>
		{
			{EncounterCategory.RaidWing1, "Spirit Vale (W1)"},
			{EncounterCategory.RaidWing2, "Salvation Pass (W2)"},
			{EncounterCategory.RaidWing3, "Stronghold of the Faithful (W3)"},
			{EncounterCategory.RaidWing4, "Bastion of the Penitent (W4)"},
			{EncounterCategory.RaidWing5, "Hall of Chains (W5)"},
			{EncounterCategory.RaidWing6, "Mythwright Gambit (W6)"},
			{EncounterCategory.RaidWing7, "The Key of Ahdashim (W7)"},
			{EncounterCategory.StrikeMission, "Strike Missions"},
			{EncounterCategory.Fractal, "Fractals of the Mists"},
			{EncounterCategory.SpecialForcesTrainingArea, "Special Forces Training Area"},
			{EncounterCategory.Other, "Uncategorized (PvE)"},
			{EncounterCategory.Festival, "Festivals"},
			{EncounterCategory.WorldVersusWorld, "WvW"},
		};

		public EncounterCategory Category { get; }

		public override string Name { get; }
		public override IEnumerable<LogGroup> Subgroups => subgroups;

		private readonly IReadOnlyList<LogGroup> subgroups;

		private CategoryLogGroup(EncounterCategory category, IEnumerable<LogGroup> subgroups)
		{
			Category = category;
			this.subgroups = subgroups.ToList();
			if (!CategoryNames.TryGetValue(Category, out string name))
			{
				name = Category.ToString();
			}

			Name = name;
		}

		public CategoryLogGroup(EncounterCategory category) : this(category, new LogGroup[0])
		{
			var encounters = ((Encounter[]) Enum.GetValues(typeof(Encounter)))
				.Where(x => x.GetEncounterCategory() == category);

			subgroups = encounters.Select(x => new EncounterLogGroup(x)).ToList();
		}

		/// <summary>
		/// Creates a category log group for <see cref="EncounterCategory.Other"/>. If specifying target names,
		/// subgroups are created for each individual target.
		/// </summary>
		/// <param name="mainTargetNames">Names of individual main targets</param>
		/// <returns>A category group for <see cref="EncounterCategory.Other"/> with subgroups for each specified target.</returns>
		public static CategoryLogGroup Other(IEnumerable<string> mainTargetNames)
		{
			var subgroups = mainTargetNames.Select(x => new OtherNamedTargetLogGroup(x));
			return new CategoryLogGroup(EncounterCategory.Other, subgroups);
		}

		public override bool IsInGroup(LogData log)
		{
			return log.Encounter.GetEncounterCategory() == Category;
		}
	}
}