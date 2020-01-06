using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters
{
	public interface IEncounterData
	{
		Encounter Encounter { get; }
		PhaseSplitter PhaseSplitter { get; }
		IResultDeterminer ResultDeterminer { get; }
		IReadOnlyList<Agent> Targets { get; }
	}
}