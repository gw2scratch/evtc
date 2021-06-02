using System;

namespace GW2Scratch.EVTCAnalytics.Exceptions
{
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