using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Sections.Clears;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// One checkbox-like status display within an <see cref="EncounterBoxRow"/> (either the normal
	/// mode or the challenge mode row) — Avalonia counterpart of the Eto <c>WeeklyCheckBox</c>
	/// control (an icon + a label), rebuilt as a plain bindable model instead of a custom control.
	/// </summary>
	public partial class EncounterCheckboxRow : ObservableObject
	{
		[ObservableProperty] private Bitmap? icon;
		[ObservableProperty] private string text = "";

		/// <summary>
		/// True when <see cref="Icon"/> is one of the dark gray line-art status icons (the
		/// "did not exist" clock / "no logs" question mark) rather than a colored one (green
		/// check / red cross) — those two are low-contrast on the dark theme's background, so
		/// the view gives them a light backdrop chip (see <c>MonochromeIconBackdropBrush</c>).
		/// </summary>
		[ObservableProperty] private bool isMonochromeIcon;
	}

	/// <summary>One encounter's normal/challenge mode status box within a <see cref="EncounterRowSection"/>.</summary>
	public class EncounterBoxRow
	{
		public string Name { get; }
		public IFinishableEncounter Encounter { get; }
		public EncounterCheckboxRow NormalMode { get; } = new();
		public EncounterCheckboxRow ChallengeMode { get; } = new();

		public EncounterBoxRow(string name, IFinishableEncounter encounter)
		{
			Name = name;
			Encounter = encounter;
		}
	}

	/// <summary>One row within an encounter group (e.g. a raid wing), with its identifying icon.</summary>
	public class EncounterRowSection
	{
		public string Name { get; }
		public Bitmap Icon { get; }
		public IReadOnlyList<EncounterBoxRow> Boxes { get; }

		public EncounterRowSection(string name, Bitmap icon, IReadOnlyList<EncounterBoxRow> boxes)
		{
			Name = name;
			Icon = icon;
			Boxes = boxes;
		}
	}

	/// <summary>
	/// One encounter category group (Raids, Icebrood Saga, End of Dragons, Secrets of the Obscure,
	/// Visions of Eternity). <see cref="IsEnabled"/> mirrors whether the category is included in
	/// <c>Settings.WeeklyClearGroups</c> and drives both this group's visibility in the encounter
	/// list and its columns' visibility in the week grid.
	/// </summary>
	public partial class EncounterGroupSection : ObservableObject
	{
		public EncounterCategory Category { get; }
		public string Name { get; }
		public IReadOnlyList<EncounterRowSection> Rows { get; }

		[ObservableProperty] private bool isEnabled;

		public EncounterGroupSection(EncounterCategory category, string name, IReadOnlyList<EncounterRowSection> rows, bool isEnabled)
		{
			Category = category;
			Name = name;
			Rows = rows;
			this.isEnabled = isEnabled;
		}
	}

	/// <summary>
	/// Display row for the reset-week grid — wraps a Core <see cref="ResetWeek"/> (which holds the
	/// actual finished-clear counts) with pre-computed percentage values for the enabled encounter
	/// categories. <see cref="Refresh"/> re-reads the wrapped week and must be called after the
	/// underlying counts change (see <c>WeeklyClearSchedule.PopulateWeekCounts</c>).
	/// </summary>
	public partial class ResetWeekRow : ObservableObject
	{
		public ResetWeek Week { get; }

		public ResetWeekRow(ResetWeek week)
		{
			Week = week;
		}

		public string WeekText => Week.Reset.ToString();

		[ObservableProperty] private int totalNormal;
		[ObservableProperty] private int totalChallenge;

		[ObservableProperty] private int raidsNormalCount;
		[ObservableProperty] private double raidsNormalPercent;
		[ObservableProperty] private int raidsChallengeCount;
		[ObservableProperty] private double raidsChallengePercent;

		[ObservableProperty] private int ibsNormalCount;
		[ObservableProperty] private double ibsNormalPercent;

		[ObservableProperty] private int eodNormalCount;
		[ObservableProperty] private double eodNormalPercent;
		[ObservableProperty] private int eodChallengeCount;
		[ObservableProperty] private double eodChallengePercent;

		[ObservableProperty] private int sotoNormalCount;
		[ObservableProperty] private double sotoNormalPercent;
		[ObservableProperty] private int sotoChallengeCount;
		[ObservableProperty] private double sotoChallengePercent;

		[ObservableProperty] private int voeNormalCount;
		[ObservableProperty] private double voeNormalPercent;
		[ObservableProperty] private int voeChallengeCount;
		[ObservableProperty] private double voeChallengePercent;

		/// <summary>Re-reads the wrapped <see cref="ResetWeek"/>'s counts and recomputes percentages.</summary>
		public void Refresh()
		{
			TotalNormal = Week.FinishedNormalModesByCategory.Values.Sum();
			TotalChallenge = Week.FinishedChallengeModesByCategory.Values.Sum();

			RaidsNormalCount = Week.FinishedNormalModesByCategory[EncounterCategory.Raids];
			RaidsNormalPercent = Percent(RaidsNormalCount, WeeklyClearSchedule.GetAvailableNormalModeCount(EncounterCategory.Raids, Week.Reset));
			RaidsChallengeCount = Week.FinishedChallengeModesByCategory[EncounterCategory.Raids];
			RaidsChallengePercent = Percent(RaidsChallengeCount, WeeklyClearSchedule.GetAvailableChallengeModeCount(EncounterCategory.Raids, Week.Reset));

			IbsNormalCount = Week.FinishedNormalModesByCategory[EncounterCategory.RaidEncountersIcebroodSaga];
			IbsNormalPercent = Percent(IbsNormalCount, WeeklyClearSchedule.GetAvailableNormalModeCount(EncounterCategory.RaidEncountersIcebroodSaga, Week.Reset));

			EodNormalCount = Week.FinishedNormalModesByCategory[EncounterCategory.RaidEncountersEndOfDragons];
			EodNormalPercent = Percent(EodNormalCount, WeeklyClearSchedule.GetAvailableNormalModeCount(EncounterCategory.RaidEncountersEndOfDragons, Week.Reset));
			EodChallengeCount = Week.FinishedChallengeModesByCategory[EncounterCategory.RaidEncountersEndOfDragons];
			EodChallengePercent = Percent(EodChallengeCount, WeeklyClearSchedule.GetAvailableChallengeModeCount(EncounterCategory.RaidEncountersEndOfDragons, Week.Reset));

			SotoNormalCount = Week.FinishedNormalModesByCategory[EncounterCategory.RaidEncountersSecretsOfTheObscure];
			SotoNormalPercent = Percent(SotoNormalCount, WeeklyClearSchedule.GetAvailableNormalModeCount(EncounterCategory.RaidEncountersSecretsOfTheObscure, Week.Reset));
			SotoChallengeCount = Week.FinishedChallengeModesByCategory[EncounterCategory.RaidEncountersSecretsOfTheObscure];
			SotoChallengePercent = Percent(SotoChallengeCount, WeeklyClearSchedule.GetAvailableChallengeModeCount(EncounterCategory.RaidEncountersSecretsOfTheObscure, Week.Reset));

			VoeNormalCount = Week.FinishedNormalModesByCategory[EncounterCategory.RaidEncountersVisionsOfEternity];
			VoeNormalPercent = Percent(VoeNormalCount, WeeklyClearSchedule.GetAvailableNormalModeCount(EncounterCategory.RaidEncountersVisionsOfEternity, Week.Reset));
			VoeChallengeCount = Week.FinishedChallengeModesByCategory[EncounterCategory.RaidEncountersVisionsOfEternity];
			VoeChallengePercent = Percent(VoeChallengeCount, WeeklyClearSchedule.GetAvailableChallengeModeCount(EncounterCategory.RaidEncountersVisionsOfEternity, Week.Reset));
		}

		private static double Percent(int count, int available) => (double) count / System.Math.Max(1, available);
	}
}
