using System;

namespace ScratchEVTCParser.Model
{
	public class LogTime
	{
		/// <summary>
		/// Time on the client. May differ from <see cref="ServerTime"/>, depending on latency.
		/// </summary>
        public DateTimeOffset LocalTime { get; }

		/// <summary>
		/// Time on the server. May differ from <see cref="LocalTime"/>, depending on latency.
		/// </summary>
        public DateTimeOffset ServerTime { get; }
		
		public long TimeMilliseconds { get; }

		public LogTime(DateTimeOffset localTime, DateTimeOffset serverTime, long timeMilliseconds)
		{
			LocalTime = localTime;
			ServerTime = serverTime;
			TimeMilliseconds = timeMilliseconds;
		}
	}
}