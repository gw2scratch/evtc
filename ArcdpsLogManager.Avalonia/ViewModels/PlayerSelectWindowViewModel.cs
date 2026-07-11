using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the player-picker dialog (Avalonia counterpart of the Eto
	/// <c>Dialogs/PlayerSelectDialog.cs</c>/<c>Sections/PlayerList.cs</c>): lets the user pick an
	/// account seen among the currently loaded logs instead of typing a raw account name. Reuses
	/// <see cref="PlayerAccountRow.BuildFromLogs"/>, the same aggregation the Players tab uses, so
	/// the "accounts seen in logs" list is not computed twice.
	/// </summary>
	public partial class PlayerSelectWindowViewModel : ObservableObject
	{
		private readonly List<PlayerAccountRow> allPlayers;

		[ObservableProperty] private string filter = "";
		[ObservableProperty] private PlayerAccountRow? selectedPlayer;

		public ObservableCollection<PlayerAccountRow> Players { get; } = new();

		public PlayerSelectWindowViewModel(IEnumerable<LogData> logs, ImageProvider images)
		{
			allPlayers = PlayerAccountRow.BuildFromLogs(logs, images);
			ApplyFilter();
		}

		partial void OnFilterChanged(string value) => ApplyFilter();

		private void ApplyFilter()
		{
			var previouslySelected = SelectedPlayer;
			Players.Clear();
			IEnumerable<PlayerAccountRow> shown = allPlayers;
			if (!string.IsNullOrWhiteSpace(Filter))
			{
				shown = shown.Where(p =>
					p.AccountName.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) ||
					p.Characters.Any(c => c.Name.Contains(Filter, StringComparison.CurrentCultureIgnoreCase)));
			}

			foreach (var row in shown)
			{
				Players.Add(row);
			}

			SelectedPlayer = previouslySelected != null && Players.Contains(previouslySelected)
				? previouslySelected
				: null;
		}
	}
}
