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

	public class CancelledSkillCastEvent : SkillCastEvent
	{
		public CancelledSkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs) : base(time,
			agent, skill, castingTimeMs)
		{
		}
	}

	public class ResetSkillCastEvent : SkillCastEvent
	{
		public ResetSkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs) : base(time, agent,
			skill, castingTimeMs)
		{
		}
	}

	public class SuccessfulSkillCastEvent : SkillCastEvent
	{
		public enum SkillCastType
		{
			Normal,
			WithQuickness
		}

		public SkillCastType CastType { get; }

		public SuccessfulSkillCastEvent(long time, Agent agent, Skill skill, int castingTimeMs,
			SkillCastType castType) : base(time, agent, skill, castingTimeMs)
		{
			CastType = castType;
		}
	}
}