using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	/// <summary>
	/// Represents an agent in the game - an entity with health, capable of using skills and movement in some cases.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A single <see cref="Agent"/> may correspond to multiple raw <see cref="Parsed.ParsedAgent"/>s.
	/// The <see cref="AgentOrigin"/> may be used to see where it comes from.
	/// This mechanism is required to handle merging NPCs that leave the reporting area.
	/// </para>
	/// </remarks>
	public abstract class Agent
	{
		/// <summary>
		/// Provides metadata about the origin of the <see cref="Agent"/> from raw <see cref="Parsed.ParsedAgent"/>s.
		/// </summary>
		public AgentOrigin AgentOrigin { get; }
		
		/// <summary>
		/// Provides the name of this Agent.
		/// </summary>
		/// <remarks>
		/// These names are translated according to the <see cref="GameData.GameLanguage"/> of the client recording the log.
		/// <br />
		/// If the name identification fails in arcdps, these values might be generated from ids
		/// of the underlying <see cref="Parsed.ParsedAgent"/> by arcdps instead of providing ingame names.
		/// </remarks>
		public string Name { get; }
		
		/// <summary>
		/// Provides the width of the hitbox of the agent in units.
		/// </summary>
		/// <remarks>
		/// These values might not be perfectly reliable, make sure to verify them in the game if you need to rely on them.
		/// </remarks>
		public int HitboxWidth { get; }
		
		/// <summary>
		/// Provides the height of the hitbox of the agent in units.
		/// </summary>
		/// <remarks>
		/// These values might not be perfectly reliable, make sure to verify them in the game if you need to rely on them.
		/// </remarks>
		public int HitboxHeight { get; }
		
		/// <summary>
		/// Represents the first time in milliseconds the log is aware of the agent.
		/// </summary>
		public long FirstAwareTime { get; internal set; } = 0;
		
		/// <summary>
		/// Represents the last time in milliseconds the log is aware of the agent.
		/// </summary>
		public long LastAwareTime { get; internal set; } = long.MaxValue;

		internal List<Agent> MinionList { get; } = new List<Agent>();
		
		/// <summary>
		/// Provides a list of <see cref="Agent"/>s that are minions of this <see cref="Agent"/> in a master-minion relationship.
		/// </summary>
		public IReadOnlyList<Agent> Minions => MinionList;
		
		/// <summary>
		/// Provides the master of this <see cref="Agent"/> if it is a part of a master-minion relationship.
		/// </summary>
		/// <remarks>
		/// This value is commonly <see langword="null"/>.
		/// </remarks>
		public Agent Master { get; internal set; }

		protected Agent(AgentOrigin agentOrigin, string name, int hitboxWidth, int hitboxHeight)
		{
			AgentOrigin = agentOrigin;
			Name = name;
			HitboxWidth = hitboxWidth;
			HitboxHeight = hitboxHeight;
		}

		/// <summary>
		/// Checks whether a time is within the time period the log was aware of this <see cref="Agent"/>.
		/// </summary>
		/// <param name="time">A time in milliseconds.</param>
		public bool IsWithinAwareTime(long time)
		{
			return FirstAwareTime <= time && time <= LastAwareTime;
		}
	}
}