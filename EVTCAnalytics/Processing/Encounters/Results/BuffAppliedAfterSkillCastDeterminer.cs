using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if a specified buff is applied to an agent
	/// if the agent has cast a specified skill earlier in the fight.
	/// </summary>
	public class BuffAppliedAfterSkillCastDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;
		private readonly int skillId;
		private readonly int buffId;

		/// <summary>
		/// Creates a new <see cref="BuffAppliedAfterSkillCastDeterminer"/>.
		/// </summary>
		/// <param name="agent">The agent to be considered.</param>
		/// <param name="skillId">The ID of the skill that has to be cast first.</param>
		/// <param name="buffId">The ID of the buff skill that has to be applied second.</param>
		public BuffAppliedAfterSkillCastDeterminer(Agent agent, int skillId, int buffId)
		{
			this.agent = agent;
			this.skillId = skillId;
			this.buffId = buffId;
		}

		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(SkillCastEvent), typeof(BuffApplyEvent) };
		
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