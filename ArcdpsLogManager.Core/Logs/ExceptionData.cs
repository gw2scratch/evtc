using System;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Logs
{
	/// <summary>
	/// A summary of exception data. This is used instead of exceptions for serialization.
	/// </summary>
	public class ExceptionData
	{
		public string ExceptionName { get; }
		public string Message { get; }
		public string StackTrace { get; }
		public string Source { get; }
		public ExceptionData InnerExceptionData { get; }

		public ExceptionData(Exception exception)
		{
			ExceptionName = exception.GetType().FullName;
			Message = exception.Message;
			StackTrace = exception.StackTrace;
			Source = exception.Source;
			if (exception.InnerException != null)
			{
				InnerExceptionData = new ExceptionData(exception.InnerException);
			}
		}

		[JsonConstructor]
		public ExceptionData(string exceptionName, string message, string stackTrace, string source, ExceptionData innerExceptionData)
		{
			ExceptionName = exceptionName;
			Message = message;
			StackTrace = stackTrace;
			Source = source;
			InnerExceptionData = innerExceptionData;
		}
	}
}