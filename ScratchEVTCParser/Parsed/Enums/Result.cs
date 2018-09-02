namespace ScratchEVTCParser.Parsed.Enums
{
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