using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCInspector
{
	public static class EventFilters
	{
		public static bool IsAgentInEvent(Event ev, Agent agent, bool ifSource = true, bool ifTarget = true)
		{
			if (ev is BuffEvent buffEvent)
			{
				var source = buffEvent.SourceAgent;
				var target = buffEvent.Agent;
				return (ifSource && agent == source) || (ifTarget && agent == target);
			}

			if (ev is EffectEvent effectEvent)
			{
				var source = effectEvent.EffectOwner;
				var target = effectEvent.AgentTarget;
				return (ifSource && agent == source) || (ifTarget && agent == target);
			}

			if (ev is EffectStartEvent effectStartEvent)
			{
				var source = effectStartEvent.EffectOwner;
				var target = effectStartEvent.AgentTarget;
				return (ifSource && agent == source) || (ifTarget && agent == target);
			}

			if (ev is AgentEvent agentEvent)
			{
				return agentEvent.Agent == agent;
			}

			if (ev is DamageEvent damageEvent)
			{
				var source = damageEvent.Attacker;
				var target = damageEvent.Defender;
				return (ifSource && agent == source) || (ifTarget && agent == target);
			}

			if (ev is CrowdControlEvent crowdControlEvent)
			{
				var source = crowdControlEvent.Attacker;
				var target = crowdControlEvent.Defender;
				return (ifSource && agent == source) || (ifTarget && agent == target);
			}

			return false;
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