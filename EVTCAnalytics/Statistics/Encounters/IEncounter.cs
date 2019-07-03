using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters
{
	public interface IEncounter
	{
		IEnumerable<Agent> ImportantEnemies { get; }

		EncounterResult GetResult();
		IEnumerable<Phase> GetPhases();
		string GetName();
	}
}