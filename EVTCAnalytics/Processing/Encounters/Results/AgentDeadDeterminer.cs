using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// Returns success if the agent has a death event.
	/// </summary>
	public class AgentDeadDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;

		public AgentDeadDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			return events.OfType<AgentDeadEvent>().FirstOrDefault(x => x.Agent == agent);
		}
	}
}
