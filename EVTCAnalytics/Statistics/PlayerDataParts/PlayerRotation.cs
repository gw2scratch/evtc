using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Statistics.RotationItems;

namespace GW2Scratch.EVTCAnalytics.Statistics.PlayerDataParts
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