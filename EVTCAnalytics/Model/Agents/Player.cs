namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	public class Player : Agent
	{
		/// <summary>
		/// A value indicating whether all values are available. If false, <see cref="Agent.Name"/>,
		/// <see cref="AccountName"/> and <see cref="Subgroup"/> will likely be random values. This
		/// is most common in WvW on enemies as they are anonymized.
		/// </summary>
		public bool Identified { get; }

		public string AccountName { get; }
		public int Subgroup { get; }
		public Profession Profession { get; }
		public EliteSpecialization EliteSpecialization { get; }
		public int Toughness { get; }
		public int Concentration { get; }
		public int Healing { get; }
		public int Condition { get; }
		public byte[] GuildGuid { get; internal set; }

		public Player(AgentOrigin agentOrigin, string name, int toughness, int concentration, int healing, int condition,
			int hitboxWidth, int hitboxHeight, string accountName, Profession profession,
			EliteSpecialization eliteSpecialization, int subgroup, bool identified)
			: base(agentOrigin, name, hitboxWidth, hitboxHeight)
		{
			Toughness = toughness;
			Concentration = concentration;
			Healing = healing;
			Condition = condition;

			Identified = identified;
			Subgroup = subgroup;
			AccountName = accountName;
			Profession = profession;
			EliteSpecialization = eliteSpecialization;
		}
	}
}