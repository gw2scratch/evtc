namespace GW2Scratch.EVTCAnalytics.Parsed
{
	/// <summary>
	/// The raw agent data from a <c>evtc_agent</c> struct as defined by arcdps.
	/// </summary>
	/// <remarks>
	/// The values of this struct encode values for different types of agents
	/// in various ways, see the arcdps EVTC readme for the main documentation.
	/// </remarks>
	public class ParsedAgent
	{
		/// <summary>
		/// Gets the address of the agent.
		/// </summary>
		public ulong Address { get; }
		
		/// <summary>
		/// Gets the name of the agent.
		/// </summary>
		public string Name { get; }
		
		/// <summary>
		/// Gets the profession value.
		/// </summary>
		public uint Prof { get; }
		
		/// <summary>
		/// Gets the elite value.
		/// </summary>
		public uint IsElite { get; }
		
		/// <summary>
		/// Gets the toughness stat value.
		/// </summary>
		public short Toughness { get; }
		
		/// <summary>
		/// Gets the concentration stat value.
		/// </summary>
		public short Concentration { get; }
		
		/// <summary>
		/// Gets the healing stat value.
		/// </summary>
		public short Healing { get; }
		
		/// <summary>
		/// Gets the condition stat value.
		/// </summary>
		public short Condition { get; }
		
		/// <summary>
		/// Gets the hitbox width.
		/// </summary>
		public short HitboxWidth { get; }
		
		/// <summary>
		/// Gets the hitbox height.
		/// </summary>
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