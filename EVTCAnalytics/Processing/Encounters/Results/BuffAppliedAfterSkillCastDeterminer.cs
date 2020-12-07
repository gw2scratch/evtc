using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	public class BuffAppliedAfterSkillCastDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;
		private readonly int skillId;
		private readonly int buffId;

		public BuffAppliedAfterSkillCastDeterminer(Agent agent, int skillId, int buffId)
		{
			this.agent = agent;
			this.skillId = skillId;
			this.buffId = buffId;
		}

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			bool cast = false;
			foreach (var e in events)
			{
				if (!cast && e is SkillCastEvent skillCast && skillCast.Agent == agent && skillCast.Skill.Id == skillId)
				{
					cast = true;
				}
				else if (cast && e is BuffApplyEvent buffApply && buffApply.Agent == agent && buffApply.Buff.Id == buffId)
				{
					return buffApply;
				}
			}

			return null;
		}
	}
}