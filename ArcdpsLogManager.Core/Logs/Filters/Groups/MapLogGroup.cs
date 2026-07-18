using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that all logs recorded on a specific game map belong to.
	/// </summary>
	public class MapLogGroup : LogGroup
	{
		public int? MapId { get; }
		public override string Name { get; }
		public override IEnumerable<LogGroup> Subgroups { get; } = new LogGroup[0];
		private readonly bool requireMapEncounter;

		/// <summary>
		/// Creates a new map log group.
		/// </summary>
		/// <param name="mapId">The id of the map that logs have to be recorded on in order to be in this group.</param>
		/// <param name="nameProvider">Name provider.</param>
		/// <param name="requireMapEncounter">If true, require the log to be a map log.</param>
		public MapLogGroup(int? mapId, ILogNameProvider nameProvider, bool requireMapEncounter = false)
		{
			MapId = mapId;
			Name = nameProvider.GetMapName(mapId);
			this.requireMapEncounter = requireMapEncounter;
		}

		/// <summary>
		/// Creates a new map log group.
		/// </summary>
		/// <param name="mapId">The id of the map that logs have to be recorded on in order to be in this group.</param>
		/// <param name="name">The name of the log group.</param>
		/// <param name="requireMapEncounter">If true, require the log to be a map log.</param>
		public MapLogGroup(int? mapId, string name, bool requireMapEncounter = false)
		{
			MapId = mapId;
			Name = name;
			this.requireMapEncounter = requireMapEncounter;
		}

		public override bool IsInGroup(LogData log)
		{
			if (requireMapEncounter && log.Encounter != Encounter.Map)
			{
				return false;
			}
			
			return log.MapId == MapId;
		}
	}
}