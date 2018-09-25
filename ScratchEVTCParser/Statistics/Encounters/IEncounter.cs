using System.Collections.Generic;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics.Encounters.Phases;

namespace ScratchEVTCParser.Statistics.Encounters
{
	public interface IEncounter
	{
		IEnumerable<Agent> ImportantAgents { get; }

		EncounterResult GetResult();
		IEnumerable<Phase> GetPhases();
	}
}