using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Sections.Statistics;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the Statistics section (Avalonia counterpart of the Eto
	/// <c>Sections/StatisticsSection.cs</c>): the Encounters tab (per-encounter counts, times and
	/// success rate) and the Specializations tab (profession + elite-spec player counts).
	/// </summary>
	public partial class StatisticsSectionViewModel : ObservableObject
	{
		private readonly ILogNameProvider nameProvider;
		private readonly ImageProvider images;

		[ObservableProperty] private string countText = "";

		public ObservableCollection<EncounterStatRow> Encounters { get; } = new();
		public ObservableCollection<ProfessionStatRow> Professions { get; } = new();
		public ObservableCollection<SpecStatRow> EliteSpecializations { get; } = new();

		public StatisticsSectionViewModel(ILogNameProvider nameProvider, ImageProvider images)
		{
			this.nameProvider = nameProvider;
			this.images = images;
		}

		public void UpdateFromLogs(IEnumerable<LogData> logs)
		{
			var statsByEncounter = new Dictionary<string, EncounterStats>();
			var professionCounts = new Dictionary<Profession, int>();
			var eliteCounts = new Dictionary<EliteSpecialization, int>();
			int logCount = 0;

			foreach (var log in logs)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed)
				{
					continue;
				}

				string encounterName = nameProvider.GetName(log);
				if (!statsByEncounter.TryGetValue(encounterName, out var stats))
				{
					stats = new EncounterStats(encounterName);
					statsByEncounter[encounterName] = stats;
				}

				if (!stats.LogCountsByResult.ContainsKey(log.EncounterResult))
				{
					stats.LogCountsByResult[log.EncounterResult] = 0;
				}

				if (!stats.TimeSpentByResult.ContainsKey(log.EncounterResult))
				{
					stats.TimeSpentByResult[log.EncounterResult] = TimeSpan.Zero;
				}

				stats.LogCount++;
				stats.LogCountsByResult[log.EncounterResult]++;
				stats.TimeSpentByResult[log.EncounterResult] += log.EncounterDuration;

				foreach (var player in log.Players ?? Enumerable.Empty<LogPlayer>())
				{
					professionCounts.TryGetValue(player.Profession, out int pc);
					professionCounts[player.Profession] = pc + 1;

					if (player.EliteSpecialization != EliteSpecialization.None)
					{
						eliteCounts.TryGetValue(player.EliteSpecialization, out int ec);
						eliteCounts[player.EliteSpecialization] = ec + 1;
					}
				}

				logCount++;
			}

			Encounters.Clear();
			foreach (var row in statsByEncounter.Values
				         .OrderByDescending(s => s.LogCount)
				         .Select(s => new EncounterStatRow(s)))
			{
				Encounters.Add(row);
			}

			CountText = $"{Encounters.Count} distinct encounters";

			Professions.Clear();
			EliteSpecializations.Clear();
			foreach (var entry in ProfessionData.Professions)
			{
				professionCounts.TryGetValue(entry.Profession, out int count);
				Professions.Add(new ProfessionStatRow(
					images.GetTinyProfessionIcon(entry.Profession),
					GameNames.GetName(entry.Profession), count, logCount));

				foreach (var spec in new[] { entry.HoT, entry.PoF, entry.EoD, entry.VoE })
				{
					eliteCounts.TryGetValue(spec, out int specCount);
					EliteSpecializations.Add(new SpecStatRow(
						images.GetTinyProfessionIcon(spec), GameNames.GetName(spec), specCount));
				}
			}
		}
	}
}
