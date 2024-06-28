namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// A log event corresponding to a simulation update within the game.
	/// </summary>
	public abstract class Event(long time)
	{
		public long Time { get; } = time;
	}
}