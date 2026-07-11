using Avalonia.Media.Imaging;
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
		public string SubgroupText { get; }

		public PlayerRow(LogPlayer player, ImageProvider images)
		{
			Icon = images.GetTinyProfessionIcon(player);
			CharacterName = player.Name;
			AccountName = player.AccountName?.TrimStart(':') ?? "";
			Subgroup = player.Subgroup;
			SubgroupText = player.Subgroup > 0 ? player.Subgroup.ToString() : "-";
		}
	}
}
