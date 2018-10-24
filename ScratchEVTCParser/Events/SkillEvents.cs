using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Events
{
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

	public class ResetSkillCastEvent : SkillCastEvent
	{
		public ResetSkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs) : base(time, agent,
			skill, castingTimeMs)
		{
		}
	}

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