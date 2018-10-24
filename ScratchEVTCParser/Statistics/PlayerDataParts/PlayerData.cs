using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Statistics.PlayerDataParts
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

		public WeaponType LandSet1Weapon1 { get; }
		public WeaponType LandSet1Weapon2 { get; }
		public WeaponType LandSet2Weapon1 { get; }
		public WeaponType LandSet2Weapon2 { get; }
		public IEnumerable<SkillData> LandSet1WeaponSkills { get; }
		public IEnumerable<SkillData> LandSet2WeaponSkills { get; }

		public IEnumerable<PlayerBadge> Badges { get; }
		public PlayerRotation Rotation { get; }

		/// <summary>
		/// Creates a new instance of <see cref="PlayerData"/>.
		/// </summary>
		/// <param name="player">The player</param>
		/// <param name="downCount">Amount of downs in encounter.</param>
		/// <param name="deathCount">Amount of deaths in encounter.</param>
		/// <param name="rotation">The rotation of the player.</param>
		/// <param name="usedSkills">All used skills in the encounter. May be null.</param>
		/// <param name="healingSkills">All used healing skills in the encounter. May be null.</param>
		/// <param name="utilitySkills">All used utility skills in the encounter. May be null.</param>
		/// <param name="eliteSkills">All used elite skills in the encounter. May be null.</param>
		/// <param name="landSet1Weapon1">Main hand weapon in first land weapon set.</param>
		/// <param name="landSet1Weapon2">Off-hand weapon in first land weapon set.</param>
		/// <param name="landSet2Weapon1">Main hand weapon in second land weapon set.</param>
		/// <param name="landSet2Weapon2">Off-hand weapon in second land weapon set.</param>
		/// <param name="land1WeaponSkills">Weapon skills for first equipped land weapon set. null if unknown.</param>
		/// <param name="land2WeaponSkills">Weapon skills for second equipped land weapon set. null if unknown.</param>
		/// <param name="badges">Player badges showing interesting data about the player.</param>
		public PlayerData(Player player, int downCount, int deathCount, PlayerRotation rotation,
			IEnumerable<Skill> usedSkills, IEnumerable<SkillData> healingSkills, IEnumerable<SkillData> utilitySkills,
			IEnumerable<SkillData> eliteSkills, WeaponType landSet1Weapon1, WeaponType landSet1Weapon2,
			WeaponType landSet2Weapon1, WeaponType landSet2Weapon2, IEnumerable<SkillData> land1WeaponSkills,
			IEnumerable<SkillData> land2WeaponSkills, IEnumerable<PlayerBadge> badges)
		{
			Player = player;
			DownCount = downCount;
			DeathCount = deathCount;
			Rotation = rotation;
			LandSet1Weapon1 = landSet1Weapon1;
			LandSet1Weapon2 = landSet1Weapon2;
			LandSet2Weapon1 = landSet2Weapon1;
			LandSet2Weapon2 = landSet2Weapon2;
			Badges = badges?.ToArray() ?? Enumerable.Empty<PlayerBadge>();
			LandSet1WeaponSkills = land1WeaponSkills?.ToArray();
			LandSet2WeaponSkills = land2WeaponSkills?.ToArray();
			UsedSkills = usedSkills.ToArray();
			HealingSkills = healingSkills?.ToArray();
			UtilitySkills = utilitySkills?.ToArray();
			EliteSkills = eliteSkills?.ToArray();
		}
	}
}