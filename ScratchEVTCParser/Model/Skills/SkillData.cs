using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Model.Skills
{
	public class SkillData
	{
		public int Id { get; }
		public string Name { get; }
		public string IconUrl { get; }
		public SkillType Type { get; }
		public WeaponType WeaponType { get; }
		public IEnumerable<Profession> Professions { get; }
		public SkillSlot Slot { get; }
		public SkillAttunement Attunement { get; }

		public SkillData(int id, string name, string iconUrl, SkillType type, WeaponType weaponType,
			IEnumerable<Profession> professions, SkillSlot slot, SkillAttunement attunement)
		{
			Id = id;
			Name = name;
			IconUrl = iconUrl;
			Type = type;
			WeaponType = weaponType;
			Professions = professions.ToArray();
			Slot = slot;
			Attunement = attunement;
		}
	}
}