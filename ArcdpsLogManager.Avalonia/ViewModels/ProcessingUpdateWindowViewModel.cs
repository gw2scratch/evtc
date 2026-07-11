using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Logs.Updates;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the "logs have outdated processed data" dialog (Avalonia counterpart of the
	/// Eto <c>ProcessingUpdateDialog</c>). Reuses the existing background-processing/log-reload
	/// path (<see cref="LogProcessingService"/> + <see cref="MainWindowViewModel"/>'s incremental
	/// refresh) instead of any new reload mechanism: scheduling a log here makes it flow back
	/// through the same <c>LogProcessed</c> event the initial discovery uses.
	/// </summary>
	public class ProcessingUpdateWindowViewModel
	{
		private readonly IReadOnlyList<LogUpdateList> updates;
		private readonly LogProcessingService processingService;

		public IReadOnlyList<LogUpdateRow> Rows { get; }

		public ProcessingUpdateWindowViewModel(IReadOnlyList<LogUpdateList> updates, LogProcessingService processingService)
		{
			this.updates = updates;
			this.processingService = processingService;
			Rows = updates.Select(u => new LogUpdateRow(u)).ToList();
		}

		/// <summary>Schedules every affected log for reprocessing in the background.</summary>
		public void ScheduleReprocessing()
		{
			Task.Run(() =>
			{
				foreach (var update in updates)
				{
					foreach (var log in update.UpdateableLogs)
					{
						processingService.ScheduleReprocessing(log);
					}
				}
			});
		}
	}
}
