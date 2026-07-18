using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Analytics;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Uploads;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Processing;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using Gw2Sharp;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Services
{
	/// <summary>
	/// Discovers logs on the filesystem and processes unparsed ones in the background — the
	/// Avalonia counterpart of the discovery/processing wiring in the Eto <c>ManagerForm</c>.
	/// Uses Core's <see cref="LogFinder"/>, <see cref="LogDataProcessor"/> and <see cref="ApiProcessor"/>.
	/// </summary>
	public sealed class LogProcessingService : IDisposable
	{
		private readonly LogFinder logFinder = new LogFinder();
		private readonly LogAnalytics analytics = new LogAnalytics(
			new EVTCParser
			{
				SinglePassFilteringOptions =
				{
					PruneForEncounterData = true,
					ExtraRequiredEventTypes = new[] { typeof(AgentMarkerEvent) },
				},
			},
			new LogProcessor(),
			new FractalInstabilityDetector(),
			log => new LogAnalyzer(log));

		private readonly LogCache cache;
		private readonly ApiData apiData;
		private readonly ApiProcessor apiProcessor;
		private readonly LogDataProcessor logDataProcessor;
		private readonly DpsReportUploader dpsReportUploader;
		private readonly UploadProcessor uploadProcessor;
		private readonly EventHandler<EventArgs> useApiChangedHandler;
		private readonly EventHandler<EventArgs> dpsReportDomainChangedHandler;
		private readonly EventHandler<EventArgs> dpsReportUploadDetailedWvwChangedHandler;

		/// <summary>Raised (on a background thread) when a scheduled log finishes processing.</summary>
		public event EventHandler<BackgroundProcessorEventArgs<LogData>>? LogProcessed
		{
			add => logDataProcessor.Processed += value;
			remove => logDataProcessor.Processed -= value;
		}

		/// <summary>
		/// Raised (on a background thread) when the API processor drains its queue, i.e. new guild
		/// data has been fetched and persisted. UI subscribers must marshal to the UI thread.
		/// </summary>
		public event EventHandler? GuildDataUpdated;

		public int ScheduledCount => logDataProcessor.GetScheduledItemCount();

		/// <summary>Exposed for the Services debug section's live background-processor status display.</summary>
		public ApiProcessor ApiProcessor => apiProcessor;

		/// <summary>Exposed for the Services debug section's live background-processor status display.</summary>
		public LogDataProcessor LogDataProcessor => logDataProcessor;

		/// <summary>Exposed for the Services debug section's live background-processor status display
		/// and for the single-/multi-log detail panels to schedule/cancel dps.report uploads.</summary>
		public UploadProcessor UploadProcessor => uploadProcessor;

		public LogProcessingService(LogCache cache, ApiData apiData)
		{
			this.cache = cache;
			this.apiData = apiData;
			apiProcessor = new ApiProcessor(apiData, new Gw2Client());
			logDataProcessor = new LogDataProcessor(cache, apiProcessor, analytics);

			dpsReportUploader = new DpsReportUploader(Settings.DpsReportUploadDetailedWvw);
			uploadProcessor = new UploadProcessor(dpsReportUploader, cache);

			// Persist API data and notify subscribers whenever the API queue drains, matching the
			// Eto ManagerForm behaviour.
			apiProcessor.Processed += (_, args) =>
			{
				if (args.CurrentScheduledItems == 0)
				{
					apiData.SaveDataToFile();
					GuildDataUpdated?.Invoke(this, EventArgs.Empty);
				}
			};

			// Fetch guild/map data from the GW2 API only while the setting is enabled, exactly as
			// the Eto ManagerForm does.
			if (Settings.UseGW2Api)
			{
				apiProcessor.StartBackgroundTask();
			}

			useApiChangedHandler = (_, _) =>
			{
				if (Settings.UseGW2Api)
				{
					if (!apiProcessor.BackgroundTaskRunning)
					{
						apiProcessor.StartBackgroundTask();
					}
				}
				else
				{
					apiProcessor.StopBackgroundTask();
				}
			};
			Settings.UseGW2ApiChanged += useApiChangedHandler;

			// Keep the uploader's domain/detailed-WvW options in sync with settings changes, exactly
			// as the Eto ManagerForm does.
			dpsReportDomainChangedHandler = (_, _) => dpsReportUploader.Domain = Settings.DpsReportDomain;
			dpsReportUploadDetailedWvwChangedHandler = (_, _) =>
				dpsReportUploader.UploadDetailedWvw = Settings.DpsReportUploadDetailedWvw;
			Settings.DpsReportDomainChanged += dpsReportDomainChangedHandler;
			Settings.DpsReportUploadDetailedWvwChanged += dpsReportUploadDetailedWvwChangedHandler;

			// Auto-upload newly processed logs to dps.report, exactly as the Eto ManagerForm's
			// LogDataProcessor.Processed handler does.
			logDataProcessor.Processed += (_, args) =>
			{
				if (Settings.DpsReportAutoUpload)
				{
					bool filtersMatch = true;
					if (Settings.DpsReportAutoUploadApplyFilters)
					{
						filtersMatch = args.Item.EncounterResult == EncounterResult.Success;
					}

					var age = DateTimeOffset.Now - args.Item.EncounterStartTime;
					if (age < TimeSpan.FromDays(1) && filtersMatch)
					{
						uploadProcessor.ScheduleDpsReportEIUpload(args.Item);
					}
				}
			};
		}

		/// <summary>
		/// Enumerates all logs found under the given root paths (using cached data where available)
		/// and schedules any unparsed logs for background processing. Returns the full set of logs.
		/// </summary>
		public IReadOnlyList<LogData> DiscoverAndSchedule(IReadOnlyList<string> rootPaths)
		{
			var found = new List<LogData>();

			foreach (var log in rootPaths.SelectMany(path => SafeGetFromDirectory(path)))
			{
				found.Add(log);

				if (log.ParsingStatus == ParsingStatus.Parsed)
				{
					apiProcessor.RegisterLog(log);
				}
				else
				{
					logDataProcessor.Schedule(log);
				}
			}

			return found;
		}

		/// <summary>
		/// Schedules a single already-known log for reprocessing (used by the "logs have outdated
		/// data" dialog). Reuses the same background processor and <see cref="LogProcessed"/>
		/// event as initial discovery, so the result flows back through the existing incremental
		/// refresh path.
		/// </summary>
		public void ScheduleReprocessing(LogData log)
		{
			logDataProcessor.Schedule(log);
		}

		/// <summary>
		/// Looks up (or creates) the <see cref="LogData"/> for a single file noticed by a filesystem
		/// watcher after the initial scan, and schedules it for processing if unparsed — the Avalonia
		/// counterpart of the Eto <c>ManagerForm.AddNewLog</c>. Does not check whether the file is
		/// already known to the caller; callers should dedupe against their own displayed list first.
		/// </summary>
		public LogData RegisterDiscoveredLog(string fullName)
		{
			if (!cache.TryGetLogData(fullName, out var log))
			{
				log = new LogData(fullName);
			}

			if (log.ParsingStatus != ParsingStatus.Parsed)
			{
				logDataProcessor.Schedule(log);
			}

			return log;
		}

		private IEnumerable<LogData> SafeGetFromDirectory(string path)
		{
			try
			{
				return logFinder.GetFromDirectory(path, cache).ToList();
			}
			catch
			{
				// A missing/inaccessible directory should not abort discovery of the others.
				return Enumerable.Empty<LogData>();
			}
		}

		public void Dispose()
		{
			Settings.UseGW2ApiChanged -= useApiChangedHandler;
			Settings.DpsReportDomainChanged -= dpsReportDomainChangedHandler;
			Settings.DpsReportUploadDetailedWvwChanged -= dpsReportUploadDetailedWvwChangedHandler;
			logDataProcessor.StopBackgroundTask();
			apiProcessor.StopBackgroundTask();
			uploadProcessor.StopBackgroundTask();
			dpsReportUploader.Dispose();

			// Best-effort final persist of any fetched API data.
			try
			{
				apiData.SaveDataToFile();
			}
			catch
			{
				// Never throw from Dispose.
			}
		}
	}
}
