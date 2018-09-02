namespace ScratchEVTCParser.Parsed
{
	public class ParsedLog
	{
		public LogVersion LogVersion { get; }
		public ParsedBossData ParsedBossData { get; }
		public ParsedAgent[] ParsedAgents { get; }
		public ParsedSkill[] ParsedSkills { get; }
		public ParsedCombatItem[] ParsedCombatItems { get; }

		public ParsedLog(LogVersion logVersion, ParsedBossData bossData, ParsedAgent[] parsedAgents, ParsedSkill[] skills, ParsedCombatItem[] combatItems)
		{
			LogVersion = logVersion;
			ParsedBossData = bossData;
			ParsedAgents = parsedAgents;
			ParsedSkills = skills;
			ParsedCombatItems = combatItems;
		}
	}
}