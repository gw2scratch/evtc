using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Parsed
{
	public class ParsedLog
	{
		public LogVersion LogVersion { get; }
		public ParsedBossData ParsedBossData { get; }
		public List<ParsedAgent> ParsedAgents { get; }
		public List<ParsedSkill> ParsedSkills { get; }
		public List<ParsedCombatItem> ParsedCombatItems { get; }

		public ParsedLog(
			LogVersion logVersion,
			ParsedBossData bossData,
			List<ParsedAgent> parsedAgents,
			List<ParsedSkill> skills,
			List<ParsedCombatItem> combatItems
		)
		{
			LogVersion = logVersion;
			ParsedBossData = bossData;
			ParsedAgents = parsedAgents;
			ParsedSkills = skills;
			ParsedCombatItems = combatItems;
		}
	}
}