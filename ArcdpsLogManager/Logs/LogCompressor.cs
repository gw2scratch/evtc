using GW2Scratch.ArcdpsLogManager.Logs.Compressing;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace GW2Scratch.ArcdpsLogManager.Logs
{
	public class LogCompressor
	{
		/// <summary>
		/// Starts compressing uncompressed logs in the background.
		/// </summary>
		public void Compress(IEnumerable<LogData> logs)
		{
			List<LogData> uncompressedLogs = new List<LogData>();
			foreach (LogData data in logs)
			{
				if (data.FileInfo.Extension == ".evtc")
				{
					uncompressedLogs.Add(data);
				}
			}

			int counter = 0;
			int length = uncompressedLogs.Count;

			Task.Run(() =>
			{
				foreach (LogData data in uncompressedLogs)
				{
					string extensionlessName = data.FileName.Substring(0, data.FileName.Length - data.FileInfo.Extension.Length);

					using (FileStream stream = File.Open($"{extensionlessName}.zevtc", FileMode.Create))
					using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
					{
						archive.CreateEntryFromFile(data.FileName, data.FileName);
					}

					counter++;

					if (File.Exists($"{extensionlessName}.zevtc"))
					{
						File.Delete(data.FileName);
						Progress?.Invoke(this, new LogCompressorProgressEventArgs(counter, length, data));
					}
				}

				Finished?.Invoke(this, EventArgs.Empty);
			});
		}

		public event EventHandler<LogCompressorProgressEventArgs> Progress;
		public event EventHandler Finished;
	}
}