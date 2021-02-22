using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Compressing;

namespace GW2Scratch.ArcdpsLogManager
{
    public class LogCompressor
    {
        /// <summary>
        /// Compresses All Uncompressed Logs
        ///
        /// return IEnumerable<LogData> List of All Uncompressed Logs
		/// </summary>
        public IEnumerable<LogData> Compress(IEnumerable<LogData> logs)
        {
            //Collection of logs fullFileName
            LinkedList<LogData> uncompressedLogs = new LinkedList<LogData>();
            foreach (LogData data in logs) {
                if (data.FileInfo.Extension == ".evtc") {
                    string fileName = data.FileName;
                    uncompressedLogs.AddLast(data);
                }
            }

			int counter = 0;
			int length = uncompressedLogs.Count;
			OperatingSystem os = Environment.OSVersion;

			//Async compression using PowerShell for Windows and Bash for Unix
			Task.Run(() => {
				using (Process shell = new Process())
				{
					shell.StartInfo.RedirectStandardInput = true;
					shell.StartInfo.RedirectStandardOutput = true;
					shell.StartInfo.CreateNoWindow = true;

					shell.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
						if (!String.IsNullOrEmpty(e.Data) && e.Data == "Log Compressed") {
							counter++;
							Progress?.Invoke(this, new LogCompressorProgressEventArgs(counter, length));
						}
					});

					switch (os.Platform) {
						case (PlatformID.Win32NT):
							shell.StartInfo.FileName = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe";
							shell.Start();
							shell.BeginOutputReadLine();

							foreach (LogData data in uncompressedLogs) {
								string name = data.FileName.Substring(0, data.FileName.Length - data.FileInfo.Extension.Length);
								shell.StandardInput.WriteLine($"Compress-Archive \"{name}.evtc\" \"{name}.zip\"");
								shell.StandardInput.WriteLine($"Remove-Item \"{name}.evtc\"");
								shell.StandardInput.WriteLine($"Move-Item \"{name}.zip\" \"{name}.zevtc\"");
								shell.StandardInput.WriteLine("Write-Host \"Log Compressed\"");
							}

							break;
						case (PlatformID.Unix):
							shell.StartInfo.FileName = "/bin/bash";
							shell.Start();
							shell.BeginOutputReadLine();

							foreach (LogData data in uncompressedLogs) {
								string name = data.FileName.Substring(0, data.FileName.Length - data.FileInfo.Extension.Length);
								shell.StandardInput.WriteLine($"zip -m \"{name}.zevtc\" \"{name}.evtc\"");
								shell.StandardInput.WriteLine("echo \"Log Compressed\"");
							}

							break;
					}

					shell.StandardInput.Close();
					shell.WaitForExit();
					Finished?.Invoke(this, EventArgs.Empty);
				}
			});

            return uncompressedLogs;
        }

		public event EventHandler<LogCompressorProgressEventArgs> Progress;
		public event EventHandler Finished;
    }
}
