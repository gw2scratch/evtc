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

	/// <summary>
	/// An event specifying the current simulation tick rate.
	/// </summary>
	/// <remarks>
	/// Introduced in arcdps version 20220520.
	/// </remarks>
	public class RateHealthEvent(long time, ulong tickRate) : Event(time)
	{
		/// <summary>
		/// Current simulation tick rate.
		///
		/// The expected tick rate is 25. This event is emitted by arcdps when tick rate is below 20.
		/// This can happen when the Guild Wars 2 server struggles to keep up, or in case of connection issues.
		/// </summary>
		public ulong TickRate { get; } = tickRate;
	}
}