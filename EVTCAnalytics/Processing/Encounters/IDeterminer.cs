using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters;

public interface IDeterminer
{
	/// <summary>
	/// A list of all event types that are required for this determiner to work.
	/// </summary>
	IReadOnlyList<Type> RequiredEventTypes { get; }
}