using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Players;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the advanced-filters window (Avalonia counterpart of the Eto
	/// <c>AdvancedFilterPanel</c>). Binds directly to Core's existing
	/// <see cref="LogFilters.CompositionFilters"/>, <see cref="LogFilters.InstabilityFilters"/>,
	/// <see cref="LogFilters.PlayerFilters"/> and the processing-status/dps.report-upload-status
	/// properties on <see cref="LogFilters"/> itself; no filter logic is reimplemented here.
	/// </summary>
	public partial class AdvancedFiltersWindowViewModel : ObservableObject
	{
		public LogFilters Filters { get; }

		/// <summary>The currently loaded logs, exposed so the view's code-behind can hand them to
		/// the player-picker dialog (Avalonia counterpart of the Eto <c>PlayerFilterPanel</c>'s
		/// <c>UpdateLogs</c>/<c>Logs</c> passed to <c>PlayerSelectDialog</c>). Named
		/// <c>LoadedLogs</c> rather than <c>Logs</c> since this class also refers to the
		/// <c>GW2Scratch.ArcdpsLogManager.Logs</c> namespace unqualified (e.g.
		/// <c>Logs.Filters.InstabilityFilters</c> below) via enclosing-namespace lookup — a member
		/// named <c>Logs</c> would shadow that and break those references.</summary>
		public IReadOnlyList<LogData> LoadedLogs { get; }

		/// <summary>Exposed so the view's code-behind can construct the player-picker dialog.</summary>
		public ImageProvider Images { get; }

		public ObservableCollection<CompositionFilterRow> CoreProfessionRows { get; } = new();
		public ObservableCollection<CompositionFilterRow> HeartOfThornsRows { get; } = new();
		public ObservableCollection<CompositionFilterRow> PathOfFireRows { get; } = new();
		public ObservableCollection<CompositionFilterRow> EndOfDragonsRows { get; } = new();
		public ObservableCollection<CompositionFilterRow> VisionsOfEternityRows { get; } = new();

		public ObservableCollection<InstabilityFilterRow> InstabilityRows { get; } = new();

		public IReadOnlyList<InstabilityFilters.FilterType> InstabilityTypeOptions { get; } = new[]
		{
			Logs.Filters.InstabilityFilters.FilterType.All,
			Logs.Filters.InstabilityFilters.FilterType.Any,
			Logs.Filters.InstabilityFilters.FilterType.None,
		};

		public IReadOnlyList<PlayerFilters.FilterType> PlayerTypeOptions { get; } = new[]
		{
			PlayerFilters.FilterType.All,
			PlayerFilters.FilterType.Any,
			PlayerFilters.FilterType.None,
		};

		[ObservableProperty] private RequiredPlayerFilter? selectedRequiredPlayer;

		public AdvancedFiltersWindowViewModel(LogFilters filters, ImageProvider images, IReadOnlyList<LogData> logs)
		{
			Filters = filters;
			LoadedLogs = logs;
			Images = images;

			foreach (var filter in filters.CompositionFilters.CoreProfessionFilters)
			{
				CoreProfessionRows.Add(new CompositionFilterRow(images.GetTinyProfessionIcon(filter.Profession),
					$"Core {GameNames.GetName(filter.Profession)}", filter));
			}

			void AddSpecializationRows(ObservableCollection<CompositionFilterRow> rows,
				IEnumerable<Logs.Filters.Composition.EliteSpecializationPlayerCountFilter> group)
			{
				foreach (var filter in group)
				{
					rows.Add(new CompositionFilterRow(images.GetTinyProfessionIcon(filter.EliteSpecialization),
						GameNames.GetName(filter.EliteSpecialization), filter));
				}
			}

			AddSpecializationRows(HeartOfThornsRows, filters.CompositionFilters.HeartOfThornsSpecializationFilters);
			AddSpecializationRows(PathOfFireRows, filters.CompositionFilters.PathOfFireSpecializationFilters);
			AddSpecializationRows(EndOfDragonsRows, filters.CompositionFilters.EndOfDragonsSpecializationFilters);
			AddSpecializationRows(VisionsOfEternityRows, filters.CompositionFilters.VisionsOfEternitySpecializationFilters);

			foreach (MistlockInstability instability in Enum.GetValues(typeof(MistlockInstability)))
			{
				InstabilityRows.Add(new InstabilityFilterRow(filters.InstabilityFilters, instability,
					images.GetMistlockInstabilityIcon(instability), GameNames.GetInstabilityName(instability)));
			}
		}

		[RelayCommand]
		private void ResetComposition()
		{
			Filters.CompositionFilters.ResetToDefault();
		}

		/// <summary>
		/// Adds a required-player filter for the given account (already in the leading-':' storage
		/// format), unless it is already present. Called from the view's code-behind once the
		/// player-picker dialog (<see cref="Views.PlayerSelectWindow"/>) returns a selection.
		/// </summary>
		public void AddPlayerByAccountName(string accountName)
		{
			if (Filters.PlayerFilters.RequiredPlayers.Any(p =>
				    string.Equals(p.AccountName, accountName, StringComparison.OrdinalIgnoreCase)))
			{
				return;
			}

			Filters.PlayerFilters.RequiredPlayers.Add(new RequiredPlayerFilter(accountName));
		}

		[RelayCommand]
		private void RemoveSelectedPlayer()
		{
			if (SelectedRequiredPlayer is { } player)
			{
				Filters.PlayerFilters.RequiredPlayers.Remove(player);
				SelectedRequiredPlayer = null;
			}
		}
	}
}
