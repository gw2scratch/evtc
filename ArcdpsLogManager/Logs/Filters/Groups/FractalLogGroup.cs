using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that all fractal logs belong to. Has subcategories for individual fractals.
	/// </summary>
	internal class FractalLogGroup : LogGroup
	{
		private readonly IReadOnlyList<LogGroup> subgroups;

		public override string Name { get; } = "Fractals of the Mists";
		public override IEnumerable<LogGroup> Subgroups => subgroups;

		public FractalLogGroup()
		{
			var fractalCategories = ((EncounterCategory[]) Enum.GetValues(typeof(EncounterCategory)))
				.Where(x => x.IsFractal());

			var fractalGroups = fractalCategories
				.Select(category => new CategoryLogGroup(category))
				.ToList<LogGroup>();

			subgroups = fractalGroups;
		}

		public override bool IsInGroup(LogData log)
		{
			return log.Encounter.GetEncounterCategory().IsFractal();
		}
	}
}
