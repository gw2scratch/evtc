using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Sections.Clears;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using EncounterCategory = GW2Scratch.ArcdpsLogManager.Sections.Clears.EncounterCategory;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the Weekly Clears section (Avalonia counterpart of the Eto
	/// <c>Sections/WeeklyClears.cs</c>): a week x encounter completion grid for a selected account,
	/// built over Core's <see cref="WeeklyClearSchedule"/> (release-date/reset table) filtered by
	/// the encounter category groups configured in <see cref="Settings.WeeklyClearGroups"/>.
	/// </summary>
	/// <remarks>
	/// All clear-detection and reset-week math is delegated to
	/// <see cref="WeeklyClearSchedule"/> (moved to Core from the Eto view so it is not
	/// reimplemented here); this view model only projects that data into bindable rows and reacts
	/// to the selected account / reset week / enabled category settings.
	/// </remarks>
	public partial class WeeklyClearsSectionViewModel : ObservableObject, IDisposable
	{
		private readonly ImageProvider images;
		private readonly Dictionary<IFinishableEncounter, EncounterBoxRow> boxesByEncounter = new();
		private readonly List<ResetWeekRow> allWeeks;
		private readonly DispatcherTimer refreshTimer;

		private List<LogData> logs = new();
		private HashSet<(string AccountName, DateOnly ResetDate, IFinishableEncounter Encounter, bool ChallengeMode)> finishedEncounters = new();
		private string accountFilter = "";

		public ObservableCollection<EncounterGroupSection> Groups { get; } = new();
		public ObservableCollection<ResetWeekRow> VisibleWeeks { get; } = new();
		public ObservableCollection<string> AccountNames { get; } = new();

		/// <summary>Exposed so the view's code-behind can construct the player-picker dialog (this
		/// tab deliberately ignores the active log filters, so these are the same unfiltered logs
		/// used for clear detection — see <see cref="UpdateFromLogs"/>).</summary>
		public IReadOnlyList<LogData> LoadedLogs => logs;

		/// <summary>Exposed so the view's code-behind can construct the player-picker dialog.</summary>
		public ImageProvider Images => images;

		public WeeklyClearsSectionViewModel(ImageProvider images)
		{
			this.images = images;

			foreach (var group in WeeklyClearSchedule.EncounterGroups)
			{
				var rows = new List<EncounterRowSection>();
				int raidWing = 0;
				foreach (var row in group.Rows)
				{
					if (group.Category == EncounterCategory.Raids)
					{
						raidWing++;
					}

					var icon = GetRowIcon(group.Category, raidWing);
					var boxes = new List<EncounterBoxRow>();
					foreach (var encounter in row.Encounters)
					{
						var box = new EncounterBoxRow(GetEncounterName(encounter), encounter);
						boxes.Add(box);
						boxesByEncounter[encounter] = box;
					}

					rows.Add(new EncounterRowSection(row.Name, icon, boxes));
				}

				bool enabled = Settings.WeeklyClearGroups.Contains(group.Category);
				var groupSection = new EncounterGroupSection(group.Category, group.Name, rows, enabled);
				groupSection.PropertyChanged += OnGroupEnabledChanged;
				Groups.Add(groupSection);
			}

			HasAnyGroupEnabled = Groups.Any(g => g.IsEnabled);
			Settings.WeeklyClearGroupsChanged += OnSettingsGroupsChanged;

			allWeeks = WeeklyClearSchedule.BuildResetWeeks().Select(w => new ResetWeekRow(w)).ToList();
			foreach (var week in allWeeks)
			{
				week.Refresh();
			}

			RefreshVisibleWeeks();
			SelectedWeek = VisibleWeeks.FirstOrDefault();

			RebuildAccountNames();
			Settings.PlayerAccountNamesChanged += OnSettingsAccountNamesChanged;

			refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
			refreshTimer.Tick += (_, _) => RefreshVisibleWeeks();
			refreshTimer.Start();

			UpdateCheckboxesForSelectedWeek();
		}

		[ObservableProperty] private ResetWeekRow? selectedWeek;
		[ObservableProperty] private string? selectedAccountName;
		[ObservableProperty] private bool hasAnyGroupEnabled = true;

		partial void OnSelectedWeekChanged(ResetWeekRow? value)
		{
			UpdateCheckboxesForSelectedWeek();
		}

		partial void OnSelectedAccountNameChanged(string? value)
		{
			accountFilter = value != null ? $":{value}" : "";
			RecomputeWeekCounts();
			UpdateCheckboxesForSelectedWeek();
		}

		/// <summary>
		/// Recomputes which encounters have been cleared from the currently loaded (filtered) logs.
		/// The actual clear-detection work runs off the UI thread via
		/// <see cref="WeeklyClearSchedule.ComputeFinishedEncounters"/>.
		/// </summary>
		public void UpdateFromLogs(IEnumerable<LogData> newLogs)
		{
			// Cached so a future account-selection dialog could offer players seen in these logs;
			// also needed because the background task must not read a field that could be replaced
			// by a subsequent call while it is still running.
			logs = newLogs.ToList();
			var usedLogs = logs;

			_ = Task.Run(() =>
			{
				var finished = WeeklyClearSchedule.ComputeFinishedEncounters(usedLogs);
				Dispatcher.UIThread.Post(() =>
				{
					finishedEncounters = finished;
					RecomputeWeekCounts();
					UpdateCheckboxesForSelectedWeek();
				});
			});
		}

		private void RecomputeWeekCounts()
		{
			WeeklyClearSchedule.PopulateWeekCounts(allWeeks.Select(w => w.Week).ToList(), finishedEncounters, accountFilter);
			foreach (var week in allWeeks)
			{
				week.Refresh();
			}
		}

		private void UpdateCheckboxesForSelectedWeek()
		{
			if (SelectedWeek == null)
			{
				return;
			}

			var reset = SelectedWeek.Week.Reset;
			foreach (var (encounter, box) in boxesByEncounter)
			{
				UpdateCheckbox(box.NormalMode, encounter, reset, false);
				UpdateCheckbox(box.ChallengeMode, encounter, reset, true);
			}
		}

		private void UpdateCheckbox(EncounterCheckboxRow checkBox, IFinishableEncounter encounter, DateOnly reset, bool isChallengeMode)
		{
			var availability = isChallengeMode ? encounter.GetChallengeModeAvailability(reset) : encounter.GetNormalModeAvailability(reset);
			string standardLabelText = isChallengeMode ? "Challenge Mode" : "Normal Mode";
			switch (availability)
			{
				case EncounterAvailability.Available:
					bool finished = finishedEncounters.Contains((accountFilter, reset, encounter, isChallengeMode));
					checkBox.Icon = finished ? images.GetGreenCheckIcon() : images.GetRedCrossIcon();
					checkBox.Text = standardLabelText;
					checkBox.IsMonochromeIcon = false;
					break;
				case EncounterAvailability.DoesNotExist:
					// We only want to show the unavailability within the normal mode row.
					checkBox.Icon = isChallengeMode ? null : images.GetNotYetAvailableIcon();
					checkBox.Text = isChallengeMode ? "" : "Did not exist";
					checkBox.IsMonochromeIcon = !isChallengeMode;
					break;
				case EncounterAvailability.NotLogged:
					// We only want to show the unavailability within the normal mode row.
					checkBox.Icon = isChallengeMode ? null : images.GetGrayQuestionMarkIcon();
					checkBox.Text = isChallengeMode ? "" : "No logs";
					checkBox.IsMonochromeIcon = !isChallengeMode;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void RefreshVisibleWeeks()
		{
			var today = DateOnly.FromDateTime(DateTime.UtcNow);
			var shouldShow = allWeeks.Where(w => w.Week.Reset <= today).ToList();
			if (shouldShow.Count == VisibleWeeks.Count)
			{
				return;
			}

			var previousSelection = SelectedWeek?.Week.Reset;
			VisibleWeeks.Clear();
			foreach (var week in shouldShow)
			{
				VisibleWeeks.Add(week);
			}

			SelectedWeek = previousSelection.HasValue
				? VisibleWeeks.FirstOrDefault(w => w.Week.Reset == previousSelection.Value) ?? VisibleWeeks.FirstOrDefault()
				: VisibleWeeks.FirstOrDefault();
		}

		private void RebuildAccountNames()
		{
			AccountNames.Clear();
			foreach (var name in Settings.PlayerAccountNames)
			{
				AccountNames.Add(name.TrimStart(':'));
			}

			if (AccountNames.Count > 0)
			{
				if (SelectedAccountName == null || !AccountNames.Contains(SelectedAccountName))
				{
					SelectedAccountName = AccountNames[0];
				}
			}
			else
			{
				SelectedAccountName = null;
				accountFilter = "";
				RecomputeWeekCounts();
				UpdateCheckboxesForSelectedWeek();
			}
		}

		private void OnSettingsAccountNamesChanged(object? sender, EventArgs e)
		{
			Dispatcher.UIThread.Post(RebuildAccountNames);
		}

		private void OnGroupEnabledChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(EncounterGroupSection.IsEnabled) || sender is not EncounterGroupSection group)
			{
				return;
			}

			var current = Settings.WeeklyClearGroups.ToList();
			if (group.IsEnabled)
			{
				if (!current.Contains(group.Category))
				{
					current.Add(group.Category);
				}
			}
			else
			{
				current.Remove(group.Category);
			}

			Settings.WeeklyClearGroups = current;
			HasAnyGroupEnabled = Groups.Any(g => g.IsEnabled);
		}

		private void OnSettingsGroupsChanged(object? sender, EventArgs e)
		{
			Dispatcher.UIThread.Post(() =>
			{
				foreach (var group in Groups)
				{
					group.IsEnabled = Settings.WeeklyClearGroups.Contains(group.Category);
				}

				HasAnyGroupEnabled = Groups.Any(g => g.IsEnabled);
			});
		}

		/// <summary>
		/// Adds (and selects) the given account, in the leading-':' storage format, unless already
		/// present. Called from the view's code-behind once the player-picker dialog
		/// (<see cref="Views.PlayerSelectWindow"/>) returns a selection — the Avalonia counterpart
		/// of the Eto <c>WeeklyClears</c>' <c>addNewAccountButton</c> handler constructing a
		/// <c>PlayerSelectDialog</c>.
		/// </summary>
		public void AddAccountByName(string accountName)
		{
			var current = Settings.PlayerAccountNames.ToList();
			if (!current.Contains(accountName))
			{
				current.Add(accountName);
				Settings.PlayerAccountNames = current;
			}

			SelectedAccountName = accountName.TrimStart(':');
		}

		[RelayCommand]
		private void RemoveAccount()
		{
			if (SelectedAccountName == null)
			{
				return;
			}

			string accountName = $":{SelectedAccountName}";
			Settings.PlayerAccountNames = Settings.PlayerAccountNames.Where(x => x != accountName).ToList();
		}

		private Bitmap GetRowIcon(EncounterCategory category, int raidWing)
		{
			return category switch
			{
				EncounterCategory.Raids => raidWing switch
				{
					1 => images.GetWideRaidWing1Icon(),
					2 => images.GetWideRaidWing2Icon(),
					3 => images.GetWideRaidWing3Icon(),
					4 => images.GetWideRaidWing4Icon(),
					5 => images.GetWideRaidWing5Icon(),
					6 => images.GetWideRaidWing6Icon(),
					7 => images.GetWideRaidWing7Icon(),
					8 => images.GetWideRaidWing8Icon(),
					_ => throw new ArgumentOutOfRangeException(nameof(raidWing)),
				},
				EncounterCategory.RaidEncountersIcebroodSaga => images.GetWideIcebroodSagaIcon(),
				EncounterCategory.RaidEncountersEndOfDragons => images.GetWideEndOfDragonsIcon(),
				EncounterCategory.RaidEncountersSecretsOfTheObscure => images.GetWideSecretsOfTheObscureIcon(),
				EncounterCategory.RaidEncountersVisionsOfEternity => images.GetWideVisionsOfEternityIcon(),
				_ => throw new ArgumentOutOfRangeException(nameof(category)),
			};
		}

		private static string GetEncounterName(IFinishableEncounter encounter)
		{
			return encounter switch
			{
				MultipartEncounter multipartEncounter => multipartEncounter.Name,
				NormalEncounter normalEncounter => EncounterNames.TryGetEncounterNameForLanguage(out var name, GameLanguage.English,
					normalEncounter.Encounter)
					? name
					: normalEncounter.Encounter.ToString(),
				UnsupportedEncounter unsupportedEncounter => unsupportedEncounter.Name,
				_ => throw new ArgumentOutOfRangeException(nameof(encounter)),
			};
		}

		public void Dispose()
		{
			refreshTimer.Stop();
			Settings.WeeklyClearGroupsChanged -= OnSettingsGroupsChanged;
			Settings.PlayerAccountNamesChanged -= OnSettingsAccountNamesChanged;
		}
	}
}
