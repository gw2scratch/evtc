namespace ScratchEVTCParser.Model.Skills
{
	public class Skill
	{
		public int Id { get; }
		public string Name { get; }

		public Skill(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}