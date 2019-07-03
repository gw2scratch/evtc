using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results
{
	public interface IResultDeterminer
	{
		EncounterResult GetResult(IEnumerable<Event> events);
	}
}