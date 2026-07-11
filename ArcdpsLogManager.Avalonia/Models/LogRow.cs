using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using EncounterEnum = GW2Scratch.EVTCAnalytics.GameData.Encounters.Encounter;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// A flattened, display-ready projection of <see cref="LogData"/> for the log grid.
	/// Formatted values and pre-resolved icon <see cref="Bitmap"/>s are computed once here so the
	/// grid cells can bind directly, keeping scrolling/virtualization cheap (no per-render work).
	/// Mirrors the columns produced by the Eto <c>Sections/LogList.cs</c>.
	/// </summary>
	public sealed partial class LogRow : ObservableObject
	{
		/// <summary>The underlying log data (for the detail panel, favorite toggling, deletion, etc.).</summary>
		public LogData Log { get; }

		/// <summary>Observable so the ★ cell refreshes when toggled.</summary>
		[ObservableProperty] private bool favorite;

		public string Encounter { get; }

		/// <summary>Observable so it can be recomputed live when
		/// Settings.ShowFailurePercentagesInLogList changes, without needing a full reload.</summary>
		[ObservableProperty] private string result;

		public string Mode { get; }
		public int Players { get; }
		public string Duration { get; }
		public DateTimeOffset Date { get; }
		public string DateText { get; }
		public string Character { get; }
		public string Account { get; }
		public string MapId { get; }
		public string GameVersion { get; }
		public string EvtcVersion { get; }
		public string FractalScale { get; }

		/// <summary>The encounter icon (with WvW / instance fallbacks), or null if none applies.</summary>
		public Bitmap? EncounterIcon { get; }

		/// <summary>
		/// One profession/specialization icon per identified player, forming the composition column.
		/// Pre-resolved (and bitmap-cached in the shared <see cref="ImageProvider"/>) to avoid
		/// per-render lookups — this column is the plan's #1 render-cost risk.
		/// </summary>
		public IReadOnlyList<Bitmap> Composition { get; }

		/// <summary>Fractal mistlock instability icons for the row, ordered by name.</summary>
		public IReadOnlyList<Bitmap> Instabilities { get; }

		public LogRow(LogData log, ImageProvider images, ILogNameProvider nameProvider)
		{
			Log = log;

			favorite = log.IsFavorite;
			Encounter = log.ParsingStatus is ParsingStatus.Unparsed or ParsingStatus.Parsing
				? "Processing..."
				: nameProvider.GetName(log);

			result = ComputeResultText(log, Settings.ShowFailurePercentagesInLogList);

			Mode = log.EncounterMode switch
			{
				EncounterMode.LegendaryChallenge => "LM",
				EncounterMode.Challenge => "CM",
				EncounterMode.Normal => "",
				EncounterMode.Emboldened1 => "E1",
				EncounterMode.Emboldened2 => "E2",
				EncounterMode.Emboldened3 => "E3",
				EncounterMode.Emboldened4 => "E4",
				EncounterMode.Emboldened5 => "E5",
				EncounterMode.Quickplay => "QP",
				EncounterMode.Unknown => "?",
				_ => "",
			};

			var players = log.Players?.ToList() ?? new List<LogPlayer>();
			Players = players.Count;
			Duration = log.ShortDurationString;

			Date = log.EncounterStartTime;
			DateText = log.EncounterStartTime == default
				? "Unknown"
				: (log.IsEncounterStartTimePrecise ? "" : "~")
				  + log.EncounterStartTime.ToLocalTime().DateTime.ToString(CultureInfo.CurrentCulture);

			Character = log.PointOfView?.CharacterName ?? "Unknown";
			Account = log.PointOfView?.AccountName?.TrimStart(':') ?? "Unknown";
			MapId = log.MapId?.ToString() ?? "Unknown";
			GameVersion = log.GameBuild?.ToString() ?? "Unknown";
			EvtcVersion = log.EvtcVersion ?? "";
			FractalScale = log.LogExtras?.FractalExtras?.FractalScale?.ToString() ?? "";

			// Encounter icon with the same fallback chain the Eto grid uses.
			EncounterIcon = images.GetTinyEncounterIcon(log.Encounter)
			                ?? images.GetWvWMapIcon(log.MapId)
			                ?? (log.Encounter == EncounterEnum.Map ? images.GetTinyInstanceIcon() : null);

			Composition = players
				.OrderBy(p => p.Profession)
				.ThenBy(p => p.EliteSpecialization)
				.Select(images.GetTinyProfessionIcon)
				.Where(icon => icon is not null)
				.Select(icon => icon!)
				.ToList();

			Instabilities = (log.LogExtras?.FractalExtras?.MistlockInstabilities ?? Enumerable.Empty<MistlockInstability>())
				.OrderBy(GameNames.GetInstabilityName)
				.Select(images.GetMistlockInstabilityIcon)
				.ToList();
		}

		/// <summary>Recomputes <see cref="Result"/> live when Settings.ShowFailurePercentagesInLogList
		/// changes, instead of requiring a full reload for already-built rows.</summary>
		public void UpdateResultText(bool showFailurePercentages)
		{
			Result = ComputeResultText(Log, showFailurePercentages);
		}

		private static string ComputeResultText(LogData log, bool showFailurePercentages)
		{
			return log.EncounterResult switch
			{
				EncounterResult.Success => "Success",
				EncounterResult.Failure => showFailurePercentages && log.HealthPercentage.HasValue
					? $"Failure ({log.HealthPercentage * 100:0.00}%)"
					: "Failure",
				EncounterResult.Unknown => "Unknown",
				_ => log.EncounterResult.ToString(),
			};
		}
	}
}
