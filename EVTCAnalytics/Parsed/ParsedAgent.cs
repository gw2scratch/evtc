namespace GW2Scratch.EVTCAnalytics.Parsed
{
	public class ParsedAgent
	{
		public ulong Address { get; }
		public string Name { get; }
		public uint Prof { get; }
		public uint IsElite { get; }
		public short Toughness { get; }
		public short Concentration { get; }
		public short Healing { get; }
		public short Condition { get; }
		public short HitboxWidth { get; }
		public short HitboxHeight { get; }

		public ParsedAgent(ulong address, string name, uint prof, uint isElite, short toughness, short concentration, short healing, short condition, short hitboxWidth, short hitboxHeight)
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