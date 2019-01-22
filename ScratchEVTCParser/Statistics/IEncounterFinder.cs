using ScratchEVTCParser.Model;
using ScratchEVTCParser.Statistics.Encounters;

namespace ScratchEVTCParser.Statistics
{
	public interface IEncounterFinder
	{
		IEncounter GetEncounter(Log log);
	}
}