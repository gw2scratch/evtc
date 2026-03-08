using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Effects;

namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// Split effect events between ground and agent effects.
	/// </summary>
	public class SplitEffectEvent(long time, Agent agent) : AgentEvent(time, agent)
	{

	}

	/// <summary>
	/// Ground effect events created.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20250526.
	/// </remarks>
	public class EffectGroundCreateEvent(
		long time,
		Agent agent,
		float[] position,
		float[] orientation,
		Effect effect,
		uint duration,
		byte flags,
		bool onNonStaticPlatform,
		float scale,
		float scaleSomething,
		uint trackableId)
		: SplitEffectEvent(time, agent)
	{
		/// <summary>
		/// The position on the ground of the effect.
		/// </summary>
		public float[] Position { get; } = position;

		/// <summary>
		/// The orientation point of the effect.
		/// </summary>
		public float[] Orientation { get; } = orientation;

		/// <summary>
		/// The Effect created.
		/// </summary>
		public Effect Effect { get; } = effect;

		/// <summary>
		/// The duration of the effect in milliseconds.
		/// </summary>
		public uint Duration { get; } = duration;

		/// <summary>
		/// Flags of the effect.
		/// </summary>
		public byte Flags { get; } = flags;

		/// <summary>
		/// If the effect is on a non-static platform. <see langword="true"/> if it's on a non-static platform, <see langword="false"/> otherwise.
		/// </summary>
		public bool OnNonStaticPlatform { get; } = onNonStaticPlatform;

		/// <summary>
		/// Scaling of the effect.
		/// </summary>
		public float Scale { get; } = scale;

		/// <summary>
		/// Another scaling value of the effect but we don't know what it does.
		/// </summary>
		public float ScaleSomething { get; } = scaleSomething;

		/// <summary>
		/// Trackable id of this effect. Used for pairing with a corresponding <see cref="EffectGroundRemoveEvent"/>.
		/// </summary>
		public uint TrackableId { get; } = trackableId;
	}

	/// <summary>
	/// Ground effect events removed.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20250526.
	/// </remarks>
	public class EffectGroundRemoveEvent(
		long time,
		Agent agent, // This is always null.
		uint trackableId)
		: SplitEffectEvent(time, agent)
	{
		/// <summary>
		/// Trackable id of this effect. Used for pairing with a corresponding <see cref="EffectGroundCreateEvent"/>.
		/// </summary>
		public uint TrackableId { get; } = trackableId;
	}

	/// <summary>
	/// Around an agent effect events created.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20250526.
	/// </remarks>
	public class EffectAgentCreateEvent(
		long time,
		Agent agent,
		Effect effect,
		uint duration,
		uint trackableId
		)
		: SplitEffectEvent(time, agent)
	{
		/// <summary>
		/// The Effect created.
		/// </summary>
		public Effect Effect { get; } = effect;

		/// <summary>
		/// The duration of the effect in milliseconds.
		/// </summary>
		public uint Duration { get; } = duration;

		/// <summary>
		/// Trackable id of this effect. Used for pairing with a corresponding <see cref="EffectAgentRemoveEvent"/>.
		/// </summary>
		public uint TrackableId { get; } = trackableId;
	}

	/// <summary>
	/// Around an agent effect events removed.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20250526.
	/// </remarks>
	public class EffectAgentRemoveEvent(
		long time,
		Agent agent,
		uint trackableId
		) : SplitEffectEvent(time, agent)
	{
		/// <summary>
		/// Trackable id of this effect. Used for pairing with a corresponding <see cref="EffectAgentCreateEvent"/>.
		/// </summary>
		public uint TrackableId { get; } = trackableId;
	}
}
