using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	/// <summary>
	/// Determines encounter mode according to max health values of an agent.
	/// </summary>
	public class AgentHealthModeDeterminer : IModeDeterminer
	{
		private readonly Agent agent;
		private readonly ulong maxHealth;
		private readonly EncounterMode enoughHealthMode;
		private readonly EncounterMode lessHealthMode;

		public AgentHealthModeDeterminer(
			Agent agent,
			ulong maxHealth,
			EncounterMode enoughHealthMode = EncounterMode.Challenge,
			EncounterMode lessHealthMode = EncounterMode.Normal)
		{
			this.agent = agent;
			this.maxHealth = maxHealth;
			this.enoughHealthMode = enoughHealthMode;
			this.lessHealthMode = lessHealthMode;
		}

		public EncounterMode GetMode(Log log)
		{
			if (agent == null)
			{
				return EncounterMode.Unknown;
			}

			foreach (var maxHealthUpdate in log.Events.OfType<AgentMaxHealthUpdateEvent>().Where(x => x.Agent == agent))
			{
				if (maxHealthUpdate.NewMaxHealth >= maxHealth)
				{
					return enoughHealthMode;
				}
			}

			return lessHealthMode;
		}
	}
}