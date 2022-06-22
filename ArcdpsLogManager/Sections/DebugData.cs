using System;
using System.Globalization;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCInspector;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class DebugData : Panel
	{
		private LogData logData;

		public event EventHandler InspectorOpened;
		
		public LogData LogData
		{
			get => logData;
			set
			{
				logData = value;
				var layout = new DynamicLayout();

				var browserButton = new Button
				{
					Text = "Open in Scratch EVTC Inspector",
					Height = 120
				};

				layout.BeginVertical(new Padding(5), new Size(5, 5));
				{
					layout.BeginHorizontal();
					{
						layout.Add(null);
						layout.Add(null, true);
					}
					layout.EndHorizontal();
					layout.AddRow("File name", logData.FileInfo.Name);
					layout.AddRow("File size", $"{logData.FileInfo.Length / 1000f / 1000f:0.000} MB");
					layout.AddRow("File creation", $"{logData.FileInfo.CreationTime}");
					layout.AddRow(null);
					layout.AddRow("Processing status", $"{logData.ParsingStatus}");
					layout.AddRow("Processing time", $"{logData.ParseMilliseconds} ms");
					layout.AddRow("Processed at date", $"{logData.ParseTime}");
					layout.AddRow("Processed with manager version", $"{logData.ParsingVersion}");
					layout.AddRow("dps.report upload state", logData.DpsReportEIUpload.UploadState.ToString());
					layout.AddRow("dps.report upload time", logData.DpsReportEIUpload.UploadTime?.ToString(CultureInfo.InvariantCulture));
					layout.AddRow("dps.report url", logData.DpsReportEIUpload.Url);
					if (logData.ParsingStatus == ParsingStatus.Failed)
					{
						layout.EndVertical();
						layout.BeginVertical(new Padding(5), yscale: true);
						layout.AddRow("Processing exception");
						string exceptionText = $"{logData.ParsingException.ExceptionName}: {logData.ParsingException.Message}\n" +
						                    $"{logData.ParsingException.StackTrace}";
						layout.AddRow(new TextArea {Text = exceptionText, ReadOnly = true});
					}
				}
				layout.EndVertical();
				layout.BeginVertical();
				{
					layout.AddRow(browserButton);
				}
				layout.EndVertical();

				browserButton.Click += (sender, args) =>
				{
					var browserForm = new InspectorForm();
					browserForm.SelectLog(logData.FileName);
					browserForm.Show();
					InspectorOpened?.Invoke(this, EventArgs.Empty);
				};

				Content = layout;
			}
		}
	}
}