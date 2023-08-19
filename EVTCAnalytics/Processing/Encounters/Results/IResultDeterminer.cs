using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// An interface for determiners which can ascertain the result of an encounter.
	/// </summary>
	public interface IResultDeterminer
	{
		/// <summary>
		/// Determines the result of the encounter from its events. 
		/// </summary>
		/// <param name="events">The events in an encounter.</param>
		ResultDeterminerResult GetResult(IEnumerable<Event> events);
		
		/// <summary>
		/// A list of all event types that are required for this determiner to work.
		/// </summary>
		IReadOnlyList<Type> RequiredEventTypes { get; }
	}
}