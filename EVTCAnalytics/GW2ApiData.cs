using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GW2Scratch.EVTCAnalytics.GW2Api.V2;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GW2Scratch.EVTCAnalytics
{
	public class GW2ApiData
	{
		private readonly Dictionary<int, SkillData> skillDataBySkillId;

		public IEnumerable<SkillData> SkillData => skillDataBySkillId.Values;

		private GW2ApiData(IEnumerable<SkillData> skills)
		{
			skillDataBySkillId = skills.ToDictionary(x => x.Id);
		}

		private GW2ApiData(Dictionary<int, SkillData> skillDataBySkillId)
		{
			this.skillDataBySkillId = skillDataBySkillId;
		}

		public SkillData GetSkillData(int skillId)
		{
			if (!skillDataBySkillId.TryGetValue(skillId, out var data))
			{
				data = null;
			}

			return data;
		}

		public SkillData GetSkillData(Skill skill)
		{
			return GetSkillData((int)skill.Id);
		}

		public async Task SaveToFileAsync(string filepath)
		{
			using (var writer = new JsonTextWriter(new StreamWriter(new FileStream(filepath, FileMode.OpenOrCreate,
				FileAccess.Write, FileShare.None, 4096, true))))
			{
				await JObject.FromObject(skillDataBySkillId).WriteToAsync(writer);
			}
		}

		public static async Task<GW2ApiData> LoadFromFileAsync(string filepath)
		{
			Dictionary<int, SkillData> dataDictionary;
			using (var reader = new JsonTextReader(new StreamReader(new FileStream(filepath, FileMode.Open,
				FileAccess.Read, FileShare.Read, 4096, true))))
			{
				var obj = await JObject.LoadAsync(reader);
				dataDictionary = obj.ToObject<Dictionary<int, SkillData>>();
			}

			return new GW2ApiData(dataDictionary);
		}

		public static async Task<GW2ApiData> LoadFromApiAsync(ApiSkillRepository skillRepository)
		{
			var apiSkills = (await skillRepository.GetAllApiSkills()).ToArray();
			var skillData = new List<SkillData>();
			foreach (var apiSkill in apiSkills)
			{
				SkillType type;
				switch (apiSkill.Type)
				{
					case null:
					case "None":
						type = SkillType.None;
						break;
					case "Heal":
						type = SkillType.Heal;
						break;
					case "Elite":
						type = SkillType.Elite;
						break;
					case "Bundle":
						type = SkillType.Bundle;
						break;
					case "Profession":
						type = SkillType.Profession;
						break;
					case "Utility":
						type = SkillType.Utility;
						break;
					case "Weapon":
						type = SkillType.Weapon;
						break;
					default:
						type = SkillType.Other;
						break;
				}

				SkillSlot slot;
				switch (apiSkill.Slot)
				{
					case null:
						slot = SkillSlot.None;
						break;
					case "Weapon_1":
						slot = SkillSlot.Weapon1;
						break;
					case "Weapon_2":
						slot = SkillSlot.Weapon2;
						break;
					case "Weapon_3":
						slot = SkillSlot.Weapon3;
						break;
					case "Weapon_4":
						slot = SkillSlot.Weapon4;
						break;
					case "Weapon_5":
						slot = SkillSlot.Weapon5;
						break;
					case "Profession_1":
						slot = SkillSlot.Profession1;
						break;
					case "Profession_2":
						slot = SkillSlot.Profession2;
						break;
					case "Profession_3":
						slot = SkillSlot.Profession3;
						break;
					case "Profession_4":
						slot = SkillSlot.Profession4;
						break;
					case "Profession_5":
						slot = SkillSlot.Profession5;
						break;
					case "Downed_1":
						slot = SkillSlot.Downed1;
						break;
					case "Downed_2":
						slot = SkillSlot.Downed2;
						break;
					case "Downed_3":
						slot = SkillSlot.Downed3;
						break;
					case "Downed_4":
						slot = SkillSlot.Downed4;
						break;
					case "Utility":
						slot = SkillSlot.Utility;
						break;
					case "Heal":
						slot = SkillSlot.Heal;
						break;
					case "Elite":
						slot = SkillSlot.Elite;
						break;
					case "Toolbelt":
						slot = SkillSlot.Toolbelt;
						break;
					case "Pet":
						slot = SkillSlot.Pet;
						break;
					default:
						slot = SkillSlot.Other;
						break;
				}

				WeaponType weaponType;
				switch (apiSkill.WeaponType)
				{
					case null:
					case "None":
						weaponType = WeaponType.None;
						break;
					case "Dagger":
						weaponType = WeaponType.Dagger;
						break;
					case "Focus":
						weaponType = WeaponType.Focus;
						break;
					case "Staff":
						weaponType = WeaponType.Staff;
						break;
					case "Scepter":
						weaponType = WeaponType.Scepter;
						break;
					case "Sword":
						weaponType = WeaponType.Sword;
						break;
					case "Trident":
						weaponType = WeaponType.Trident;
						break;
					case "Pistol":
						weaponType = WeaponType.Pistol;
						break;
					case "Rifle":
						weaponType = WeaponType.Rifle;
						break;
					case "Shield":
						weaponType = WeaponType.Shield;
						break;
					case "Speargun":
						weaponType = WeaponType.Speargun;
						break;
					case "Greatsword":
						weaponType = WeaponType.Greatsword;
						break;
					case "Mace":
						weaponType = WeaponType.Mace;
						break;
					case "Torch":
						weaponType = WeaponType.Torch;
						break;
					case "Hammer":
						weaponType = WeaponType.Hammer;
						break;
					case "Spear":
						weaponType = WeaponType.Spear;
						break;
					case "Axe":
						weaponType = WeaponType.Axe;
						break;
					case "Warhorn":
						weaponType = WeaponType.Warhorn;
						break;
					case "Shortbow":
						weaponType = WeaponType.Shortbow;
						break;
					case "Longbow":
						weaponType = WeaponType.Longbow;
						break;
					default:
						weaponType = WeaponType.Other;
						break;
				}

				var professions = new List<Profession>();
				if (apiSkill.Professions != null)
				{
					foreach (var apiProfession in apiSkill.Professions)
					{
						Profession profession;
						switch (apiProfession)
						{
							case "Guardian":
								profession = Profession.Guardian;
								break;
							case "Warrior":
								profession = Profession.Warrior;
								break;
							case "Engineer":
								profession = Profession.Engineer;
								break;
							case "Ranger":
								profession = Profession.Ranger;
								break;
							case "Thief":
								profession = Profession.Thief;
								break;
							case "Elementalist":
								profession = Profession.Elementalist;
								break;
							case "Mesmer":
								profession = Profession.Mesmer;
								break;
							case "Necromancer":
								profession = Profession.Necromancer;
								break;
							case "Revenant":
								profession = Profession.Revenant;
								break;
							default:
								profession = Profession.None;
								break;
						}

						professions.Add(profession);
					}
				}

				SkillAttunement attunement;
				switch (apiSkill.Attunement)
				{
					case null:
						attunement = SkillAttunement.None;
						break;
					case "Air":
						attunement = SkillAttunement.Air;
						break;
					case "Fire":
						attunement = SkillAttunement.Fire;
						break;
					case "Water":
						attunement = SkillAttunement.Water;
						break;
					case "Earth":
						attunement = SkillAttunement.Earth;
						break;
					default:
						attunement = SkillAttunement.Other;
						break;
				}

				skillData.Add(new SkillData(apiSkill.Id, apiSkill.Name, apiSkill.Icon, type, weaponType, professions,
					slot, attunement));
			}

			/* TODO: This makes serialization significantly more difficult (although still doable)
			var skillDataById = skillData.ToDictionary(x => x.Id);
			foreach (var skill in apiSkills)
			{
				if (skill.PrevChain > 0 && skillDataById.TryGetValue(skill.PrevChain, out var prevSkill))
				{
					skillDataById[skill.Id].PrevChain = prevSkill;
				}
				if (skill.NextChain > 0 && skillDataById.TryGetValue(skill.NextChain, out var nextSkill))
				{
					skillDataById[skill.Id].NextChain = nextSkill;
				}
			}
			*/

			return new GW2ApiData(skillData);
		}

	}
}