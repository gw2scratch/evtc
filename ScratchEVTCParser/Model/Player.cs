namespace ScratchEVTCParser.Model
{
	public class Player : Agent
	{
		public string AccountName { get; }
		public int Subgroup { get; }
		public Profession Profession { get; }
		public EliteSpecialization EliteSpecialization { get; }
		public int Toughness { get; }
		public int Concentration { get; }
		public int Healing { get; }
		public int Condition { get; }

		public Player(ulong address, int id, string name, int toughness, int concentration, int healing, int condition,
			int hitboxWidth, int hitboxHeight, string accountName, Profession profession, EliteSpecialization eliteSpecialization, int subgroup)
			: base(address, id, name, hitboxWidth, hitboxHeight)
		{
			Toughness = toughness;
			Concentration = concentration;
			Healing = healing;
			Condition = condition;

			Subgroup = subgroup;
			AccountName = accountName;
			Profession = profession;
			EliteSpecialization = eliteSpecialization;
		}
	}
}