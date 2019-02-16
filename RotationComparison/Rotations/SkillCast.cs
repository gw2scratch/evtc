namespace RotationComparison.Rotations
{
	public class SkillCast : RotationItem
	{
		public override RotationItemType Type { get; } = RotationItemType.SkillCast;
		public override long Time { get; }
		public override long Duration { get; }
		public SkillCastType CastType { get; }
		public uint SkillId { get; }
		public string SkillName { get; }

		public SkillCast(long time, long duration, SkillCastType castTypeType, uint skillId, string skillName)
		{
			Time = time;
			Duration = duration;
			CastType = castTypeType;
			SkillId = skillId;
			SkillName = skillName;
		}
	}
}