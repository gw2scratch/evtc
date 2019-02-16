namespace RotationComparison.Rotations
{
	public abstract class RotationItem
	{
		public abstract RotationItemType Type { get; }
		public abstract long Time { get; }
		public abstract long Duration { get; }
		public long TimeEnd => Time + Duration;
	}
}