using System;

namespace GW2Scratch.EVTCAnalytics.Exceptions
{
	/// <summary>
	/// The exception thrown when EVTC log processing fails.
	/// </summary>
	public class LogProcessingException : Exception
	{
		public LogProcessingException()
		{
		}

		public LogProcessingException(string message) : base(message)
		{
		}

		public LogProcessingException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}