using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that results in success if an agent exits combat.
	/// </summary>
	public class AgentCombatExitDeterminer : EventFoundResultDeterminer
	{
		public long MinTimeSinceSpawn { get; set; } = 0;

		private readonly Agent agent;

		public AgentCombatExitDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentEvent), typeof(AgentCombatExitDeterminer) };

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