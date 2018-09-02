namespace ScratchEVTCParser.Model
{
	public class Enemy : Agent
	{
		public Enemy(int id, string name, int toughness, int concentration, int healing, int condition, int hitboxWidth, int hitboxHeight) : base(id, name, toughness, concentration, healing, condition, hitboxWidth, hitboxHeight)
		{
		}
	}
}