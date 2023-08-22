using GW2Scratch.EVTCAnalytics.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Transformers;

/// <summary>
/// Chooses a result from multiple determiners, depending on which boolean is true.
/// </summary>
/// <remarks>
/// This may seem unnecessary, but this allows to properly combine required event types if multiple different determiners
/// are used depending on which conditions are satisfied.
/// </remarks>
public class ConditionalResultDeterminer : IResultDeterminer
{
	private readonly (bool Chosen, IResultDeterminer Determiner)[] determiners;

	public ConditionalResultDeterminer(params (bool Chosen, IResultDeterminer Determiner)[] determiners)
	{
		if (determiners.Length == 0)
		{
			throw new ArgumentException("At least one determiner has to be provided", nameof(determiners));
		}

		this.determiners = determiners;
	}

	public IReadOnlyList<Type> RequiredEventTypes => determiners.SelectMany(x => x.Determiner.RequiredEventTypes).Distinct().ToList();

	public ResultDeterminerResult GetResult(IEnumerable<Event> events)
	{
		foreach ((bool chosen, IResultDeterminer determiner) in determiners)
		{
			if (chosen)
			{
				return determiner.GetResult(events);
			}
		}

		return new ResultDeterminerResult(EncounterResult.Unknown, null);
	}
}