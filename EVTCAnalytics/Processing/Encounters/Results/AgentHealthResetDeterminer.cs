using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// Returns success if an agent resets their health
	/// </summary>
	public class AgentHealthResetDeterminer : IResultDeterminer
	{
		private readonly Agent agent;

		public AgentHealthResetDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
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
					return EncounterResult.Success;
				}

				lastHealth = healthUpdate.HealthFraction;
			}

			return EncounterResult.Failure;
		}
	}
}