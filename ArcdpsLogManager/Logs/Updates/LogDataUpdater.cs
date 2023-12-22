using GW2Scratch.EVTCAnalytics.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Logs.Updates
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
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 0, 0)
			                     && string.Compare(log.EvtcVersion, "EVTC20200609", StringComparison.OrdinalIgnoreCase) >= 0,
				"Commander tag identification is now available."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 0, 0)
			                     && log.EncounterResult == EncounterResult.Failure,
				"Add health percentage for failed logs"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 0, 0)
			                     && log.Encounter == Encounter.TwinLargos
			                     && log.EncounterResult == EncounterResult.Unknown,
				"Twin Largos logs had Unknown results if Kenut did not appear in the log."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 0, 0),
				"Durations are significantly more accurate."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 0, 1)
			                     && log.Encounter == Encounter.Other,
				"Support for Ai, Keeper of the Peak"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 3, 1)
			                     && log.Encounter == Encounter.KeepConstruct,
				"Adds Keep Construct CM detection."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 3, 2)
			                     && log.Encounter == Encounter.Xera
			                     && log.EncounterResult == EncounterResult.Failure
			                     && log.HealthPercentage > 0.998,
				"Twisted Castle logs may have been incorrectly identified as Xera."),
#pragma warning disable 618
			new LogUpdate(log => log.ParsingVersion < new Version(1, 0, 3, 3)
			                     && log.Encounter == Encounter.AiKeeperOfThePeak,
				"Ai, Keeper of the Peak is now three different encounters (by phases) and successful if group resets after first phase."),
