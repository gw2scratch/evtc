using System;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using NUnit.Framework;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Tests.GameData.Encounters
{
	public class EncounterCategoryTests
	{
		[Test]
		[TestCaseSource(nameof(GetEncounters))]
		public void CategoryAvailableForEncounter(Encounter encounter)
		{
			Assert.DoesNotThrow(() => encounter.GetEncounterCategory());
		}

		public static IEnumerable<Encounter> GetEncounters()
		{
			return Enum.GetValues<Encounter>();
		}
	}
}