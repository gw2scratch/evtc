using GW2Scratch.EVTCAnalytics.Model.Agents;

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

	/// <summary>
	/// Arcdps stats reset event.
	/// </summary>
	/// <param name="time"></param>
	/// <param name="specieId"></param>
	public class StatResetEvent(long time, ulong specieId) : Event(time)
	{
		/// <summary>
		/// Specie ID of the Agent that triggered the reset.
		/// </summary>
		public ulong SpecieId { get; } = specieId;
	}

	/// <summary>
	/// Log boss agent changed event.
	/// </summary>
	public class LogNPCUpdateEvent(long time, ulong specieId, Agent agent, int timestamp) : Event(time)
	{
		/// <summary>
		/// Specie ID of the Agent triggering the update.
		/// </summary>
		public ulong SpecieId { get; } = specieId;

		/// <summary>
		/// The Agent that triggered the update.
		/// </summary>
		public Agent Agent { get; } = agent;

		/// <summary>
		/// Server unix timestamp of the update.
		/// </summary>
		public int Timestamp { get; } = timestamp;
	}

	/// <summary>
	/// Agent address IID changed.
	/// </summary>
	public class IIDChangeEvent(long time, ulong oldIID, ulong newIID) : Event(time)
	{
		/// <summary>
		/// Old address IID.
		/// </summary>
		public ulong OldIID { get; } = oldIID;
		/// <summary>
		/// New address IID.
		/// </summary>
		public ulong NewIID { get; } = newIID;
	}

	/// <summary>
	/// Player changed map event.
	/// </summary>
	public class MapChangeEvent(long time, ulong newMapID, ulong oldMapID) : Event(time)
	{
		/// <summary>
		/// New Map ID.
		/// </summary>
		public ulong NewMapID { get; } = newMapID;
		/// <summary>
		/// Old Map ID.
		/// </summary>
		public ulong OldMapID { get; } = oldMapID;
	}
}