using System;

namespace GW2Scratch.EVTCAnalytics.Model
{
	/// <summary>
	/// Represents a time instant with both the current time as seen on the computer used to play the game
	/// and the time provided by the game server.
	/// </summary>
	public class LogTime
	{
		/// <summary>
		/// Time on the client. May differ from <see cref="ServerTime"/>, depending on latency.
		/// </summary>
		/// <remarks>
		/// This time may be significantly desynchronized as it depends on the configuration of the computer used to play the game.
		/// </remarks>
        public DateTimeOffset LocalTime { get; }

		/// <summary>
		/// Time on the server. May differ from <see cref="LocalTime"/>, depending on latency.
		/// </summary>
        public DateTimeOffset ServerTime { get; }
		
		/// <summary>
		/// The system time in milliseconds.
		/// </summary>
		public long TimeMilliseconds { get; }

		/// <summary>
		/// Creates a new instance of <see cref="LogTime"/>.
		/// </summary>
		public LogTime(DateTimeOffset localTime, DateTimeOffset serverTime, long timeMilliseconds)
		{
			LocalTime = localTime;
			ServerTime = serverTime;
			TimeMilliseconds = timeMilliseconds;
		}
	}
}