using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics.Encounters.Phases
{
	public class MultipleAgentsDeadTrigger : IPhaseTrigger
	{
		private readonly Agent[] agents;
		private readonly bool[] dead;
		private int deadCount = 0;

		public MultipleAgentsDeadTrigger(PhaseDefinition phaseDefinition, params Agent[] agents)
		{
			this.agents = agents;
			PhaseDefinition = phaseDefinition;
			dead = new bool[agents.Length];
		}

		public bool IsTrigger(Event e)
		{
			if (e is AgentDeadEvent agentDeadEvent)
			{
				for (int i = 0; i < agents.Length; i++)
				{
					if (agents[i] == agentDeadEvent.Agent)
					{
						if (!dead[i]) deadCount++;
						dead[i] = true;
					}
				}
			}

			return deadCount == agents.Length;
		}

		public PhaseDefinition PhaseDefinition { get; }
	}
}