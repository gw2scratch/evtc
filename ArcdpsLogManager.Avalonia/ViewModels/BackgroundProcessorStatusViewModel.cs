using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Processing;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// Live status display for a single <see cref="IBackgroundProcessor{T}"/> (Avalonia counterpart
	/// of the Eto <c>Controls/BackgroundProcessorDetail.cs</c>). Kept non-generic (unlike the Eto
	/// control) so the Services debug section can hold a plain, uniformly-typed list/set of these
	/// regardless of which processor's item type (<c>LogData</c>, <c>string</c>, ...) backs each one;
	/// <see cref="Attach{T}"/> subscribes to whichever processor is supplied.
	/// </summary>
	public partial class BackgroundProcessorStatusViewModel : ObservableObject
	{
		public string Name { get; }

		[ObservableProperty] private bool isAvailable;
		[ObservableProperty] private string statusText = "Not available";
		[ObservableProperty] private int queuedCount;
		[ObservableProperty] private int processedCount;
		[ObservableProperty] private int totalQueuedCount;
		[ObservableProperty] private int maxConcurrentProcesses;

		public BackgroundProcessorStatusViewModel(string name)
		{
			Name = name;
		}

		/// <summary>Subscribes to the given processor's status-changing events, updating the
		/// displayed fields on the UI thread (mirrors the Eto control's
		/// <c>Application.Instance.AsyncInvoke</c> marshalling).</summary>
		public void Attach<T>(IBackgroundProcessor<T> processor)
		{
			IsAvailable = true;

			void Update()
			{
				StatusText = processor.BackgroundTaskRunning ? "Running" : "Stopped";
				QueuedCount = processor.GetScheduledItemCount();
				ProcessedCount = processor.ProcessedItemCount;
				TotalQueuedCount = processor.TotalScheduledCount;
				MaxConcurrentProcesses = processor.MaxConcurrentProcesses;
			}

			processor.Starting += (_, _) => Dispatcher.UIThread.Post(Update);
			processor.Stopping += (_, _) => Dispatcher.UIThread.Post(Update);
			processor.StoppingWithError += (_, _) => Dispatcher.UIThread.Post(Update);
			processor.Scheduled += (_, _) => Dispatcher.UIThread.Post(Update);
			processor.Unscheduled += (_, _) => Dispatcher.UIThread.Post(Update);
			processor.Processed += (_, _) => Dispatcher.UIThread.Post(Update);

			Update();
		}
	}
}
