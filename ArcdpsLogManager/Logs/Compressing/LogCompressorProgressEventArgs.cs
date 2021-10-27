using System;

namespace GW2Scratch.ArcdpsLogManager.Logs.Compressing
{
	public class LogCompressorProgressEventArgs : EventArgs
	{
		public int Current { get; }
		public int Total { get; }
		public LogData LogData { get; }

		public LogCompressorProgressEventArgs(int current, int total, LogData logData) {
			Current = current;
			Total = total;
			LogData = logData;
		}
	}
}
