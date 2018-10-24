using ScratchEVTCParser.Model;

namespace ScratchEVTCParser.Statistics.RotationItems
{
	public class WeaponSwapItem : RotationItem
	{
		public WeaponSet NewWeaponSet { get; }

		public WeaponSwapItem(long time, WeaponSet newWeaponSet) : base(time)
		{
			NewWeaponSet = newWeaponSet;
		}
	}
}