using Avalonia.Media.Imaging;
using GW2Scratch.ArcdpsLogManager.Sections.Guilds;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// A display-ready projection of a <see cref="GuildMember"/> (an account within a guild) for
	/// the "Accounts" tab of <c>GuildDetailPanelView</c> (Avalonia counterpart of the Eto
	/// <c>Controls/GuildDetailPanel.cs</c> account grid).
	/// </summary>
	public sealed class GuildMemberRow
	{
		public string AccountName { get; }
		public int CharacterCount { get; }
		public int LogCount { get; }

		public GuildMemberRow(GuildMember member)
		{
			// Account names are stored with a leading ':'; trim it for display, matching the Eto UI.
			AccountName = member.Name.TrimStart(':');
			CharacterCount = member.Characters?.Count ?? 0;
			LogCount = member.Logs.Count;
		}
	}

	/// <summary>
	/// A display-ready projection of a <see cref="GuildCharacter"/> for the "Characters" tab of
	/// <c>GuildDetailPanelView</c> (Avalonia counterpart of the Eto <c>Controls/GuildDetailPanel.cs</c>
	/// character grid).
	/// </summary>
	public sealed class GuildCharacterRow
	{
		public Bitmap? Icon { get; }
		public string Name { get; }
		public string AccountName { get; }
		public int LogCount { get; }

		public GuildCharacterRow(GuildCharacter character, ImageProvider images)
		{
			Icon = images.GetTinyProfessionIcon(character.Profession);
			Name = character.Name;
			AccountName = character.Account.Name.TrimStart(':');
			LogCount = character.Logs.Count;
		}
	}
}
