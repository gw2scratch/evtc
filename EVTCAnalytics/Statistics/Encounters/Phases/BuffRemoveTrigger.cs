using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Phases
{
	public class BuffRemoveTrigger : IPhaseTrigger
	{
		private readonly Agent agent;
		private readonly Skill buff;

		public PhaseDefinition PhaseDefinition { get; }

		public BuffRemoveTrigger(Agent agent, Skill buff, PhaseDefinition phaseDefinition)
		{
			this.agent = agent;
			this.buff = buff;
			PhaseDefinition = phaseDefinition;
		}

		public bool IsTrigger(Event e)
		{
			if (e is BuffRemoveEvent buffRemoveEvent)
			{
				return buffRemoveEvent.Agent == agent && buffRemoveEvent.Buff == buff;
			}

			return false;
		}
	}
}