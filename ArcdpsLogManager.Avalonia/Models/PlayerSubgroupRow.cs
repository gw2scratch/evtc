using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// A subgroup of players in the log detail panel's group composition list (Avalonia counterpart
	/// of the Eto <c>GroupCompositionControl</c>'s per-subgroup <c>GroupBox</c>).
	/// </summary>
	public sealed class PlayerSubgroupRow
	{
		public string Header { get; }
		public IReadOnlyList<PlayerRow> Players { get; }

		public PlayerSubgroupRow(int subgroup, IReadOnlyList<PlayerRow> players)
		{
			Header = subgroup > 0 ? $"Subgroup {subgroup}" : "No subgroup";
			Players = players;
		}
	}
}
