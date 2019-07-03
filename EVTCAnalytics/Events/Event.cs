namespace GW2Scratch.EVTCAnalytics.Events
{
	public abstract class Event
	{
		public long Time { get; }

		public Event(long time)
		{
			Time = time;
		}
	}
}