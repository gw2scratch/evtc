using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health;

/// <summary>
/// A health determiner that runs a provided function. Useful for determiners that are inherently not reusable.
/// </summary>
/// <remarks>
/// <see cref="EncounterIdentifierBuilder" /> provides an overload which creates this automatically.
/// </remarks>
public class AdHocHealthDeterminer : IHealthDeterminer
{
	private readonly Func<Log, float?> func;
	public IReadOnlyList<Type> RequiredEventTypes { get; }

	public AdHocHealthDeterminer(Func<Log, float?> func, IReadOnlyList<Type> requiredEventTypes)
	{
		this.func = func;
		RequiredEventTypes = requiredEventTypes;
	}

	public float? GetMainEnemyHealthFraction(Log log)
	{
		return func(log);
	}
}