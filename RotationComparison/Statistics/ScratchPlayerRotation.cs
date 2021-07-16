using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.RotationComparison.Statistics.RotationItems;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.RotationComparison.Statistics
{
	public class ScratchPlayerRotation
	{
		public Player Player { get; }
		public IEnumerable<RotationItem> Items { get; }

		public ScratchPlayerRotation(Player player, IEnumerable<RotationItem> rotationItems)
		{
			Player = player;
			Items = rotationItems as RotationItem[] ?? rotationItems.ToArray();
		}
	}
}