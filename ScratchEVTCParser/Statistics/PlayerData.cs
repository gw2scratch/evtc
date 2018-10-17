using System;
using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Statistics
{
	public class PlayerData
	{
		public Player Player { get; }
		public int DownCount { get; }
		public int DeathCount { get; }
		public IEnumerable<Skill> UsedSkills { get; }
		public IEnumerable<SkillData> HealingSkills { get; }
		public IEnumerable<SkillData> UtilitySkills { get; }
		public IEnumerable<SkillData> EliteSkills { get; }

		public PlayerData(Player player, int downCount, int deathCount, IEnumerable<Skill> usedSkills,
			IEnumerable<SkillData> healingSkills, IEnumerable<SkillData> utilitySkills,
			IEnumerable<SkillData> eliteSkills)
		{
			Player = player;
			DownCount = downCount;
			DeathCount = deathCount;
			UsedSkills = usedSkills.ToArray();
			HealingSkills = healingSkills?.ToArray();
			UtilitySkills = utilitySkills?.ToArray();
			EliteSkills = eliteSkills?.ToArray();
		}
	}
}