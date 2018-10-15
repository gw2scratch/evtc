namespace ScratchEVTCParser.Statistics.Buffs
{
	public class BuffStatusSegment
	{
		public long TimeStart { get; }
		public long TimeEnd { get; }
		public int StackCount { get; }

		public BuffStatusSegment(long timeStart, long timeEnd, int stackCount)
		{
			TimeStart = timeStart;
			TimeEnd = timeEnd;
			StackCount = stackCount;
		}
	}
}