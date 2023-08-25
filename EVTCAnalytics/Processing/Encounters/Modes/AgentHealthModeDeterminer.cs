using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	/// <summary>
	/// Determines encounter mode according to max health values of an agent.
	/// </summary>
	public class AgentHealthModeDeterminer : IModeDeterminer
	{
		private const ulong MaximumValidValue = 1152921504606846976; // 2^60; old logs sometimes have max health events with nonsensical values such as 2^63.

		private readonly Agent agent;
		private readonly ulong maxHealth;
		private readonly EncounterMode enoughHealthMode;

		public AgentHealthModeDeterminer(
			Agent agent,
			ulong maxHealth,
			EncounterMode enoughHealthMode = EncounterMode.Challenge)
		{
			this.agent = agent;
			this.maxHealth = maxHealth;
			this.enoughHealthMode = enoughHealthMode;
		}

		public IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentMaxHealthUpdateEvent) };
		public IReadOnlyList<uint> RequiredBuffSkillIds => new List<uint>();
		public IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

		public EncounterMode? GetMode(Log log)
		{
			if (agent == null)
			{
				return EncounterMode.Unknown;
			}
			foreach (var maxHealthUpdate in log.Events.OfType<AgentMaxHealthUpdateEvent>().Where(x => x.Agent == agent))
			{
				if (maxHealthUpdate.NewMaxHealth >= MaximumValidValue)
				{
					continue;
				}

				if (maxHealthUpdate.NewMaxHealth >= maxHealth)
				{
					return enoughHealthMode;
				}
			}

			return null;
		}
	}
}