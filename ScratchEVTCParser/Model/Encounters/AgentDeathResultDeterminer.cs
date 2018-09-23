using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Model.Encounters
{
	public class AgentDeathResultDeterminer : IResultDeterminer
	{
		private readonly Agent agent;

		public AgentDeathResultDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			bool agentDead = events.OfType<AgentDeadEvent>().Any(x => x.Agent == agent);

			return agentDead ? EncounterResult.Success : EncounterResult.Failure;
		}
	}
}