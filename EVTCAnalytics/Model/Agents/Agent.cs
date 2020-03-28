using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	public abstract class Agent
	{
		public AgentOrigin AgentOrigin { get; }
		public string Name { get; }
		public int HitboxWidth { get; }
		public int HitboxHeight { get; }
		public long FirstAwareTime { get; internal set; } = 0;
		public long LastAwareTime { get; internal set; } = long.MaxValue;

		internal List<Agent> MinionList { get; } = new List<Agent>();
		public IReadOnlyList<Agent> Minions => MinionList;
		public Agent Master { get; internal set; }

		protected Agent(AgentOrigin agentOrigin, string name, int hitboxWidth, int hitboxHeight)
		{
			AgentOrigin = agentOrigin;
			Name = name;
			HitboxWidth = hitboxWidth;
			HitboxHeight = hitboxHeight;
		}

		public bool IsWithinAwareTime(long time)
		{
			return FirstAwareTime <= time && time <= LastAwareTime;
		}
	}
}