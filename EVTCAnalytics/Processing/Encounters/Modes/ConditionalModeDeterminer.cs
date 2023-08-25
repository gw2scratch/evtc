using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;

/// <summary>
/// Chooses a mode from multiple determiners, depending on which boolean is true.
/// </summary>
/// <remarks>
/// This may seem unnecessary, but this allows to properly combine required event types if multiple different determiners
/// are used depending on which conditions are satisfied.
/// </remarks>
public class ConditionalModeDeterminer : IModeDeterminer
{
	private readonly (bool Chosen, IModeDeterminer Determiner)[] determiners;

	public ConditionalModeDeterminer(params (bool Chosen, IModeDeterminer Determiner)[] determiners)
	{
		if (determiners.Length == 0)
		{
			throw new ArgumentException("At least one determiner has to be provided", nameof(determiners));
		}

		this.determiners = determiners;
	}

	public IReadOnlyList<Type> RequiredEventTypes => determiners.SelectMany(x => x.Determiner.RequiredEventTypes).Distinct().ToList();
	public IReadOnlyList<uint> RequiredBuffSkillIds => determiners.SelectMany(x => x.Determiner.RequiredBuffSkillIds).Distinct().ToList();
	public IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults => determiners.SelectMany(x => x.Determiner.RequiredPhysicalDamageEventResults).Distinct().ToList();

	public EncounterMode? GetMode(Log log)
	{
		foreach ((bool chosen, IModeDeterminer determiner) in determiners)
		{
			if (chosen)
			{
				return determiner.GetMode(log);
			}
		}

		return null;
	}
}