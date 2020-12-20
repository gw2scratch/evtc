using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Parsed
{
	/// <summary>
	/// An object containing raw data from an EVTC log.
	/// </summary>
	public class ParsedLog
	{
		/// <summary>
		/// Gets data related to the version of arcdps.
		/// </summary>
		public LogVersion LogVersion { get; }
		
		/// <summary>
		/// Gets data related to the main target of the encounter.
		/// </summary>
		public ParsedBossData ParsedBossData { get; }
		
		/// <summary>
		/// Gets the list of all raw agents contained in the EVTC log.
		/// </summary>
		public List<ParsedAgent> ParsedAgents { get; }
		
		/// <summary>
		/// Gets the list of all raw skills contained in the EVTC log.
		/// </summary>
		public List<ParsedSkill> ParsedSkills { get; }
		
		/// <summary>
		/// Gets the list of all raw events contained in the EVTC log.
		/// </summary>
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