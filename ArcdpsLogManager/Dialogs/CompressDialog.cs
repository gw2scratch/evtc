using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Compressing;

namespace GW2Scratch.ArcdpsLogManager.Dialogs
{
    public class CompressDialog : Dialog
    {
        public CompressDialog(ManagerForm managerForm)
        {
            Title = "Compress Logs";
            DynamicLayout formLayout = new DynamicLayout();
            Content = formLayout;

            Button closeButton = new Button {Text = "Close"};
            closeButton.Click += (sender, args) => Close();
            PositiveButtons.Add(closeButton);

            // Form components
            Label explanationLabel = new Label {
                Text = "Compress uncompressed logs to save space."
            };

			string buttonText = "Compress logs";
            Button compressLogsButton = new Button {
				Text = buttonText
			};
            compressLogsButton.Click += (sender, args) => {
				LogCompressor compressor = new LogCompressor();

				compressor.Progress += (object sender, LogCompressorProgressEventArgs args) => {
					Application.Instance.AsyncInvoke(() => {
						if (args.current == args.total) {
							compressLogsButton.Text = buttonText;
						} else {
							compressLogsButton.Text = $"{args.current} / {args.total}";
						}
					});
				};

				compressor.Finished += (object sender, EventArgs args) => {
					managerForm.LogCache?.SaveToFile();
					managerForm.ReloadLogs();
				};

                IEnumerable<LogData> uncompressedLogs = compressor.Compress(managerForm.LoadedLogs);
                
                foreach (LogData log in uncompressedLogs) {
                    managerForm.LogCache?.ClearLogDataEntry(log.FileName);
                }
            };

            // Form layout
            formLayout.BeginVertical(new Padding(10), new Size(0, 0));
            {
                formLayout.AddRow(explanationLabel);
            }
            formLayout.EndVertical();
            formLayout.BeginVertical(new Padding(10), new Size(5, 5));
            {
                formLayout.AddRow(compressLogsButton);
            }
            formLayout.EndVertical();
        }
    }
}
