using System.Collections.Generic;
using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Statistics.Encounters
{
	public interface IResultDeterminer
	{
		EncounterResult GetResult(IEnumerable<Event> events);
	}
}