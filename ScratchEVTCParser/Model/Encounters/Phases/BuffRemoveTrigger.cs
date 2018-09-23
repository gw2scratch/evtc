using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Model.Encounters.Phases
{
	public class BuffRemoveTrigger : IPhaseTrigger
	{
		private readonly Agent agent;
		private readonly Skill buff;

		public string PhaseName { get; }

		public BuffRemoveTrigger(Agent agent, Skill buff, string phaseName)
		{
			this.agent = agent;
			this.buff = buff;
			PhaseName = phaseName;
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