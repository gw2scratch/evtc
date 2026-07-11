using System.Collections.Generic;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Sections.GameData;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// Flattened display projection of a <see cref="SpeciesGatherResult"/> for the Game data
	/// section's species grid (Avalonia counterpart of the Eto
	/// <c>Sections/GameDataGathering.cs</c>'s species grid row).
	/// </summary>
	public class SpeciesDataRow
	{
		private readonly SpeciesGatherResult data;

		public int SpeciesId => data.SpeciesId;
		public string Name => data.Name;
		public int TimesSeen => data.Logs.Count;
		public string LogsText => $"{data.Logs.Count}, click me to open log list";
		public IReadOnlyList<LogData> Logs => data.Logs;

		public SpeciesDataRow(SpeciesGatherResult data)
		{
			this.data = data;
		}
	}

	/// <summary>
	/// Flattened display projection of a <see cref="SkillGatherResult"/> for the Game data
	/// section's skill grid.
	/// </summary>
	public class SkillDataRow
	{
		private readonly SkillGatherResult data;

		public uint SkillId => data.SkillId;
		public string Name => data.Name;
		public string TypeText => data.Type.ToString();
		public int TimesSeen => data.Logs.Count;
		public string LogsText => $"{data.Logs.Count}, click me to open log list";
		public IReadOnlyList<LogData> Logs => data.Logs;

		public SkillDataRow(SkillGatherResult data)
		{
			this.data = data;
		}
	}
}
