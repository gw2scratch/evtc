namespace ScratchEVTCParser.Model
{
	public class NPC : Agent
	{
		public uint SpeciesId { get; }

		public NPC(ulong address, int id, string name, uint speciesId, int toughness, int concentration, int healing,
			int condition,
			int hitboxWidth, int hitboxHeight) : base(address, id, name, toughness, concentration, healing, condition,
			hitboxWidth, hitboxHeight)
		{
			SpeciesId = speciesId;
		}
	}
}