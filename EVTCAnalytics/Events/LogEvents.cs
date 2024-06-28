namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// An event specifying a reward chest was awarded to the recording player.
	/// </summary>
	public class RewardEvent(long time, ulong rewardId, int rewardType) : Event(time)
	{
		public ulong RewardId { get; } = rewardId;
		public int RewardType { get; } = rewardType;
	}
}