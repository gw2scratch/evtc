using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters
{
	public class DefaultEncounter : BaseEncounter
	{
		public DefaultEncounter(Agent boss, IEnumerable<Event> events) : base(new[] {boss}, events,
			new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
			new AgentDeathResultDeterminer(boss),
			new AgentNameEncounterNameProvider(boss))
		{
		}
	}
}