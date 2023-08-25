using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if an agent changes team
	/// when the agent's health is below a specified threshold.
	/// </summary>
	public class TeamChangedBelowHealthThresholdDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;
		private readonly float healthThreshold;

		/// <summary>
		/// Creates a new <see cref="TeamChangedBelowHealthThresholdDeterminer"/>.
		/// </summary>
		/// <param name="agent">The agent to be considered.</param>
		/// <param name="healthThreshold">The health threshold that has to be reached,
		/// where 1 is 100% and 0 is 0% of maximum health of the <paramref name="agent"/>.</param>
		public TeamChangedBelowHealthThresholdDeterminer(Agent agent, float healthThreshold)
		{
			this.agent = agent;
			this.healthThreshold = healthThreshold;
		}
		
		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentHealthUpdateEvent), typeof(TeamChangeEvent) };
		public override IReadOnlyList<uint> RequiredBuffSkillIds { get; } = new List<uint>();
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
				else if (belowThreshold && e is TeamChangeEvent teamChange && teamChange.Agent == agent)
				{
					return teamChange;
				}
			}

			return null;
		}
	}
}