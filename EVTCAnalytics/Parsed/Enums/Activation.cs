namespace GW2Scratch.EVTCAnalytics.Parsed.Enums
{
	/// <summary>
	/// A skill activation type as defined by the <c>cbtactivation</c> arcdps enum.
	/// </summary>
	public enum Activation : byte
	{
		None = 0, // not used - not this kind of event
		Normal = 1, // started skill/animation activation
		Quickness = 2, // unused as of nov 5 2019
		Minimim = 3, // stopped animation with reaching minimum of first trigger point or tooltip time
		Cancel = 4, // stopped animation without reaching minimum of first trigger point or tooltip time
		Reset = 5, // animation completed fully
		NoData = 6, // same as Minimum but on 0/uncertain expected duration

		Unknown
	}
}