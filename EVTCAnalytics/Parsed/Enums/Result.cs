namespace GW2Scratch.EVTCAnalytics.Parsed.Enums
{
	/// <summary>
	/// The result of a damage event as defined by the <c>cbtresult</c> arcdps enum.
	/// </summary>
	public enum Result : byte
	{
		Normal = 0,
		Critical = 1,
		Glance = 2,
		Block = 3,
		Evade = 4,
		Interrupt = 5,
		Absorb = 6,
		Blind = 7,
		KillingBlow = 8,
		Downed = 9,
		DefianceBar = 10,
		Activation = 11, // on-skill-use signal event
		CrowdControl = 12, // Introduced in arcdps 20240627.
		Invert = 13, // damage was inverted
		DamageCycle = 14, // buff damage happened on tick timer
		DamageNotCycle = 15, // buff damage happened outside tick timer
		DamageNotCycleDamageToTargetOnHit = 16, // buff damage happened to target on hitting target
		DamageNotCycleDamageToSourceOnHit = 17, // buff damage happened to source on hitting target
		DamageNotCycleDamageToTargetOnStackRemove = 18, // buff damage happened to target on buff removal

		Unknown
	}
}