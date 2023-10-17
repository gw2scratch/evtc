using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if an agent has cast a specified skill
	/// in a specified timespan around a specified buff application.
	/// </summary>
	public class SkillCastAroundBuffApplyDeterminer : EventFoundResultDeterminer
	{
		private readonly Func<Agent, bool> agentSelector;
		private readonly int skillId;
		private readonly uint buffId;
		private readonly long timeSpan;

		/// <summary>
		/// Creates a new <see cref="BuffAppliedAfterSkillCastDeterminer"/>.
		/// </summary>
		/// <param name="agentSelector">A delegate that should return true for *all* agents that have to have a skill cast following the buff apply.</param>
		/// <param name="skillId">The ID of the skill that has to be cast.</param>
		/// <param name="buffId">The ID of the buff skill that has to be applied.</param>
		/// <param name="timeSpan">The timespan in milliseconds between the skill cast and the buff application</param>
		public SkillCastAroundBuffApplyDeterminer(Func<Agent, bool> agentSelector, int skillId, uint buffId, long timeSpan)
		{
			this.agentSelector = agentSelector;
			this.skillId = skillId;
			this.buffId = buffId;
			this.timeSpan = timeSpan;
		}

		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(SkillCastEvent), typeof(BuffApplyEvent) };
		public override IReadOnlyList<uint> RequiredBuffSkillIds => new List<uint> { buffId };
		public override IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			var buffAppliesByAgent = new Dictionary<Agent, List<long>>();
			var skillCastsByAgent = new Dictionary<Agent, List<long>>();

			bool IsEventEligible(Dictionary<Agent, List<long>> timesByAgent, AgentEvent e)
			{
				if (timesByAgent.TryGetValue(e.Agent, out var times))
				{
					foreach (var time in times)
					{
						if (Math.Abs(time - e.Time) < timeSpan)
						{
							return true;
						}
					}
				}

				return false;
			}

			void SaveEvent(Dictionary<Agent, List<long>> timesByAgent, AgentEvent e)
			{
				if (!timesByAgent.ContainsKey(e.Agent))
				{
					timesByAgent[e.Agent] = new List<long>();
				}

				timesByAgent[e.Agent].Add(e.Time);
			}

			foreach (var e in events)
			{
				if (e is BuffApplyEvent buffApply && agentSelector(buffApply.Agent) && buffApply.Buff.Id == buffId)
				{
					if (IsEventEligible(skillCastsByAgent, buffApply))
					{
						return buffApply;
					}

					SaveEvent(buffAppliesByAgent, buffApply);
				}
				else if (e is SkillCastEvent skillCast && agentSelector(skillCast.Agent) && skillCast.Skill.Id == skillId)
				{
					if (IsEventEligible(buffAppliesByAgent, skillCast))
					{
						return skillCast;
					}
					SaveEvent(skillCastsByAgent, skillCast);
				}
			}

			return null;
		}
	}
}