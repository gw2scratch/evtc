namespace ScratchEVTCParser.Model.Agents
{
	public class NPC : Agent
	{
		public int SpeciesId { get; }
		public int Toughness { get; }
		public int Concentration { get; }
		public int Healing { get; }
		public int Condition { get; }

		public NPC(ulong address, int id, string name, int speciesId, int toughness, int concentration, int healing,
			int condition, int hitboxWidth, int hitboxHeight) : base(address, id, name, hitboxWidth, hitboxHeight)
		{
			SpeciesId = speciesId;
			Toughness = toughness;
			Concentration = concentration;
			Healing = healing;
			Condition = condition;
		}
	}
}