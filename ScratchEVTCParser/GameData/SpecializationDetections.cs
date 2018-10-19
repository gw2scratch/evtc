using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.GameData.Detections;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.GameData
{
	public class SpecializationDetections
	{
		public IEnumerable<IDetection> GetSpecializationDetections(Profession profession)
		{
			if (profession == Profession.Mesmer)
			{
				// Dueling - Fencer's Finesse - gains a stacking buff
				yield return new SelfBuffApplyDetection(SkillIds.FencersFinesse, CoreSpecialization.Dueling);
				// Illusions - Shatter Storm - Mind Wrack becomes an ammo skill
				yield return new DamageWithSkillDetection(SkillIds.MindWrackAmmo, CoreSpecialization.Illusions);
				// Chaos - Descent Into Madness - Lesser Chaos Storm on heal/fall damage
				yield return new DamageWithSkillDetection(SkillIds.LesserChaosStorm, CoreSpecialization.Chaos);
				// Chaos - Illusionary Defense - gain a stacking buff
				yield return new SelfBuffApplyDetection(SkillIds.IllusionaryDefense, CoreSpecialization.Chaos);
				// TODO: Chaos - Bountiful Disillusionment

				// TODO: Inspiration - Inspiring Distortion
				// Inspiration - Healing Prism - only as initial buff on PoV player
				yield return new InitialBuffDetection(SkillIds.HealingPrism, CoreSpecialization.Inspiration);
				// TODO: Inspiration - Blurred Inscriptions
			}
		}
	}
}