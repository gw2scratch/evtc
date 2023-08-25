using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if a specified buff is applied to an agent
	/// when the agent's health is below a specified threshold.
	/// </summary>
	public class BuffAppliedBelowHealthThresholdDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;
		private readonly float healthThreshold;
		private readonly uint buffId;

		/// <summary>
		/// Creates a new <see cref="BuffAppliedBelowHealthThresholdDeterminer"/>.
		/// </summary>
		/// <param name="agent">The agent to be considered.</param>
		/// <param name="healthThreshold">The health threshold that has to be reached,
		/// where 1 is 100% and 0 is 0% of maximum health of the <paramref name="agent"/>.</param>
		/// <param name="buffId">The ID of the buff skill that has to be applied second.</param>
		public BuffAppliedBelowHealthThresholdDeterminer(Agent agent, float healthThreshold, uint buffId)
		{
			this.agent = agent;
			this.healthThreshold = healthThreshold;
			this.buffId = buffId;
		}

		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentHealthUpdateEvent), typeof(BuffApplyEvent) };
		public override IReadOnlyList<uint> RequiredBuffSkillIds => new List<uint> { buffId };
		public override IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			bool belowThreshold = false;
			foreach (var e in events)
			{
				if (e is AgentHealthUpdateEvent healthUpdate && healthUpdate.Agent == agent)
				{
					belowThreshold = healthUpdate.HealthFraction <= healthThreshold;
				}
				else if (belowThreshold && e is BuffApplyEvent buffApply && buffApply.Agent == agent && buffApply.Buff.Id == buffId)
				{
					return buffApply;
				}
			}

			return null;
		}
	}
}