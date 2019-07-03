using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Detections;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.GameData
{
	public class SpecializationDetections
	{
		public IEnumerable<SpecializationDetection> GetSpecializationDetections(Profession profession)
		{
			if (profession == Profession.Mesmer)
			{
				yield return new SpecializationDetection(new SelfBuffApplyDetection(SkillIds.FencersFinesse), CoreSpecialization.Dueling);
				// Illusions - Shatter Storm - Mind Wrack becomes an ammo skill
				yield return new SpecializationDetection(new DamageWithSkillDetection(SkillIds.MindWrackAmmo), CoreSpecialization.Illusions);
				// Chaos - Descent Into Madness - Lesser Chaos Storm on heal/fall damage
				yield return new SpecializationDetection(new DamageWithSkillDetection(SkillIds.LesserChaosStorm), CoreSpecialization.Chaos);
				// Chaos - Illusionary Defense - gain a stacking buff
				yield return new SpecializationDetection(new SelfBuffApplyDetection(SkillIds.IllusionaryDefense), CoreSpecialization.Chaos);
				// TODO: Chaos - Bountiful Disillusionment

				// TODO: Inspiration - Inspiring Distortion
				// Inspiration - Healing Prism - only as initial buff on PoV player
				yield return new SpecializationDetection(new InitialBuffDetection(SkillIds.HealingPrism), CoreSpecialization.Inspiration);
				// TODO: Inspiration - Blurred Inscriptions
			}
		}
	}
}