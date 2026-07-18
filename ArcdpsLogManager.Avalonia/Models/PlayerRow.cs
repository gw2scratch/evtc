using Avalonia.Media.Imaging;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// A display-ready projection of a <see cref="LogPlayer"/> for the log detail panel's
	/// group composition list.
	/// </summary>
	public sealed class PlayerRow
	{
		public Bitmap? Icon { get; }
		public string CharacterName { get; }
		public string AccountName { get; }
		public int Subgroup { get; }
		public Bitmap? TagIcon { get; }
		public bool IsCommander { get; }
		public string GuildTooltip { get; }

		/// <summary>Matches the Eto <c>GroupCompositionControl</c>'s guild tag suffix behavior
		/// exactly, including that an uncached guild renders as "[???]" rather than being omitted
		/// until the API data resolves.</summary>
		public PlayerRow(LogPlayer player, ImageProvider images, ApiData apiData, bool showGuildTags)
		{
			Icon = images.GetTinyProfessionIcon(player);

			string guildTagSuffix = "";
			if (player.GuildGuid != null && showGuildTags)
			{
				string guildTag = apiData.GetGuildTag(player.GuildGuid);
				if (guildTag != "")
				{
					guildTagSuffix = $" [{guildTag}]";
				}
			}

			CharacterName = $"{player.Name}{guildTagSuffix}";
			GuildTooltip = player.GuildGuid != null ? apiData.GetGuildName(player.GuildGuid) : "No guild data";
			AccountName = player.AccountName?.TrimStart(':') ?? "";
			Subgroup = player.Subgroup;
			IsCommander = player.Tag == PlayerTag.Commander;
			TagIcon = IsCommander ? images.GetTinyCommanderIcon() : null;
		}
	}
}
