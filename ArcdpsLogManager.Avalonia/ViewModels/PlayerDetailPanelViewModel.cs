using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the player-account detail panel (Avalonia counterpart of the Eto
	/// <c>Controls/PlayerDetailPanel.cs</c>). Shows the selected account's known characters
	/// (already available inline in <see cref="PlayersSectionViewModel"/>) plus the list of logs
	/// the account appears in, which the inline section view did not previously surface.
	/// </summary>
	public partial class PlayerDetailPanelViewModel : ObservableObject
	{
		private readonly ImageProvider images;
		private readonly ILogNameProvider nameProvider;

		[ObservableProperty] private bool hasSelection;
		[ObservableProperty] private string accountName = "";
		[ObservableProperty] private string logCountText = "";

		public IReadOnlyList<CharacterRow> Characters { get; private set; } = new List<CharacterRow>();
		public IReadOnlyList<LogRow> Logs { get; private set; } = new List<LogRow>();

		public PlayerDetailPanelViewModel(ImageProvider images, ILogNameProvider nameProvider)
		{
			this.images = images;
			this.nameProvider = nameProvider;
		}

		public void Show(PlayerAccountRow? row)
		{
			if (row == null)
			{
				HasSelection = false;
				return;
			}

			HasSelection = true;
			AccountName = row.AccountName;
			LogCountText = $"Appears in {row.LogCount} {(row.LogCount == 1 ? "log" : "logs")}";

			Characters = row.Characters;
			OnPropertyChanged(nameof(Characters));

			Logs = row.Data.Logs
				.OrderByDescending(l => l.EncounterStartTime)
				.Select(l => new LogRow(l, images, nameProvider))
				.ToList();
			OnPropertyChanged(nameof(Logs));
		}
	}
}
