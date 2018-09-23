using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Model.Encounters.Phases
{
	public class BuffAddTrigger : IPhaseTrigger
	{
		private readonly Agent agent;
		private readonly Skill buff;

		public string PhaseName { get; }

		public BuffAddTrigger(Agent agent, Skill buff, string phaseName)
		{
			this.agent = agent;
			this.buff = buff;
			PhaseName = phaseName;
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