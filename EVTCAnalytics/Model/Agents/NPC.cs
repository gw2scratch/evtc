namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	public class NPC : Agent
	{
		public int SpeciesId { get; }
		public int Toughness { get; }
		public int Concentration { get; }
		public int Healing { get; }
		public int Condition { get; }

		public NPC(AgentOrigin agentOrigin, string name, int speciesId, int toughness, int concentration, int healing,
			int condition, int hitboxWidth, int hitboxHeight) : base(agentOrigin, name, hitboxWidth, hitboxHeight)
		{
			SpeciesId = speciesId;
			Toughness = toughness;
			Concentration = concentration;
			Healing = healing;
			Condition = condition;
		}
	}
}