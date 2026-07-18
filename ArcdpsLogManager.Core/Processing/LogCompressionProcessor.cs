using GW2Scratch.ArcdpsLogManager.Logs;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace GW2Scratch.ArcdpsLogManager.Processing
{
	public class LogCompressionProcessor : BackgroundProcessor<LogData>
	{
		private const string ProducedExtension = "zevtc";

		private readonly LogCache logCache;
		
		private long savedBytes;
		public long SavedBytes => savedBytes;

		public LogCompressionProcessor(LogCache logCache)
		{
			this.logCache = logCache;
		}

		protected override Task Process(LogData item, CancellationToken cancellationToken)
		{
			string compressedFileName = Path.ChangeExtension(item.FileName, ProducedExtension);
			if (item.FileName == compressedFileName)
			{
				throw new ArgumentException("The file would be compressed while overwriting itself.", nameof(item));
			}

			using (FileStream stream = File.Open(compressedFileName, FileMode.Create))
			using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
			{
				archive.CreateEntryFromFile(item.FileName, item.FileName);
			}

			var oldFilename = item.FileName;

			long oldSize = item.FileInfo.Length;
			
			if (File.Exists(compressedFileName))
			{
				logCache.MoveLogDataEntry(item, compressedFileName);
				File.Delete(oldFilename);
				
				long newSize = item.FileInfo.Length;
				Interlocked.Add(ref savedBytes, oldSize - newSize);
			}

			return Task.CompletedTask;
		}
	}
}