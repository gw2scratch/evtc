using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics
{
	public class PlayerData
	{
		public PlayerData(Player player, int downCount, int deathCount)
		{
			Player = player;
			DownCount = downCount;
			DeathCount = deathCount;
		}

		public Player Player { get; }
		public int DownCount { get; }
		public int DeathCount { get; }
	}
}