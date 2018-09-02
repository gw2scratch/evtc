namespace ScratchEVTCParser.Model
{
	public class NPC : Agent
	{
		public NPC(ulong address, int id, string name, int toughness, int concentration, int healing, int condition,
			int hitboxWidth, int hitboxHeight) : base(address, id, name, toughness, concentration, healing, condition,
			hitboxWidth, hitboxHeight)
		{
		}
	}
}