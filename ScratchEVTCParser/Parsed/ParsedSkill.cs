namespace ScratchEVTCParser.Parsed
{
	public class ParsedSkill
	{
		public int SkillId { get; }
		public string Name { get; }

		public ParsedSkill(int skillId, string name)
		{
			SkillId = skillId;
			Name = name;
		}
	}
}