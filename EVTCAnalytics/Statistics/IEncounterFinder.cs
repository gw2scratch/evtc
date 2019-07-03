using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters;

namespace GW2Scratch.EVTCAnalytics.Statistics
{
	public interface IEncounterFinder
	{
		IEncounter GetEncounter(Log log);
	}
}