using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Model.Encounters
{
	public class AgentExitCombatDeterminer : IResultDeterminer
	{
		private readonly Agent agent;

		public AgentExitCombatDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			bool agentDead = events.OfType<AgentExitCombatEvent>().Any(x => x.Agent == agent);

			return agentDead ? EncounterResult.Success : EncounterResult.Failure;
		}
	}
}