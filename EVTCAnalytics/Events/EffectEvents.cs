using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Effects;

namespace GW2Scratch.EVTCAnalytics.Events
{
	public class SplitEffectEvent(long time, Agent agent) : AgentEvent(time, agent)
	{

	}

	public class EffectGroundCreateEvent(
		long time,
		Agent agent,
		float[] position,
		float[] orientation,
		Effect effect,
		uint duration,
		byte flags,
		bool isOnNonStaticPlatform,
		float scale,
		float scaleSomething,
		uint trackableId)
		: SplitEffectEvent(time, agent)
	{
		public float[] Position { get; } = position;
		public float[] Orientation { get; } = orientation;
		public Effect Effect { get; } = effect;
		public uint Duration { get; } = duration;
		public byte Flags { get; } = flags;
		public bool IsOnNonStaticPlatform { get; } = isOnNonStaticPlatform;
		public float Scale { get; } = scale;
		public float ScaleSomething { get; } = scaleSomething;
		public uint TrackableId { get; } = trackableId;
	}

	public class EffectGroundRemoveEvent(
		long time,
		Agent agent,
		uint trackableId)
		: SplitEffectEvent(time, agent)
	{
		public uint TrackableId { get; } = trackableId;
	}

	public class EffectAgentCreateEvent(
		long time,
		Agent agent,
		Effect effect,
		uint duration,
		uint trackableId
		)
		: SplitEffectEvent(time, agent)
	{
		public Effect Effect { get; } = effect;
		public uint Duration { get; } = duration;
		public uint TrackableId { get; } = trackableId;
	}

	public class EffectAgentRemoveEvent(
		long time,
		Agent agent,
		uint trackableId
		) : SplitEffectEvent(time, agent)
	{
		public uint TrackableId { get; } = trackableId;
	}
}
