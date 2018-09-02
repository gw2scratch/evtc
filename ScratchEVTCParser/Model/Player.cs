namespace ScratchEVTCParser.Model
{
	public class Player : Agent
	{
		public string AccountName { get; }
		public Profession Profession { get; }
		public EliteSpecialization EliteSpecialization { get; }

		public Player(int id, string name, int toughness, int concentration, int healing, int condition,
			int hitboxWidth, int hitboxHeight, string accountName, Profession profession, EliteSpecialization eliteSpecialization) : base(id, name, toughness, concentration,
			healing, condition, hitboxWidth, hitboxHeight)
		{
			AccountName = accountName;
			Profession = profession;
			EliteSpecialization = eliteSpecialization;
		}
	}
}