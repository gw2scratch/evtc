using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GW2Scratch.EVTCAnalytics.Parsing;

public class FilteringOptions : INotifyPropertyChanged
{
	private bool pruneForEncounterData = false;
	private IReadOnlyList<Type> extraRequiredEventTypes = Array.Empty<Type>();

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
	public bool PruneForEncounterData
	{
		get => pruneForEncounterData;
		set => SetField(ref pruneForEncounterData, value);
	}

	/// <summary>
	/// Event types that will be kept even if <see cref="PruneForEncounterData"/> is enabled.
	/// </summary>
	public IReadOnlyList<Type> ExtraRequiredEventTypes
	{
		get => extraRequiredEventTypes;
		set => SetField(ref extraRequiredEventTypes, value);
	}

	public ICombatItemFilters CreateFilters(IReadOnlyList<IEncounterData> encounterDatas)
	{
		if (!PruneForEncounterData)
		{
			return new KeepAllCombatItemFilters();
		}
		
		var requiredEvents = encounterDatas.SelectMany(encounter => new[]
		{
			encounter.ResultDeterminer.RequiredEventTypes, encounter.HealthDeterminer.RequiredEventTypes, encounter.ModeDeterminer.RequiredEventTypes,
			ExtraRequiredEventTypes,
		}).SelectMany(x => x).Distinct().ToList();
		
		var requiredBuffIds = encounterDatas.SelectMany(encounter => new[]
		{
			encounter.ResultDeterminer.RequiredBuffSkillIds, encounter.HealthDeterminer.RequiredBuffSkillIds, encounter.ModeDeterminer.RequiredBuffSkillIds,
		}).SelectMany(x => x).Distinct().ToList();
		
		var requiredPhysicalResults = encounterDatas.SelectMany(encounter => new[]
		{
			encounter.ResultDeterminer.RequiredPhysicalDamageEventResults, encounter.HealthDeterminer.RequiredPhysicalDamageEventResults, encounter.ModeDeterminer.RequiredPhysicalDamageEventResults,
		}).SelectMany(x => x).Distinct().ToList();
		
		var filters = new CombatItemFilters(requiredEvents, requiredBuffIds, requiredPhysicalResults);

		return filters;
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value)) return false;
		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}
}