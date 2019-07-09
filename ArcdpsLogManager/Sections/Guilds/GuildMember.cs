using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Sections.Guilds
{
	public class GuildMember
	{
        public string Name { get; }
        public IReadOnlyList<GuildCharacter> Characters { get; internal set; }
        public IReadOnlyList<LogData> Logs { get; }

        public GuildMember(string name, IEnumerable<LogData> logs)
        {
	        Name = name;
	        Logs = logs.ToArray();
        }
	}
}