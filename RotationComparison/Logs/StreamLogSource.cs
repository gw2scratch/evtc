using System;
using System.IO;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Processing;

namespace GW2Scratch.RotationComparison.Logs
{
	public class StreamLogSource : ScratchParserLogSource, IDisposable
	{
		private readonly Stream stream;
		private readonly string logName;
		private Log processedLog;

		public StreamLogSource(Stream stream, string logName)
		{
			this.stream = stream;
			this.logName = logName;
		}

		protected override Log GetLog()
		{
			byte[] bytes;
			using (var memoryStream = new MemoryStream())
			{
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
			}

			if (processedLog == null)
			{
				var parser = new EVTCParser();
				var processor = new LogProcessor();
				var parsedLog = parser.ParseLog(bytes);
				processedLog = processor.ProcessLog(parsedLog);
			}

			return processedLog;
		}

		public override string GetLogName()
		{
			return logName;
		}

		public void Dispose()
		{
			stream?.Dispose();
		}
	}
}