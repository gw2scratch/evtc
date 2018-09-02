using System;

namespace ScratchEVTCParser.Events
{
	public class LogStartEvent : Event
	{
		public DateTimeOffset ServerTime { get; }
		public DateTimeOffset LocalTime { get; }

		public LogStartEvent(long time, DateTimeOffset serverTime, DateTimeOffset localTime) : base(time)
		{
			ServerTime = serverTime;
			LocalTime = localTime;
		}
	}

	public class LogEndEvent : Event
	{
		public DateTimeOffset ServerTime { get; }
		public DateTimeOffset LocalTime { get; }

		public LogEndEvent(long time, DateTimeOffset serverTime, DateTimeOffset localTime) : base(time)
		{
			ServerTime = serverTime;
			LocalTime = localTime;
		}
	}

	public class LanguageEvent : Event
	{
		public int LanguageId { get; }

		public LanguageEvent(long time, int languageId) : base(time)
		{
			LanguageId = languageId;
		}
	}

	public class PointOfViewEvent : Event
	{
		public ulong RecordingAgent { get; }

		public PointOfViewEvent(long time, ulong recordingAgent) : base(time)
		{
			RecordingAgent = recordingAgent;
		}
	}

	public class GameBuildEvent : Event
	{
		public int Build { get; }

		public GameBuildEvent(long time, int build) : base(time)
		{
			Build = build;
		}
	}

	public class GameShardEvent : Event
	{
		public ulong ShardId { get; }

		public GameShardEvent(long time, ulong shardId) : base(time)
		{
			ShardId = shardId;
		}
	}

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