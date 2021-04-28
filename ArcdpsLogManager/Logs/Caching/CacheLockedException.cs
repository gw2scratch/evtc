using System;
using System.Runtime.Serialization;

namespace GW2Scratch.ArcdpsLogManager.Logs.Caching
{
	public class CacheLockedException : Exception
	{
		public CacheLockedException()
		{
		}

		protected CacheLockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public CacheLockedException(string message) : base(message)
		{
		}

		public CacheLockedException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}