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
	public abstract class AgentEvent : Event
	{
		public Agent Agent { get; internal set; }

		protected AgentEvent(long time, Agent agent) : base(time)
		{
			Agent = agent;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> entered combat.
	/// </summary>
	public class AgentEnterCombatEvent : AgentEvent
	{
		public int Subgroup { get; }

		public AgentEnterCombatEvent(long time, Agent agent, int subgroup) : base(time, agent)
		{
			Subgroup = subgroup;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> exited combat.
	/// </summary>
	public class AgentExitCombatEvent : AgentEvent
	{
		public AgentExitCombatEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> was revived (either from downed or dead state).
	/// </summary>
	public class AgentRevivedEvent : AgentEvent
	{
		public AgentRevivedEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> is now downed.
	/// </summary>
	public class AgentDownedEvent : AgentEvent
	{
		public AgentDownedEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> is now dead.
	/// </summary>
	public class AgentDeadEvent : AgentEvent
	{
		public AgentDeadEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> is now tracked.
	/// </summary>
	public class AgentSpawnEvent : AgentEvent
	{
		public AgentSpawnEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> is no longer tracked.
	/// </summary>
	public class AgentDespawnEvent : AgentEvent
	{
		public AgentDespawnEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	/// <summary>
	/// An event specifying that the health percentage of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if health changes more often.
	/// </remarks>
	public class AgentHealthUpdateEvent : AgentEvent
	{
		/// <summary>
		/// The current health fraction, where 1 is 100% and 0 is 0% of maximum health of the agent.
		/// </summary>
		public float HealthFraction { get; }

		public AgentHealthUpdateEvent(long time, Agent agent, float healthFraction) : base(time, agent)
		{
			HealthFraction = healthFraction;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> changed their active weapon set.
	/// </summary>
	/// <remarks>
	/// A weapon set does not necessarily correspond to what players use the weapon swap action for.
	/// Swapping to a bundle that provides a set of skills will also trigger this event.
	/// </remarks>
	public class AgentWeaponSwapEvent : AgentEvent
	{
		public WeaponSet NewWeaponSet { get; }

		public AgentWeaponSwapEvent(long time, Agent agent, WeaponSet newWeaponSet) : base(time, agent)
		{
			NewWeaponSet = newWeaponSet;
		}
	}

	/// <summary>
	/// An event specifying that the maximum health of an <see cref="Agent"/> has been changed.
	/// </summary>
	public class AgentMaxHealthUpdateEvent : AgentEvent
	{
		public ulong NewMaxHealth { get; }

		public AgentMaxHealthUpdateEvent(long time, Agent agent, ulong newMaxHealth) : base(time, agent)
		{
			NewMaxHealth = newMaxHealth;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> has a tag. Typically a <see cref="Player"/> with a Commander tag.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20200609.
	/// </remarks>
	public class AgentTagEvent : AgentEvent
	{
		/// <summary>
		/// The ID of the tag may be volatile and change with each game build.
		/// </summary>
		public int Id { get; }

		public AgentTagEvent(long time, Agent agent, int id) : base(time, agent)
		{
			Id = id;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> has an ongoing buff at the time tracking starts.
	/// </summary>
	public class InitialBuffEvent : AgentEvent
	{
		public Skill Skill { get; }

		public InitialBuffEvent(long time, Agent agent, Skill skill) : base(time, agent)
		{
			Skill = skill;
		}
	}

	/// <summary>
	/// An event specifying that the position (3D coordinates) of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if position changes more often.
	/// </remarks>
	public class PositionChangeEvent : AgentEvent
	{
		public float X { get; }
		public float Y { get; }
		public float Z { get; }

		public PositionChangeEvent(long time, Agent agent, float x, float y, float z) : base(time, agent)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}

	/// <summary>
	/// An event specifying that the velocity (3D vector) of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if velocity changes more often.
	/// </remarks>
	public class VelocityChangeEvent : AgentEvent
	{
		public float X { get; }
		public float Y { get; }
		public float Z { get; }

		public VelocityChangeEvent(long time, Agent agent, float x, float y, float z) : base(time, agent)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}

	/// <summary>
	/// An event specifying that the facing (2D vector) of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if facing changes more often.
	/// </remarks>
	public class FacingChangeEvent : AgentEvent
	{
		public float X { get; }
		public float Y { get; }

		public FacingChangeEvent(long time, Agent agent, float x, float y) : base(time, agent)
		{
			X = x;
			Y = y;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> changed their current team.
	/// <br />
	/// Teams affect hostility of targets.
	/// </summary>
	public class TeamChangeEvent : AgentEvent
	{
		public ulong NewTeamId { get; }

		public TeamChangeEvent(long time, Agent agent, ulong newTeamId) : base(time, agent)
		{
			NewTeamId = newTeamId;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="AttackTarget"/> is now (un)targetable.
	/// </summary>
	public class TargetableChangeEvent : AgentEvent
	{
		public AttackTarget AttackTarget => (AttackTarget) Agent;

		public bool IsTargetable { get; }

		public TargetableChangeEvent(long time, AttackTarget agent, bool targetable) : base(time, agent)
		{
			IsTargetable = targetable;
		}
	}

	/// <summary>
	/// An event specifying that the health percentage of the defiance bar of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if health changes more often.
	/// <br/>
	/// Introduced in EVTC20200506.
	/// </remarks>
	public class DefianceBarHealthUpdateEvent : AgentEvent
	{
		public float HealthFraction { get; }

		public DefianceBarHealthUpdateEvent(long time, Agent agent, float healthFraction) : base(time, agent)
		{
			HealthFraction = healthFraction;
		}
	}
	
	/// <summary>
	/// An event specifying that the barrier percentage of a health bar belonging an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// This event is typically only provided once per a period, even if barrier state changes more often.
	/// <br/>
	/// Introduced in EVTC20201201.
	/// </remarks>
	public class BarrierUpdateEvent : AgentEvent
	{
		public float BarrierFraction { get; }

		public BarrierUpdateEvent(long time, Agent agent, float barrierFraction) : base(time, agent)
		{
			BarrierFraction = barrierFraction;
		}
	}

	/// <summary>
	/// An event specifying that the state of the defiance bar of an <see cref="Agent"/> has been changed.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20200506.
	/// </remarks>
	public class DefianceBarStateUpdateEvent : AgentEvent
	{
		public enum DefianceBarState
		{
			Active,
			Recovering,
			Immune,
			None,
			Unknown
		}

		public DefianceBarState State { get; }

		public DefianceBarStateUpdateEvent(long time, Agent agent, DefianceBarState state) : base(time, agent)
		{
			State = state;
		}
	}

	/// <summary>
	/// An event specifying that an effect was created.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20220602.
	/// </remarks>
	public class EffectEvent : AgentEvent
	{
		/// <summary>
		/// The owner of this effect.
		/// </summary>
		public Agent EffectOwner => Agent;

		/// <summary>
		/// The Effect created.
		/// </summary>
		public Effect Effect { get; }

		/// <summary>
		/// The <see cref="Agent"/> this effect is anchored to if anchored. May be <see langword="null" />.
		/// </summary>
		/// <seealso cref="Position"/>
		public Agent AgentTarget { get; }

		/// <summary>
		/// Position (x, y, z) of the effect when not anchored to a target. May be <see langword="null" />.
		/// </summary>
		/// <seealso cref="AgentTarget"/>
		public float[] Position { get; }

		/// <summary>
		/// The orientation (x, y, z) of the effect. If <see cref="ZAxisOrientationOnly"/> is <see langword="true"/>,
		/// first two values are not valid.
		/// </summary>
		public float[] Orientation { get; }

		/// <summary>
		/// Indicates whether the first two values of <see cref="Orientation"/> are valid.
		/// </summary>
		public bool ZAxisOrientationOnly { get; }

		/// <summary>
		/// The duration of the effect in milliseconds.
		/// </summary>
		public ushort Duration { get; }

		public EffectEvent(long time, Agent agent, Effect effect, Agent agentTarget, float[] position,
			float[] orientation, bool zAxisOrientationOnly, ushort duration) : base(time, agent)
		{
			Effect = effect;
			AgentTarget = agentTarget;
			Position = position;
			Orientation = orientation;
			ZAxisOrientationOnly = zAxisOrientationOnly;
			Duration = duration;
		}
	}
}