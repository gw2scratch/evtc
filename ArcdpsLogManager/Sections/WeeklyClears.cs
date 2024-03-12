using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Dialogs;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Sections.Clears;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GW2Scratch.ArcdpsLogManager.Sections;

public class WeeklyClears : DynamicLayout
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

	private static readonly DateOnly EoDRelease = new DateOnly(2022, 2, 28);

	// With EoD, strike mission CMs started to be released individually at later dates.
	private static readonly DateOnly AHCMRelease = new DateOnly(2022, 4, 18);
	private static readonly DateOnly XJJCMRelease = new DateOnly(2022, 5, 9);
	private static readonly DateOnly KOCMRelease = new DateOnly(2022, 5, 23);
	private static readonly DateOnly HTCMRelease = new DateOnly(2022, 6, 27);
	private static readonly DateOnly OLCRelease = new DateOnly(2022, 11, 7);
	private static readonly DateOnly OLCCMRelease = new DateOnly(2022, 11, 28);

	private static readonly DateOnly SotORelease = new DateOnly(2023, 8, 21);
	private static readonly DateOnly COCMRelease = new DateOnly(2023, 11, 6);
	private static readonly DateOnly ToFCMRelease = new DateOnly(2024, 2, 26);


	private static readonly List<EncounterColumn> EncounterColumns =
	[
		new EncounterColumn("Raids", [
			new EncounterRow("Spirit Woods (W1)", [
				new NormalEncounter(Encounter.ValeGuardian, Category.Raid, normalModeSince: W1Release, challengeModeSince: null),
				new UnsupportedEncounter("Spirit Woods", Category.Raid),
				new NormalEncounter(Encounter.Gorseval, Category.Raid, normalModeSince: W1Release, challengeModeSince: null),
				new NormalEncounter(Encounter.Sabetha, Category.Raid, normalModeSince: W1Release, challengeModeSince: null),
			]),
			new EncounterRow("Salvation Pass (W2)", [
				new NormalEncounter(Encounter.Slothasor, Category.Raid, normalModeSince: W2Release, challengeModeSince: null),
				new NormalEncounter(Encounter.BanditTrio, Category.Raid, normalModeSince: W2Release, challengeModeSince: null),
				new NormalEncounter(Encounter.Matthias, Category.Raid, normalModeSince: W2Release, challengeModeSince: null),
			]),
			new EncounterRow("Stronghold of the Faithful (W3)", [
				// nov.11.2022: removed drawbridge, added mcleod (16253) as siege the stronghold npc.
				// Since this is a Friday, we have to decide if to use the previous Monday or the next one.
				// arcdps did not have an easy update mechanism back then, so adoption of update was slow,
				// and a lot of people do not raid that late in the week (and most people do not care about Escort regardless)
				// We use the next Monday.
				new NormalEncounter(Encounter.Escort, Category.Raid, normalModeSince: W3Release, challengeModeSince: null, logsSince: new DateOnly(2022, 11, 14)),
				new NormalEncounter(Encounter.KeepConstruct, Category.Raid, normalModeSince: W3Release, challengeModeSince: W3Release),
				// nov.07.2019: added twisted castle to logging defaults.
				// This is a Thursday, similarly to Escort, we use the next Monday (might as well be consistent).
				new NormalEncounter(Encounter.TwistedCastle, Category.Raid, normalModeSince: W3Release, challengeModeSince: null, logsSince: new DateOnly(2019, 11, 11)),
				new NormalEncounter(Encounter.Xera, Category.Raid, normalModeSince: W3Release, challengeModeSince: null),
			]),
			new EncounterRow("Bastion of the Penitent (W4)", [
				new NormalEncounter(Encounter.Cairn, Category.Raid, normalModeSince: W4Release, challengeModeSince: W4Release),
				new NormalEncounter(Encounter.MursaatOverseer, Category.Raid, normalModeSince: W4Release, challengeModeSince: W4Release),
				new NormalEncounter(Encounter.Samarog, Category.Raid, normalModeSince: W4Release, challengeModeSince: W4Release),
				new NormalEncounter(Encounter.Deimos, Category.Raid, normalModeSince: W4Release, challengeModeSince: W4Release),
			]),
			new EncounterRow("Hall of Chains (W5)", [
				new NormalEncounter(Encounter.SoullessHorror, Category.Raid, normalModeSince: W5Release, challengeModeSince: W5Release),
				new NormalEncounter(Encounter.RiverOfSouls, Category.Raid, normalModeSince: W5Release, challengeModeSince: null),
				new MultipartEncounter("Statues", [Encounter.BrokenKing, Encounter.EaterOfSouls, Encounter.Eyes], Category.Raid, normalModeSince: W5Release,
					challengeModeSince: null),
				new NormalEncounter(Encounter.Dhuum, Category.Raid, normalModeSince: W5Release, challengeModeSince: W5Release),
			]),
			new EncounterRow("Mythwright Gambit (W6)", [
				new NormalEncounter(Encounter.ConjuredAmalgamate, Category.Raid, normalModeSince: W6Release, challengeModeSince: W6Release),
				new NormalEncounter(Encounter.TwinLargos, Category.Raid, normalModeSince: W6Release, challengeModeSince: W6Release),
				new NormalEncounter(Encounter.Qadim, Category.Raid, normalModeSince: W6Release, challengeModeSince: W6Release),
			]),
			new EncounterRow("The Key of Ahdashim (W7)", [
				new NormalEncounter(Encounter.Adina, Category.Raid, normalModeSince: W7Release, challengeModeSince: W7Release),
				new NormalEncounter(Encounter.Sabir, Category.Raid, normalModeSince: W7Release, challengeModeSince: W7Release),
				new NormalEncounter(Encounter.QadimThePeerless, Category.Raid, normalModeSince: W7Release, challengeModeSince: W7Release),
			]),
			new EncounterRow("End of Dragons", [
				new NormalEncounter(Encounter.AetherbladeHideout, Category.StrikeEndOfDragons, normalModeSince: EoDRelease, challengeModeSince: AHCMRelease),
				new NormalEncounter(Encounter.XunlaiJadeJunkyard, Category.StrikeEndOfDragons, normalModeSince: EoDRelease, challengeModeSince: XJJCMRelease),
				new NormalEncounter(Encounter.KainengOverlook, Category.StrikeEndOfDragons, normalModeSince: EoDRelease, challengeModeSince: KOCMRelease),
				new NormalEncounter(Encounter.HarvestTemple, Category.StrikeEndOfDragons, normalModeSince: EoDRelease, challengeModeSince: HTCMRelease),
				// The Old Lion's Court strike mission was released as part of Living World Season 1 and is accessible without End of Dragons (EoD).
				// However, achievements for it are within EoD categories and it is usually considered part of the EoD strike missions.
				// Since adding more categories is problematic for layout, it is a simple decision to include it in the EoD category.
				new NormalEncounter(Encounter.OldLionsCourt, Category.StrikeEndOfDragons, normalModeSince: OLCRelease, challengeModeSince: OLCCMRelease),
			]),
			new EncounterRow("Secrets of the Obscure", [
				new NormalEncounter(Encounter.CosmicObservatory, Category.StrikeSecretsOfTheObscure, normalModeSince: SotORelease, challengeModeSince: COCMRelease),
				new NormalEncounter(Encounter.TempleOfFebe, Category.StrikeSecretsOfTheObscure, normalModeSince: SotORelease, challengeModeSince: ToFCMRelease),
			]),
		])
	];

	private string accountFilter = "";

	private string AccountFilter
	{
		get => accountFilter;
		set
		{
			accountFilter = value;
			UpdateWeeks();
		}
	}

	private DateOnly reset = GetResetBefore(DateTimeOffset.Now);

	private DateOnly Reset
	{
		get => reset;
		set
		{
			reset = value;
			SelectedResetChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private readonly List<ResetWeek> weeks = GetAllResets().Select(x => new ResetWeek(x)).ToList();

	// Not set directly, instead set through UpdateFinishedLogs
	private HashSet<(string AccountName, DateOnly ResetDate, IFinishableEncounter Encounter, bool ChallengeMode)> finishedEncounters = [];

	// Cached from the update, we need this to be able to construct a player selection dialog.
	private List<LogData> logs = [];

	private void UpdateFinishedLogs(HashSet<(string AccountName, DateOnly ResetDate, IFinishableEncounter Encounter, bool ChallengeMode)> finished)
	{
		finishedEncounters = finished;
		DataUpdated?.Invoke(this, EventArgs.Empty);
		UpdateWeeks();
	}

	private void UpdateWeeks()
	{
		foreach (var week in weeks)
		{
			foreach (var category in Enum.GetValues<Category>())
			{
				week.FinishedNormalModesByCategory[category] = 0;
				week.FinishedChallengeModesByCategory[category] = 0;
			}
		}

		var weeksByReset = weeks.ToDictionary(x => x.Reset);

		foreach ((string accountName, DateOnly resetDate, IFinishableEncounter encounter, bool challengeMode) in finishedEncounters)
		{
			if (accountName != AccountFilter)
			{
				continue;
			}

			var week = weeksByReset[resetDate];

			if (challengeMode)
			{
				week.FinishedChallengeModesByCategory[encounter.Category] += 1;
			}
			else
			{
				week.FinishedNormalModesByCategory[encounter.Category] += 1;
			}
		}
	}

	private event EventHandler SelectedResetChanged;
	private event EventHandler DataUpdated;

	public WeeklyClears(ImageProvider imageProvider)
	{
		var accountFilterBox = new DropDown { Width = 350 };
		accountFilterBox.DataStore = Settings.PlayerAccountNames.Select(x => x.TrimStart(':'));
		if (Settings.PlayerAccountNames.Count != 0)
		{
			accountFilterBox.SelectedIndex = 0;
			AccountFilter = Settings.PlayerAccountNames[0];
		}

		accountFilterBox.SelectedValueChanged += (_, _) =>
		{
			AccountFilter = $":{accountFilterBox.SelectedValue}";
			DataUpdated?.Invoke(this, EventArgs.Empty);
		};

		Settings.PlayerAccountNamesChanged += (_, _) =>
		{
			accountFilterBox.SelectedIndex = -1;
			accountFilterBox.DataStore = Settings.PlayerAccountNames.Select(x => x.TrimStart(':'));
			accountFilterBox.SelectedIndex = Settings.PlayerAccountNames.Count > 0 ? 0 : -1;
			DataUpdated?.Invoke(this, EventArgs.Empty);
		};

		var addNewAccountButton = new Button { Text = "Add account" };
		var removeAccountButton = new Button { Text = "Remove", Enabled = accountFilterBox.SelectedIndex != -1 };
		addNewAccountButton.Click += (_, _) =>
		{
			var dialog = new PlayerSelectDialog(null, null, null, null, imageProvider, null, logs);
			var selectedPlayer = dialog.ShowDialog(this);
			if (selectedPlayer != null)
			{
				var selectedAccount = selectedPlayer.AccountName;
				// If this name is already added, we just select it.
				if (!Settings.PlayerAccountNames.Contains(selectedAccount))
				{
					Settings.PlayerAccountNames = Settings.PlayerAccountNames.Append(selectedAccount).ToList();
				}
				accountFilterBox.SelectedIndex = Settings.PlayerAccountNames.Count - 1;
				AccountFilter = selectedAccount;
				removeAccountButton.Enabled = true;
				DataUpdated?.Invoke(this, EventArgs.Empty);
			}
		};
		removeAccountButton.Click += (_, _) =>
		{
			var oldIndex = accountFilterBox.SelectedIndex;
			if (oldIndex >= 0)
			{
				var newIndex = -1;
				if (Settings.PlayerAccountNames.Count == 1)
				{
					AccountFilter = "";
					removeAccountButton.Enabled = false;
				}
				else
				{
					newIndex = 0;
					AccountFilter = Settings.PlayerAccountNames[0];
				}

				// We need to make a copy of the list to edit it.
				var newList = Settings.PlayerAccountNames.ToList();
				newList.RemoveAt(oldIndex);

				Settings.PlayerAccountNames = newList;
				accountFilterBox.DataStore = Settings.PlayerAccountNames.Select(x => x.TrimStart(':'));
				accountFilterBox.SelectedIndex = newIndex;

				DataUpdated?.Invoke(this, EventArgs.Empty);
			}
		};

		var tables = new List<TableLayout>();
		foreach (var column in EncounterColumns)
		{
			var table = new TableLayout(column.Rows.Max(x => x.Encounters.Count), column.Rows.Count);
			table.Spacing = new Size(15, 8);
			tables.Add(table);

			for (int row = 0; row < column.Rows.Count; row++)
			{
				EncounterRow encounterRow = column.Rows[row];
				for (int col = 0; col < encounterRow.Encounters.Count; col++)
				{
					var encounter = encounterRow.Encounters[col];
					var name = encounter switch
					{
						MultipartEncounter multipartEncounter => multipartEncounter.Name,
						NormalEncounter normalEncounter => EncounterNames.TryGetEncounterNameForLanguage(GameLanguage.English, normalEncounter.Encounter,
							out var normalName)
							? normalName
							: normalEncounter.Encounter.ToString(),
						UnsupportedEncounter unsupportedEncounter => unsupportedEncounter.Name,
						_ => throw new ArgumentOutOfRangeException()
					};

					Size iconSize = new Size(16, 16);

					var normalModeCheckbox = new ImageView { Size = iconSize };
					var normalModeLabel = new Label();
					var challengeModeCheckbox = new ImageView { Size = iconSize };
					var challengeModeLabel = new Label();

					void UpdateCheckbox(ImageView checkBox, Label label, bool isChallengeMode)
					{
						var availability = isChallengeMode ? encounter.GetChallengeModeAvailability(Reset) : encounter.GetNormalModeAvailability(Reset);
						var standardLabelText = isChallengeMode ? "Challenge Mode" : "Normal Mode";
						switch (availability)
						{
							case EncounterAvailability.Available:
								var finished = finishedEncounters.Contains((AccountFilter, Reset, encounter, isChallengeMode));
								checkBox.Image = finished ? imageProvider.GetGreenCheckIcon() : imageProvider.GetRedCrossIcon();
								label.Text = standardLabelText;
								break;
							case EncounterAvailability.DoesNotExist:
								// We only want to show the unavailability within the normal mode row.
								checkBox.Image = isChallengeMode ? null : imageProvider.GetGrayQuestionMarkIcon();
								label.Text = isChallengeMode ? "" : "Did not exist";
								break;
							case EncounterAvailability.NotLogged:
								// We only want to show the unavailability within the normal mode row.
								checkBox.Image = isChallengeMode ? null : imageProvider.GetGrayQuestionMarkIcon();
								label.Text = isChallengeMode ? "" : standardLabelText;
								label.Text = isChallengeMode ? "" : "No logs";
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}

					DataUpdated += (_, _) => UpdateCheckbox(normalModeCheckbox, normalModeLabel, false);
					SelectedResetChanged += (_, _) => UpdateCheckbox(normalModeCheckbox, normalModeLabel, false);
					DataUpdated += (_, _) => UpdateCheckbox(challengeModeCheckbox, challengeModeLabel, true);
					SelectedResetChanged += (_, _) => UpdateCheckbox(challengeModeCheckbox, challengeModeLabel, true);

					var layout = new StackLayout
					{
						Items =
						{
							new StackLayout
							{
								Items = { normalModeCheckbox, normalModeLabel },
								Orientation = Orientation.Horizontal,
								Spacing = 6,
							},
							new StackLayout
							{
								Items = { challengeModeCheckbox, challengeModeLabel },
								Orientation = Orientation.Horizontal,
								Spacing = 6,
							}
						},
						Spacing = 6
					};

					var box = new GroupBox { Text = name, Content = layout, Padding = new Padding(4, 2) };

					table.Add(box, col, row);
				}
			}
		}

		var weekGrid = new GridView<ResetWeek> { Height = 150 };
		weekGrid.Columns.Add(new GridColumn
		{
			HeaderText = "Week", DataCell = new TextBoxCell { Binding = new DelegateBinding<ResetWeek, string>(x => x.Reset.ToString()) }
		});
		weekGrid.Columns.Add(new GridColumn
		{
			HeaderText = "NMs",
			DataCell = new TextBoxCell
			{
				Binding = new DelegateBinding<ResetWeek, string>(x => x.FinishedNormalModesByCategory.Values.Sum().ToString()),
				TextAlignment = TextAlignment.Center
			}
		});
		weekGrid.Columns.Add(new GridColumn
		{
			HeaderText = "CMs",
			DataCell = new TextBoxCell
			{
				Binding = new DelegateBinding<ResetWeek, string>(x => x.FinishedChallengeModesByCategory.Values.Sum().ToString()),
				TextAlignment = TextAlignment.Center
			}
		});
		foreach ((string name, Category category) in (ReadOnlySpan<(string, Category)>)
		         [("Raid", Category.Raid), ("EoD", Category.StrikeEndOfDragons), ("SotO", Category.StrikeSecretsOfTheObscure)])
		{
			weekGrid.Columns.Add(new GridColumn
			{
				HeaderText = $"{name} NMs",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<ResetWeek, string>(week => week.FinishedNormalModesByCategory[category].ToString()),
					TextAlignment = TextAlignment.Center
				}
			});
			weekGrid.Columns.Add(new GridColumn
			{
				HeaderText = $"{name} NMs",
				DataCell = new ProgressCell
				{
					Binding = new DelegateBinding<ResetWeek, float?>(week =>
						(float) week.FinishedNormalModesByCategory[category] / Math.Max(1, EncounterColumns
							.SelectMany(col => col.Rows.SelectMany(row => row.Encounters))
							.Count(encounter => encounter.GetNormalModeAvailability(week.Reset) == EncounterAvailability.Available && encounter.Category == category)))
				}
			});
			weekGrid.Columns.Add(new GridColumn
			{
				HeaderText = $"{name} CMs",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<ResetWeek, string>(week => week.FinishedChallengeModesByCategory[category].ToString()),
					TextAlignment = TextAlignment.Center,
				}
			});
			weekGrid.Columns.Add(new GridColumn
			{
				HeaderText = $"{name} CMs",
				DataCell = new ProgressCell
				{
					Binding = new DelegateBinding<ResetWeek, float?>(week =>
						(float) week.FinishedChallengeModesByCategory[category] / Math.Max(1, EncounterColumns
							.SelectMany(col => col.Rows.SelectMany(row => row.Encounters))
							.Count(encounter => encounter.GetChallengeModeAvailability(week.Reset) == EncounterAvailability.Available && encounter.Category == category))),
				}
			});
		}

		weekGrid.DataStore = weeks;
		weekGrid.SelectedRow = 0;

		weekGrid.SelectionChanged += (_, _) =>
		{
			Reset = weekGrid.SelectedItem?.Reset ?? GetResetBefore(DateTimeOffset.Now);
		};

		DataUpdated += (_, _) =>
		{
			weekGrid.ReloadData(Eto.Forms.Range.FromLength(0, weeks.Count));
		};

		BeginVertical(padding: new Padding(0, 2), spacing: new Size(10, 10));
		{
			AddRow(accountFilterBox, addNewAccountButton, removeAccountButton, null);
			// TODO: add checkboxes for categories to show
		}
		EndBeginVertical(spacing: new Size(10, 10));
		{
			BeginScrollable();
			BeginHorizontal();
			foreach (var table in tables)
			{
				AddSeparateColumn(table, null);
			}

			Add(null);
			EndHorizontal();
			EndScrollable();
		}
		EndVertical();
		AddSeparateRow(controls: [weekGrid]);
	}

	public void UpdateDataFromLogs(IEnumerable<LogData> logs)
	{
		this.logs = logs.ToList();
		Task.Run(() =>
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
				foreach (var encounter in EncounterColumns.SelectMany(column => column.Rows.SelectMany(row => row.Encounters)))
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

			Application.Instance.Invoke(() => { UpdateFinishedLogs(finished); });
		});
	}

	private static DateOnly GetResetBefore(DateTimeOffset time)
	{
		// Weekly reset occurs on Monday 07:30 UTC
		// Rewards are given when the encounter ends, so we need to check the end time

		var weekStart = time.ToUniversalTime();

		if (time.DayOfWeek == DayOfWeek.Monday)
		{
			var reset = time.Date.AddHours(7.5);
			if (time < reset)
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
	/// <returns></returns>
	private static List<DateOnly> GetAllResets()
	{
		var resets = new List<DateOnly>();
		var now = DateTimeOffset.Now;
		do
		{
			resets.Add(GetResetBefore(now));
			now = now.AddDays(-7);
		} while (now > new DateTimeOffset(new DateTime(2016, 12, 12, 7, 0, 0), TimeSpan.Zero));

		return resets;
	}
}