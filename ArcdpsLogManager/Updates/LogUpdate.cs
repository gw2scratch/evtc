using System;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;

namespace GW2Scratch.ArcdpsLogManager.Updates
{
	public class LogUpdate
	{
		private class FuncLogFilter : ILogFilter
		{
			private readonly Func<LogData, bool> filter;

			public FuncLogFilter(Func<LogData, bool> filter)
			{
				this.filter = filter;
			}

			public bool FilterLog(LogData log)
			{
				return filter(log);
			}
		}

		public ILogFilter Filter { get; }
		public string Reason { get; }

		public LogUpdate(ILogFilter filter, string reason)
		{
			Filter = filter;
			Reason = reason;
		}

		public LogUpdate(Func<LogData, bool> filter, string reason)
			: this(new FuncLogFilter(filter), reason)
		{
		}
	}
}