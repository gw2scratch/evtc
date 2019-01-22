using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Statistics.Encounters.Phases
{
	public class BuffAddTrigger : IPhaseTrigger
	{
		private readonly Agent agent;
		private readonly Skill buff;

		public PhaseDefinition PhaseDefinition { get; }

		public BuffAddTrigger(Agent agent, Skill buff, PhaseDefinition phaseDefinition)
		{
			this.agent = agent;
			this.buff = buff;
			PhaseDefinition = phaseDefinition;
		}

		public bool IsTrigger(Event e)
		{
			if (e is BuffApplyEvent buffApplyEvent)
			{
				return buffApplyEvent.Agent == agent && buffApplyEvent.Buff == buff;
			}

			return false;
		}
	}
}