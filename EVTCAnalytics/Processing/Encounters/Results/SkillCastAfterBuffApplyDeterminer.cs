using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if a an agent has cast a specified skill
	/// after a specified buff is applied earlier in the fight.
	/// </summary>
	public class SkillCastAfterBuffApplyDeterminer : EventFoundResultDeterminer
	{
		private readonly Func<Agent, bool> agentSelector;
		private readonly int skillId;
		private readonly uint buffId;

		/// <summary>
		/// Creates a new <see cref="BuffAppliedAfterSkillCastDeterminer"/>.
		/// </summary>
		/// <param name="agentSelector">A delegate that should return true for *all* agents that have to have a skill cast following the buff apply.</param>
		/// <param name="skillId">The ID of the skill that has to be cast first.</param>
		/// <param name="buffId">The ID of the buff skill that has to be applied second.</param>
		public SkillCastAfterBuffApplyDeterminer(Func<Agent, bool> agentSelector, int skillId, uint buffId)
		{
			this.agentSelector = agentSelector;
			this.skillId = skillId;
			this.buffId = buffId;
		}

		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(SkillCastEvent), typeof(BuffApplyEvent) };
		public override IReadOnlyList<uint> RequiredBuffSkillIds => new List<uint> { buffId };
		
		protected override Event GetEvent(IEnumerable<Event> events)
		{
			var buffApplied = new HashSet<Agent>();
			foreach (var e in events)
			{
				if (e is BuffApplyEvent buffApply && agentSelector(buffApply.Agent) && buffApply.Buff.Id == buffId)
				{
					buffApplied.Add(buffApply.Agent);
				}
				else if (e is SkillCastEvent skillCast && agentSelector(skillCast.Agent) && skillCast.Skill.Id == skillId)
				{
					buffApplied.Remove(skillCast.Agent);
					if (buffApplied.Count == 0)
					{
						return skillCast;
					}
				}
			}

			return null;
		}
	}
}