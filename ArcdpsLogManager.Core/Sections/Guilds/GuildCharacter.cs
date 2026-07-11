using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager.Sections.Guilds
{
	public class GuildCharacter
	{
		public GuildMember Account { get; }
		public Profession Profession { get; }
		public string Name { get; }

		public IReadOnlyList<LogData> Logs { get; }

		public GuildCharacter(GuildMember account, Profession profession, string name, IEnumerable<LogData> logs)
		{
			Account = account;
			Profession = profession;
			Name = name;
			Logs = logs.ToArray();
		}
	}
}