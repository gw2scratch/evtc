using System;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using NUnit.Framework;

namespace GW2Scratch.EVTCAnalytics.Tests.GameData.Encounters
{
	public class EncounterCategoryTests
	{
		[Test]
		public void CategoryAvailableForAllEncounters()
		{
			foreach (Encounter encounter in Enum.GetValues(typeof(Encounter)))
			{
				Assert.DoesNotThrow(() => encounter.GetEncounterCategory());
			}
		}
	}
}