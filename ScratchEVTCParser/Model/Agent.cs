namespace ScratchEVTCParser.Model
{
	public abstract class Agent
	{
		public ulong Address { get; }
		public int Id { get; }
		public string Name { get; }
		public int HitboxWidth { get; }
		public int HitboxHeight { get; }

		protected Agent(ulong address, int id, string name, int hitboxWidth, int hitboxHeight)
		{
			Id = id;
			Name = name;
			HitboxWidth = hitboxWidth;
			HitboxHeight = hitboxHeight;
			Address = address;
		}
	}
}