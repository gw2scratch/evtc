using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics.Encounters
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