namespace GW2Scratch.EVTCAnalytics.Parsed
{
	/// <summary>
	/// The raw skill data from a <c>evtc_skill</c> struct as defined by arcdps.
	/// </summary>
	public readonly struct ParsedSkill
	{
		/// <summary>
		/// Gets the ID of the skill.
		/// </summary>
		public int SkillId { get; }
		
		/// <summary>
		/// Gets the name of the skill.
		/// </summary>
		public string Name { get; }

		public ParsedSkill(int skillId, string name)
		{
			SkillId = skillId;
			Name = name;
		}
	}
}