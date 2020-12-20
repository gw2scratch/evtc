namespace GW2Scratch.EVTCAnalytics.Parsed.Enums
{
	/// <summary>
	/// A buff removal type as defined by the <c>cbtbuffremove</c> arcdps enum.
	/// </summary>
	public enum BuffRemove : byte
	{
		None   = 0,
		All    = 1,
		Single = 2,
		Manual = 3,
	};
}