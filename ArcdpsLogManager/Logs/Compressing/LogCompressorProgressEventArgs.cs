using System;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Logs.Compressing
{
	public class LogCompressorProgressEventArgs : EventArgs
	{
		public int Current;
		public int Total;
		public LogData LogData;

		public LogCompressorProgressEventArgs(int current, int total, LogData logData) {
			Current = current;
			Total = total;
			LogData = logData;
		}
	}
}
