using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Compressing;
using GW2Scratch.ArcdpsLogManager.Processing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Dialogs
{
	public class CompressDialog : Dialog
	{
		public CompressDialog(IReadOnlyList<LogData> logs, LogCompressionProcessor logCompressionProcessor)
		{
			var compressibleLogs = logs
				.Where(x => !CompressionUtils.HasZipExtension(x.FileName))
				.Where(x => !logCompressionProcessor.IsScheduledOrBeingProcessed(x))
				.ToList();
			
			Title = "Compress logs - arcdps Log Manager";
			ClientSize = new Size(500, -1);
			DynamicLayout layout = new DynamicLayout();
			
			Button closeButton = new Button { Text = "Close" };
			closeButton.Click += (_, _) => Close();
			PositiveButtons.Add(closeButton);

			Label savedSpaceLabel = new Label { Text = "0.00 MB" };
			Button compressLogsButton = new Button
			{
				Text = "Compress logs",
				Enabled = logCompressionProcessor.GetScheduledItemCount() == 0 && compressibleLogs.Count > 0
			};
			
			Button cancelCompressionButton = new Button
			{
				Text = "Stop compression",
				Enabled = logCompressionProcessor.GetScheduledItemCount() > 0
			};
				
			Label compressibleCountLabel = new Label { Text = compressibleLogs.Count.ToString() };
			Label compressedCountLabel = new Label { Text = logCompressionProcessor.ProcessedItemCount.ToString() };

			var progressBar = new ProgressBar { Value = 0, MaxValue = compressibleLogs.Count };

			compressLogsButton.Click += (_, _) =>
			{
				compressLogsButton.Enabled = false;

				foreach (var log in compressibleLogs)
				{
					logCompressionProcessor.Schedule(log);
				}
			};

			cancelCompressionButton.Click += (_, _) =>
			{
				logCompressionProcessor.StopBackgroundTask();
			};

			void OnLogCompressionProcessorOnStopping(object o, EventArgs eventArgs)
			{
				Application.Instance.Invoke(() =>
				{
					compressLogsButton.Enabled = true;
					cancelCompressionButton.Enabled = false;
				});
			}

			void OnLogCompressionProcessorOnStoppingWithError(object _, BackgroundProcessorErrorEventArgs args)
			{
				Application.Instance.Invoke(() => { MessageBox.Show(this, $"Failed to compress a log.\n\n{args.Exception}", MessageBoxType.Error); });
				compressLogsButton.Enabled = true;
				cancelCompressionButton.Enabled = false;
			}

			void OnLogCompressionProcessorOnProcessed(object sender, BackgroundProcessorEventArgs args)
			{
				if (args.CurrentScheduledItems == 0)
				{
					Application.Instance.Invoke(() => compressLogsButton.Enabled = true);
					Application.Instance.Invoke(() => cancelCompressionButton.Enabled = false);
				}

				Application.Instance.Invoke(() =>
				{
					progressBar.Value = args.TotalProcessedItems;
					progressBar.MaxValue = args.TotalScheduledItems;

					double megabytes = logCompressionProcessor.SavedBytes / 1000.0 / 1000.0; // Yes, 1000-based MB.
					compressibleCountLabel.Text = args.CurrentScheduledItems.ToString();
					compressedCountLabel.Text = args.TotalProcessedItems.ToString();
					savedSpaceLabel.Text = $"{megabytes:0.00} MB";
				});
			}

			logCompressionProcessor.Stopping += OnLogCompressionProcessorOnStopping;
			logCompressionProcessor.StoppingWithError += OnLogCompressionProcessorOnStoppingWithError;
			logCompressionProcessor.Processed += OnLogCompressionProcessorOnProcessed;
			
			Closing += (_, _) =>
			{
				logCompressionProcessor.Stopping -= OnLogCompressionProcessorOnStopping;
				logCompressionProcessor.StoppingWithError -= OnLogCompressionProcessorOnStoppingWithError;
				logCompressionProcessor.Processed -= OnLogCompressionProcessorOnProcessed;
			};

			layout.BeginVertical(new Padding(10), new Size(0, 0));
			{
				layout.AddRow(new Label
				{
					Text = "You may compress your uncompressed logs to save a significant amount of storage space. " +
					       "This will transform your .evtc logs into .zevtc logs.",
					Wrap = WrapMode.Word
				});
			}
			layout.EndVertical();
			layout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				layout.AddRow(compressLogsButton);
				layout.AddRow(cancelCompressionButton);
			}
			layout.EndVertical();
			layout.BeginVertical(new Padding(10), new Size(5, 5));
			{
				layout.AddRow("Uncompressed logs: ", compressibleCountLabel);
				layout.AddRow("Newly compressed logs: ", compressedCountLabel);
				layout.AddRow("Saved storage space: ", savedSpaceLabel);
			}
			layout.EndVertical();
			layout.BeginHorizontal();
			{
				layout.AddRow(progressBar);
			}
			layout.EndHorizontal();
			
			Content = layout;
		}
	}
}