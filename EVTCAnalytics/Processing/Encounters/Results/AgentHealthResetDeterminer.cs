using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if an agent resets their health
	/// </summary>
	public class AgentHealthResetDeterminer : IResultDeterminer
	{
		private readonly Agent agent;

		public AgentHealthResetDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentHealthUpdateEvent) };
		public IReadOnlyList<uint> RequiredBuffSkillIds { get; } = new List<uint>();
		public IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			float lastHealth = 1f;
			foreach (var healthUpdate in events.OfType<AgentHealthUpdateEvent>())
			{
				if (healthUpdate.Agent != agent)
				{
					continue;
				}

				bool healthResetToFull = Math.Abs(healthUpdate.HealthFraction - 1f) <= 0.00001f;
				if (healthUpdate.HealthFraction > lastHealth && healthResetToFull)
				{
					return new ResultDeterminerResult(EncounterResult.Success, healthUpdate.Time);
				}

				lastHealth = healthUpdate.HealthFraction;
			}

			return new ResultDeterminerResult(EncounterResult.Failure, null);
		}
	}
}