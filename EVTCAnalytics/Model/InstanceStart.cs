namespace GW2Scratch.EVTCAnalytics.Model
{
	/// <summary>
	/// Represents a time when an instance was started.
	/// </summary>
	public class InstanceStart
	{
		/// <summary>
		/// Approximate time of the instance being started. This time is using the same timer as event times.
		/// </summary>
        public ulong TimeMilliseconds { get; }

		public InstanceStart(ulong timeMilliseconds)
		{
			TimeMilliseconds = timeMilliseconds;
		}
	}
}