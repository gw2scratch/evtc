using System;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Effects;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// An event relevant to a specific <see cref="Agent"/>.
	/// </summary>
	public abstract class AgentEvent(long time, Agent agent) : Event(time)
	{
		public Agent Agent { get; internal set; } = agent;
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> entered combat.
	/// </summary>
	public class AgentEnterCombatEvent(long time, Agent agent, int subgroup, Profession profession, EliteSpecialization eliteSpecialization)
		: AgentEvent(time, agent)
	{
		public int Subgroup { get; } = subgroup;

		/// <summary>
		/// The Profession of the agent; may be null.
		/// </summary>
		/// <remarks>
		/// Introduced in EVTC20240612.
		/// </remarks>
		public Profession? Profession { get; } = profession;

		/// <summary>
		/// The Elite Specialization of the agent; may be null.
		/// </summary>
		/// <remarks>
		/// Introduced in EVTC20240612.
		/// </remarks>
		public EliteSpecialization? EliteSpecialization { get; } = eliteSpecialization;
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> exited combat.
	/// </summary>
	public class AgentExitCombatEvent(long time, Agent agent) : AgentEvent(time, agent);

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> was revived (either from downed or dead state).
	/// </summary>
	public class AgentRevivedEvent(long time, Agent agent) : AgentEvent(time, agent);

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> is now downed.
	/// </summary>
	public class AgentDownedEvent(long time, Agent agent) : AgentEvent(time, agent);

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> is now dead.
	/// </summary>
	public class AgentDeadEvent(long time, Agent agent) : AgentEvent(time, agent);

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> is now tracked.
	/// </summary>
	public class AgentSpawnEvent(long time, Agent agent) : AgentEvent(time, agent);

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> is no longer tracked.
	/// </summary>
	public class AgentDespawnEvent(long time, Agent agent) : AgentEvent(time, agent);

	/// <summary>
	/// An event specifying that the health percentage of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if health changes more often.
	/// </remarks>
	public class AgentHealthUpdateEvent(long time, Agent agent, float healthFraction) : AgentEvent(time, agent)
	{
		/// <summary>
		/// The current health fraction, where 1 is 100% and 0 is 0% of maximum health of the agent.
		/// </summary>
		public float HealthFraction { get; } = healthFraction;
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> changed their active weapon set.
	/// </summary>
	/// <remarks>
	/// A weapon set does not necessarily correspond to what players use the weapon swap action for.
	/// Swapping to a bundle that provides a set of skills will also trigger this event.
	/// </remarks>
	public class AgentWeaponSwapEvent(long time, Agent agent, WeaponSet newWeaponSet, WeaponSet? oldWeaponSet) : AgentEvent(time, agent)
	{
		public WeaponSet NewWeaponSet { get; } = newWeaponSet;
		/// <summary>
		/// Introduced in EVTC20240627. May be null before.
		/// </summary>
		public WeaponSet? OldWeaponSet { get; } = oldWeaponSet;
	}

	/// <summary>
	/// An event specifying that the maximum health of an <see cref="Agent"/> has been changed.
	/// </summary>
	public class AgentMaxHealthUpdateEvent(long time, Agent agent, ulong newMaxHealth) : AgentEvent(time, agent)
	{
		public ulong NewMaxHealth { get; } = newMaxHealth;
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> has a tag/marker. Typically, a <see cref="Player"/> with a Commander tag.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20200609. Prior to EVTC20240328, this only happened at the start of a log, since then it can happen at any time.
	/// </remarks>
	public class AgentMarkerEvent(long time, Agent agent, Marker marker, bool? isCommander) : AgentEvent(time, agent)
	{
		public Marker Marker { get; } = marker;

		/// <summary>
		/// True if this is a commander tag.
		/// </summary>
		/// <remarks>
		/// Introduced in EVTC20220823.
		/// </remarks>
		public bool? IsCommander { get; } = isCommander;
	}

	/// <summary>
	/// An event specifying that all tags/markers of an <see cref="Agent"/> have been removed.
	/// </summary>
	/// <remarks>
	/// Commonyl used since EVTC20240328.
	/// </remarks>
	public class AgentMarkerRemoveAllEvent(long time, Agent agent) : AgentEvent(time, agent);

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> has an ongoing buff at the time tracking starts.
	/// </summary>
	public class InitialBuffEvent(
		long time,
		Agent agent,
		Skill buff,
		Agent sourceAgent,
		int durationApplied,
		uint durationOfRemovedStack,
		uint instanceId,
		bool isActive)
		: BuffApplyEvent(time, agent, buff, sourceAgent, durationApplied, durationOfRemovedStack, instanceId, isActive);

	/// <summary>
	/// An event specifying that the position (3D coordinates) of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if position changes more often.
	/// </remarks>
	public class PositionChangeEvent(long time, Agent agent, float x, float y, float z)
		: AgentEvent(time, agent)
	{
		public float X { get; } = x;
		public float Y { get; } = y;
		public float Z { get; } = z;
	}

	/// <summary>
	/// An event specifying that the velocity (3D vector) of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if velocity changes more often.
	/// </remarks>
	public class VelocityChangeEvent(long time, Agent agent, float x, float y, float z)
		: AgentEvent(time, agent)
	{
		public float X { get; } = x;
		public float Y { get; } = y;
		public float Z { get; } = z;
	}

	/// <summary>
	/// An event specifying that the facing (2D vector) of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if facing changes more often.
	/// </remarks>
	public class FacingChangeEvent(long time, Agent agent, float x, float y)
		: AgentEvent(time, agent)
	{
		public float X { get; } = x;
		public float Y { get; } = y;
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> changed their current team.
	/// <br />
	/// Teams affect hostility of targets.
	/// </summary>
	public class TeamChangeEvent(long time, Agent agent, ulong newTeamId, int? oldTeamId) : AgentEvent(time, agent)
	{
		public ulong NewTeamId { get; } = newTeamId;
		/// <remarks>
		/// Introduced in EVTC20240612. May be null before.
		/// </remarks>
		public int? OldTeamId { get; } = oldTeamId;
	}

	/// <summary>
	/// An event specifying that an <see cref="AttackTarget"/> is now (un)targetable.
	/// </summary>
	public class TargetableChangeEvent(long time, AttackTarget agent, bool targetable) : AgentEvent(time, agent)
	{
		public AttackTarget AttackTarget => (AttackTarget) Agent;

		public bool IsTargetable { get; } = targetable;
	}

	/// <summary>
	/// An event specifying that the health percentage of the defiance bar of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if health changes more often.
	/// <br/>
	/// Introduced in EVTC20200506.
	/// </remarks>
	public class DefianceBarHealthUpdateEvent(long time, Agent agent, float healthFraction) : AgentEvent(time, agent)
	{
		public float HealthFraction { get; } = healthFraction;
	}
	
	/// <summary>
	/// An event specifying that the barrier percentage of a health bar belonging an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if barrier state changes more often.
	/// <br/>
	/// Introduced in EVTC20201201.
	/// </remarks>
	public class BarrierUpdateEvent(long time, Agent agent, float barrierFraction) : AgentEvent(time, agent)
	{
		public float BarrierFraction { get; } = barrierFraction;
	}

	/// <summary>
	/// An event specifying that the state of the defiance bar of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20200506.
	/// </remarks>
	public class DefianceBarStateUpdateEvent(long time, Agent agent, DefianceBarStateUpdateEvent.DefianceBarState state) : AgentEvent(time, agent)
	{
		public enum DefianceBarState
		{
			Active,
			Recovering,
			Immune,
			None,
			Unknown
		}

		public DefianceBarState State { get; } = state;
	}

	/// <summary>
	/// An event specifying that an effect was created.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20220602. Retired in EVTC20230718.
	/// 
	/// Meaning of some values silently changed sometime in-between without any mention in the changelog.
	/// Notably, ZAxisOrientationOnly likely changed meaning.
	/// </remarks>
	public class EffectEvent(
		long time,
		Agent agent,
		Effect effect,
		Agent agentTarget,
		float[] position,
		float[] orientation,
		bool zAxisOrientationOnly,
		ushort duration)
		: AgentEvent(time, agent)
	{
		/// <summary>
		/// The owner of this effect.
		/// </summary>
		public Agent EffectOwner => Agent;

		/// <summary>
		/// The Effect created.
		/// </summary>
		public Effect Effect { get; } = effect;

		/// <summary>
		/// The <see cref="Agent"/> this effect is anchored to if anchored. May be <see langword="null" />.
		/// </summary>
		/// <seealso cref="Position"/>
		public Agent AgentTarget { get; } = agentTarget;

		/// <summary>
		/// Position (x, y, z) of the effect when not anchored to a target. May be <see langword="null" />.
		/// </summary>
		/// <seealso cref="AgentTarget"/>
		public float[] Position { get; } = position;

		/// <summary>
		/// The orientation (x, y, z) of the effect. If <see cref="ZAxisOrientationOnly"/> is <see langword="true"/>,
		/// first two values are not valid.
		/// </summary>
		public float[] Orientation { get; } = orientation;

		/// <summary>
		/// Indicates whether the first two values of <see cref="Orientation"/> are valid.
		/// </summary>
		public bool ZAxisOrientationOnly { get; } = zAxisOrientationOnly;

		/// <summary>
		/// The duration of the effect in milliseconds.
		/// </summary>
		public ushort Duration { get; } = duration;
	}
	
	/// <summary>
	/// An event specifying that an effect was created.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20230718.
	/// </remarks>
	public class EffectStartEvent(
		long time,
		Agent effectOwner,
		Effect effect,
		Agent agentTarget,
		float[] position,
		short[] orientation,
		uint duration,
		uint trackableId)
		: AgentEvent(time, effectOwner)
	{
		/// <summary>
		/// The owner of this effect.
		/// </summary>
		public Agent EffectOwner => Agent;

		/// <summary>
		/// The Effect created.
		/// </summary>
		public Effect Effect { get; } = effect;

		/// <summary>
		/// The <see cref="Agent"/> this effect is anchored to if anchored. May be <see langword="null" />.
		/// </summary>
		/// <seealso cref="Position"/>
		public Agent AgentTarget { get; internal set; } = agentTarget;

		/// <summary>
		/// Position (x, y, z) of the effect when not anchored to a target. May be <see langword="null" />.
		/// </summary>
		/// <seealso cref="AgentTarget"/>
		public float[] Position { get; } = position;

		/// <summary>
		/// The orientation (x, y, z) of the effect.
		/// </summary>
		public short[] Orientation { get; } = orientation;

		/// <summary>
		/// The duration of the effect in milliseconds.
		/// </summary>
		public uint Duration { get; } = duration;

		/// <summary>
		/// Trackable id of this effect. Used for pairing with a corresponding EffectEndEvent.
		/// </summary>
		public uint TrackableId { get; } = trackableId;
	}
	
	/// <summary>
	/// An event specifying that an effect ended.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20230718.
	/// </remarks>
	public class EffectEndEvent(
		long time,
		Agent effectOwner,
		Effect effect,
		Agent agentTarget,
		float[] position,
		short[] orientation,
		uint? duration,
		uint trackableId)
		: AgentEvent(time, effectOwner)
	{
		/// <summary>
		/// The owner of this effect. May be <see langword="null" /> if the corresponding start event could not be paired.
		/// </summary>
		public Agent EffectOwner => Agent;

		/// <summary>
		/// The Effect created. May be <see langword="null" /> if the corresponding start event could not be paired.
		/// </summary>
		public Effect Effect { get; } = effect;

		/// <summary>
		/// The <see cref="Agent"/> this effect is anchored to if anchored. May be <see langword="null" />.
		/// May be <see langword="null" /> if the corresponding start event could not be paired.
		/// </summary>
		/// <seealso cref="Position"/>
		public Agent AgentTarget { get; internal set; } = agentTarget;

		/// <summary>
		/// Position (x, y, z) of the effect when not anchored to a target. May be <see langword="null" />.
		/// May be <see langword="null" /> if the corresponding start event could not be paired.
		/// </summary>
		/// <seealso cref="AgentTarget"/>
		public float[] Position { get; } = position;

		/// <summary>
		/// The orientation (x, y, z) of the effect.
		/// May be <see langword="null" /> if the corresponding start event could not be paired.
		/// </summary>
		public short[] Orientation { get; } = orientation;

		/// <summary>
		/// The duration of the effect in milliseconds.
		/// May be <see langword="null" /> if the corresponding start event could not be paired.
		/// </summary>
		public uint? Duration { get; } = duration;
		/// <summary>
		/// Trackable id of this effect. Used for pairing with a corresponding EffectStartEvent.
		/// </summary>
		public uint TrackableId { get; } = trackableId;
	}

	/// <summary>
	/// Indicates that an agent opened their glider.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20240627.
	/// </remarks>
	public class AgentGliderOpenEvent(long time, Agent agent) : AgentEvent(time, agent);
	/// <summary>
	/// Indicates that an agent closed their glider.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20240627.
	/// </remarks>
	public class AgentGliderCloseEvent(long time, Agent agent) : AgentEvent(time, agent);
}