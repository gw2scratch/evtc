using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Dialogs;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Sections.Clears;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GW2Scratch.ArcdpsLogManager.Sections;

public class WeeklyClears : DynamicLayout
{
	private static readonly List<EncounterColumn> EncounterColumns =
	[
		new EncounterColumn("Raids", [
			new EncounterRow("Spirit Woods (W1)", [
				new NormalEncounter(Encounter.ValeGuardian, false, Category.Raid),
				new UnsupportedEncounter("Spirit Woods", Category.Raid),
				new NormalEncounter(Encounter.Gorseval, false, Category.Raid),
				new NormalEncounter(Encounter.Sabetha, false, Category.Raid)
			]),
			new EncounterRow("Salvation Pass (W2)", [
				new NormalEncounter(Encounter.Slothasor, false, Category.Raid),
				new NormalEncounter(Encounter.BanditTrio, false, Category.Raid),
				new NormalEncounter(Encounter.Matthias, false, Category.Raid)
			]),
			new EncounterRow("Stronghold of the Faithful (W3)", [
				new NormalEncounter(Encounter.Escort, false, Category.Raid),
				new NormalEncounter(Encounter.KeepConstruct, true, Category.Raid),
				new NormalEncounter(Encounter.TwistedCastle, false, Category.Raid),
				new NormalEncounter(Encounter.Xera, false, Category.Raid)
			]),
			new EncounterRow("Bastion of the Penitent (W4)", [
				new NormalEncounter(Encounter.Cairn, true, Category.Raid),
				new NormalEncounter(Encounter.MursaatOverseer, true, Category.Raid),
				new NormalEncounter(Encounter.Samarog, true, Category.Raid),
				new NormalEncounter(Encounter.Deimos, true, Category.Raid)
			]),
			new EncounterRow("Hall of Chains (W5)", [
				new NormalEncounter(Encounter.SoullessHorror, true, Category.Raid),
				new NormalEncounter(Encounter.RiverOfSouls, false, Category.Raid),
				new MultipartEncounter("Statues", [Encounter.BrokenKing, Encounter.EaterOfSouls, Encounter.Eyes], false, Category.Raid),
				new NormalEncounter(Encounter.Dhuum, true, Category.Raid)
			]),
			new EncounterRow("Mythwright Gambit (W6)", [
				new NormalEncounter(Encounter.ConjuredAmalgamate, true, Category.Raid),
				new NormalEncounter(Encounter.TwinLargos, true, Category.Raid),
				new NormalEncounter(Encounter.Qadim, true, Category.Raid)
			]),
			new EncounterRow("The Key of Ahdashim (W7)", [
				new NormalEncounter(Encounter.Adina, true, Category.Raid),
				new NormalEncounter(Encounter.Sabir, true, Category.Raid),
				new NormalEncounter(Encounter.QadimThePeerless, true, Category.Raid)
			]),
			new EncounterRow("End of Dragons", [
				new NormalEncounter(Encounter.AetherbladeHideout, true, Category.StrikeEndOfDragons),
				new NormalEncounter(Encounter.XunlaiJadeJunkyard, true, Category.StrikeEndOfDragons),
				new NormalEncounter(Encounter.KainengOverlook, true, Category.StrikeEndOfDragons),
				new NormalEncounter(Encounter.HarvestTemple, true, Category.StrikeEndOfDragons),
				new NormalEncounter(Encounter.OldLionsCourt, true, Category.StrikeEndOfDragons),
			]),
			new EncounterRow("Secrets of the Obscure", [
				new NormalEncounter(Encounter.CosmicObservatory, true, Category.StrikeSecretsOfTheObscure),
				new NormalEncounter(Encounter.TempleOfFebe, true, Category.StrikeSecretsOfTheObscure),
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
	private HashSet<(string AccountName, DateOnly ResetDate, IFinishableEncounter Encounter, bool ChallengeMode)> finishedEncounters;

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
		// TODO: Add persistence
		var accounts = new ObservableCollection<string>();
		var accountFilterBox = new DropDown { Width = 350 };
		accountFilterBox.DataStore = accounts;
		accountFilterBox.SelectedValueChanged += (_, _) =>
		{
			AccountFilter = $":{accountFilterBox.SelectedValue}";
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
				if (!accounts.Contains(selectedAccount))
				{
					accounts.Add(selectedAccount.TrimStart(':'));
				}

				accountFilterBox.SelectedIndex = accounts.Count - 1;
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
				if (accounts.Count == 1)
				{
					accountFilterBox.SelectedIndex = -1;
					AccountFilter = "";
					removeAccountButton.Enabled = false;
				}
				else
				{
					accountFilterBox.SelectedIndex = 0;
					AccountFilter = $":{accounts[0]}";
				}
				accounts.RemoveAt(oldIndex);

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

					// For simplicity, we create the checkbox even if they are not used,
					// but we do not wire them up to updates if the encounter does support the mode.
					var normalModeCheckbox = new ImageView { Size = iconSize };
					var challengeModeCheckbox = new ImageView { Size = iconSize };

					if (encounter.HasNormalMode)
					{
						void UpdateNormalModeCheckbox()
						{
							var finished = finishedEncounters.Contains((AccountFilter, Reset, encounter, false));
							normalModeCheckbox.Image = finished ? imageProvider.GetGreenCheckIcon() : imageProvider.GetRedCrossIcon();
						}

						DataUpdated += (_, _) => UpdateNormalModeCheckbox();
						SelectedResetChanged += (_, _) => UpdateNormalModeCheckbox();
					}

					if (encounter.HasChallengeMode)
					{
						void UpdateChallengeModeCheckbox()
						{
							var finished = finishedEncounters.Contains((AccountFilter, Reset, encounter, true));
							challengeModeCheckbox.Image = finished ? imageProvider.GetGreenCheckIcon() : imageProvider.GetRedCrossIcon();
						}

						DataUpdated += (_, _) => UpdateChallengeModeCheckbox();
						SelectedResetChanged += (_, _) => UpdateChallengeModeCheckbox();
					}

					Control content;
					switch ((encounter.HasNormalMode, encounter.HasChallengeMode))
					{
						case (false, false):
						{
							var layout = new StackLayout
							{
								Items =
								{
									new ImageView { Image = imageProvider.GetGrayQuestionMarkIcon(), Size = iconSize }, new Label { Text = "No logs" }
								},
								Orientation = Orientation.Horizontal,
								Spacing = 6,
							};
							content = layout;
							break;
						}
						case (true, false):
						{
							var layout = new StackLayout
							{
								Items = { normalModeCheckbox, new Label { Text = "Normal mode" } }, Orientation = Orientation.Horizontal, Spacing = 6,
							};
							content = layout;
							break;
						}
						case (false, true):
						{
							var layout = new StackLayout
							{
								Items = { challengeModeCheckbox, new Label { Text = "Challenge mode" } }, Orientation = Orientation.Horizontal, Spacing = 6,
							};
							content = layout;
							break;
						}
						case (true, true):
						{
							var layout = new StackLayout
							{
								Items =
								{
									new StackLayout
									{
										Items = { normalModeCheckbox, new Label { Text = "Normal mode" } },
										Orientation = Orientation.Horizontal,
										Spacing = 6,
									},
									new StackLayout
									{
										Items = { challengeModeCheckbox, new Label { Text = "Challenge mode" } },
										Orientation = Orientation.Horizontal,
										Spacing = 6,
									}
								},
								Spacing = 6
							};
							content = layout;
							break;
						}
					}

					var box = new GroupBox { Text = name, Content = content, Padding = new Padding(4, 2) };

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
						(float) week.FinishedNormalModesByCategory[category] / EncounterColumns
							.SelectMany(col => col.Rows.SelectMany(row => row.Encounters))
							.Count(encounter => encounter.HasNormalMode && encounter.Category == category))
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
						(float) week.FinishedChallengeModesByCategory[category] / EncounterColumns
							.SelectMany(col => col.Rows.SelectMany(row => row.Encounters))
							.Count(encounter => encounter.HasChallengeMode && encounter.Category == category)),
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
					if (encounter.HasNormalMode && encounter.IsSatisfiedBy(weekLogs))
					{
						finished.Add((key.accountName, key.resetDate, encounter, false));
					}

					if (encounter.HasChallengeMode && encounter.IsChallengeModeSatisfiedBy(weekLogs))
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