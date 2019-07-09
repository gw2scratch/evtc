using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager.Logs
{
	public class LogPlayer
	{
		public string Name { get; set; }
		public string AccountName { get; set; }
		public int Subgroup { get; set; }
		public Profession Profession { get; set; }
		public EliteSpecialization EliteSpecialization { get; set; }
		public string GuildGuid { get; set; }

		public LogPlayer(string name, string accountName, int subgroup, Profession profession,
			EliteSpecialization eliteSpecialization, string guildGuid)
		{
			Name = name;
			AccountName = accountName;
			Subgroup = subgroup;
			Profession = profession;
			EliteSpecialization = eliteSpecialization;
			GuildGuid = guildGuid;
		}
	}
}