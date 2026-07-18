using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the guild detail panel (Avalonia counterpart of the Eto
	/// <c>Controls/GuildDetailPanel.cs</c>). Shows the guild name/tag, member counts, the
	/// account/character grids the Eto panel tabbed between, and the list of logs the guild
	/// appears in (the Eto panel opened this in a separate window via "Show logs with this
	/// guild"; here it is shown inline as its own tab, consistent with how this port already
	/// favors inline panels over extra windows).
	/// </summary>
	public partial class GuildDetailPanelViewModel : ObservableObject
	{
		private readonly ImageProvider images;
		private readonly ILogNameProvider nameProvider;

		[ObservableProperty] private bool hasSelection;
		[ObservableProperty] private string guildName = "";
		[ObservableProperty] private string guildTag = "";
		[ObservableProperty] private string memberCountText = "";

		public IReadOnlyList<GuildMemberRow> Members { get; private set; } = new List<GuildMemberRow>();
		public IReadOnlyList<GuildCharacterRow> Characters { get; private set; } = new List<GuildCharacterRow>();
		public IReadOnlyList<LogRow> Logs { get; private set; } = new List<LogRow>();

		public GuildDetailPanelViewModel(ImageProvider images, ILogNameProvider nameProvider)
		{
			this.images = images;
			this.nameProvider = nameProvider;
		}

		public void Show(GuildRow? row)
		{
			if (row == null)
			{
				HasSelection = false;
				return;
			}

			HasSelection = true;
			GuildName = row.Name;
			GuildTag = row.Tag;
			MemberCountText = $"{row.PlayerCount} {(row.PlayerCount == 1 ? "member" : "members")}, " +
			                   $"{row.CharacterCount} {(row.CharacterCount == 1 ? "character" : "characters")}";

			Members = row.Data.Accounts.Select(a => new GuildMemberRow(a)).ToList();
			OnPropertyChanged(nameof(Members));

			Characters = row.Data.Characters.Select(c => new GuildCharacterRow(c, images)).ToList();
			OnPropertyChanged(nameof(Characters));

			Logs = row.Data.Logs
				.OrderByDescending(l => l.EncounterStartTime)
				.Select(l => new LogRow(l, images, nameProvider))
				.ToList();
			OnPropertyChanged(nameof(Logs));
		}
	}
}
