using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.EVTCAnalytics;
using ScratchLogHTMLGenerator;

namespace GW2Scratch.HtmlLogMaker
{
	public class MakerForm : Form
	{
		private const string ResultFilePrefix = "scratch_";

		private readonly Queue<string> notFinishedFileNames = new Queue<string>();
		private readonly List<string> finishedFileNames = new List<string>();

		private GW2ApiData ApiData { get; set; } = null; // TODO: Add support

		public MakerForm()
		{
			Title = "Scratch HTML log maker";
			ClientSize = new Size(800, 600);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			var openFileDialog = new OpenFileDialog {MultiSelect = true};
			openFileDialog.Filters.Add(new FileFilter("EVTC logs", ".evtc", ".evtc.zip", ".zevtc"));

			var openFilesButton = new Button {Text = "Select EVTC logs"};
			var processButton = new Button {Text = "Create HTML logs"};

			var notDoneListBox = new ListBox();
			var doneListBox = new ListBox();
			doneListBox.DataStore = finishedFileNames.ToArray();

			var splitter = new Splitter {Panel1 = notDoneListBox, Panel2 = doneListBox, Position = 400};

			formLayout.BeginVertical(spacing: new Size(10, 10));
			formLayout.AddRow(openFilesButton, processButton);
			formLayout.EndVertical();
			formLayout.BeginVertical();
			formLayout.Add(splitter);
			formLayout.EndVertical();

			openFilesButton.Click += (sender, args) =>
			{
				if (openFileDialog.ShowDialog((Control) sender) == DialogResult.Ok)
				{
					foreach (var file in openFileDialog.Filenames)
					{
						notFinishedFileNames.Enqueue(file);
					}

					notDoneListBox.DataStore = notFinishedFileNames.Select(Path.GetFileName).ToArray();
				}
			};

			processButton.Click += (sender, args) =>
			{
				Task.Run(() =>
				{
					var times = new List<(string taskName, double milliseconds)>();
					var totalTimeStopwatch = Stopwatch.StartNew();

					var parser = new EVTCParser();
					var processor = new LogProcessor();
					var analysis = new LogAnalyser();
					var generator = new HtmlGenerator(ApiData);

					int finishedTaskCount = 0;
					while (notFinishedFileNames.Count > 0)
					{
						var taskStopwatch = Stopwatch.StartNew();
						string filename = "";
						filename = notFinishedFileNames.Dequeue();

                        var fileDirectory = Path.GetDirectoryName(filename);
                        var newName = Path.GetFileNameWithoutExtension(filename);
                        if (newName.EndsWith(".evtc")) newName = newName.Substring(0, newName.Length - 5);
                        var resultFilename = Path.Combine(fileDirectory, ResultFilePrefix + newName + ".html");

						try
						{
                            var lastElapsed = taskStopwatch.Elapsed;

							var parsedLog = parser.ParseLog(filename);
							times.Add(("parsing", (taskStopwatch.Elapsed - lastElapsed).TotalMilliseconds));
							lastElapsed = taskStopwatch.Elapsed;

							var processedLog = processor.GetProcessedLog(parsedLog);
							times.Add(("processing", (taskStopwatch.Elapsed - lastElapsed).TotalMilliseconds));
							lastElapsed = taskStopwatch.Elapsed;

							var stats = analysis.GetStatistics(processedLog, ApiData);

							times.Add(("stats", (taskStopwatch.Elapsed - lastElapsed).TotalMilliseconds));
							lastElapsed = taskStopwatch.Elapsed;

							using (var htmlStringWriter = new StreamWriter(resultFilename))
							{
								generator.WriteHtml(htmlStringWriter, stats);
							}

							times.Add(("html", (taskStopwatch.Elapsed - lastElapsed).TotalMilliseconds));
							lastElapsed = taskStopwatch.Elapsed;

							finishedFileNames.Add(resultFilename);
							Application.Instance.Invoke(() =>
							{
								notDoneListBox.DataStore = notFinishedFileNames.Select(Path.GetFileName).ToArray();
								doneListBox.DataStore = finishedFileNames.Select(Path.GetFileName).ToArray();
							});
						}
						catch (Exception e)
						{
							finishedFileNames.Add($"FAILED: {resultFilename} ({e.Message})");
							Application.Instance.Invoke(() =>
							{
								notDoneListBox.DataStore = notFinishedFileNames.Select(Path.GetFileName).ToArray();
								doneListBox.DataStore = finishedFileNames.Select(Path.GetFileName).ToArray();
							});
						}
						Console.WriteLine($"{newName} done, time {taskStopwatch.Elapsed}");
						finishedTaskCount++;
					}

					Console.WriteLine($"All done, total time {totalTimeStopwatch.Elapsed}");

					foreach ((string taskName, double totalMs) in times.GroupBy(x => x.taskName).Select(x => (x.Key, x.Sum(y => y.milliseconds))).OrderByDescending(x => x.Item2))
					{
						Console.WriteLine($"{taskName}, total {totalMs}ms, average {totalMs/Math.Max(finishedTaskCount, 1)}ms");
					}
				});
			};
		}
	}
}