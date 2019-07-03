namespace GW2Scratch.EVTCAnalytics.Parsed
{
	public class ParsedAgent
	{
		public ulong Address { get; }
		public string Name { get; }
		public uint Prof { get; }
		public uint IsElite { get; }
		public int Toughness { get; }
		public int Concentration { get; }
		public int Healing { get; }
		public int Condition { get; }
		public int HitboxWidth { get; }
		public int HitboxHeight { get; }

		public ParsedAgent(ulong address, string name, uint prof, uint isElite, int toughness, int concentration, int healing, int condition, int hitboxWidth, int hitboxHeight)
		{
			Address = address;
			Name = name;
			Prof = prof;
			IsElite = isElite;
			Toughness = toughness;
			Concentration = concentration;
			Healing = healing;
			Condition = condition;
			HitboxWidth = hitboxWidth;
			HitboxHeight = hitboxHeight;
		}
	}
}