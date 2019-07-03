using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results
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
