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
		public float ConditionDamageRatio { get; }
		public IEnumerable<Skill> UsedSkills { get; }
		public IEnumerable<SkillData> HealingSkills { get; }
		public IEnumerable<SkillData> UtilitySkills { get; }
		public IEnumerable<SkillData> EliteSkills { get; }

		/// <summary>
		/// Creates a new instance of <see cref="PlayerData"/>.
		/// </summary>
		/// <param name="player">The player</param>
		/// <param name="downCount">Amount of downs in encounter.</param>
		/// <param name="deathCount">Amount of deaths in encounter.</param>
		/// <param name="conditionDamageFraction">A float between 0-1. Power damage fraction is 1-<paramref name="conditionDamageFraction"/>.</param>
		/// <param name="usedSkills">All used skills in the encounter. May be null.</param>
		/// <param name="healingSkills">All used healing skills in the encounter. May be null.</param>
		/// <param name="utilitySkills">All used utility skills in the encounter. May be null.</param>
		/// <param name="eliteSkills">All used elite skills in the encounter. May be null.</param>
		public PlayerData(Player player, int downCount, int deathCount, float conditionDamageFraction,
			IEnumerable<Skill> usedSkills, IEnumerable<SkillData> healingSkills, IEnumerable<SkillData> utilitySkills,
			IEnumerable<SkillData> eliteSkills)
		{
			Player = player;
			DownCount = downCount;
			DeathCount = deathCount;
			ConditionDamageRatio = conditionDamageFraction;
			UsedSkills = usedSkills.ToArray();
			HealingSkills = healingSkills?.ToArray();
			UtilitySkills = utilitySkills?.ToArray();
			EliteSkills = eliteSkills?.ToArray();
		}
	}
}