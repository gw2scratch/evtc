using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// A tree node wrapping a <see cref="LogGroup"/> for the encounter-group filter tree
	/// (Avalonia counterpart of the Eto <c>LogEncounterFilterTree</c>). Selecting a node filters
	/// the logs to that group's subtree.
	/// </summary>
	public sealed partial class LogGroupNode : ObservableObject
	{
		public LogGroup Group { get; }
		public string Name => Group.Name;
		public Bitmap? Icon { get; }
		public IReadOnlyList<LogGroupNode> Children { get; }

		/// <summary>Count of (unfiltered) logs belonging to this group, matching the Eto
		/// <c>LogEncounterFilterTree</c>'s "Count" column.</summary>
		public int LogCount { get; }

		[ObservableProperty] private bool isExpanded;

		public LogGroupNode(LogGroup group, ImageProvider imageProvider, IReadOnlyList<LogData> allLogs)
		{
			Group = group;
			Icon = GetIcon(group, imageProvider);
			LogCount = allLogs.Count(group.IsInGroup);

			// Mirrors the Eto LogEncounterFilterTree default-expanded groups.
			isExpanded = group is RootLogGroup or RaidLogGroup or RaidEncounterLogGroup or FractalLogGroup;

			Children = group.Subgroups.Select(g => new LogGroupNode(g, imageProvider, allLogs)).ToList();
		}

		private static Bitmap? GetIcon(LogGroup group, ImageProvider imageProvider)
		{
			return group switch
			{
				RootLogGroup => imageProvider.GetTinyLogIcon(),
				RaidEncounterLogGroup => imageProvider.GetTinyRaidEncounterIcon(),
				RaidLogGroup => imageProvider.GetTinyRaidIcon(),
				FractalLogGroup => imageProvider.GetTinyFractalsIcon(),
				CategoryLogGroup categoryGroup => GetCategoryIcon(categoryGroup.Category, imageProvider),
				EncounterLogGroup encounterGroup => imageProvider.GetTinyEncounterIcon(encounterGroup.Encounter),
				MapLogGroup mapGroup => imageProvider.GetWvWMapIcon(mapGroup.MapId),
				_ => null
			};
		}

		private static Bitmap? GetCategoryIcon(EncounterCategory category, ImageProvider imageProvider)
		{
			return category switch
			{
				EncounterCategory.Other => imageProvider.GetTinyUncategorizedIcon(),
				EncounterCategory.WorldVersusWorld => imageProvider.GetTinyWorldVersusWorldIcon(),
				EncounterCategory.Festival => imageProvider.GetTinyFestivalIcon(),
				EncounterCategory.Fractal => imageProvider.GetTinyFractalsIcon(),
				EncounterCategory.RaidEncounterIcebroodSaga => imageProvider.GetTinyIcebroodSagaIcon(),
				EncounterCategory.RaidEncounterEndOfDragons => imageProvider.GetTinyEndOfDragonsIcon(),
				EncounterCategory.RaidEncounterSecretsOfTheObscure => imageProvider.GetTinySecretsOfTheObscureIcon(),
				EncounterCategory.RaidEncounterVisionsOfEternity => imageProvider.GetTinyVisionsOfEternityIcon(),
				EncounterCategory.RaidEncounterFestival => imageProvider.GetTinyFestivalIcon(),
				EncounterCategory.SpecialForcesTrainingArea => imageProvider.GetTinyTrainingAreaIcon(),
				EncounterCategory.RaidWing1 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing2 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing3 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing4 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing5 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing6 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing7 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing8 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.FractalNightmare => imageProvider.GetFractalMapIcon(),
				EncounterCategory.FractalShatteredObservatory => imageProvider.GetFractalMapIcon(),
				EncounterCategory.FractalSunquaPeak => imageProvider.GetFractalMapIcon(),
				EncounterCategory.FractalSilentSurf => imageProvider.GetFractalMapIcon(),
				EncounterCategory.FractalLonelyTower => imageProvider.GetFractalMapIcon(),
				EncounterCategory.FractalKinfall => imageProvider.GetFractalMapIcon(),
				EncounterCategory.Map => imageProvider.GetTinyInstanceIcon(),
				_ => null
			};
		}
	}
}
