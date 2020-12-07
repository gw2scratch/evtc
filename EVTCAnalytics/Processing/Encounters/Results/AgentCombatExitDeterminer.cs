using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	public class AgentCombatExitDeterminer : EventFoundResultDeterminer
	{
		public long MinTimeSinceSpawn { get; set; } = 0;

		private readonly Agent agent;

		public AgentCombatExitDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			long? firstTime = null;
			foreach (var e in events)
			{
				if (firstTime == null)
				{
					if (e is AgentEvent agentEvent && agentEvent.Agent == agent)
					{
						firstTime = agentEvent.Time;
					}
				}

				if (firstTime != null)
				{
					if (e is AgentExitCombatEvent combatExit && combatExit.Agent == agent && combatExit.Time > firstTime + MinTimeSinceSpawn)
					{
						return e;
					}
				}
			}

			return null;
		}
	}
}