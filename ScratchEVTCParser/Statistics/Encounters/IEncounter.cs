using System.Collections.Generic;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics.Encounters.Phases;
using ScratchEVTCParser.Statistics.Encounters.Results;

namespace ScratchEVTCParser.Statistics.Encounters
{
	public interface IEncounter
	{
		IEnumerable<Agent> ImportantEnemies { get; }

		EncounterResult GetResult();
		IEnumerable<Phase> GetPhases();
		string GetName();
	}
}