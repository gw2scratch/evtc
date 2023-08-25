using GW2Scratch.EVTCAnalytics.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health;

/// <summary>
/// Chooses a health from multiple determiners, depending on which boolean is true.
/// </summary>
/// <remarks>
/// This may seem unnecessary, but this allows to properly combine required event types if multiple different determiners
/// are used depending on which conditions are satisfied.
/// </remarks>
public class ConditionalHealthDeterminer : IHealthDeterminer
{
	private readonly (bool Chosen, IHealthDeterminer Determiner)[] determiners;

	public ConditionalHealthDeterminer(params (bool Chosen, IHealthDeterminer Determiner)[] determiners)
	{
		if (determiners.Length == 0)
		{
			throw new ArgumentException("At least one determiner has to be provided", nameof(determiners));
		}

		this.determiners = determiners;
	}

	public IReadOnlyList<Type> RequiredEventTypes => determiners.SelectMany(x => x.Determiner.RequiredEventTypes).Distinct().ToList();
	public IReadOnlyList<uint> RequiredBuffSkillIds => determiners.SelectMany(x => x.Determiner.RequiredBuffSkillIds).Distinct().ToList();

	public float? GetMainEnemyHealthFraction(Log log)
	{
		foreach ((bool chosen, IHealthDeterminer determiner) in determiners)
		{
			if (chosen)
			{
				return determiner.GetMainEnemyHealthFraction(log);
			}
		}

		return null;
	}
}