using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchLogBrowser
{
	public static class EventFilters
	{
		public static bool IsAgentInEvent(Event ev, string agentName)
		{
			if (!string.IsNullOrWhiteSpace(agentName))
			{
				if (ev is AgentEvent agentEvent)
				{
					var agent = agentEvent.Agent;
					if (agent == null)
					{
						return false;
					}

					return agent.Name.Contains(agentName);
				}
				else if (ev is DamageEvent damageEvent)
				{
					var attacker = damageEvent.Attacker;
					var defender = damageEvent.Defender;
					if (attacker == null || defender == null)
					{
						return false;
					}

					return attacker.Name.Contains(agentName) || defender.Name.Contains(agentName);
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public static bool IsAgentInEvent(Event ev, Agent agent, bool ifAttacker = true, bool ifDefender = true)
		{
			if (ev is AgentEvent agentEvent)
			{
				return agentEvent.Agent == agent;
			}
			else if (ev is DamageEvent damageEvent)
			{
				var attacker = damageEvent.Attacker;
				var defender = damageEvent.Defender;
				return (ifAttacker && agent == attacker) || (ifDefender && agent == defender);
			}
			else
			{
				return false;
			}
		}

		public static bool IsTypeName(Event ev, string eventName)
		{
			if (!string.IsNullOrWhiteSpace(eventName))
			{
				return ev.GetType().Name.Contains(eventName);
			}

			return true;
		}
	}
}