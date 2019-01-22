using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics.Encounters.Phases
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