using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

/// <summary>
/// The static weekly-clear schedule (which raid/CM encounters exist, and since when) plus the
/// pure reset-week and clear-detection math used by the Weekly Clears UI section.
/// </summary>
/// <remarks>
/// This is shared, Eto-free data/logic extracted so it can be reused by any UI layer (currently
/// the Avalonia Weekly Clears section). It intentionally mirrors the table and math that used to
/// live only in the Eto <c>Sections/WeeklyClears.cs</c> view so neither the schedule nor the
/// clear-detection/reset-week logic needs to be reimplemented per UI.
/// </remarks>
public static class WeeklyClearSchedule
{
	// Note that these are reset dates (Mondays) preceding the release,
	// not necessarily the exact date of release (those tend to be Tuesdays).
	private static readonly DateOnly W1Release = new DateOnly(2015, 11, 16);
	private static readonly DateOnly W2Release = new DateOnly(2016, 3, 7);
	private static readonly DateOnly W3Release = new DateOnly(2016, 6, 13);
	private static readonly DateOnly W4Release = new DateOnly(2017, 2, 6);
	private static readonly DateOnly W5Release = new DateOnly(2017, 11, 27);
	private static readonly DateOnly W6Release = new DateOnly(2018, 9, 17);
	private static readonly DateOnly W7Release = new DateOnly(2019, 6, 10);
	private static readonly DateOnly W8Release = new DateOnly(2024, 11, 18);
	private static readonly DateOnly W8CMRelease = new DateOnly(2025, 03, 10);

	private static readonly DateOnly EoDRelease = new DateOnly(2022, 2, 28);

	private static readonly DateOnly ShiverpeaksPassRelease = new DateOnly(2019, 9, 16);
	private static readonly DateOnly VoiceClawRelease = new DateOnly(2019, 11, 18);
	private static readonly DateOnly FraenirOfJormagRelease = new DateOnly(2019, 11, 18);
	private static readonly DateOnly BoneskinnerRelease = new DateOnly(2019, 11, 18);
	private static readonly DateOnly WhisperOfJormagRelease = new DateOnly(2020, 1, 27);

	// With EoD, raid encounter CMs started to be released individually at later dates.
	private static readonly DateOnly AHCMRelease = new DateOnly(2022, 4, 18);
	private static readonly DateOnly XJJCMRelease = new DateOnly(2022, 5, 9);
	private static readonly DateOnly KOCMRelease = new DateOnly(2022, 5, 23);
	private static readonly DateOnly HTCMRelease = new DateOnly(2022, 6, 27);
	private static readonly DateOnly OLCRelease = new DateOnly(2022, 11, 7);
	private static readonly DateOnly OLCCMRelease = new DateOnly(2022, 11, 28);

	private static readonly DateOnly SotORelease = new DateOnly(2023, 8, 21);
	private static readonly DateOnly COCMRelease = new DateOnly(2023, 11, 6);
	private static readonly DateOnly ToFCMRelease = new DateOnly(2024, 2, 26);

	private static readonly DateOnly GuardiansGladeRelease = new DateOnly(2026, 02, 02);
	private static readonly DateOnly GuardiansGladeCMRelease = new DateOnly(2026, 02, 23);

