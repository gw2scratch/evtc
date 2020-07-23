using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	public class AgentCombatExitDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;

		public AgentCombatExitDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			return events.OfType<AgentExitCombatEvent>().FirstOrDefault(x => x.Agent == agent);
		}
	}
}