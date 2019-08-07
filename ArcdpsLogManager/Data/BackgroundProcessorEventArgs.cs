using System;

namespace GW2Scratch.ArcdpsLogManager.Data
{
	public class BackgroundProcessorEventArgs : EventArgs
	{
		public int TotalProcessedItems { get; }
		public int TotalScheduledItems { get; }
		public int CurrentScheduledItems { get; }

		public BackgroundProcessorEventArgs(int currentScheduledItems, int totalProcessedItems, int totalScheduledItems)
		{
			CurrentScheduledItems = currentScheduledItems;
			TotalProcessedItems = totalProcessedItems;
			TotalScheduledItems = totalScheduledItems;
		}
	}
}