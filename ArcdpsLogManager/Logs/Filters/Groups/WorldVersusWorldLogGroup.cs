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
			new MapLogGroup(38, "Eternal Battlegrounds"),
			new MapLogGroup(1099, "Red Desert Borderlands"),
			new MapLogGroup(96, "Blue Alpine Borderlands"),
			new MapLogGroup(95, "Green Alpine Borderlands"),
			new MapLogGroup(899, "Obsidian Sanctum"),
			new MapLogGroup(968, "Edge of the Mists"),
			new MapLogGroup(1315, "Armistice Bastion"),
		};

		public WorldVersusWorldLogGroup() : base(EncounterCategory.WorldVersusWorld)
		{
		}
	}
}