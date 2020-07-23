using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Updates
{
	public class LogDataUpdater
	{
		public static readonly IReadOnlyList<LogUpdate> Updates = new List<LogUpdate>()
		{
			new LogUpdate(log => log.ParsingVersion < new Version(0, 7, 1)
			                     && log.Encounter == Encounter.TwinLargos
			                     && log.EncounterResult == EncounterResult.Success,
				"Twin Largos logs were marked successful even if only one of the largos died."),
			new LogUpdate(log => log.ParsingVersion < new Version(0, 7, 1)
			                     && log.Encounter == Encounter.Deimos
			                     && log.EncounterMode == EncounterMode.Challenge,
				"Old Deimos logs were sometimes detected as CM when they were in fact normal mode."),
			new LogUpdate(log => log.ParsingVersion < new Version(0, 7, 1)
			                     && log.Encounter == Encounter.Deimos
			                     && log.EncounterResult == EncounterResult.Success,
				"Very rarely, Deimos logs were detected as success when they were in fact a failure."),
			new LogUpdate(log => log.ParsingVersion < new Version(0, 7, 2)
			                     && log.Encounter == Encounter.Skorvald,
				"Skorvald the Shattered logs did not differentiate between normal and challenge mode."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 0)
			                     && string.Compare(log.EvtcVersion, "EVTC20200609", StringComparison.OrdinalIgnoreCase) >= 0,
				"Commander tag identification is now available."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 0)
					&& log.EncounterResult == EncounterResult.Failure,
				"Add health percentage for failed logs"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 0)
					&& log.Encounter == Encounter.TwinLargos
					&& log.EncounterResult == EncounterResult.Unknown,
				"Twin Largos logs had Unknown results if Kenut did not appear in the log."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 0),
				"Durations reflect the duration of the encounter instead of the duration of the log."),
		};

		public IEnumerable<LogUpdateList> GetUpdates(IEnumerable<LogData> logs)
		{
			// If there are multiple updates for a log, the log is added to the first one encountered.
			var groups = logs
				.Where(x => x.ParsingVersion != null)
				.GroupBy(log => Updates.FirstOrDefault(u => u.Filter.FilterLog(log)));

			foreach (var group in groups)
			{
				var update = group.Key;
				if (update != null)
				{
					yield return new LogUpdateList(update, group);
				}
			}
		}
	}
}