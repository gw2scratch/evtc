using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// An event representing a <see cref="Skill"/> cast by an <see cref="Agent"/>.
	/// </summary>
	/// <remarks>
	/// Internally, skill casts are significantly tied to their animations.
	/// </remarks>
	public abstract class SkillCastEvent : AgentEvent
	{
		public Skill Skill { get; }
		public int CastingTimeMs { get; }

		public SkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs) : base(time, agent)
		{
			Skill = skill;
			CastingTimeMs = castingTimeMs;
		}
	}

	/// <summary>
	/// An event representing a <see cref="Skill"/> cast ending.
	/// </summary>
	public class EndSkillCastEvent : SkillCastEvent
	{
		public enum SkillEndType
		{
			Cancel,
			Fire
		}

		public SkillEndType EndType { get; }

		public EndSkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs, SkillEndType skillEndType) : base(time,
			agent, skill, castingTimeMs)
		{
			EndType = skillEndType;
		}
	}

	/// <summary>
	/// An event representing a <see cref="Skill"/> cast animation ending by being reset.
	/// </summary>
	public class ResetSkillCastEvent : SkillCastEvent
	{
		public ResetSkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs) : base(time, agent,
			skill, castingTimeMs)
		{
		}
	}

	/// <summary>
	/// An event representing a <see cref="Skill"/> cast starting.
	/// </summary>
	public class StartSkillCastEvent : SkillCastEvent
	{
		public enum SkillCastType
		{
			Normal,
			WithQuickness
		}

		public SkillCastType CastType { get; }

		public StartSkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs,
			SkillCastType castType) : base(time, agent, skill, castingTimeMs)
		{
			CastType = castType;
		}
	}
}