using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;

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
	}
}