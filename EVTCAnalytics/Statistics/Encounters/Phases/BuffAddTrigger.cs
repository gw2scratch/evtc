using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Phases
{
	public class BuffAddTrigger : IPhaseTrigger
	{
		private readonly Agent agent;
		private readonly uint buffId;

		public PhaseDefinition PhaseDefinition { get; }

		public BuffAddTrigger(Agent agent, uint buffId, PhaseDefinition phaseDefinition)
		{
			this.agent = agent;
			this.buffId = buffId;
			PhaseDefinition = phaseDefinition;
		}

		public bool IsTrigger(Event e)
		{
			if (e is BuffApplyEvent buffApplyEvent)
			{
				return buffApplyEvent.Agent == agent && buffApplyEvent.Buff.Id == buffId;
			}

			return false;
		}
	}
}