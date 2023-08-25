using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health
{
	/// <summary>
	/// A health determiner that takes the lowest health amount reached by an enemy,
	/// or the maximum of such amounts of there are multiple enemies.
	/// </summary>
	/// <remarks>
	/// This is useful when there are multiple enemies at the same time that all need to be defeated.
	/// </remarks>
	public class MaxMinHealthDeterminer : IHealthDeterminer
	{
		public virtual IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentHealthUpdateEvent) };
		public virtual IReadOnlyList<uint> RequiredBuffSkillIds { get; } = new List<uint>();
		public virtual IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();
		
		public virtual float? GetMainEnemyHealthFraction(Log log)
		{
			var targets = log.EncounterData.Targets.ToHashSet();
			if (targets.Count == 0)
			{
				return null;
			}

			float? healthPercentage = log.Events.OfType<AgentHealthUpdateEvent>()
				.Where(e => targets.Contains(e.Agent))
				.GroupBy(e => e.Agent)
				.Select(agentGroup => agentGroup
					.Select(agent => (float?) agent.HealthFraction)
					.DefaultIfEmpty(1)
					.Last()
				)
				.DefaultIfEmpty(null)
				.Max();

			return healthPercentage;
		}
	}
}