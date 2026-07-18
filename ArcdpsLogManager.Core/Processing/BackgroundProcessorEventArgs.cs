using System;

namespace GW2Scratch.ArcdpsLogManager.Processing
{
	public class BackgroundProcessorEventArgs<T>(T item, int currentScheduledItems, int totalProcessedItems, int totalScheduledItems) : EventArgs
	{
		public int TotalProcessedItems { get; } = totalProcessedItems;
		public int TotalScheduledItems { get; } = totalScheduledItems;
		public int CurrentScheduledItems { get; } = currentScheduledItems;
		public T Item { get; } = item;
	}
}