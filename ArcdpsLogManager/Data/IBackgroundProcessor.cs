using System;

namespace GW2Scratch.ArcdpsLogManager.Data
{
	public interface IBackgroundProcessor
	{
		TimeSpan PauseWhenQueueEmpty { get; set; }
		bool BackgroundTaskRunning { get; }
		int ProcessedItemCount { get; }
		int TotalScheduledCount { get; }
		int MaxConcurrentProcesses { get; }

		/// <summary>
		/// Start the background processing task, there may only be one running at a time.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the task is running already.</exception>
		void StartBackgroundTask();

		/// <summary>
		/// Calculates the size of the current queue.
		/// </summary>
		/// <returns>The count of currently scheduled items.</returns>
		int GetScheduledItemCount();

		/// <summary>
		/// Stop the background task. Might not stop instantly. Does nothing if the task is not running.
		/// </summary>
		void StopBackgroundTask();

		/// <summary>
		/// Invoked when an item has been processed.
		/// </summary>
		event EventHandler<BackgroundProcessorEventArgs> Processed;

		/// <summary>
		/// Invoked when a new item has been scheduled.
		/// </summary>
		event EventHandler<BackgroundProcessorEventArgs> Scheduled;

		/// <summary>
		/// Invoked when an item has been unscheduled.
		/// </summary>
		event EventHandler<BackgroundProcessorEventArgs> Unscheduled;

		/// <summary>
		/// Invoked when the background thread is starting.
		/// </summary>
		event EventHandler<EventArgs> Starting;

		/// <summary>
		/// Invoked when the background thread is stopping.
		/// </summary>
		event EventHandler<EventArgs> Stopping;
	}
}