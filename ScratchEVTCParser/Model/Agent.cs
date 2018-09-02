namespace ScratchEVTCParser.Model
{
	public abstract class Agent
	{
		private int Id { get; }
		public string Name { get; }
		public int Toughness { get; }
		public int Concentration { get; }
		public int Healing { get; }
		public int Condition { get; }
		public int HitboxWidth { get; }
		public int HitboxHeight { get; }

		protected Agent(int id, string name, int toughness, int concentration, int healing, int condition, int hitboxWidth, int hitboxHeight)
		{
			Id = id;
			Name = name;
			Toughness = toughness;
			Concentration = concentration;
			Healing = healing;
			Condition = condition;
			HitboxWidth = hitboxWidth;
			HitboxHeight = hitboxHeight;
		}
	}
}