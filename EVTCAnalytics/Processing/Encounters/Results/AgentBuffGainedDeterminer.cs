using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	public class AgentBuffGainedDeterminer : IResultDeterminer
	{
		private readonly Agent agent;
		private readonly int buffId;

		public AgentBuffGainedDeterminer(Agent agent, int buffId)
		{
			this.agent = agent;
			this.buffId = buffId;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			bool buffApplied = events.OfType<BuffApplyEvent>().Any(x => x.Agent == agent && x.Buff.Id == buffId);

			return buffApplied ? EncounterResult.Success : EncounterResult.Failure;
		}
	}
}
