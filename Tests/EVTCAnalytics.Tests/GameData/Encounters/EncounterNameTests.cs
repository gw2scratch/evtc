using System;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using NUnit.Framework;

namespace GW2Scratch.EVTCAnalytics.Tests.GameData.Encounters
{
	public class EncounterNameTests
	{
		[Test]
		public void EnglishNameAvailableForAllEncounters()
		{
			foreach (Encounter encounter in Enum.GetValues(typeof(Encounter)))
			{
				if (encounter == Encounter.Other)
				{
					// No name is expected for the uncommon encounters
					continue;
				}
				
				if (encounter == Encounter.Map)
				{
					// No name is expected for map logs
					continue;
				}

				Assert.That(EncounterNames.EnglishNames.ContainsKey(encounter),
					$"Encounter {encounter} does not have an English name.");
			}
		}
	}
}