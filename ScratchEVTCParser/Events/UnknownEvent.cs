namespace ScratchEVTCParser.Events
{
	public class UnknownEvent : Event
	{
		public object EventData { get; }

		public UnknownEvent(long time, object eventData) : base(time)
		{
			EventData = eventData;
		}
	}
}