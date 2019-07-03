using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Statistics.RotationItems
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