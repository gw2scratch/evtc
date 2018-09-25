using System.Collections.Generic;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters.Phases;

namespace ScratchEVTCParser.Model.Encounters
{
	public class DefaultEncounter : BaseEncounter
	{
		public DefaultEncounter(Agent boss, IEnumerable<Event> events) : base(new[] {boss}, events,
			new PhaseSplitter(new StartTrigger("Default phase")), new AgentDeathResultDeterminer(boss),
			new AgentNameEncounterNameProvider(boss))
		{
		}
	}
}