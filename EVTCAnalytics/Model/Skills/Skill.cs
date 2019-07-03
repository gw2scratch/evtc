namespace GW2Scratch.EVTCAnalytics.Model.Skills
{
	public class Skill
	{
		public uint Id { get; }
		public string Name { get; }

		public Skill(uint id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}