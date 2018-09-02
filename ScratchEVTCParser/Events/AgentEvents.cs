using System;

namespace ScratchEVTCParser.Events
{
	public abstract class AgentEvent : Event
	{
		public ushort AgentId { get; }

		protected AgentEvent(long time, ushort agentId) : base(time)
		{
			AgentId = agentId;
		}
	}

	public class AgentEnterCombatEvent : AgentEvent
	{
		public int Subgroup { get; }

		public AgentEnterCombatEvent(long time, ushort agentId, int subgroup) : base(time, agentId)
		{
			Subgroup = subgroup;
		}
	}

	public class AgentExitCombatEvent : AgentEvent
	{
		public AgentExitCombatEvent(long time, ushort agentId) : base(time, agentId)
		{
		}
	}

	public class AgentRevivedEvent : AgentEvent
	{
		public AgentRevivedEvent(long time, ushort agentId) : base(time, agentId)
		{
		}
	}

	public class AgentDownedEvent : AgentEvent
	{
		public AgentDownedEvent(long time, ushort agentId) : base(time, agentId)
		{
		}
	}

	public class AgentDeadEvent : AgentEvent
	{
		public AgentDeadEvent(long time, ushort agentId) : base(time, agentId)
		{
		}
	}

	public class AgentSpawnEvent : AgentEvent
	{
		public AgentSpawnEvent(long time, ushort agentId) : base(time, agentId)
		{
		}
	}

	public class AgentDespawnEvent : AgentEvent
	{
		public AgentDespawnEvent(long time, ushort agentId) : base(time, agentId)
		{
		}
	}

	public class AgentHealthUpdateEvent : AgentEvent
	{
		public float HealthFraction { get; }

		public AgentHealthUpdateEvent(long time, ushort agentId, float healthFraction) : base(time, agentId)
		{
			HealthFraction = healthFraction;
		}
	}

	public class AgentWeaponSwapEvent : AgentEvent
	{
		public enum WeaponSet
		{
			Land1,
			Land2,
			Water1,
			Water2,
			Unknown
		}

		public WeaponSet NewWeaponSet { get; }

		public AgentWeaponSwapEvent(long time, ushort agentId, WeaponSet newWeaponSet) : base(time, agentId)
		{
			NewWeaponSet = newWeaponSet;
		}
	}

	public class AgentMaxHealthUpdateEvent : AgentEvent
	{
		public ulong NewMaxHealth { get; }

		public AgentMaxHealthUpdateEvent(long time, ushort agentId, ulong newMaxHealth) : base(time, agentId)
		{
			NewMaxHealth = newMaxHealth;
		}
	}

	public class InitialBuffEvent : AgentEvent
	{
		public int SkillId { get; }

		public InitialBuffEvent(long time, ushort agentId, int skillId) : base(time, agentId)
		{
			SkillId = skillId;
		}
	}

	public class PositionChangeEvent : AgentEvent
	{
		public float X { get; }
		public float Y { get; }
		public float Z { get; }

		public PositionChangeEvent(long time, ushort agentId, float x, float y, float z) : base(time, agentId)
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

		public VelocityChangeEvent(long time, ushort agentId, float x, float y, float z) : base(time, agentId)
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

		public FacingChangeEvent(long time, ushort agentId, float x, float y) : base(time, agentId)
		{
			X = x;
			Y = y;
		}
	}

	public class TeamChangeEvent : AgentEvent
	{
		public ulong NewTeamId { get; }

		public TeamChangeEvent(long time, ushort agentId, ulong newTeamId) : base(time, agentId)
		{
			NewTeamId = newTeamId;
		}
	}
}