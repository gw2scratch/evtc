namespace ScratchEVTCParser.Events
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