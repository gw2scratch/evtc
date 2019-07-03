using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Phases
{
	public class AgentEventTrigger<T> : IPhaseTrigger where T : AgentEvent
	{
		private readonly Agent agent;

		public PhaseDefinition PhaseDefinition { get; }

		public AgentEventTrigger(Agent agent, PhaseDefinition phaseDefinition)
		{
			this.agent = agent;
			PhaseDefinition = phaseDefinition;
		}

		public bool IsTrigger(Event e)
		{
			if (e is T agentEvent)
			{
				return agentEvent.Agent == agent;
			}

			return false;
		}
	}
}