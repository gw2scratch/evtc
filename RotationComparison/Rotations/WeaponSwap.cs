using ScratchEVTCParser.Model;

namespace RotationComparison.Rotations
{
	public class WeaponSwap : RotationItem
	{
		public override RotationItemType Type { get; } = RotationItemType.WeaponSwap;
		public override long Time { get; }
		public override long Duration { get; } = 0;
		public WeaponSet NewWeaponSet { get; }

		public WeaponSwap(long time, WeaponSet newWeaponSet)
		{
			Time = time;
			NewWeaponSet = newWeaponSet;
		}
	}
}