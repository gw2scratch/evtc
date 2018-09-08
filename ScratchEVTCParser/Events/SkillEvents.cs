using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Events
{
	public abstract class SkillCastEvent : AgentEvent
	{
		public int SkillId { get; }
		public int CastingTimeMs { get; }

		public SkillCastEvent(long time, Agent agent, int skillId, int castingTimeMs) : base(time, agent)
		{
			SkillId = skillId;
			CastingTimeMs = castingTimeMs;
		}
	}

	public class CancelledSkillCastEvent : SkillCastEvent
	{
		public CancelledSkillCastEvent(long time, Agent agent, int skillId, int castingTimeMs) : base(time,
			agent, skillId, castingTimeMs)
		{
		}
	}

	public class ResetSkillCastEvent : SkillCastEvent
	{
		public ResetSkillCastEvent(long time, Agent agent, int skillId, int castingTimeMs) : base(time, agent,
			skillId, castingTimeMs)
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

		public SuccessfulSkillCastEvent(long time, Agent agent, int skillId, int castingTimeMs,
			SkillCastType castType) : base(time, agent, skillId, castingTimeMs)
		{
			CastType = castType;
		}
	}
}