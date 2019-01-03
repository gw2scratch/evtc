namespace ScratchEVTCParser.Events
{
	public class RewardEvent : Event
	{
		public ulong RewardId { get; }
		public int RewardType { get; }

		public RewardEvent(long time, ulong rewardId, int rewardType) : base(time)
		{
			RewardId = rewardId;
			RewardType = rewardType;
		}
	}
}