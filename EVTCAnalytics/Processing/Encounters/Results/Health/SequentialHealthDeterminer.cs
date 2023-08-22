using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health
{
	/// <summary>
	/// A health determiner that considers health of one agent
	/// until a second agent receives enough health updates.
	/// After that, the health updates of the first agent are ignored.
	/// </summary>
	/// <remarks>
	/// This is useful when one agent becomes invulnerable and a second one is fought instead.
	/// The initial health update is ignored, as it may stay stale from a previous encounter
	/// in the same instance if the second enemy is a <see cref="Gadget"/>.
	/// </remarks>
	public class SequentialHealthDeterminer : IHealthDeterminer
	{
		private readonly Agent firstEnemy;
		private readonly Agent secondEnemy;

		public SequentialHealthDeterminer(Agent firstEnemy, Agent secondEnemy)
		{
			this.firstEnemy = firstEnemy;
			this.secondEnemy = secondEnemy;
		}

		public IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentHealthUpdateEvent) };
		public IReadOnlyList<uint> RequiredBuffSkillIds { get; } = new List<uint>();

		public float? GetMainEnemyHealthFraction(Log log)
		{
			float? lastHealthUpdate = null;
			int secondaryUpdates = 0;
			foreach (var e in log.Events)
			{
				if (e is AgentHealthUpdateEvent healthUpdateEvent)
				{
					if (healthUpdateEvent.Agent == firstEnemy)
					{
						if (secondaryUpdates < 2)
						{
							lastHealthUpdate = healthUpdateEvent.HealthFraction;
						}
					}

					if (healthUpdateEvent.Agent == secondEnemy)
					{
						secondaryUpdates++;
						// Ignore the first health update, if the enemy is a gadget, the health
						// might be outdated from a previous encounter.
						if (secondaryUpdates > 1)
						{
							lastHealthUpdate = healthUpdateEvent.HealthFraction;
						}
					}
				}
			}

			return lastHealthUpdate;
		}
	}
}