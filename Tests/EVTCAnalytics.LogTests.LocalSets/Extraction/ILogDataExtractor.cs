using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.LogTests.LocalSets.Extraction;

public interface ILogDataExtractor
{
	public (Encounter, EncounterMode, EncounterResult, IReadOnlyList<Player>) ExtractData(string filename);
}