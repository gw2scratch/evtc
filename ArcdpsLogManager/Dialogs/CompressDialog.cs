using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Dialogs
{
	public class CompressDialog : Dialog
	{
		public CompressDialog(ManagerForm managerForm)
		{
			Title = "Compress Logs";
			DynamicLayout formLayout = new DynamicLayout();
			Content = formLayout;

			Button closeButton = new Button { Text = "Close" };
			closeButton.Click += (_, _) => Close();
			PositiveButtons.Add(closeButton);

			Label explanationLabel = new Label { Text = "Compress uncompressed logs to save space." };

			const string buttonText = "Compress logs";
			Button compressLogsButton = new Button { Text = buttonText };

			compressLogsButton.Click += (_, _) =>
			{
				LogCompressor compressor = new LogCompressor();

				compressor.Progress += (_, args) =>
				{
					managerForm.LogCache?.ClearLogDataEntry(args.LogData.FileName);

					Application.Instance.AsyncInvoke(() =>
					{
						if (args.Current == args.Total)
						{
							compressLogsButton.Text = buttonText;
						}
						else
						{
							compressLogsButton.Text = $"{args.Current} / {args.Total}";
						}
					});
				};

				compressor.Finished += (_, _) =>
				{
					managerForm.LogCache?.SaveToFile();
					managerForm.ReloadLogs();
				};

				compressor.Compress(managerForm.LoadedLogs);
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