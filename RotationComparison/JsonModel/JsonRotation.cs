using System.Collections.Generic;
using System.Linq;
using RotationComparison.Rotations;

namespace RotationComparison.JsonModel
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