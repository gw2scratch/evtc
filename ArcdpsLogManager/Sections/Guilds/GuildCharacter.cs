using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Sections.Guilds
{
	public class GuildCharacter
	{
		public GuildMember Account { get; }
        public string Name { get; }

        public IReadOnlyList<LogData> Logs { get; }

		public GuildCharacter(GuildMember account, string name, IEnumerable<LogData> logs)
		{
			Account = account;
			Name = name;
			Logs = logs.ToArray();
		}
	}
}