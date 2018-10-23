using System.Collections.Generic;
using ScratchEVTCParser.GameData.Detections;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.GameData
{
	public class SkillDetections
	{
		public IEnumerable<SkillDetection> GetSkillDetections(Profession profession)
		{
			if (profession == Profession.Mesmer)
			{
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.SignetOfTheEtherPassive), SkillIds.SignetOfTheEther, SkillSlot.Heal);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.SignetOfTheEtherPassive), SkillIds.SignetOfTheEther, SkillSlot.Heal);
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.SignetOfDominationPassive), SkillIds.SignetOfDomination, SkillSlot.Utility);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.SignetOfDominationPassive), SkillIds.SignetOfDomination, SkillSlot.Utility);
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.SignetOfIllusionsPassive), SkillIds.SignetOfIllusions, SkillSlot.Utility);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.SignetOfIllusionsPassive), SkillIds.SignetOfIllusions, SkillSlot.Utility);
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.SignetOfInspirationPassive), SkillIds.SignetOfInspiration, SkillSlot.Utility);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.SignetOfInspirationPassive), SkillIds.SignetOfInspiration, SkillSlot.Utility);
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.SignetOfMidnightPassive), SkillIds.SignetOfMidnight, SkillSlot.Utility);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.SignetOfMidnightPassive), SkillIds.SignetOfMidnight, SkillSlot.Utility);
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.SignetOfHumilityPassive), SkillIds.SignetOfHumility, SkillSlot.Elite);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.SignetOfHumilityPassive), SkillIds.SignetOfHumility, SkillSlot.Elite);
			}

			if (profession == Profession.Thief)
			{
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.SignetOfMalicePassive), SkillIds.SignetOfMalice, SkillSlot.Heal);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.SignetOfMalicePassive), SkillIds.SignetOfMalice, SkillSlot.Heal);
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.SignetOfShadowsPassive), SkillIds.SignetOfShadows, SkillSlot.Utility);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.SignetOfShadowsPassive), SkillIds.SignetOfShadows, SkillSlot.Utility);
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.SignetOfAgilityPassive), SkillIds.SignetOfAgility, SkillSlot.Utility);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.SignetOfAgilityPassive), SkillIds.SignetOfAgility, SkillSlot.Utility);
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.AssassinsSignetPassive), SkillIds.AssassinsSignet, SkillSlot.Utility);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.AssassinsSignetPassive), SkillIds.AssassinsSignet, SkillSlot.Utility);
				yield return new SkillDetection(new InitialBuffDetection(SkillIds.InfiltratorsSignetPassive), SkillIds.InfiltratorsSignet, SkillSlot.Utility);
				yield return new SkillDetection(new SelfBuffApplyDetection(SkillIds.InfiltratorsSignetPassive), SkillIds.InfiltratorsSignet, SkillSlot.Utility);
			}
		}

		public IEnumerable<int> GetIgnoredSkillIds(Profession profession)
		{
			if (profession == Profession.Thief)
			{
				yield return SkillIds.PalmStrike; // Second in utility skill chain
			}

			if (profession == Profession.Mesmer)
			{
				yield return SkillIds.MirageMirror; // Not an actual standalone skill
			}
		}
	}
}