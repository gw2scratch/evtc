using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Events
{
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

	public class BuffDamageEvent : DamageEvent
	{
		public BuffDamageEvent(long time, Agent attacker, Agent defender, Skill buff, int damage, bool isMoving,
			bool isNinety, bool isFlanking) : base(time, attacker, defender, buff, damage, isMoving, isNinety,
			isFlanking)
		{
		}
	}

	public class OffCycleBuffDamageEvent : BuffDamageEvent
	{
		public OffCycleBuffDamageEvent(long time, Agent attacker, Agent defender, Skill skill, int damage,
			bool isMoving, bool isNinety, bool isFlanking) : base(time, attacker, defender, skill, damage, isMoving,
			isNinety, isFlanking)
		{
		}
	}

	public abstract class DamageEvent : Event
	{
		public Skill Skill { get; }
		public Agent Attacker { get; }
		public Agent Defender { get; }
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