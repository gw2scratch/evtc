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
using System.Timers;
using EncounterCategory = GW2Scratch.ArcdpsLogManager.Sections.Clears.EncounterCategory;

namespace GW2Scratch.ArcdpsLogManager.Sections;

public class WeeklyClears : DynamicLayout
{
	private readonly ImageProvider imageProvider;

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

	private static readonly DateOnly ShiverpeaksPassRelease = new DateOnly(2019, 9, 16);
	private static readonly DateOnly VoiceClawRelease = new DateOnly(2019, 11, 18);
	private static readonly DateOnly FraenirOfJormagRelease = new DateOnly(2019, 11, 18);
	private static readonly DateOnly BoneskinnerRelease = new DateOnly(2019, 11, 18);
	private static readonly DateOnly WhisperOfJormagRelease = new DateOnly(2020, 1, 27);
	private static readonly DateOnly ColdWarRelease = new DateOnly(2020, 5, 25);

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


	private static readonly List<EncounterGroup> EncounterGroups =
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
		]),
		new EncounterGroup(EncounterCategory.StrikeIcebroodSaga, "Icebrood Saga", [
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
		new EncounterGroup(EncounterCategory.StrikeEndOfDragons, "End of Dragons", [
			new EncounterRow("End of Dragons", [
				new NormalEncounter(Encounter.AetherbladeHideout, normalModeSince: EoDRelease, challengeModeSince: AHCMRelease),
				new NormalEncounter(Encounter.XunlaiJadeJunkyard, normalModeSince: EoDRelease, challengeModeSince: XJJCMRelease),
				new NormalEncounter(Encounter.KainengOverlook, normalModeSince: EoDRelease, challengeModeSince: KOCMRelease),
				new NormalEncounter(Encounter.HarvestTemple, normalModeSince: EoDRelease, challengeModeSince: HTCMRelease),
				// The Old Lion's Court strike mission was released as part of Living World Season 1 and is accessible without End of Dragons (EoD).
				// However, achievements for it are within EoD categories and it is usually considered part of the EoD strike missions.
				// Since adding more categories is problematic for layout, it is a simple decision to include it in the EoD category.
				new NormalEncounter(Encounter.OldLionsCourt, normalModeSince: OLCRelease, challengeModeSince: OLCCMRelease),
			]),
		]),
		new EncounterGroup(EncounterCategory.StrikeSecretsOfTheObscure, "Secrets of the Obscure", [
			new EncounterRow("Secrets of the Obscure", [
				new NormalEncounter(Encounter.CosmicObservatory, normalModeSince: SotORelease,
					challengeModeSince: COCMRelease),
				new NormalEncounter(Encounter.TempleOfFebe, normalModeSince: SotORelease, challengeModeSince: ToFCMRelease),
			]),
		])
	];

	private static readonly Dictionary<IFinishableEncounter, EncounterCategory> CategoriesByEncounter = EncounterGroups
		.SelectMany(group => group.Rows.SelectMany(row => row.Encounters.Select(encounter => (encounter, Id: group.Category))))
		.ToDictionary(x => x.encounter, x => x.Id);

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
			foreach (var category in Enum.GetValues<EncounterCategory>())
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

	private event EventHandler SelectedResetChanged;
	private event EventHandler DataUpdated;

	private readonly Dictionary<IFinishableEncounter, (WeeklyCheckBox, WeeklyCheckBox)> encounterCheckboxes = [];
	private readonly DropDown accountFilterBox;
	private readonly Button addNewAccountButton;
	private readonly Button removeAccountButton;
	private readonly GridView<ResetWeek> weekGrid;
	private readonly List<CheckBox> groupCheckboxes = [];
	private readonly Timer timer = new Timer(TimeSpan.FromMinutes(1));

	public WeeklyClears(ImageProvider imageProvider)
	{
		this.imageProvider = imageProvider;
		accountFilterBox = new DropDown { Width = 350 };
		addNewAccountButton = new Button { Text = "Add account" };
		removeAccountButton = new Button { Text = "Remove", Enabled = accountFilterBox.SelectedIndex != -1 };
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
			removeAccountButton.Enabled = accountFilterBox.SelectedIndex != -1;
			DataUpdated?.Invoke(this, EventArgs.Empty);
		};

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
					accountFilterBox.SelectedIndex = Settings.PlayerAccountNames.Count - 1;
				}
				else
				{
					accountFilterBox.SelectedIndex = Settings.PlayerAccountNames.ToList().IndexOf(selectedAccount);
				}

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

		foreach (var group in EncounterGroups)
		{
			foreach (var row in group.Rows)
			{
				foreach (var encounter in row.Encounters)
				{
					var normalModeCheckbox = new WeeklyCheckBox();
					var challengeModeCheckbox = new WeeklyCheckBox();

					void UpdateCheckbox(WeeklyCheckBox checkBox, bool isChallengeMode)
					{
						var availability = isChallengeMode ? encounter.GetChallengeModeAvailability(Reset) : encounter.GetNormalModeAvailability(Reset);
						var standardLabelText = isChallengeMode ? "Challenge Mode" : "Normal Mode";
						switch (availability)
						{
							case EncounterAvailability.Available:
								var finished = finishedEncounters.Contains((AccountFilter, Reset, encounter, isChallengeMode));
								checkBox.Image = finished ? imageProvider.GetGreenCheckIcon() : imageProvider.GetRedCrossIcon();
								checkBox.Text = standardLabelText;
								break;
							case EncounterAvailability.DoesNotExist:
								// We only want to show the unavailability within the normal mode row.
								checkBox.Image = isChallengeMode ? null : imageProvider.GetNotYetAvailableIcon();
								checkBox.Text = isChallengeMode ? "" : "Did not exist";
								break;
							case EncounterAvailability.NotLogged:
								// We only want to show the unavailability within the normal mode row.
								checkBox.Image = isChallengeMode ? null : imageProvider.GetGrayQuestionMarkIcon();
								checkBox.Text = isChallengeMode ? "" : standardLabelText;
								checkBox.Text = isChallengeMode ? "" : "No logs";
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}

					DataUpdated += (_, _) => UpdateCheckbox(normalModeCheckbox, false);
					SelectedResetChanged += (_, _) => UpdateCheckbox(normalModeCheckbox, false);
					DataUpdated += (_, _) => UpdateCheckbox(challengeModeCheckbox, true);
					SelectedResetChanged += (_, _) => UpdateCheckbox(challengeModeCheckbox, true);

					encounterCheckboxes[encounter] = (normalModeCheckbox, challengeModeCheckbox);
				}
			}
		}

		weekGrid = new GridView<ResetWeek> { Height = 150 };
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

		foreach ((string name, EncounterCategory category, bool hasCMs) in (ReadOnlySpan<(string, EncounterCategory, bool)>)
		         [
			         ("Raid", EncounterCategory.Raids, true),
			         ("IBS", EncounterCategory.StrikeIcebroodSaga, false),
			         ("EoD", EncounterCategory.StrikeEndOfDragons, true),
			         ("SotO", EncounterCategory.StrikeSecretsOfTheObscure, true)
		         ])
		{
			var visible = Settings.WeeklyClearGroups.Contains(category);
			var nmCountColumn = new GridColumn
			{
				HeaderText = $"{name} NMs",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<ResetWeek, string>(week => week.FinishedNormalModesByCategory[category].ToString()),
					TextAlignment = TextAlignment.Center
				},
				Visible = visible
			};
			var nmPercentageColumn = new GridColumn
			{
				HeaderText = $"{name} NMs",
				DataCell = new ProgressCell
				{
					Binding = new DelegateBinding<ResetWeek, float?>(week =>
						(float) week.FinishedNormalModesByCategory[category] / Math.Max(1, EncounterGroups
							.Where(group => group.Category == category)
							.SelectMany(group => group.Rows.SelectMany(row => row.Encounters))
							.Count(encounter =>
								encounter.GetNormalModeAvailability(week.Reset) == EncounterAvailability.Available))),
				},
				Visible = visible
			};
			var cmCountColumn = new GridColumn
			{
				HeaderText = $"{name} CMs",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<ResetWeek, string>(week => week.FinishedChallengeModesByCategory[category].ToString()),
					TextAlignment = TextAlignment.Center
				},
				Visible = visible
			};
			var cmPercentageColumn = new GridColumn
			{
				HeaderText = $"{name} CMs",
				DataCell = new ProgressCell
				{
					Binding = new DelegateBinding<ResetWeek, float?>(week =>
						(float) week.FinishedChallengeModesByCategory[category] / Math.Max(1, EncounterGroups
							.Where(group => group.Category == category)
							.SelectMany(group => group.Rows.SelectMany(row => row.Encounters))
							.Count(encounter =>
								encounter.GetChallengeModeAvailability(week.Reset) == EncounterAvailability.Available))),
				},
				Visible = visible
			};

			weekGrid.Columns.Add(nmCountColumn);
			weekGrid.Columns.Add(nmPercentageColumn);
			if (hasCMs)
			{
				weekGrid.Columns.Add(cmCountColumn);
				weekGrid.Columns.Add(cmPercentageColumn);
			}

			Settings.WeeklyClearGroupsChanged += (_, _) =>
			{
				var visibility = Settings.WeeklyClearGroups.Contains(category);
				nmCountColumn.Visible = visibility;
				nmPercentageColumn.Visible = visibility;
				cmCountColumn.Visible = visibility;
				cmPercentageColumn.Visible = visibility;
			};
		}

		var filteredWeeks = new FilterCollection<ResetWeek>(weeks);
		filteredWeeks.Filter = week => week.Reset <= DateOnly.FromDateTime(DateTime.UtcNow);
		timer.Elapsed += (_, _) => Application.Instance.Invoke(() => filteredWeeks.Refresh());
		
		weekGrid.DataStore = filteredWeeks;
		weekGrid.SelectedRow = 0;

		weekGrid.SelectionChanged += (_, _) =>
		{
			Reset = weekGrid.SelectedItem?.Reset ?? GetResetBefore(DateTimeOffset.Now);
		};

		DataUpdated += (_, _) =>
		{
			weekGrid.ReloadData(Eto.Forms.Range.FromLength(0, weeks.Count));
		};

		foreach (var group in EncounterGroups)
		{
			var isEnabled = Settings.WeeklyClearGroups.Contains(group.Category);
			var groupCheckbox = new CheckBox { Checked = isEnabled, Text = group.Name };
			groupCheckbox.CheckedChanged += (_, _) =>
			{
				if (groupCheckbox.Checked ?? false)
				{
					Settings.WeeklyClearGroups = Settings.WeeklyClearGroups.Append(group.Category).ToList();
				}
				else
				{
					Settings.WeeklyClearGroups = Settings.WeeklyClearGroups.Where(x => x != group.Category).ToList();
				}

				RecreateLayout();
			};

			groupCheckboxes.Add(groupCheckbox);
		}

		RecreateLayout();

		Shown += (_, _) => timer.Start();
		UnLoad += (_, _) => timer.Stop();
	}

	private void RecreateLayout()
	{
		// Maintaining everything within one table works best for aligning boxes vertically AND horizontally.
		// Since we need to be able to hide individual rows according to user preferences,
		// we need to recreate the layout every time these preferences change as the table does not support removing/adding rows dynamically.
		// Checkboxes are stored within `encounterCheckboxes` and are reused on table rebuilds.

		SuspendLayout();
		Clear();
		var enabledGroups = EncounterGroups.Where(group => Settings.WeeklyClearGroups.Contains(group.Category)).ToList();
		Control middleControl;
		if (enabledGroups.Count > 0)
		{
			// We use the first column for an image identifying the row
			var cols = enabledGroups.Max(group => group.Rows.Max(row => row.Encounters.Count)) + 1;
			var rows = enabledGroups.Sum(group => group.Rows.Count);
			var table = new TableLayout(cols, rows);
			table.Spacing = new Size(10, 6);

			int rowIndex = 0;
			int raidWing = 0;
			foreach (var group in enabledGroups)
			{
				foreach (var row in group.Rows)
				{
					if (group.Category == EncounterCategory.Raids)
					{
						raidWing++;
					}

					// An image at the start of the row
					var rowImage = new ImageView
					{
						Size = new Size(48, 32),
						Image = group.Category switch
						{
							EncounterCategory.Raids => raidWing switch
							{
								1 => imageProvider.GetWideRaidWing1Icon(),
								2 => imageProvider.GetWideRaidWing2Icon(),
								3 => imageProvider.GetWideRaidWing3Icon(),
								4 => imageProvider.GetWideRaidWing4Icon(),
								5 => imageProvider.GetWideRaidWing5Icon(),
								6 => imageProvider.GetWideRaidWing6Icon(),
								7 => imageProvider.GetWideRaidWing7Icon(),
								_ => throw new ArgumentOutOfRangeException()
							},
							EncounterCategory.StrikeIcebroodSaga => imageProvider.GetWideIcebroodSagaIcon(),
							EncounterCategory.StrikeEndOfDragons => imageProvider.GetWideEndOfDragonsIcon(),
							EncounterCategory.StrikeSecretsOfTheObscure => imageProvider.GetWideSecretsOfTheObscureIcon(),
							_ => throw new ArgumentOutOfRangeException()
						}
					};
					table.Add(rowImage, 0, rowIndex);

					// Checkboxes for encounters
					for (int col = 0; col < row.Encounters.Count; col++)
					{
						var encounter = row.Encounters[col];
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

						var (normalModeCheckbox, challengeModeCheckbox) = encounterCheckboxes[encounter];

						// Setting Spacing of the Stack Layout results in a trailing gap which we cannot really afford here,
						// we instead use this image view without an image to create a spacer of a specific height.
						var spacer = new ImageView { Size = new Size(6, 6) };
						var layout = new StackLayout { Items = { normalModeCheckbox, spacer, challengeModeCheckbox } };
						var box = new GroupBox { Text = name, Content = layout, Padding = new Padding(4, 2) };

						table.Add(box, col + 1, rowIndex);
					}

					rowIndex++;
				}
			}

			middleControl = table;
		}
		else
		{
			middleControl = new StackLayout
			{
				Padding = new Padding(20),
				Items =
				{
					new Label
					{
						Text = "To see weekly clears of encounters, select groups you are interested in by checking checkboxes above.",
						Wrap = WrapMode.Word
					}
				},
			};
		}

		BeginVertical(padding: new Padding(0, 2), spacing: new Size(10, 10));
		{
			AddRow(accountFilterBox, addNewAccountButton, removeAccountButton, null);
		}
		EndBeginVertical(padding: new Padding(0, 2), spacing: new Size(10, 10));
		{
			BeginHorizontal();
			foreach (var groupCheckbox in groupCheckboxes)
			{
				Add(groupCheckbox);
			}

			Add(null);
			EndHorizontal();
		}
		EndBeginVertical(spacing: new Size(10, 10), yscale: true);
		{
			Add(new Splitter
			{
				Panel1 = new Scrollable
				{
					Content = middleControl,
					ExpandContentWidth = false,
					ExpandContentHeight = false,
					Padding = new Padding(10, 0, 0, 0),
					MinimumSize = new Size(0, 0)
				},
				Panel2 = weekGrid,
				Orientation = Orientation.Vertical,
				Panel1MinimumSize = 0,
				Panel2MinimumSize = 0,
				FixedPanel = SplitterFixedPanel.Panel2,
			});
		}
		EndVertical();
		Create();
		ResumeLayout();
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

			Application.Instance.Invoke(() => { UpdateFinishedLogs(finished); });
		});
	}

	private static DateOnly GetResetBefore(DateTimeOffset time)
	{
		// Weekly reset occurs on Monday 07:30 UTC
		// Rewards are given when the encounter ends, so we need to check the end time

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
	/// <returns></returns>
	private static List<DateOnly> GetAllResets()
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
}