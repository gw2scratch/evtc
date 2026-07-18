using System;

namespace GW2Scratch.ArcdpsLogManager.Processing
{
	public class BackgroundProcessorErrorEventArgs : EventArgs
	{
		public Exception Exception { get; }
		
		public BackgroundProcessorErrorEventArgs(Exception exception)
		{
			Exception = exception;
		}
	}
}