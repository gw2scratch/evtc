using GW2Scratch.EVTCAnalytics.Model.Agents;
using static GW2Scratch.EVTCAnalytics.Events.RulesetEvent;

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
	/// <param name="speciesId"></param>
	public class StatResetEvent(long time, ulong speciesId) : Event(time)
	{
		/// <summary>
		/// Species ID of the Agent that triggered the reset.
		/// </summary>
		public ulong SpeciesId { get; } = speciesId;
	}

	/// <summary>
	/// Log boss agent changed event.
	/// </summary>
	public class LogNPCUpdateEvent(long time, ulong speciesId, Agent agent, int timestamp) : Event(time)
	{
		/// <summary>
		/// Specie ID of the Agent triggering the update.
		/// </summary>
		public ulong SpeciesId { get; } = speciesId;

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
		/// Old address instance ID.
		/// </summary>
		public ulong OldIID { get; } = oldIID;
		/// <summary>
		/// New address instance ID.
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

	/// <summary>
	/// Attack Target to Gadget association.
	/// </summary>
	public class AttackTargetEvent(long time, Agent attackTarget, Agent gadget) : Event(time)
	{
		/// <summary>
		/// The attack target.
		/// </summary>
		public Agent AttackTarget { get; }  = attackTarget;
		/// <summary>
		/// Associated gadget.
		/// </summary>
		public Agent Gadget { get; } = gadget;
	}

	/// <summary>
	/// Self-only ruleset event.
	/// </summary>
	/// <remarks>
	/// Present in guild hall logs.
	/// </remarks>
	public class RulesetEvent(long time, RulesetBit ruleset) : Event(time)
	{
		/// <summary>
		/// Ruleset bitfield.
		/// </summary>
		public enum RulesetBit : byte
		{
			PvE = 1,
			WvW = 2,
			PvP = 4,
		}

		/// <summary>
		/// The ruleset for self.
		/// </summary>
		public RulesetBit Ruleset { get; } = ruleset;
	}

	/// <summary>
	/// WvW team shard and color IDs.
	/// </summary>
	/// <remarks>
	/// Added in EVTCEVTC20260505. The Team ID will be 0 if there aren't players present from that team in the log.
	/// </remarks>
	public class WvWTeamsEvent(
		long time,
		uint redShardID,
		uint blueShardID,
		uint greenShardID,
		uint redTeamID,
		uint blueTeamID,
		uint greenTeamID
		) : Event(time)
	{
		public uint RedShardID { get; } = redShardID;
		public uint BlueShardID { get; } = blueShardID;
		public uint GreenShardID { get; } = greenShardID;
		public uint RedTeamID { get; } = redTeamID;
		public uint BlueTeamID { get; } = blueTeamID;
		public uint GreenTeamID { get; } = greenTeamID;
	}

	/// <summary>
	/// WvW objective status (ownership).
	/// </summary>
	/// <remarks>
	/// Added in EVTCEVTC20260507.
	/// </remarks>
	public class WvWObjectiveStatusEvent(
		long time,
		int mapId,
		int teamId,
		int objectiveId,
		byte objectiveType,
		uint upgradeProgress
		) : Event(time)
	{
		/// <summary>
		/// WvW map ID.
		/// </summary>
		public int MapId { get; } = mapId;
		/// <summary>
		/// Structure objective ID.
		/// </summary>
		public int ObjectiveId { get; } = objectiveId;
		/// <summary>
		/// Team ID of the objective.
		/// </summary>
		public int TeamId { get; } = teamId;
		/// <summary>
		/// Type of the objective.
		/// </summary>
		public byte ObjectiveType { get; } = objectiveType;
		/// <summary>
		/// Tier upgrade progress.
		/// </summary>
		public uint UpgradeProgress { get; } = upgradeProgress;
	}
}