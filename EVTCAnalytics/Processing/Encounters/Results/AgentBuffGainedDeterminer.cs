using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that results in success if an agent gains a specified buff.
	/// </summary>
	public class AgentBuffGainedDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;
		private readonly int buffId;

		public AgentBuffGainedDeterminer(Agent agent, int buffId)
		{
			this.agent = agent;
			this.buffId = buffId;
		}

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			return events.OfType<BuffApplyEvent>().FirstOrDefault(x => x.Agent == agent && x.Buff.Id == buffId);
		}
	}
}
