using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that results in success if an agent's health drops
	/// under a threshold at some point during the encounter.
	/// </summary>
	/// <remarks>
	/// The first such event is returned, be careful if using this for encounter duration,
	/// you may want to override the time.
	/// </remarks>
	public class AgentBelowHealthThresholdDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;
		private readonly float healthThreshold;

		/// <summary>
		/// Creates a new <see cref="AgentBelowHealthThresholdDeterminer"/>.
		/// </summary>
		/// <param name="agent">The agent to be checked.</param>
		/// <param name="healthThreshold">The health threshold that has to be reached,
		/// where 1 is 100% and 0 is 0% of maximum health of the <paramref name="agent"/>.</param>
		public AgentBelowHealthThresholdDeterminer(Agent agent, float healthThreshold)
		{
			this.agent = agent;
			this.healthThreshold = healthThreshold;
		}
		
		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentHealthUpdateEvent) };
		public override IReadOnlyList<uint> RequiredBuffSkillIds => new List<uint>();
		public override IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			foreach (var e in events)
			{
				if (e is AgentHealthUpdateEvent healthUpdate)
				{
					if (healthUpdate.Agent == agent && healthUpdate.HealthFraction <= healthThreshold)
					{
						return healthUpdate;
					}
				}
			}

			return null;
		}
	}
}