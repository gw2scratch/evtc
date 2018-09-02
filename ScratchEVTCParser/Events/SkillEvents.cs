namespace ScratchEVTCParser.Events
{
	public abstract class SkillCastEvent : AgentEvent
	{
		public int SkillId { get; }
		public int CastingTimeMs { get; }

		public SkillCastEvent(long time, ushort agentId, int skillId, int castingTimeMs) : base(time, agentId)
		{
			SkillId = skillId;
			CastingTimeMs = castingTimeMs;
		}
	}

	public class CancelledSkillCastEvent : SkillCastEvent
	{
		public CancelledSkillCastEvent(long time, ushort agentId, int skillId, int castingTimeMs) : base(time,
			agentId, skillId, castingTimeMs)
		{
		}
	}

	public class ResetSkillCastEvent : SkillCastEvent
	{
		public ResetSkillCastEvent(long time, ushort agentId, int skillId, int castingTimeMs) : base(time, agentId,
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

		public SuccessfulSkillCastEvent(long time, ushort agentId, int skillId, int castingTimeMs,
			SkillCastType castType) : base(time, agentId, skillId, castingTimeMs)
		{
			CastType = castType;
		}
	}
}