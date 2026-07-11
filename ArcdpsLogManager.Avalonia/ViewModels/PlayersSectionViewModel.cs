using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the Players section (Avalonia counterpart of the Eto <c>Sections/PlayerList.cs</c>):
	/// a grid of player accounts aggregated from the logs, a name filter, and count labels.
	/// </summary>
	public partial class PlayersSectionViewModel : ObservableObject
	{
		private readonly ImageProvider images;
		private IReadOnlyList<PlayerAccountRow> allPlayers = Array.Empty<PlayerAccountRow>();

		[ObservableProperty] private string filter = "";
		[ObservableProperty] private PlayerAccountRow? selectedPlayer;
		[ObservableProperty] private string countText = "";

		public ObservableCollection<PlayerAccountRow> Players { get; } = new();

		/// <summary>The detail panel for the selected account (Avalonia counterpart of the Eto
		/// <c>Controls/PlayerDetailPanel.cs</c>), showing its known characters and log list.</summary>
		public PlayerDetailPanelViewModel Detail { get; }

		public PlayersSectionViewModel(ImageProvider images, ILogNameProvider nameProvider)
		{
			this.images = images;
			Detail = new PlayerDetailPanelViewModel(images, nameProvider);
		}

		partial void OnSelectedPlayerChanged(PlayerAccountRow? value) => Detail.Show(value);

		partial void OnFilterChanged(string value) => ApplyFilter();

		public void UpdateFromLogs(IEnumerable<LogData> logs)
		{
			allPlayers = PlayerAccountRow.BuildFromLogs(logs, images);
			ApplyFilter();
		}

		private void ApplyFilter()
		{
			Players.Clear();
			IEnumerable<PlayerAccountRow> shown = allPlayers;
			if (!string.IsNullOrWhiteSpace(Filter))
			{
				shown = shown.Where(p =>
					p.AccountName.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) ||
					p.Characters.Any(c => c.Name.Contains(Filter, StringComparison.CurrentCultureIgnoreCase)));
			}

			var list = shown.ToList();
			foreach (var row in list)
			{
				Players.Add(row);
			}

			int characterCount = list.SelectMany(p => p.Characters.Select(c => c.Name)).Distinct().Count();
			CountText = $"{list.Count} accounts, {characterCount} characters";
		}
	}
}
