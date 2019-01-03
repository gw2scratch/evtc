using System;

namespace ScratchEVTCParser.Exceptions
{
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