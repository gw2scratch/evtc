using GW2Scratch.EVTCAnalytics.GameData;
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
			new MapLogGroup(MapIds.EternalBattlegrounds, "Eternal Battlegrounds"),
			new MapLogGroup(MapIds.RedDesertBorderlands, "Red Desert Borderlands"),
			new MapLogGroup(MapIds.BlueAlpineBorderlands, "Blue Alpine Borderlands"),
			new MapLogGroup(MapIds.GreenAlpineBorderlands, "Green Alpine Borderlands"),
			new MapLogGroup(MapIds.ObsidianSanctum, "Obsidian Sanctum"),
			new MapLogGroup(MapIds.EdgeOfTheMists, "Edge of the Mists"),
			new MapLogGroup(MapIds.ArmisticeBastion, "Armistice Bastion"),
		};

		public WorldVersusWorldLogGroup() : base(EncounterCategory.WorldVersusWorld)
		{
		}
	}
}