using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters
{
	public class AgentNameEncounterNameProvider : IEncounterNameProvider
	{
		private readonly Agent agent;

		public AgentNameEncounterNameProvider(Agent agent)
		{
			this.agent = agent;
		}

		public string GetEncounterName()
		{
			return agent.Name;
		}
	}
}