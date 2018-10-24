using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics.RotationItems;

namespace ScratchEVTCParser.Statistics.PlayerDataParts
{
	public class PlayerRotation
	{
		public Player Player { get; }
		public IEnumerable<RotationItem> Rotation { get; }

		public PlayerRotation(Player player, IEnumerable<RotationItem> rotationItems)
		{
			Player = player;
			Rotation = rotationItems as RotationItem[] ?? rotationItems.ToArray();
		}
	}
}