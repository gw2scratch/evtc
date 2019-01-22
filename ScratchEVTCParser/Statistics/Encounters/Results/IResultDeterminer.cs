using System.Collections.Generic;
using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Statistics.Encounters.Results
{
	public interface IResultDeterminer
	{
		EncounterResult GetResult(IEnumerable<Event> events);
	}
}