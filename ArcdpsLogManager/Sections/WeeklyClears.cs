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

	// The release-date schedule, the CategoriesByEncounter lookup, the reset-week math
	// (GetResetBefore/GetAllResets below) and the finished-encounter aggregation
	// (UpdateDataFromLogs below) were moved to
	// GW2Scratch.ArcdpsLogManager.Sections.Clears.WeeklyClearSchedule (in ArcdpsLogManager.Core)
	// so the Avalonia port's Weekly Clears section can reuse them without duplicating this
	// table/logic. This class now just aliases the schedule for the display code below.
	private static IReadOnlyList<EncounterGroup> EncounterGroups => WeeklyClearSchedule.EncounterGroups;

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

	private DateOnly reset = WeeklyClearSchedule.GetResetBefore(DateTimeOffset.Now);

	private DateOnly Reset
	{
		get => reset;
		set
		{
			reset = value;
			SelectedResetChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private readonly List<ResetWeek> weeks = WeeklyClearSchedule.GetAllResets().Select(x => new ResetWeek(x)).ToList();

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
		WeeklyClearSchedule.PopulateWeekCounts(weeks, finishedEncounters, AccountFilter);
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
			         ("IBS", EncounterCategory.RaidEncountersIcebroodSaga, false),
			         ("EoD", EncounterCategory.RaidEncountersEndOfDragons, true),
			         ("SotO", EncounterCategory.RaidEncountersSecretsOfTheObscure, true),
					 ("VoE", EncounterCategory.RaidEncountersVisionsOfEternity, true)
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
			Reset = weekGrid.SelectedItem?.Reset ?? WeeklyClearSchedule.GetResetBefore(DateTimeOffset.Now);
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
								8 => imageProvider.GetWideRaidWing8Icon(),
								_ => throw new ArgumentOutOfRangeException()
							},
							EncounterCategory.RaidEncountersIcebroodSaga => imageProvider.GetWideIcebroodSagaIcon(),
							EncounterCategory.RaidEncountersEndOfDragons => imageProvider.GetWideEndOfDragonsIcon(),
							EncounterCategory.RaidEncountersSecretsOfTheObscure => imageProvider.GetWideSecretsOfTheObscureIcon(),
							EncounterCategory.RaidEncountersVisionsOfEternity => imageProvider.GetWideVisionsOfEternityIcon(),
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
							NormalEncounter normalEncounter => EncounterNames.TryGetEncounterNameForLanguage(out var normalName, GameLanguage.English, normalEncounter.Encounter)
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
		// We cache the logs to be able to select players from them later.
		this.logs = logs.ToList();
		// We need to store this specific list for the Task as a race condition could change this.logs before it is accessed.
		var usedLogs = this.logs;
		Task.Run(() =>
		{
			var finished = WeeklyClearSchedule.ComputeFinishedEncounters(usedLogs);

			Application.Instance.Invoke(() => { UpdateFinishedLogs(finished); });
		});
	}
}