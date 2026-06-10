namespace GW2Scratch.EVTCAnalytics.Parsed.Enums
{
	/// <summary>
	/// A friend or foe distinction as defined by the <c>iff</c> arcdps enum.
	/// </summary>
	public enum Moving : byte
	{
		No = 0,
		SourceIsMoving = 1,
		TargetIsMoving = 2,
		SourceAndTargetMoving = 3,

		Unknown,
	}
}