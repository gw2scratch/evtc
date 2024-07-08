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
	public abstract class SkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs)
		: AgentEvent(time, agent)
	{
		public Skill Skill { get; } = skill;
		public int CastingTimeMs { get; } = castingTimeMs;
	}

	/// <summary>
	/// An event representing a <see cref="Skill"/> cast ending.
	/// </summary>
	public class EndSkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs, EndSkillCastEvent.SkillEndType skillEndType)
		: SkillCastEvent(time,
			agent, skill, castingTimeMs)
	{
		public enum SkillEndType
		{
			Cancel,
			Fire
		}

		public SkillEndType EndType { get; } = skillEndType;
	}

	/// <summary>
	/// An event representing a <see cref="Skill"/> cast animation ending by being reset.
	/// </summary>
	public class ResetSkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs)
		: SkillCastEvent(time, agent,
			skill, castingTimeMs);

	/// <summary>
	/// An event representing a <see cref="Skill"/> cast starting.
	/// </summary>
	public class StartSkillCastEvent(
		long time,
		Agent agent,
		Skill skill,
		int castingTimeMs,
		StartSkillCastEvent.SkillCastType castType)
		: SkillCastEvent(time, agent, skill, castingTimeMs)
	{
		public enum SkillCastType
		{
			Normal,
			WithQuickness
		}

		public SkillCastType CastType { get; } = castType;
	}
}