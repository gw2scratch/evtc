namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// An <see cref="Event"/> that is not recognized or implemented yet.
	/// </summary>
	public class UnknownEvent(long time, object eventData) : Event(time)
	{
		/// <summary>
		/// Data associated with the unknown event.
		/// </summary>
		/// <remarks>
		/// If showing this data, keep in mind the underlying class may change in the future.
		/// </remarks>
		public object EventData { get; } = eventData;
	}
}