namespace GW2Scratch.RotationComparison.Statistics.RotationItems
{
	public class TemporaryStatusItem : RotationItem
	{
		public long StatusEndTime { get; }
		public TemporaryStatus TemporaryStatus { get; }

		public TemporaryStatusItem(long statusStartTime, long statusEndTime, TemporaryStatus temporaryStatus) : base(statusStartTime)
		{
			StatusEndTime = statusEndTime;
			TemporaryStatus = temporaryStatus;
		}
	}
}