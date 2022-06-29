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
		private readonly bool ignoreInitial;

		public AgentBuffGainedDeterminer(Agent agent, int buffId, bool ignoreInitial = true)
		{
			this.agent = agent;
			this.buffId = buffId;
			this.ignoreInitial = ignoreInitial;
		}

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			if (ignoreInitial)
			{
				return events
					.OfType<BuffApplyEvent>()
					.Where(x => x is not InitialBuffEvent)
					.FirstOrDefault(x => x.Agent == agent && x.Buff.Id == buffId);
			}
			else
			{
				return events
					.OfType<BuffApplyEvent>()
					.FirstOrDefault(x => x.Agent == agent && x.Buff.Id == buffId);
			}
		}
	}
}
