namespace ScratchEVTCParser.Model
{
	public class Boss : NPC
	{
		public Boss(ulong address, int id, string name, int seriesId, int toughness, int concentration, int healing, int condition,
			int hitboxWidth, int hitboxHeight) : base(address, id, name, seriesId, toughness, concentration, healing, condition,
			hitboxWidth, hitboxHeight)
		{
		}
	}
}