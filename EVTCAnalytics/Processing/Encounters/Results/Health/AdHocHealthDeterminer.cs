using GW2Scratch.EVTCAnalytics.Model;
using System;

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

	public AdHocHealthDeterminer(Func<Log, float?> func)
	{
		this.func = func;
	}

	public float? GetMainEnemyHealthFraction(Log log)
	{
		return func(log);
	}
}