#pragma warning restore 618
			new LogUpdate(log => log.ParsingVersion < new Version(1, 1, 2, 0)
			                     && (log.Players?.Any(x => x.Profession == Profession.None) ?? false),
				"Fixes broken profession detection for Core Guardians."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 2, 0, 0)
			                     && log.GameBuild >= 118697
			                     && (log.Players?.Any(x => x.EliteSpecialization == EliteSpecialization.None) ?? false),
				"Add support for Virtuoso, Harbinger and Willbender."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 3, 0, 1)
			                     && log.GameBuild >= 119939
			                     && (log.Players?.Any(x =>
				                     x.Profession is Profession.Warrior or Profession.Revenant or Profession.Elementalist
				                     && x.EliteSpecialization == EliteSpecialization.None) ?? false),
				"Add support for Bladesworn, Vindicator, and Catalyst."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 3, 0, 1)
			                     && log.GameBuild >= 121168
			                     && (log.Players?.Any(x =>
				                     x.Profession is Profession.Thief or Profession.Engineer or Profession.Ranger
				                     && x.EliteSpecialization == EliteSpecialization.None) ?? false),
				"Add support for Specter, Mechanist, and Untamed."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 3, 0, 2)
			                     && log.Encounter == Encounter.Other,
				"Add support for the Mordremoth fight."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 4, 0, 0)
			                     && log.Encounter.GetEncounterCategory() == EncounterCategory.Fractal,
				"Adds Mistlock Instabilities for fractal log details, filters, and log list column."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 4, 0, 1)
			                     && log.Encounter == Encounter.Other,
				"Add support for End of Dragons strike missions."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 5, 0, 0)
			                     && log.Encounter == Encounter.AetherbladeHideout
			                     && log.GameBuild >= 127931,
				"Add CM detection for Aetherblade Hideout."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 5, 0, 1)
			                     && log.Encounter == Encounter.XunlaiJadeJunkyard
			                     && log.GameBuild >= 128773,
				"Add CM detection for Xunlai Jade Junkyard."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 5, 1, 1)
			                     && log.Encounter == Encounter.Other
			                     && log.MapId == MapIds.XunlaiJadeJunkyard
			                     && log.GameBuild >= 129355,
				"Add CM detection for Kaineng Overlook."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 6, 0, 0)
			                     // Some raid enemies can be manually added and they would be categorized as Other.
			                     && (log.Encounter.IsRaid() || (log.MapId != null && MapIds.IsRaidMap(log.MapId.Value)))
			                     && log.GameBuild >= 130910,
				"Add Emboldened (easy) mode detection for raids."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 6, 0, 1)
			                     // Some raid enemies can be manually added and they would be categorized as Other.
			                     && log.Encounter == Encounter.HarvestTemple
			                     && log.GameBuild >= 130910,
				"Add CM detection for Harvest Temple."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 6, 0, 2)
			                     && log.Encounter == Encounter.AiKeeperOfThePeakDayAndNight
			                     && log.ParsingVersion >= new Version(1, 6, 0, 0),
				"Fixes Ai logs always being categorized as both phases"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 6, 0, 3)
			                     && log.Encounter == Encounter.HarvestTemple,
				"Slightly improved health percentage display for Harvest Temple (16.66% per phase)"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 7, 0, 0)
			                     && log.Encounter == Encounter.Other
			                     && log.MapId == MapIds.OldLionsCourt,
				"Add support for Old Lion's Court"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 7, 1, 0)
			                     && log.Encounter == Encounter.Other
			                     && log.MapId == MapIds.OldLionsCourt,
				"Add support for Old Lion's Court CM"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 7, 1, 1)
			                     && log.Encounter == Encounter.Other,
				"Add basic support for map logs"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 7, 1, 2)
			                     && log.ParsingVersion >= new Version(1, 7, 1, 0)
			                     && log.Encounter == Encounter.OldLionsCourt,
				"Fix CM detection for Old Lion's Court"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 8, 1, 0)
			                     && log.Encounter == Encounter.Other
			                     && log.MapId == MapIds.HarvestTemple,
				"Add support for Harvest Temple logs triggered by Void Melters"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 9, 0, 0)
			                     && log.Encounter == Encounter.Other
			                     && log.MapId == MapIds.SilentSurf,
				"Add support for Silent Surf CM"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 9, 0, 1)
			                     // There may be fractal encounters that are added manually and are not supported, so we also go through other logs.
			                     && (log.Encounter == Encounter.Other || log.Encounter.GetEncounterCategory() == EncounterCategory.Fractal)
			                     && string.Compare(log.EvtcVersion, "EVTC20230716", StringComparison.OrdinalIgnoreCase) >= 0,
				"Add support for fractal scale."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 9, 0, 2)
			                     && log.Encounter == Encounter.Other
			                     && log.MapId == MapIds.SilentSurf,
				"Add support for Silent Surf NM"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 9, 0, 3)
			                     && log.Encounter == Encounter.Map
			                     && string.Compare(log.EvtcVersion, "EVTC20230716", StringComparison.OrdinalIgnoreCase) >= 0,
				"Add support for fractal scale."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 10, 0, 1)
			                     && (log.Encounter == Encounter.BanditTrio),
				"Slightly improved precision for Bandit Trio success detection when recording player leaves the fort."),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 10, 0, 2)
			                     && log.Encounter == Encounter.Other
			                     && log.MapId == MapIds.CosmicObservatory,
				"Add support for Cosmic Observatory"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 10, 0, 2)
			                     && log.Encounter == Encounter.Other
			                     && log.MapId == MapIds.TempleOfFebe,
				"Add support for Temple of Febe"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 10, 0, 3)
			                     && log.Encounter == Encounter.Other
			                     && log.MapId == MapIds.RaidWing3,
				"Add support for Escort"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 10, 0, 4)
			                     && log.Encounter == Encounter.HarvestTemple,
				"Fix success detection for Harvest Temple"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 10, 0, 5)
			                     && log.GameBuild >= 151966
			                     && log.Encounter == Encounter.AiKeeperOfThePeakDayOnly,
				"Fix success detection for Ai, Keeper of the Peak – Elemental"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 10, 1, 0)
			                     && log.Encounter == Encounter.HarvestTemple
			                     && log.EncounterResult == EncounterResult.Failure,
				"Fix very rare false failures for Harvest Temple"),
			new LogUpdate(log => log.ParsingVersion < new Version(1, 10, 1, 1)
			                     && log.GameBuild >= 153978
			                     && log.Encounter == Encounter.CosmicObservatory,
				"Add support for Cosmic Observatory CM"),
			// When adding a new update, you need to increase the revision (last value) of the version in the .csproj file
			// unless the version changes more significantly, in that case it can be reset to 0.
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