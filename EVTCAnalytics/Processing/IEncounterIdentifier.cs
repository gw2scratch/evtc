using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	public interface IEncounterIdentifier
	{
		IEncounterData GetEncounterData(Agent mainTarget, IReadOnlyList<Event> events, IReadOnlyList<Agent> agents, int? gameBuild, LogType logType);
	}
}