namespace ScratchEVTCParser.Model
{
	public enum WeaponType
	{
		None,
		Dagger,
		Focus,
		Staff,
		Scepter,
		Sword,
		Trident,
		Pistol,
		Rifle,
		Shield,
		Speargun,
		Greatsword,
		Mace,
		Torch,
		Hammer,
		Spear,
		Axe,
		Warhorn,
		Shortbow,
		Longbow,
		Other
	}

	public static class WeaponTypeExtensions
	{
		public static bool IsTwoHanded(this WeaponType type)
		{
			return type == WeaponType.Rifle || type == WeaponType.Spear || type == WeaponType.Staff ||
			       type == WeaponType.Hammer || type == WeaponType.Longbow || type == WeaponType.Trident ||
			       type == WeaponType.Shortbow || type == WeaponType.Speargun ||
			       type == WeaponType.Greatsword;
		}
	}
}