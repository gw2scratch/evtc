using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Model.Skills
{
	/// <summary>
	/// Data associated with a <see cref="Skill"/> as it may be acquired from the official Guild Wars 2 API.
	/// </summary>
	public class SkillData
	{
		/// <summary>
		/// The numeric ID of the skill.
		/// </summary>
		public int Id { get; }
		
		/// <summary>
		/// The localized name of the skill.
		/// </summary>
		public string Name { get; }
		
		/// <summary>
		/// A URL to an icon representing the skill.
		/// </summary>
		public string IconUrl { get; }
		
		/// <summary>
		/// The type of the skill
		/// </summary>
		public SkillType Type { get; }
		
		/// <summary>
		/// The type of weapon associated with this skill.
		/// </summary>
		public WeaponType WeaponType { get; }
		
		/// <summary>
		/// A list of <see cref="Profession"/>s with access to this skill.
		/// </summary>
		public IEnumerable<Profession> Professions { get; }
		
		/// <summary>
		/// The slot this skill appears in.
		/// </summary>
		public SkillSlot Slot { get; }
		
		/// <summary>
		/// The attunement this skill is associated with.
		/// </summary>
		public SkillAttunement Attunement { get; }

		/* TODO: This makes serialization significantly more difficult (although doable)
		public SkillData NextChain { get; internal set; }
		public SkillData PrevChain { get; internal set; }
		*/

		/// <summary>
		/// Creates a new instance of <see cref="SkillData"/>.
		/// </summary>
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