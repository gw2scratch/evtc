using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics.RotationItems;

namespace ScratchEVTCParser.Statistics.PlayerDataParts
{
	public class PlayerRotation
	{
		public Player Player { get; }
		public IEnumerable<RotationItem> Items { get; }

		public PlayerRotation(Player player, IEnumerable<RotationItem> rotationItems)
		{
			Player = player;
			Items = rotationItems as RotationItem[] ?? rotationItems.ToArray();
		}
	}
}