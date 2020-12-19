namespace GW2Scratch.EVTCAnalytics.Model
{
	/// <summary>
	/// A trait specialization that is not a <see cref="Model.Agents.EliteSpecialization"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Each player may have up to 3 <see cref="CoreSpecialization"/> accessible
	/// to their <see cref="Model.Agents.Profession"/> at a time, and one of them may
	/// be an <see cref="Model.Agents.EliteSpecialization"/> instead.
	/// </para>
	/// <para>
	/// Each <see cref="CoreSpecialization"/> is only available to one <see cref="Model.Agents.Profession"/>
	/// as of the Path of Fire expansion.
	/// </para>
	/// </remarks>
	public enum CoreSpecialization
	{
		// Warrior
		Arms,
		Defense,
		Discipline,
		Strength,
		Tactics,

		// Guardian
		Honor,
		Radiance,
		Valor,
		Virtues,
		Zeal,

		// Revenant
		Corruption,
		Devastation,
		Invocation,
		Retribution,
		Salvation,

		// Ranger
		Beastmastery,
		NatureMagic,
		Marksmanship,
		Skirmishing,
		WildernessSurvival,

		// Thief
		Acrobatics,
		CriticalStrikes,
		DeadlyArts,
		ShadowArts,
		Trickery,


		// Engineer
		Alchemy,
		Explosives,
		Firearms,
		Inventions,
		Tools,

		// Necromancer
		Curses,
		BloodMagic,
		DeathMagic,
		SoulReaping,
		Spite,

		// Elementalist
		Air,
		Arcane,
		Earth,
		Fire,
		Water,

		// Mesmer
		Chaos,
		Domination,
		Dueling,
		Illusions,
		Inspiration,
	}
}