	public static readonly IReadOnlyList<EncounterGroup> EncounterGroups =
	[
		new EncounterGroup(EncounterCategory.Raids, "Raids", [
			new EncounterRow("Spirit Woods (W1)", [
				new NormalEncounter(Encounter.ValeGuardian, normalModeSince: W1Release, challengeModeSince: null),
				new UnsupportedEncounter("Spirit Woods"),
				new NormalEncounter(Encounter.Gorseval, normalModeSince: W1Release, challengeModeSince: null),
				new NormalEncounter(Encounter.Sabetha, normalModeSince: W1Release, challengeModeSince: null),
			]),
			new EncounterRow("Salvation Pass (W2)", [
				new NormalEncounter(Encounter.Slothasor, normalModeSince: W2Release, challengeModeSince: null),
				new NormalEncounter(Encounter.BanditTrio, normalModeSince: W2Release, challengeModeSince: null),
				new NormalEncounter(Encounter.Matthias, normalModeSince: W2Release, challengeModeSince: null),
			]),
			new EncounterRow("Stronghold of the Faithful (W3)", [
				// nov.11.2022: removed drawbridge, added mcleod (16253) as siege the stronghold npc.
				// Since this is a Friday, we have to decide if to use the previous Monday or the next one.
				// arcdps did not have an easy update mechanism back then, so adoption of update was slow,
				// and a lot of people do not raid that late in the week (and most people do not care about Escort regardless)
				// We use the next Monday.
				new NormalEncounter(Encounter.Escort, normalModeSince: W3Release, challengeModeSince: null,
					logsSince: new DateOnly(2022, 11, 14)),
				new NormalEncounter(Encounter.KeepConstruct, normalModeSince: W3Release, challengeModeSince: W3Release),
				// nov.07.2019: added twisted castle to logging defaults.
				// This is a Thursday, similarly to Escort, we use the next Monday (might as well be consistent).
				new NormalEncounter(Encounter.TwistedCastle, normalModeSince: W3Release, challengeModeSince: null,
					logsSince: new DateOnly(2019, 11, 11)),
				new NormalEncounter(Encounter.Xera, normalModeSince: W3Release, challengeModeSince: null),
			]),
			new EncounterRow("Bastion of the Penitent (W4)", [
				new NormalEncounter(Encounter.Cairn, normalModeSince: W4Release, challengeModeSince: W4Release),
				new NormalEncounter(Encounter.MursaatOverseer, normalModeSince: W4Release, challengeModeSince: W4Release),
				new NormalEncounter(Encounter.Samarog, normalModeSince: W4Release, challengeModeSince: W4Release),
				new NormalEncounter(Encounter.Deimos, normalModeSince: W4Release, challengeModeSince: W4Release),
			]),
			new EncounterRow("Hall of Chains (W5)", [
				new NormalEncounter(Encounter.SoullessHorror, normalModeSince: W5Release, challengeModeSince: W5Release),
				new NormalEncounter(Encounter.RiverOfSouls, normalModeSince: W5Release, challengeModeSince: null),
				new MultipartEncounter("Statues", [Encounter.BrokenKing, Encounter.EaterOfSouls, Encounter.Eyes], normalModeSince: W5Release,
					challengeModeSince: null),
				new NormalEncounter(Encounter.Dhuum, normalModeSince: W5Release, challengeModeSince: W5Release),
			]),
			new EncounterRow("Mythwright Gambit (W6)", [
				new NormalEncounter(Encounter.ConjuredAmalgamate, normalModeSince: W6Release, challengeModeSince: W6Release),
				new NormalEncounter(Encounter.TwinLargos, normalModeSince: W6Release, challengeModeSince: W6Release),
				new NormalEncounter(Encounter.Qadim, normalModeSince: W6Release, challengeModeSince: W6Release),
			]),
			new EncounterRow("The Key of Ahdashim (W7)", [
				new NormalEncounter(Encounter.Adina, normalModeSince: W7Release, challengeModeSince: W7Release),
				new NormalEncounter(Encounter.Sabir, normalModeSince: W7Release, challengeModeSince: W7Release),
				new NormalEncounter(Encounter.QadimThePeerless, normalModeSince: W7Release, challengeModeSince: W7Release),
			]),
			new EncounterRow("Mount Balrior (W8)", [
				new NormalEncounter(Encounter.Greer, normalModeSince: W8Release, challengeModeSince: W8CMRelease),
				new NormalEncounter(Encounter.Decima, normalModeSince: W8Release, challengeModeSince: W8CMRelease),
				new NormalEncounter(Encounter.Ura, normalModeSince: W8Release, challengeModeSince: W8CMRelease),
				])
		]),
		new EncounterGroup(EncounterCategory.RaidEncountersIcebroodSaga, "Icebrood Saga", [
			new EncounterRow("Icebrood Saga", [
				new NormalEncounter(Encounter.ShiverpeaksPass, normalModeSince: ShiverpeaksPassRelease, challengeModeSince: null),
				new NormalEncounter(Encounter.VoiceAndClawOfTheFallen, normalModeSince: VoiceClawRelease, challengeModeSince: null),
				new NormalEncounter(Encounter.FraenirOfJormag, normalModeSince: FraenirOfJormagRelease, challengeModeSince: null),
				new NormalEncounter(Encounter.Boneskinner, normalModeSince: BoneskinnerRelease, challengeModeSince: null),
				new NormalEncounter(Encounter.WhisperOfJormag, normalModeSince: WhisperOfJormagRelease, challengeModeSince: null),
				// Not sure this is logged by default, it definitely was not initially, and it is often not done by players at all.
				//new NormalEncounter(Encounter.VariniaStormsounder, normalModeSince: ColdWarRelease, challengeModeSince: null),
			])
		]),
		new EncounterGroup(EncounterCategory.RaidEncountersEndOfDragons, "End of Dragons", [
			new EncounterRow("End of Dragons", [
				new NormalEncounter(Encounter.AetherbladeHideout, normalModeSince: EoDRelease, challengeModeSince: AHCMRelease),
				new NormalEncounter(Encounter.XunlaiJadeJunkyard, normalModeSince: EoDRelease, challengeModeSince: XJJCMRelease),
				new NormalEncounter(Encounter.KainengOverlook, normalModeSince: EoDRelease, challengeModeSince: KOCMRelease),
				new NormalEncounter(Encounter.HarvestTemple, normalModeSince: EoDRelease, challengeModeSince: HTCMRelease),
				// The Old Lion's Court raid encounter was released as part of Living World Season 1 and is accessible without End of Dragons (EoD).
				// However, achievements for it are within EoD categories and it is usually considered part of the EoD raid encounters.
				// Since adding more categories is problematic for layout, it is a simple decision to include it in the EoD category.
				new NormalEncounter(Encounter.OldLionsCourt, normalModeSince: OLCRelease, challengeModeSince: OLCCMRelease),
			]),
		]),
		new EncounterGroup(EncounterCategory.RaidEncountersSecretsOfTheObscure, "Secrets of the Obscure", [
			new EncounterRow("Secrets of the Obscure", [
				new NormalEncounter(Encounter.CosmicObservatory, normalModeSince: SotORelease,
					challengeModeSince: COCMRelease),
				new NormalEncounter(Encounter.TempleOfFebe, normalModeSince: SotORelease, challengeModeSince: ToFCMRelease),
			]),
		]),
		new EncounterGroup(EncounterCategory.RaidEncountersVisionsOfEternity, "Visions of Eternity", [
			new EncounterRow("Visions of Eternity", [
				new NormalEncounter(Encounter.GuardiansGlade, normalModeSince: GuardiansGladeRelease, challengeModeSince: GuardiansGladeCMRelease),
			]),
		])
	];

