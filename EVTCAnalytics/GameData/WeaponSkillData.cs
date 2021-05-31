using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.GameData
{
	public class WeaponSkillData
	{
		private static readonly Dictionary<Profession, Dictionary<WeaponType, int[]>> WeaponSkills =
			new Dictionary<Profession, Dictionary<WeaponType, int[]>>
			{
				{
					Profession.Guardian, new Dictionary<WeaponType, int[]>
					{
						{WeaponType.Focus, new[] {9112, 9082}},
						{WeaponType.Staff, new[] {9122, 9140, 9143, 9265, 9144}},
						{WeaponType.Scepter, new[] {9098, 9090, 9099}},
						{WeaponType.Sword, new[] {9105, 9097, 9107}},
						{WeaponType.Trident, new[] {9205, 9207, 9208, 9209, 9206}},
						{WeaponType.Shield, new[] {9087, 9091}},
						{WeaponType.Greatsword, new[] {9137, 9081, 9080, 9146, 9147}},
						{WeaponType.Mace, new[] {9109, 9111, 9086}},
						{WeaponType.Torch, new[] {9104, 9088}},
						{WeaponType.Hammer, new[] {9159, 9194, 9260, 9124, 9195}},
						{WeaponType.Spear, new[] {9189, 9190, 9191, 9192, 9193}},
						{WeaponType.Axe, new[] {45047, 40624, 45402}},
						{WeaponType.Longbow, new[] {30471, 30229, 29630, 29789, 30628}}
					}
				},
				{
					Profession.Warrior, new Dictionary<WeaponType, int[]>
					{
						{WeaponType.Dagger, new[] {42745, 46233, 44937, 44004, 45160}},
						{WeaponType.Sword, new[] {14364, 14366, 14497, 14498, 14400}},
						{WeaponType.Rifle, new[] {14432, 14416, 14472, 14395, 14360}},
						{WeaponType.Shield, new[] {14361, 14362}},
						{WeaponType.Speargun, new[] {14552, 14466, 14481, 14467, 14465}},
						{WeaponType.Greatsword, new[] {14356, 14554, 14447, 14510, 14446}},
						{WeaponType.Mace, new[] {14376, 14507, 14503, 14518, 14415}},
						{WeaponType.Torch, new[] {29845, 29940}},
						{WeaponType.Hammer, new[] {14358, 14386, 14482, 14359, 14511}},
						{WeaponType.Spear, new[] {14437, 14440, 14448, 14441, 14480}},
						{WeaponType.Axe, new[] {14369, 14421, 14398, 14418, 14399}},
						{WeaponType.Warhorn, new[] {14393, 14394}},
						{WeaponType.Longbow, new[] {14431, 14519, 14381, 14505, 14504}}
					}
				},
				{
					Profession.Engineer, new Dictionary<WeaponType, int[]>
					{
						{WeaponType.Sword, new[] {43476, 44110, 40160}},
						{WeaponType.Pistol, new[] {5827, 5828, 5829, 5831, 5830}},
						{WeaponType.Rifle, new[] {6003, 6004, 6153, 6154, 6005}},
						{WeaponType.Shield, new[] {6053, 6054}},
						{WeaponType.Speargun, new[] {6148, 6147, 50380, 6149, 6145}},
						{WeaponType.Hammer, new[] {30501, 30088, 30665, 29840, 30713}}
					}
				},
				{
					Profession.Ranger, new Dictionary<WeaponType, int[]>
					{
						{WeaponType.Dagger, new[] {45426, 43536, 46123, 12478, 12477}},
						{WeaponType.Staff, new[] {31710, 31889, 31535, 31700, 31496}},
						{WeaponType.Sword, new[] {12471, 12482, 12481}},
						{WeaponType.Speargun, new[] {12526, 12529, 12528, 12527, 12530}},
						{WeaponType.Greatsword, new[] {12474, 12525, 12521, 12522, 12475}},
						{WeaponType.Torch, new[] {12635, 12504}},
						{WeaponType.Spear, new[] {12553, 12559, 12557, 12561, 12552}},
						{WeaponType.Axe, new[] {12466, 12480, 12490, 12638, 12639}},
						{WeaponType.Warhorn, new[] {12620, 12621}},
						{WeaponType.Shortbow, new[] {12470, 12468, 12517, 12507, 12508}},
						{WeaponType.Longbow, new[] {12510, 12509, 12573, 12511, 12469}}
					}
				},
				{
					// TODO: Dual Wield
					Profession.Thief, new Dictionary<WeaponType, int[]>
					{
						{WeaponType.Dagger, new[] {13004, 13097, -1, 13019, 16432}}, // 13006, 13040, 13110 - dual wield
						{WeaponType.Staff, new[] {30614, 29911, 30520, 30775, 30597}},
						{WeaponType.Sword, new[] {13009, 13015, -1}}, //13007, 13016, 13031, 13112 - dual wield
						{WeaponType.Pistol, new[] {13084, 13008, -1, 13012, 13113}}, // 13010, 13011, 13111 - dual wield
						{WeaponType.Rifle, new[] {41422, 41494, 43916, 41937, 40600}},
						{WeaponType.Speargun, new[] {13072, 13073, 13074, 13075, 13076}},
						{WeaponType.Spear, new[] {13119, 13069, 13122, 13070, 13068}},
						{WeaponType.Shortbow, new[] {13022, 13041, 13083, 13024, 13025}}
					}
				},
				{
					// TODO: Attunements. Right now only contains Fire attunement skills.
					Profession.Elementalist, new Dictionary<WeaponType, int[]>
					{
						{WeaponType.Dagger, new[] {15718, 5496, 5644, 5691, 5557}},
						{WeaponType.Focus, new[] {5497, 5678}},
						{WeaponType.Staff, new[] {5491, 5548, 5679, 5680, 5501}},
						{WeaponType.Scepter, new[] {5508, 5692, 5675}},
						{WeaponType.Sword, new[] {39964, 45313, 44451}},
						{WeaponType.Trident, new[] {5598, 5597, 5566, 5599, 5600}},
						{WeaponType.Warhorn, new[] {29548, 29533}}
					}
				},
				{
					Profession.Mesmer, new Dictionary<WeaponType, int[]>
					{
						{WeaponType.Focus, new[] {10186, 10282}},
						{WeaponType.Staff, new[] {10273, 10310, 10216, 10331, 10169}},
						{WeaponType.Scepter, new[] {10289, 10276, 10168}},
						{WeaponType.Sword, new[] {10170, 10334, 10173, 10280, 10174}},
						{WeaponType.Trident, new[] {10258, 10259, 10327, 10328, 10260}},
						{WeaponType.Pistol, new[] {10175, 10229}},
						{WeaponType.Shield, new[] {30769, 30643}},
						{WeaponType.Greatsword, new[] {10219, 10333, 10218, 10221, 10220}},
						{WeaponType.Torch, new[] {10285, 10189}},
						{WeaponType.Spear, new[] {10315, 10318, 10251, 10325, 10255}},
						{WeaponType.Axe, new[] {44791, 45243, 43761}}
					}
				},
				{
					Profession.Necromancer, new Dictionary<WeaponType, int[]>
					{
						{WeaponType.Dagger, new[] {10702, 10563, 10529, 10705, 10706}},
						{WeaponType.Focus, new[] {10707, 10555}},
						{WeaponType.Staff, new[] {10596, 19117, 10605, 19116, 19115}},
						{WeaponType.Scepter, new[] {10698, 10532, 10709}},
						{WeaponType.Trident, new[] {10623, 10624, 10625, 10628, 10629}},
						{WeaponType.Greatsword, new[] {29705, 30163, 30860, 29855, 29740}},
						{WeaponType.Torch, new[] {45846, 44296}},
						{WeaponType.Spear, new[] {10692, 10694, 10619, 10695, 10616}},
						{WeaponType.Axe, new[] {10561, 10528, 10701}},
						{WeaponType.Warhorn, new[] {10556, 10557}}
					}
				},
				{
					Profession.Revenant, new Dictionary<WeaponType, int[]>
					{
						{WeaponType.Staff, new[] {29180, 29145, 29288, 29321, 28978}},
						{WeaponType.Sword, new[] {29057, 29233, 26699, 28472, 27074}},
						{WeaponType.Trident, new[] {50395, 50456, 50390, 50410, 50483}},
						{WeaponType.Shield, new[] {29386, 28262}},
						{WeaponType.Mace, new[] {27066, 28357, 27964}},
						{WeaponType.Hammer, new[] {28549, 28253, 27976, 27665, 28110}},
						{WeaponType.Spear, new[] {28714, 28915, 28827, 28692, 28930}},
						{WeaponType.Axe, new[] {28029, 28409}},
						{WeaponType.Shortbow, new[] {40497, 40175, 41829, 43993, 41820}}
					}
				},
				{
					Profession.None, new Dictionary<WeaponType, int[]>()
				}
			};

		private IEnumerable<int> GetOffHandSkillIds(Profession profession, WeaponType weapon)
		{
			var skills = WeaponSkills[profession][weapon];

			if (skills.Length == 5)
			{
				return skills.Skip(3).Take(2);
			}
			else if (skills.Length == 2)
			{
				return skills;
			}

			throw new ArgumentException("This weapon is not an off-hand or skill ids are missing", nameof(weapon));
		}

		private IEnumerable<int> GetMainHandSkillIds(Profession profession, WeaponType weapon)
		{
			var skills = WeaponSkills[profession][weapon];

			if (skills.Length == 5)
			{
				return skills.Take(3);
			}
			else if (skills.Length == 3)
			{
				return skills;
			}

			throw new ArgumentException("This weapon is not a main hand weapon or skill ids are missing",
				nameof(weapon));
		}

		/// <summary>
		/// Returns weapon skill ids for a weapon set for a specific profession.
		/// Skill ID will be -1 if skill is unknown.
		/// </summary>
		public IEnumerable<int> GetWeaponSkillIds(Profession profession, WeaponType weapon, WeaponType weapon2)
		{
			if (weapon == WeaponType.Other && weapon2 == WeaponType.Other)
			{
				return Enumerable.Repeat(-1, 5);
			}

			if (weapon == WeaponType.Other)
			{
				return new[] {-1, -1, -1}.Concat(GetOffHandSkillIds(profession, weapon2));
			}

			if (weapon2 == WeaponType.Other)
			{
				return GetMainHandSkillIds(profession, weapon).Append(-1).Append(-1);
			}

			return GetMainHandSkillIds(profession, weapon).Concat(GetOffHandSkillIds(profession, weapon2));
		}
	}
}