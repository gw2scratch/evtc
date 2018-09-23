using System.Collections.Generic;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters.Phases;

namespace ScratchEVTCParser.Model.Encounters
{
	public interface IEncounter
	{
		IEnumerable<Agent> ImportantAgents { get; }

		EncounterResult GetResult();
		IEnumerable<Phase> GetPhases();
	}
}