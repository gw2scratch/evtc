using System.Collections.Generic;
using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Model.Encounters
{
	public interface IResultDeterminer
	{
		EncounterResult GetResult(IEnumerable<Event> events);
	}
}