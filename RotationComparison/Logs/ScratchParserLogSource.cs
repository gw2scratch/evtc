using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Names;
using GW2Scratch.EVTCAnalytics.Statistics;
using GW2Scratch.EVTCAnalytics.Statistics.RotationItems;
using GW2Scratch.RotationComparison.Rotations;
using RotationItem = GW2Scratch.RotationComparison.Rotations.RotationItem;
using SkillCastType = GW2Scratch.RotationComparison.Rotations.SkillCastType;

namespace GW2Scratch.RotationComparison.Logs
{
	public abstract class ScratchParserLogSource : ILogSource
	{
		private string[] characterNames;

		public void SetCharacterNameFilter(string[] names)
		{
			characterNames = names;
		}

		protected abstract Log GetLog();

		public IEnumerable<Rotation> GetRotations()
		{
			var log = GetLog();
			var players = log.Agents.OfType<Player>();
			if (characterNames != null)
			{
				players = players.Where(x => characterNames.Contains(x.Name));
			}

			var rotationCalculator = new RotationCalculator();
			foreach (var player in players)
			{
				var playerRotation = rotationCalculator.GetRotation(log, player);
				var items = new List<RotationItem>();
				foreach (var item in playerRotation.Items)
				{
					if (item is SkillCastItem skillCast)
					{
						items.Add(GetSkillCast(skillCast));
					}
					else if (item is WeaponSwapItem weaponSwap)
					{
						items.Add(GetWeaponSwap(weaponSwap));
					}
				}
				yield return new Rotation(player.Name, player.Profession, player.EliteSpecialization, items);
			}
		}

		private SkillCast GetSkillCast(SkillCastItem skillCastItem)
		{
			var time = skillCastItem.ItemTime;
			var skillId = skillCastItem.Skill.Id;
			var skillName = skillCastItem.Skill.Name;
			var duration = skillCastItem.Duration;
			SkillCastType type;
			switch (skillCastItem.Type)
			{
				case GW2Scratch.EVTCAnalytics.Statistics.RotationItems.SkillCastType.Success:
					type = SkillCastType.Success;
					break;
				case GW2Scratch.EVTCAnalytics.Statistics.RotationItems.SkillCastType.Cancel:
					type = SkillCastType.Cancel;
					break;
				case GW2Scratch.EVTCAnalytics.Statistics.RotationItems.SkillCastType.Reset:
					type = SkillCastType.Reset;
					break;
				default:
					type = SkillCastType.Unknown;
					break;
			}

			return new SkillCast(time, duration, type, skillId, skillName);
		}

		private WeaponSwap GetWeaponSwap(WeaponSwapItem weaponSwapItem)
		{
            return new WeaponSwap(weaponSwapItem.ItemTime, weaponSwapItem.NewWeaponSet);
		}

		public string GetEncounterName()
		{
			var log = GetLog();
			return new LocalizedEncounterNameProvider().GetEncounterName(log.EncounterData, log.GameLanguage);
		}


		public abstract string GetLogName();
	}
}