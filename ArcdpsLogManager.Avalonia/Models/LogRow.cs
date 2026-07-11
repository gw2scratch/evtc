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
	/// <remarks>
	/// All displayed fields are observable and can be recomputed in place via <see cref="Refresh"/>
	/// (e.g. after a reparse) without replacing this object — the grid's row/cell selection is tied
	/// to LogRow identity, so keeping the same instance around is what lets a reparse update the
	/// row's contents without dropping the user's selection.
	/// </remarks>
	public sealed partial class LogRow : ObservableObject
	{
		private readonly ImageProvider images;
		private readonly ILogNameProvider nameProvider;

		/// <summary>The underlying log data (for the detail panel, favorite toggling, deletion, etc.).</summary>
		public LogData Log { get; }

		/// <summary>Observable so the ★ cell refreshes when toggled.</summary>
		[ObservableProperty] private bool favorite;

		[ObservableProperty] private string encounter = "";

		/// <summary>Observable so it can be recomputed live when
		/// Settings.ShowFailurePercentagesInLogList changes, without needing a full reload.</summary>
		[ObservableProperty] private string result = "";

		[ObservableProperty] private string mode = "";
		[ObservableProperty] private int players;
		[ObservableProperty] private string duration = "";
		[ObservableProperty] private DateTimeOffset date;
		[ObservableProperty] private string dateText = "";
		[ObservableProperty] private string character = "";
		[ObservableProperty] private string account = "";
		[ObservableProperty] private string mapId = "";
		[ObservableProperty] private string gameVersion = "";
		[ObservableProperty] private string evtcVersion = "";
		[ObservableProperty] private string fractalScale = "";

		/// <summary>The encounter icon (with WvW / instance fallbacks), or null if none applies.</summary>
		[ObservableProperty] private Bitmap? encounterIcon;

		/// <summary>
		/// One profession/specialization icon per identified player, forming the composition column.
		/// Pre-resolved (and bitmap-cached in the shared <see cref="ImageProvider"/>) to avoid
		/// per-render lookups — this column is the plan's #1 render-cost risk.
		/// </summary>
		[ObservableProperty] private IReadOnlyList<Bitmap> composition = Array.Empty<Bitmap>();

		/// <summary>Fractal mistlock instability icons for the row, ordered by name.</summary>
		[ObservableProperty] private IReadOnlyList<Bitmap> instabilities = Array.Empty<Bitmap>();

		public LogRow(LogData log, ImageProvider images, ILogNameProvider nameProvider)
		{
			Log = log;
			this.images = images;
			this.nameProvider = nameProvider;
			Refresh();
		}

		/// <summary>
		/// Recomputes every displayed field from the current state of <see cref="Log"/>, in place.
		/// Called on initial construction, and again after a background reparse finishes so the row
		/// reflects the new data without being replaced (which would drop the grid's selection).
		/// </summary>
		public void Refresh()
		{
			var log = Log;

			Favorite = log.IsFavorite;
			Encounter = log.ParsingStatus is ParsingStatus.Unparsed or ParsingStatus.Parsing
				? "Processing..."
				: nameProvider.GetName(log);

			Result = ComputeResultText(log, Settings.ShowFailurePercentagesInLogList);

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

			var playerList = log.Players?.ToList() ?? new List<LogPlayer>();
			Players = playerList.Count;
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

			Composition = playerList
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

		/// <summary>
		/// Immediately shows a "Processing..." placeholder without waiting for the background
		/// processor to actually pick up the job (it may be queued behind other scheduled logs) —
		/// used right after scheduling a reparse so the row visibly changes at once.
		/// </summary>
		public void MarkProcessing()
		{
			Encounter = "Processing...";
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
