using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

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

		public AgentBelowHealthThresholdDeterminer(Agent agent, float healthThreshold)
		{
			this.agent = agent;
			this.healthThreshold = healthThreshold;
		}

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