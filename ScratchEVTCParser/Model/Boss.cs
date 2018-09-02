namespace ScratchEVTCParser.Model
{
	public class Boss : Enemy
	{
		public Boss(int id, string name, int toughness, int concentration, int healing, int condition, int hitboxWidth, int hitboxHeight) : base(id, name, toughness, concentration, healing, condition, hitboxWidth, hitboxHeight)
		{
		}
	}
}