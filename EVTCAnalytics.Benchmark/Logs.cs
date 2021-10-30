using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;

namespace GW2Scratch.EVTCAnalytics.Benchmark
{
	public static class Logs
	{
		public static List<byte[]> LoadEmbeddedLogs()
		{
			var logs = new List<byte[]>();
			foreach (var resourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
			{
				if (resourceName.EndsWith(".zevtc"))
				{
					var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
					using var arch = new ZipArchive(stream, ZipArchiveMode.Read);

					if (arch.Entries.Count == 0)
					{
						throw new Exception("No EVTC file in ZIP archive.");
					}

					using var data = arch.Entries[0].Open();

					var bytes = new byte[arch.Entries[0].Length];
					int read;
					int offset = 0;
					while ((read = data.Read(bytes, offset, bytes.Length - offset)) > 0)
					{
						offset += read;
					}

					logs.Add(bytes);
				}
			}

			if (logs.Count == 0)
			{
				throw new Exception("No logs found. Make sure that you have a logs directory next to the .csproj file when compiling, and only use .zevtc logs.");
			}

			return logs;
		}
	}
}