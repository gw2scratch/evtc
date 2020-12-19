namespace GW2Scratch.EVTCAnalytics.Model
{
	/// <summary>
	/// Represents a weapon type.
	/// </summary>
	/// <remarks>
	/// Not all <see cref="Model.Agents.Profession"/>s have access to all weapon types.
	/// </remarks>
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
		/// <summary>
		/// Checks whether a weapon type is a weapon that occupies both weapon slots.
		/// </summary>
		/// <param name="type">The type of a weapon.</param>
		/// <returns>A value indicating whether a weapon is two-handed.</returns>
		public static bool IsTwoHanded(this WeaponType type)
		{
			return type == WeaponType.Rifle || type == WeaponType.Spear || type == WeaponType.Staff ||
			       type == WeaponType.Hammer || type == WeaponType.Longbow || type == WeaponType.Trident ||
			       type == WeaponType.Shortbow || type == WeaponType.Speargun ||
			       type == WeaponType.Greatsword;
		}
	}
}