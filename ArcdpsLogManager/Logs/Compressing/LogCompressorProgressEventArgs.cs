using System;

namespace GW2Scratch.ArcdpsLogManager.Logs.Compressing
{
	public class LogCompressorProgressEventArgs : EventArgs
	{
		public int current;
		public int total;

		public LogCompressorProgressEventArgs(int current, int total) {
			this.current = current;
			this.total = total;
		}
	}
}
