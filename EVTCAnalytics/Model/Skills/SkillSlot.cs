namespace GW2Scratch.EVTCAnalytics.Model.Skills
{
	/// <summary>
	/// A slot for skills used by players in the game.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Some slots may only be accessible for certain <see cref="Model.Agents.Profession"/>s.
	/// </para>
	/// <para>
	/// A player may have access to multiple slots of the same kind (notably the <see cref="Utility"/> slot).
	/// </para>
	/// </remarks>
	public enum SkillSlot
	{
		Weapon1,
		Weapon2,
		Weapon3,
		Weapon4,
		Weapon5,
		Profession1,
		Profession2,
		Profession3,
		Profession4,
		Profession5,
		Downed1,
		Downed2,
		Downed3,
		Downed4,
		Utility,
		Heal,
		Elite,
		Toolbelt,
		Pet,
		Other,
		None
	}
}