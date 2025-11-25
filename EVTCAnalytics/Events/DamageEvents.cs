using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// An event specifying that an <see cref="Agent"/> avoided being damaged directly by a <see cref="Skill"/>.
	/// </summary>
	public class IgnoredPhysicalDamageEvent(
		long time,
		Agent attacker,
		Agent defender,
		Skill skill,
		int damage,
		bool isMoving,
		bool isNinety,
		bool isFlanking,
		uint shieldDamage,
		IgnoredPhysicalDamageEvent.Reason reason)
		: PhysicalDamageEvent(time, attacker, defender, skill, 0, isMoving, isNinety, isFlanking, 0, Result.Ignored)
	{
		public enum Reason
		{
			Blocked,
			Evaded,
			Absorbed,
			Missed,
		}

		public Reason IgnoreReason { get; } = reason;
		public int IgnoredDamage { get; } = damage;
		public uint IgnoredShieldDamage { get; } = shieldDamage;
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> was damaged directly by a <see cref="Skill"/>.
	/// </summary>
	public class PhysicalDamageEvent(
		long time,
		Agent attacker,
		Agent defender,
		Skill skill,
		int damage,
		bool isMoving,
		bool isNinety,
		bool isFlanking,
		uint shieldDamage,
		PhysicalDamageEvent.Result result)
		: DamageEvent(time, attacker, defender, skill, damage, isMoving, isNinety, isFlanking)
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

		public Result HitResult { get; } = result;
		public uint ShieldDamage { get; } = shieldDamage;
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> avoided being damaged by a buff.
	/// </summary>
	public class IgnoredBuffDamageEvent(
		long time,
		Agent attacker,
		Agent defender,
		Skill skill,
		int damage,
		bool isMoving,
		bool isNinety,
		bool isFlanking,
		IgnoredBuffDamageEvent.Reason reason)
		: BuffDamageEvent(time, attacker, defender, skill, 0, isMoving, isNinety, isFlanking)
	{
		public enum Reason
		{
			InvulnerableBuff,
			InvulnerableSkill
		}

		public Reason IgnoreReason { get; } = reason;
		public int IgnoredDamage { get; } = damage;
	}

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> was damaged by a buff.
	/// </summary>
	public class BuffDamageEvent(
		long time,
		Agent attacker,
		Agent defender,
		Skill buff,
		int damage,
		bool isMoving,
		bool isNinety,
		bool isFlanking)
		: DamageEvent(time, attacker, defender, buff, damage, isMoving, isNinety, isFlanking);

	/// <summary>
	/// An event specifying that an <see cref="Agent"/> was damaged by a buff in an unscheduled update.
	/// An example of this would buffs that not only do damage over time, but also trigger on skill casts.
	/// </summary>
	public class OffCycleBuffDamageEvent(
		long time,
		Agent attacker,
		Agent defender,
		Skill skill,
		int damage,
		bool isMoving,
		bool isNinety,
		bool isFlanking)
		: BuffDamageEvent(time, attacker, defender, skill, damage, isMoving, isNinety, isFlanking);

	/// <summary>
	/// An event specifying that the defiance bar of a <see cref="Agent"/> was damaged directly by a <see cref="Skill"/>.
	/// </summary>
	public class DefianceBarDamageEvent(
		long time,
		Agent attacker,
		Agent defender,
		Skill skill,
		int damage,
		bool isMoving,
		bool isNinety,
		bool isFlanking)
		: DamageEvent(time, attacker, defender, skill, damage, isMoving, isNinety, isFlanking);

	/// <summary>
	/// An event specifying damage inflicted to an agent.
	/// </summary>
	/// <remarks>
	/// This event does not an <see cref="AgentEvent"/> as it is not clear whether it would be
	/// classified under the attacker or the defender.
	/// </remarks>
	public abstract class DamageEvent(
		long time,
		Agent attacker,
		Agent defender,
		Skill skill,
		int damage,
		bool isMoving,
		bool isNinety,
		bool isFlanking)
		: Event(time), ISkillEvent
	{
		public Skill Skill { get; } = skill;
		public Agent Attacker { get; internal set; } = attacker;
		public Agent Defender { get; internal set; } = defender;
		public int Damage { get; } = damage;
		public bool IsMoving { get; } = isMoving;
		public bool IsNinety { get; } = isNinety;
		public bool IsFlanking { get; } = isFlanking;
	}

	/// <summary>
	/// A crowd control event affecting an enemy without a defiance bar.
	/// </summary>
	public class CrowdControlEvent(
		long time,
		Agent attacker,
		Agent defender,
		Skill skill,
		bool isMoving,
		bool isNinety,
		bool isFlanking)
		: Event(time)
	{
		public Skill Skill { get; } = skill;
		public Agent Attacker { get; internal set; } = attacker;
		public Agent Defender { get; internal set; } = defender;
		public bool IsMoving { get; } = isMoving;
		public bool IsNinety { get; } = isNinety;
		public bool IsFlanking { get; } = isFlanking;
	}
}