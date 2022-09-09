using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health;

/// <summary>
/// A health determiner that takes the last health amount reached by an agent.
/// </summary>
public class AgentHealthDeterminer : IHealthDeterminer
{
	private readonly Agent agent;

	public AgentHealthDeterminer(Agent agent)
	{
		this.agent = agent;
	}

	public float? GetMainEnemyHealthFraction(Log log)
	{
		var healthPercentage = log.Events.OfType<AgentHealthUpdateEvent>()
			.Where(e => e.Agent == agent)
			.Select(a => (float?) a.HealthFraction)
			.Reverse()
			.Take(2)
			.DefaultIfEmpty(1)
			.Min();

		return healthPercentage;
	}
}