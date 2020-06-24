using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// An event specifying that an <see cref="Agent"/> avoided being damaged directly by a <see cref="Skill"/>.
	/// </summary>
	public class IgnoredPhysicalDamageEvent : PhysicalDamageEvent
	{
		public enum Reason
		{
			Blocked,
			Evaded,
			Absorbed,
			Missed,
		}

		public Reason IgnoreReason { get; }
		public int IgnoredDamage { get; }
		public uint IgnoredShieldDamage { get; }

		public IgnoredPhysicalDamageEvent(long time, Agent attacker, Agent defender, Skill skill, int damage,
			bool isMoving, bool isNinety, bool isFlanking, uint shieldDamage, Reason reason) : base(time, attacker,
			defender, skill, 0, isMoving, isNinety, isFlanking, 0, Result.Ignored)
		{
			IgnoreReason = reason;
			IgnoredDamage = damage;
			IgnoredShieldDamage = shieldDamage;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> was damaged directly by a <see cref="Skill"/>.
	/// </summary>
	public class PhysicalDamageEvent : DamageEvent
	{
		public enum Result
		{
			Normal,
			Critical,
			Glance,
			Interrupt,
			DowningBlow,
			KillingBlow,
			Ignored
		}

		public Result HitResult { get; }
		public uint ShieldDamage { get; }

		public PhysicalDamageEvent(long time, Agent attacker, Agent defender, Skill skill, int damage, bool isMoving,
			bool isNinety, bool isFlanking, uint shieldDamage, Result result) : base(time, attacker, defender, skill,
			damage, isMoving, isNinety, isFlanking)
		{
			ShieldDamage = shieldDamage;
			HitResult = result;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> avoided being damaged by a buff.
	/// </summary>
	public class IgnoredBuffDamageEvent : BuffDamageEvent
	{
		public enum Reason
		{
			InvulnerableBuff,
			InvulnerableSkill
		}

		public Reason IgnoreReason { get; }
		public int IgnoredDamage { get; }

		public IgnoredBuffDamageEvent(long time, Agent attacker, Agent defender, Skill skill, int damage, bool isMoving,
			bool isNinety, bool isFlanking, Reason reason) : base(time, attacker, defender, skill, 0, isMoving,
			isNinety, isFlanking)
		{
			IgnoredDamage = damage;
			IgnoreReason = reason;
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> was damaged by a buff.
	/// </summary>
	public class BuffDamageEvent : DamageEvent
	{
		public BuffDamageEvent(long time, Agent attacker, Agent defender, Skill buff, int damage, bool isMoving,
			bool isNinety, bool isFlanking) : base(time, attacker, defender, buff, damage, isMoving, isNinety,
			isFlanking)
		{
		}
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> was damaged by a buff in an unscheduled update.
	/// An example of this would buffs that not only do damage over time, but also trigger on skill casts.
	/// </summary>
	public class OffCycleBuffDamageEvent : BuffDamageEvent
	{
		public OffCycleBuffDamageEvent(long time, Agent attacker, Agent defender, Skill skill, int damage,
			bool isMoving, bool isNinety, bool isFlanking) : base(time, attacker, defender, skill, damage, isMoving,
			isNinety, isFlanking)
		{
		}
	}

	/// <summary>
	/// An event specifying damage inflicted to an agent.
	/// </summary>
	/// <remarks>
	/// This event does not an <see cref="AgentEvent"/> as it is not clear whether it would be
	/// classified under the attacker or the defender.
	/// </remarks>
	public abstract class DamageEvent : Event
	{
		public Skill Skill { get; }
		public Agent Attacker { get; internal set; }
		public Agent Defender { get; internal set; }
		public int Damage { get; }
		public bool IsMoving { get; }
		public bool IsNinety { get; }
		public bool IsFlanking { get; }

		protected DamageEvent(long time, Agent attacker, Agent defender, Skill skill, int damage, bool isMoving,
			bool isNinety, bool isFlanking) : base(time)
		{
			Attacker = attacker;
			Defender = defender;
			Skill = skill;
			Damage = damage;
			IsMoving = isMoving;
			IsNinety = isNinety;
			IsFlanking = isFlanking;
		}
	}
}