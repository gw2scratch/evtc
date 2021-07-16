using System;

namespace GW2Scratch.EVTCAnalytics.Exceptions
{
	/// <summary>
	/// The exception thrown when EVTC log parsing fails.
	/// </summary>
	public class LogParsingException : Exception
	{
		public LogParsingException()
		{
		}

		public LogParsingException(string message) : base(message)
		{
		}

		public LogParsingException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}