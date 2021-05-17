using GW2Scratch.ArcdpsLogManager.GameData;
using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that all logs with <see cref="EncounterCategory.WorldVersusWorld"/> belong to.
	/// </summary>
	public class WorldVersusWorldLogGroup : CategoryLogGroup
	{
		private const string GroupName = "World vs. World";

		public override string Name => GroupName;
		public override IEnumerable<LogGroup> Subgroups { get; } = new LogGroup[]
		{
			new MapLogGroup(GameMap.EternalBattlegrounds, "Eternal Battlegrounds"),
			new MapLogGroup(GameMap.RedDesertBorderlands, "Red Desert Borderlands"),
			new MapLogGroup(GameMap.BlueAlpineBorderlands, "Blue Alpine Borderlands"),
			new MapLogGroup(GameMap.GreenAlpineBorderlands, "Green Alpine Borderlands"),
			new MapLogGroup(GameMap.ObsidianSanctum, "Obsidian Sanctum"),
			new MapLogGroup(GameMap.EdgeOfTheMists, "Edge of the Mists"),
			new MapLogGroup(GameMap.ArmisticeBastion, "Armistice Bastion"),
		};

		public WorldVersusWorldLogGroup() : base(EncounterCategory.WorldVersusWorld)
		{
		}
	}
}