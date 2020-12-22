using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// An interface for determiners which can ascertain the result of an encounter.
	/// </summary>
	public interface IResultDeterminer
	{
		ResultDeterminerResult GetResult(IEnumerable<Event> events);
	}
}