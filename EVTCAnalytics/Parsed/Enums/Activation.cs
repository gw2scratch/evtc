namespace GW2Scratch.EVTCAnalytics.Parsed.Enums
{
	/// <summary>
	/// A skill activation type as defined by the <c>cbtactivation</c> arcdps enum.
	/// </summary>
	public enum Activation : byte
	{
		None = 0,
		Normal = 1,
		Quickness = 2,
		CancelFire = 3,
		CancelCancel = 4,
		Reset = 5,
	}
}