	public static readonly IReadOnlyDictionary<IFinishableEncounter, EncounterCategory> CategoriesByEncounter = EncounterGroups
		.SelectMany(group => group.Rows.SelectMany(row => row.Encounters.Select(encounter => (encounter, Id: group.Category))))
		.ToDictionary(x => x.encounter, x => x.Id);

	/// <summary>
	/// Weekly reset occurs on Monday 07:30 UTC. Rewards are given when the encounter ends, so the
	/// end time (not the start time) must be used to determine which reset week a clear counts for.
	/// </summary>
	public static DateOnly GetResetBefore(DateTimeOffset time)
	{
		var weekStart = time.ToUniversalTime();

		if (weekStart.DayOfWeek == DayOfWeek.Monday)
		{
			var reset = weekStart.Date.AddHours(7.5);
			if (weekStart < reset)
			{
				weekStart = weekStart.AddDays(-7);
			}
		}
		else
		{
			// This may not be particularly efficient, but all approaches that try to get fancy and do modular arithmetic
			// tend to break in some edge cases, especially with the awkward Sunday = 0 numbering.
			while (weekStart.DayOfWeek != DayOfWeek.Monday)
			{
				weekStart = weekStart.AddDays(-1);
			}
		}

		return DateOnly.FromDateTime(weekStart.Date);
	}

	/// <summary>
	/// Gets all reset dates for which logs could be available, in descending order (from newest).
	/// </summary>
	/// <remarks>
	/// The earliest logs known to survive are from early 2017.
	/// arcdps released on 2016-12-12 revamped how skills are saved, and as far as we are aware,
	/// no implementations of processing the older format exist.
	/// </remarks>
	public static List<DateOnly> GetAllResets()
	{
		var resets = new List<DateOnly>();
		var now = DateTimeOffset.Now + TimeSpan.FromDays(365 * 10);
		do
		{
			resets.Add(GetResetBefore(now));
			now = now.AddDays(-7);
		} while (now > new DateTimeOffset(new DateTime(2016, 12, 12, 7, 0, 0), TimeSpan.Zero));

		return resets;
	}

