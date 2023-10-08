using System;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using NUnit.Framework;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Tests.GameData.Encounters
{
	public class EncounterNameTests
	{
		[Test]
		[TestCaseSource(nameof(GetEncounters))]
		public void EnglishNameAvailableForAllEncounters(Encounter encounter)
		{
			if (encounter == Encounter.Other)
			{
				// No name is expected for the uncommon encounters
				return;
			}

			if (encounter == Encounter.Map)
			{
				// No name is expected for map logs
				return;
			}

			Assert.That(EncounterNames.EnglishNames.ContainsKey(encounter), $"Encounter {encounter} does not have an English name.");
		}


		public static IEnumerable<Encounter> GetEncounters()
		{
			return Enum.GetValues<Encounter>();
		}
	}
}