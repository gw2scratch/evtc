using ScratchEVTCParser.Model.Agents;

namespace ArcdpsLogManager.Logs
{
	public class LogPlayer
	{
		public string Name { get; set; }
		public string AccountName { get; set; }
		public int Subgroup { get; set; }
		public Profession Profession { get; set; }
		public EliteSpecialization EliteSpecialization { get; set; }

		public LogPlayer(string name, string accountName, int subgroup, Profession profession,
			EliteSpecialization eliteSpecialization)
		{
			Name = name;
			AccountName = accountName;
			Subgroup = subgroup;
			Profession = profession;
			EliteSpecialization = eliteSpecialization;
		}
	}
}