	/// <summary>
	/// Computes, for every (account, reset week) pair present in the given logs, which encounters
	/// (normal and challenge mode) were cleared. Pure function; safe to call off the UI thread.
	/// </summary>
	public static HashSet<(string AccountName, DateOnly ResetDate, IFinishableEncounter Encounter, bool ChallengeMode)>
		ComputeFinishedEncounters(IEnumerable<LogData> logs)
	{
		var logsByAccountNameWeek = new Dictionary<(string accountName, DateOnly resetDate), List<LogData>>();
		foreach (var log in logs)
		{
			if (log.ParsingStatus != ParsingStatus.Parsed) continue;

			var encounterEndTime = log.EncounterStartTime + log.EncounterDuration;

			foreach (var player in log.Players)
			{
				var key = (player.AccountName, GetResetBefore(encounterEndTime));
				if (!logsByAccountNameWeek.ContainsKey(key))
				{
					logsByAccountNameWeek[key] = [];
				}

				logsByAccountNameWeek[key].Add(log);
			}
		}

		var finished = new HashSet<(string AccountName, DateOnly ResetDate, IFinishableEncounter Encounter, bool ChallengeMode)>();

		foreach (((string accountName, DateOnly resetDate) key, List<LogData> weekLogs) in logsByAccountNameWeek)
		{
			foreach (var encounter in EncounterGroups.SelectMany(group => group.Rows.SelectMany(row => row.Encounters)))
			{
				if (encounter.IsSatisfiedBy(weekLogs))
				{
					finished.Add((key.accountName, key.resetDate, encounter, false));
				}

				if (encounter.IsChallengeModeSatisfiedBy(weekLogs))
				{
					finished.Add((key.accountName, key.resetDate, encounter, true));
				}
			}
		}

		return finished;
	}

	/// <summary>Number of encounters within a category whose normal mode is available at the given reset.</summary>
	public static int GetAvailableNormalModeCount(EncounterCategory category, DateOnly reset)
	{
		return EncounterGroups
			.Where(group => group.Category == category)
			.SelectMany(group => group.Rows.SelectMany(row => row.Encounters))
			.Count(encounter => encounter.GetNormalModeAvailability(reset) == EncounterAvailability.Available);
	}

	/// <summary>Number of encounters within a category whose challenge mode is available at the given reset.</summary>
	public static int GetAvailableChallengeModeCount(EncounterCategory category, DateOnly reset)
	{
		return EncounterGroups
			.Where(group => group.Category == category)
			.SelectMany(group => group.Rows.SelectMany(row => row.Encounters))
			.Count(encounter => encounter.GetChallengeModeAvailability(reset) == EncounterAvailability.Available);
	}

	/// <summary>Builds one <see cref="ResetWeek"/> per reset date returned by <see cref="GetAllResets"/>.</summary>
	public static List<ResetWeek> BuildResetWeeks()
	{
		return GetAllResets().Select(reset => new ResetWeek(reset)).ToList();
	}

	/// <summary>
	/// (Re)populates the per-category finished normal/challenge mode counts on each week for the
	/// given account, from a <see cref="ComputeFinishedEncounters"/> result. Pure function; safe to
	/// call off the UI thread.
	/// </summary>
	public static void PopulateWeekCounts(IReadOnlyList<ResetWeek> weeks,
		HashSet<(string AccountName, DateOnly ResetDate, IFinishableEncounter Encounter, bool ChallengeMode)> finishedEncounters,
		string accountFilter)
	{
		var weeksByReset = weeks.ToDictionary(x => x.Reset);

		foreach (var week in weeks)
		{
			foreach (var category in Enum.GetValues<EncounterCategory>())
			{
				week.FinishedNormalModesByCategory[category] = 0;
				week.FinishedChallengeModesByCategory[category] = 0;
			}
		}

		foreach ((string accountName, DateOnly resetDate, IFinishableEncounter encounter, bool challengeMode) in finishedEncounters)
		{
			if (accountName != accountFilter)
			{
				continue;
			}

			if (!weeksByReset.TryGetValue(resetDate, out var week))
			{
				continue;
			}

			var category = CategoriesByEncounter[encounter];

			if (challengeMode)
			{
				week.FinishedChallengeModesByCategory[category] += 1;
			}
			else
			{
				week.FinishedNormalModesByCategory[category] += 1;
			}
		}
	}
}
