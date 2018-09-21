using System.Collections.Generic;

namespace ScratchEVTCParser.Model.Agents
{
	public abstract class Agent
	{
		public ulong Address { get; }
		public int Id { get; }
		public string Name { get; }
		public int HitboxWidth { get; }
		public int HitboxHeight { get; }
		public long FirstAwareTime { get; internal set; } = 0;
		public long LastAwareTime { get; internal set; } = long.MaxValue;

		internal List<Agent> MinionList { get; } = new List<Agent>();
		public IEnumerable<Agent> Minions => MinionList;
		public Agent Master { get; internal set; }

		protected Agent(ulong address, int id, string name, int hitboxWidth, int hitboxHeight)
		{
			Id = id;
			Name = name;
			HitboxWidth = hitboxWidth;
			HitboxHeight = hitboxHeight;
			Address = address;
		}

		public bool IsWithinAwareTime(long time)
		{
			return FirstAwareTime <= time && time <= LastAwareTime;
		}
	}
}