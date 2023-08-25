using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Processing;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Tests.Parsing;

public class EncounterIdentifierTests
{
	[Test]
	public void AllEncountersAreAccessibleAsPotentialEncounters()
	{
		var expectedUnreachableEncounters = new HashSet<Encounter>
		{
#pragma warning disable CS0618 // Type or member is obsolete
			Encounter.AiKeeperOfThePeak,
#pragma warning restore CS0618 // Type or member is obsolete
		};
		
		
		var identifier = new EncounterIdentifier();
		var reachableEncounters = new HashSet<Encounter>();

		void CheckId(ushort id)
		{
			var encounters = identifier.IdentifyPotentialEncounters(new ParsedBossData(id));
			foreach (var encounter in encounters)
			{
				reachableEncounters.Add(encounter);
			}
		}

		for (ushort id = 0; id < ushort.MaxValue; id++)
		{
			CheckId(id);
		}
		CheckId(ushort.MaxValue);
		
		foreach (Encounter encounter in Enum.GetValues(typeof(Encounter)))
		{
			if (expectedUnreachableEncounters.Contains(encounter)) continue;
			
			Assert.IsTrue(reachableEncounters.Contains(encounter), $"Encounter {encounter} is not reachable");
		}
	}
}