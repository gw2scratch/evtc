using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that all logs recorded on a specific game map belong to.
	/// </summary>
	public class MapLogGroup : LogGroup
	{
		private readonly int mapId;
		public override string Name { get; }
		public override IEnumerable<LogGroup> Subgroups { get; } = new LogGroup[0];

		/// <summary>
		/// Creates a new map log group.
		/// </summary>
		/// <param name="mapId">The ID of the map that logs have to be recorded on in order to be in this group.</param>
		/// <param name="name">The name of the log group.</param>
		public MapLogGroup(int mapId, string name)
		{
			this.mapId = mapId;
			Name = name;
		}

		public override bool IsInGroup(LogData log)
		{
			return log.MapId == mapId;
		}
	}
}