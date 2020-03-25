using System;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Events
{
	public abstract class AgentEvent : Event
	{
		public Agent Agent { get; internal set; }

		protected AgentEvent(long time, Agent agent) : base(time)
		{
			Agent = agent;
		}
	}

	public class AgentEnterCombatEvent : AgentEvent
	{
		public int Subgroup { get; }

		public AgentEnterCombatEvent(long time, Agent agent, int subgroup) : base(time, agent)
		{
			Subgroup = subgroup;
		}
	}

	public class AgentExitCombatEvent : AgentEvent
	{
		public AgentExitCombatEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	public class AgentRevivedEvent : AgentEvent
	{
		public AgentRevivedEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	public class AgentDownedEvent : AgentEvent
	{
		public AgentDownedEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	public class AgentDeadEvent : AgentEvent
	{
		public AgentDeadEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	public class AgentSpawnEvent : AgentEvent
	{
		public AgentSpawnEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	public class AgentDespawnEvent : AgentEvent
	{
		public AgentDespawnEvent(long time, Agent agent) : base(time, agent)
		{
		}
	}

	public class AgentHealthUpdateEvent : AgentEvent
	{
		public float HealthFraction { get; }

		public AgentHealthUpdateEvent(long time, Agent agent, float healthFraction) : base(time, agent)
		{
			HealthFraction = healthFraction;
		}
	}

	public class AgentWeaponSwapEvent : AgentEvent
	{
		public WeaponSet NewWeaponSet { get; }

		public AgentWeaponSwapEvent(long time, Agent agent, WeaponSet newWeaponSet) : base(time, agent)
		{
			NewWeaponSet = newWeaponSet;
		}
	}

	public class AgentMaxHealthUpdateEvent : AgentEvent
	{
		public ulong NewMaxHealth { get; }

		public AgentMaxHealthUpdateEvent(long time, Agent agent, ulong newMaxHealth) : base(time, agent)
		{
			NewMaxHealth = newMaxHealth;
		}
	}

	public class InitialBuffEvent : AgentEvent
	{
		public Skill Skill { get; }

		public InitialBuffEvent(long time, Agent agent, Skill skill) : base(time, agent)
		{
			Skill = skill;
		}
	}

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

	public class TeamChangeEvent : AgentEvent
	{
		public ulong NewTeamId { get; }

		public TeamChangeEvent(long time, Agent agent, ulong newTeamId) : base(time, agent)
		{
			NewTeamId = newTeamId;
		}
	}

	public class TargetableChangeEvent : AgentEvent
	{
		public AttackTarget AttackTarget => (AttackTarget) Agent;

		public bool IsTargetable { get; }

		public TargetableChangeEvent(long time, AttackTarget agent, bool targetable) : base(time, agent)
		{
			IsTargetable = targetable;
		}
	}
}