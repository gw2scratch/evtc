using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	public interface IResultDeterminer
	{
		EncounterResult GetResult(IEnumerable<Event> events);
	}
}