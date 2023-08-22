using GW2Scratch.EVTCAnalytics.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;

/// <summary>
/// A mode determiner that tries a determiner and fallbacks to another one. If the mode is not identified
/// and the default (<see langword="null"/>) is returned a fallback determiner is used instead.
/// </summary>
public class FallbackModeDeterminer : IModeDeterminer
{
	private readonly IModeDeterminer firstDeterminer;
	private readonly IModeDeterminer fallbackDeterminer;

	public FallbackModeDeterminer(IModeDeterminer firstDeterminer, IModeDeterminer fallbackDeterminer)
	{
		this.firstDeterminer = firstDeterminer;
		this.fallbackDeterminer = fallbackDeterminer;
	}

	public IReadOnlyList<Type> RequiredEventTypes => firstDeterminer.RequiredEventTypes.Concat(fallbackDeterminer.RequiredEventTypes).Distinct().ToList();
	public IReadOnlyList<uint> RequiredBuffSkillIds => firstDeterminer.RequiredBuffSkillIds.Concat(fallbackDeterminer.RequiredBuffSkillIds).Distinct().ToList();

	public EncounterMode? GetMode(Log log)
	{
		return firstDeterminer.GetMode(log) ?? fallbackDeterminer.GetMode(log) ?? EncounterMode.Unknown;
	}
}