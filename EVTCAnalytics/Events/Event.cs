namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// A log event corresponding to a simulation update within the game.
	/// </summary>
	public abstract class Event
	{
		public long Time { get; }

		public Event(long time)
		{
			Time = time;
		}
	}
}