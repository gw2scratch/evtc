using System.Collections.Generic;
using System.Linq;
using GW2Scratch.RotationComparison.Rotations;

namespace GW2Scratch.RotationComparison.JsonModel
{
	public class JsonRotation
	{
		public JsonRotation(PlayerData playerData, IEnumerable<RotationItem> items)
		{
			PlayerData = playerData;
			Items = items.ToArray();
		}

		public PlayerData PlayerData { get; }
		public IEnumerable<RotationItem> Items { get; }
	}
}