using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Statistics.RotationItems
{
	public class SkillCastItem : RotationItem
	{
		public Skill Skill { get; }
		public SkillData SkillData { get; }
		public SkillCastType Type { get; }

		public long CastEndTime { get; }
		public long Duration => CastEndTime - ItemTime;

		public SkillCastItem(long castStartTime, long castEndTime, SkillCastType type, Skill skill, SkillData skillData) : base(castStartTime)
		{
			CastEndTime = castEndTime;
			Type = type;
			Skill = skill;
			SkillData = skillData;
		}
	}
}