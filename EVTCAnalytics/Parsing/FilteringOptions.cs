using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Parsing;

public class FilteringOptions
{
	/// <summary>
	/// Prune events not required for determiners contained in the <see cref="IEncounterData"/>.
	///
	/// Do not use if you need any specific events without also specifying them <see cref="ExtraRequiredEventTypes"/>, as they could be pruned away.
	/// </summary>
	/// <remarks>
	/// Does not affect agents and skills in any way.
	/// 
	/// The pruning occurs during the parsing phase, while there may still be multiple possible encounters for the log (sharing the same boss id).
	/// In this case, potentially needed events are kept for all possible encounters, reducing the effectiveness.
	/// </remarks>
	public bool PruneForEncounterData { get; set; } = false;

	/// <summary>
	/// Event types that will be kept even if <see cref="PruneForEncounterData"/> is enabled.
	/// </summary>
	public IEnumerable<Type> ExtraRequiredEventTypes { get; } = Array.Empty<Type>();

	public ICombatItemFilters CreateFilters(IEnumerable<IEncounterData> encounterDatas)
	{
		if (!PruneForEncounterData)
		{
			return new AllowAllCombatItemFilters();
		}
		
		var requiredEvents = encounterDatas.SelectMany(encounter => new[]
		{
			encounter.ResultDeterminer.RequiredEventTypes, encounter.HealthDeterminer.RequiredEventTypes, encounter.ModeDeterminer.RequiredEventTypes,
			ExtraRequiredEventTypes,
		}).SelectMany(x => x).Distinct().ToList();
		
		var filters = new CombatItemFilters(requiredEvents);

		return filters;
	}
}