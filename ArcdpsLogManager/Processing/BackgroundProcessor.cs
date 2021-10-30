using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GW2Scratch.ArcdpsLogManager.Processing
{
	public abstract class BackgroundProcessor<T> : IBackgroundProcessor
	{
		private readonly HashSet<T> queueSet = new HashSet<T>();
		private readonly Queue<T> processingQueue = new Queue<T>();
		private readonly HashSet<T> currentlyProcessing = new HashSet<T>();
		private Task backgroundTask = Task.CompletedTask;
		private CancellationTokenSource taskCancellation;

		private readonly object queueLock = new object();
		private readonly object taskLock = new object();

		public TimeSpan PauseWhenQueueEmpty { get; set; } = TimeSpan.FromSeconds(1);
		public bool BackgroundTaskRunning => !backgroundTask.IsCompleted;

		private int processedItemCount = 0;
		private int totalScheduledCount = 0;
		public int ProcessedItemCount => processedItemCount;
		public int TotalScheduledCount => totalScheduledCount;

		public int MaxConcurrentProcesses { get; } = 1;

		/// <summary>
		/// Start the background processing task, there may only be one running at a time.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the task is running already.</exception>
		public void StartBackgroundTask()
		{
			lock (taskLock)
			{
				if (!backgroundTask.IsCompleted)
				{
					throw new InvalidOperationException("The background task is running already");
				}

				taskCancellation = new CancellationTokenSource();
				backgroundTask = Task.Factory
					.StartNew(() => ProcessQueue(taskCancellation.Token), TaskCreationOptions.LongRunning)
					.Unwrap();
				Starting?.Invoke(this, EventArgs.Empty);
				backgroundTask.ContinueWith(HandleStoppingBackgroundTask);
			}
		}

		private void HandleStoppingBackgroundTask(Task task)
		{
			if (task.IsFaulted)
			{
				StoppingWithError?.Invoke(this, new BackgroundProcessorErrorEventArgs(task.Exception));
			}
		}

		private async Task ProcessQueue(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				cancellationToken.ThrowIfCancellationRequested();
				{
					T item = default(T);
					bool dequeued = false;

					lock (queueLock)
					{
						if (processingQueue.Count > 0)
						{
							item = processingQueue.Dequeue();
							queueSet.Remove(item);
							currentlyProcessing.Add(item);
							dequeued = true;
						}
					}

					if (dequeued)
					{
						try
						{
							await Process(item, cancellationToken);
						}
						finally
						{
							lock (queueLock)
							{
								currentlyProcessing.Remove(item);
							}
						}

						Interlocked.Increment(ref processedItemCount);
						Processed?.Invoke(this, BuildEventArgs());
					}
					else
					{
						await Task.Delay(PauseWhenQueueEmpty, cancellationToken);
					}
				}
			}

			cancellationToken.ThrowIfCancellationRequested();
		}

		/// <summary>
		/// Checks whether an item is currently scheduled for processing.
		/// </summary>
		/// <returns>A value indicating whether the <paramref name="item"/> is scheduled.</returns>
		public bool IsScheduled(T item)
		{
			lock (queueLock)
			{
				return queueSet.Contains(item);
			}
		}

		/// <summary>
		/// Checks whether an item is currently being processed.
		/// </summary>
		/// <returns>A value indicating whether the <paramref name="item"/> is currently being processed.</returns>
		public bool IsBeingProcessed(T item)
		{
			lock (queueLock)
			{
				return currentlyProcessing.Contains(item);
			}
		}

		/// <summary>
		/// Checks whether an item is currently scheduled or being processed.
		/// </summary>
		/// <remarks>This check is performed atomically unlike chaining <see cref="IsScheduled"/>
		/// and <see cref="IsBeingProcessed"/> would be.</remarks>
		/// <returns>A value indicating whether the <paramref name="item"/> is scheduled or being processed.</returns>
		public bool IsScheduledOrBeingProcessed(T item)
		{
			lock (queueLock)
			{
				return queueSet.Contains(item) || currentlyProcessing.Contains(item);
			}
		}

		/// <summary>
		/// Calculates the size of the current queue.
		/// </summary>
		/// <returns>The count of currently scheduled items.</returns>
		public int GetScheduledItemCount()
		{
			lock (queueLock)
			{
				return queueSet.Count;
			}
		}

		/// <summary>
		/// Stop the background task. Might not stop instantly. Does nothing if the task is not running.
		/// </summary>
		public void StopBackgroundTask()
		{
			bool stopped = false;
			if (taskCancellation != null)
			{
				if (!taskCancellation.IsCancellationRequested)
				{
					taskCancellation.Cancel();
					stopped = true;
				}
			}

			if (stopped)
			{
				Stopping?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Schedule an item for processing, potentially also starting the background process if it is not running.
		/// </summary>
		/// <remarks>If an item is already scheduled, it won't be added again, but the background task will still
		/// start if <paramref name="startProcess"/> is true.</remarks>
		/// <param name="item">An item that will be scheduled.</param>
		/// <param name="startProcess">A value indicating whether the background should be started if it is not running already.</param>
		public void Schedule(T item, bool startProcess = true)
		{
			bool scheduled = false;
			lock (queueLock)
			{
				if (queueSet.Add(item))
				{
					processingQueue.Enqueue(item);
					scheduled = true;
				}
			}

			if (scheduled)
			{
				Interlocked.Increment(ref totalScheduledCount);
				Scheduled?.Invoke(this, BuildEventArgs());
			}

			if (startProcess && !BackgroundTaskRunning)
			{
				StartBackgroundTask();
			}
		}

		/// <summary>
		/// Unschedule provided items for processing.
		/// </summary>
		/// <remarks>If a provided item is not scheduled, it will be ignored.</remarks>
		/// <param name="items">Items that will be unscheduled for processing.</param>
		public void Unschedule(params T[] items)
		{
			Unschedule(items.Contains);
		}

		/// <summary>
		/// Unschedule selected items for processing and apply an action to each unscheduled item.
		/// </summary>
		/// <param name="itemSelector">A function to test each element for removal.</param>
		/// <param name="processUnscheduled">An optional action to execute on each unscheduled item.</param>
		public void Unschedule(Func<T, bool> itemSelector, Action<T> processUnscheduled = null)
		{
			if (itemSelector == null) throw new ArgumentNullException(nameof(itemSelector));

			T[] unscheduledItems;

			lock (queueLock)
			{
				var keptItems = processingQueue.Where(x => !itemSelector(x)).ToArray();
				unscheduledItems = processingQueue.Where(itemSelector).ToArray();

				processingQueue.Clear();
				queueSet.Clear();
				foreach (var item in keptItems)
				{
					processingQueue.Enqueue(item);
					queueSet.Add(item);
				}
			}

			if (processUnscheduled != null)
			{
				foreach (var item in unscheduledItems)
				{
					processUnscheduled(item);
				}
			}

			if (unscheduledItems.Length > 0)
			{
				Unscheduled?.Invoke(this, BuildEventArgs());
			}
		}

		/// <summary>
		/// Unschedule all scheduled items. Does not affect the currently processed item.
		/// </summary>
		/// <param name="processUnscheduled">An optional action to execute on each unscheduled item.</param>
		public void UnscheduleAll(Action<T> processUnscheduled = null)
		{
			Unschedule(_ => true, processUnscheduled);
		}

		/// <summary>
		/// Resets <see cref="TotalScheduledCount"/> and <see cref="ProcessedItemCount"/> to 0.
		/// </summary>
		public void ResetTotalCounters()
		{
			Interlocked.Exchange(ref totalScheduledCount, 0);
			Interlocked.Exchange(ref processedItemCount, 0);
		}

		/// <summary>
		/// Process an item after being dequeued.
		/// </summary>
		/// <param name="item">An item to process.</param>
		/// <param name="cancellationToken">A cancellation token.</param>
		protected abstract Task Process(T item, CancellationToken cancellationToken);

		/// <summary>
		/// Invoked when an item has been processed.
		/// </summary>
		public event EventHandler<BackgroundProcessorEventArgs> Processed;

		/// <summary>
		/// Invoked when a new item has been scheduled.
		/// </summary>
		public event EventHandler<BackgroundProcessorEventArgs> Scheduled;

		/// <summary>
		/// Invoked when an item has been unscheduled.
		/// </summary>
		public event EventHandler<BackgroundProcessorEventArgs> Unscheduled;

		/// <summary>
		/// Invoked when the background thread is starting.
		/// </summary>
		public event EventHandler<EventArgs> Starting;

		/// <summary>
		/// Invoked when the background thread is stopping.
		/// </summary>
		public event EventHandler<EventArgs> Stopping;
		
		/// <summary>
		/// Invoked when the background thread stopped with an exception.
		/// </summary>
		public event EventHandler<BackgroundProcessorErrorEventArgs> StoppingWithError;

		/// <summary>
		/// Creates event args, locks.
		/// </summary>
		private BackgroundProcessorEventArgs BuildEventArgs()
		{
			return new BackgroundProcessorEventArgs(GetScheduledItemCount(), ProcessedItemCount, TotalScheduledCount);
		}
	}
}