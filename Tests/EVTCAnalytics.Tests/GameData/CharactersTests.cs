using System;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using NUnit.Framework;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Tests.GameData
{
	public class CharactersTests
	{
		[Test]
		[TestCaseSource(nameof(GetProfessions))]
		public void AllProfessionsHaveAHeartOfThornsEliteSpecialization(Profession profession)
		{
			EliteSpecialization specialization = EliteSpecialization.None;
			Assert.DoesNotThrow(() => specialization = Characters.GetHeartOfThornsEliteSpecialization(profession));
			Assert.AreNotEqual(EliteSpecialization.None, specialization);
		}

		[Test]
		[TestCaseSource(nameof(GetProfessions))]
		public void AllProfessionsHaveAPathOfFireEliteSpecialization(Profession profession)
		{
			EliteSpecialization specialization = EliteSpecialization.None;
			Assert.DoesNotThrow(() => specialization = Characters.GetPathOfFireEliteSpecialization(profession));
			Assert.AreNotEqual(EliteSpecialization.None, specialization);
		}

		[Test]
		[TestCaseSource(nameof(GetProfessions))]
		public void AllProfessionsHaveAnEndOfDragonsEliteSpecialization(Profession profession)
		{
			EliteSpecialization specialization = EliteSpecialization.None;
			Assert.DoesNotThrow(() => specialization = Characters.GetEndOfDragonsEliteSpecialization(profession));
			Assert.AreNotEqual(EliteSpecialization.None, specialization);
		}

		[Test]
		[TestCaseSource(nameof(GetEliteSpecializations))]
		public void AllEliteSpecializationsHaveABaseProfession(EliteSpecialization eliteSpecialization)
		{
			if (eliteSpecialization == EliteSpecialization.None)
			{
				return;
			}

			Profession profession = Profession.None;
			Assert.DoesNotThrow(() => profession = Characters.GetProfession(eliteSpecialization));
			Assert.AreNotEqual(EliteSpecialization.None, profession);
		}

		public static IEnumerable<EliteSpecialization> GetEliteSpecializations()
		{
			return Enum.GetValues<EliteSpecialization>();
		}

		public static IEnumerable<Profession> GetProfessions()
		{
			return Characters.Professions;
		}
	}
}