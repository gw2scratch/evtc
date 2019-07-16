using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Phases
{
	public class BuffRemoveTrigger : IPhaseTrigger
	{
		private readonly Agent agent;
		private readonly uint buffId;

		public PhaseDefinition PhaseDefinition { get; }

		public BuffRemoveTrigger(Agent agent, uint buffId, PhaseDefinition phaseDefinition)
		{
			this.agent = agent;
			this.buffId = buffId;
			PhaseDefinition = phaseDefinition;
		}

		public bool IsTrigger(Event e)
		{
			if (e is BuffRemoveEvent buffRemoveEvent)
			{
				return buffRemoveEvent.Agent == agent && buffRemoveEvent.Buff.Id == buffId;
			}

			return false;
		}
	}
}