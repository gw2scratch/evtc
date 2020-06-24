namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// An event specifying a reward chest was awarded to the recording player.
	/// </summary>
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