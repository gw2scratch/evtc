using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Compressing;

namespace GW2Scratch.ArcdpsLogManager
{
	public class LogCompressor
	{
		/// <summary>
		/// Compresses All Uncompressed Logs
		/// <returns> IEnumerable<LogData> List of All Uncompressed Logs</returns>
		/// </summary>
		public IEnumerable<LogData> Compress(IEnumerable<LogData> logs)
		{
			//Collection of uncompressed LogData
			LinkedList<LogData> uncompressedLogs = new LinkedList<LogData>();
			foreach (LogData data in logs) {
				if (data.FileInfo.Extension == ".evtc") {
					uncompressedLogs.AddLast(data);
				}
			}

			int counter = 0;
			int length = uncompressedLogs.Count;

			//Async compression using PowerShell for Windows and Bash for Unix
			Task.Run(() => {
				foreach (LogData data in uncompressedLogs) {
					string extensionlessName = data.FileName.Substring(0, data.FileName.Length - data.FileInfo.Extension.Length);

					using (FileStream stream = File.Open($"{extensionlessName}.zevtc", FileMode.Create))
					using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
					{
						archive.CreateEntryFromFile(data.FileName, data.FileName);
					}

					counter++;

					if (File.Exists($"{extensionlessName}.zevtc")) {
						File.Delete(data.FileName);
						Progress?.Invoke(this, new LogCompressorProgressEventArgs(counter, length, data));
					}
				}

				Finished?.Invoke(this, EventArgs.Empty);
			});

			return uncompressedLogs;
		}

		public event EventHandler<LogCompressorProgressEventArgs> Progress;
		public event EventHandler Finished;
	}
}
