using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Model.Encounters.Phases
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