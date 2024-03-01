using DebounceThrottle;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
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
	private static readonly List<List<IFinishableEncounter>> Categories =
	[
		[
			new NormalEncounter(Encounter.ValeGuardian, false),
			new UnsupportedEncounter("Spirit Woods"),
			new NormalEncounter(Encounter.Gorseval, false),
			new NormalEncounter(Encounter.Sabetha, false)
		],
		[
			new NormalEncounter(Encounter.Slothasor, false),
			new NormalEncounter(Encounter.BanditTrio, false),
			new NormalEncounter(Encounter.Matthias, false)
		],
		[
			new NormalEncounter(Encounter.Escort, false),
			new NormalEncounter(Encounter.KeepConstruct, true),
			new NormalEncounter(Encounter.TwistedCastle, false),
			new NormalEncounter(Encounter.Xera, false)
		],
		[
			new NormalEncounter(Encounter.Cairn, true),
			new NormalEncounter(Encounter.MursaatOverseer, true),
			new NormalEncounter(Encounter.Samarog, true),
			new NormalEncounter(Encounter.Deimos, true)
		],
		[
			new NormalEncounter(Encounter.SoullessHorror, true),
			new NormalEncounter(Encounter.RiverOfSouls, false),
			new MultipartEncounter("Statues", [Encounter.BrokenKing, Encounter.EaterOfSouls, Encounter.Eyes], false),
			new NormalEncounter(Encounter.Dhuum, true)
		],
		[
			new NormalEncounter(Encounter.ConjuredAmalgamate, true),
			new NormalEncounter(Encounter.TwinLargos, true),
			new NormalEncounter(Encounter.Qadim, true)
		],
		[
			new NormalEncounter(Encounter.Adina, true),
			new NormalEncounter(Encounter.Sabir, true),
			new NormalEncounter(Encounter.QadimThePeerless, true)
		],
	];

	private readonly DebounceDispatcher debounceDispatcher = new DebounceDispatcher(200);
	private string AccountFilter { get; set; } = "";

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

	// Not set directly, instead set through UpdateFinishedLogs
	private HashSet<(string AccountName, DateOnly ResetDate, IFinishableEncounter Encounter, bool ChallengeMode)> finishedEncounters;
	private List<ResetWeek> weeks = [];

	private void UpdateFinishedLogs(HashSet<(string AccountName, DateOnly ResetDate, IFinishableEncounter Encounter, bool ChallengeMode)> finished)
	{
		finishedEncounters = finished;
		weeks = finished.Select(x => x.ResetDate).Distinct().OrderByDescending(x => x).Select(x => new ResetWeek(x)).ToList();
		DataUpdated?.Invoke(this, EventArgs.Empty);
	}

	private event EventHandler SelectedResetChanged;
	private event EventHandler DataUpdated;

	public WeeklyClears()
	{
		var accountFilterBox = new TextBox();
		accountFilterBox.TextBinding.Bind(this, x => x.AccountFilter);
		accountFilterBox.TextChanged += (_, _) =>
		{
			debounceDispatcher.Debounce(() =>
				Application.Instance.InvokeAsync(() =>
				{
					DataUpdated?.Invoke(this, EventArgs.Empty);
				})
			);
		};

		var table = new TableLayout(Categories.Max(x => x.Count), Categories.Count);
		table.Spacing = new Size(30, 15);
		for (int row = 0; row < Categories.Count; row++)
		{
			List<IFinishableEncounter> category = Categories[row];
			for (int col = 0; col < category.Count; col++)
			{
				var encounter = category[col];
				var name = encounter switch
				{
					MultipartEncounter multipartEncounter => multipartEncounter.Name,
					NormalEncounter normalEncounter => EncounterNames.TryGetEncounterNameForLanguage(GameLanguage.English, normalEncounter.Encounter,
						out var normalName)
						? normalName
						: normalEncounter.Encounter.ToString(),
					UnsupportedEncounter unsupportedEncounter => unsupportedEncounter.Name,
					_ => throw new ArgumentOutOfRangeException(nameof(encounter))
				};

				// For simplicity, we create the checkbox even if they are not used,
				// but we do not wire them up to updates if the encounter does support the mode.
				var normalModeCheckbox = new CheckBox { Text = "Normal mode" };
				var challengeModeCheckbox = new CheckBox { Text = "Challenge mode" };
				if (encounter.HasNormalMode)
				{
					void UpdateNormalModeCheckbox()
					{
						var finished = finishedEncounters.Contains((AccountFilter, Reset, encounter, false));
						normalModeCheckbox.Checked = finished;
					}

					DataUpdated += (_, _) => UpdateNormalModeCheckbox();
					SelectedResetChanged += (_, _) => UpdateNormalModeCheckbox();
				}

				if (encounter.HasChallengeMode)
				{
					void UpdateChallengeModeCheckbox()
					{
						var finished = finishedEncounters.Contains((AccountFilter, Reset, encounter, true));
						challengeModeCheckbox.Checked = finished;
					}

					DataUpdated += (_, _) => UpdateChallengeModeCheckbox();
					SelectedResetChanged += (_, _) => UpdateChallengeModeCheckbox();
				}

				Control content = (encounter.HasNormalMode, encounter.HasChallengeMode) switch
				{
					(false, false) => new StackLayout { Items = { new Label { Text = "No logs" } }, Padding = new Padding(10) },
					(true, false) => new StackLayout { Items = { normalModeCheckbox } },
					(false, true) => new StackLayout { Items = { challengeModeCheckbox } },
					(true, true) => new StackLayout { Items = { normalModeCheckbox, challengeModeCheckbox } },
				};

				var box = new GroupBox { Text = name, Content = content };

				table.Add(box, col, row);
			}
		}

		var weekGrid = new GridView<ResetWeek>();
		weekGrid.Columns.Add(new GridColumn
		{
			DataCell = new TextBoxCell { Binding = new DelegateBinding<ResetWeek, string>(x => x.Reset.ToString()) }, HeaderText = "Week"
		});
		weekGrid.DataStore = weeks;

		weekGrid.SelectionChanged += (_, _) =>
		{
			Reset = weekGrid.SelectedItem?.Reset ?? GetResetBefore(DateTimeOffset.Now);
		};

		DataUpdated += (_, _) =>
		{
			// To do this properly, we would need to use SelectableFilterCollection
			// and update/remove the data within (without replacing it fully).
			// For now, we try this simplistic approach and see if it's good enough
			// (likely not).
			weekGrid.UnselectAll();
			weekGrid.DataStore = weeks;
		};

		BeginVertical();
		{
			// TODO: A better UI for this
			AddRow(accountFilterBox);
			AddRow(table, null);
		}
		EndVertical();
		AddSeparateRow(controls: [weekGrid], xscale: true);
	}

	public void UpdateDataFromLogs(IEnumerable<LogData> logs)
	{
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
				foreach (var encounter in Categories.SelectMany(x => x))
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
}