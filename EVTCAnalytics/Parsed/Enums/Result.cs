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

		Unknown
	}
}