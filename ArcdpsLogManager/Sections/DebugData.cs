using System.Globalization;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using Eto.Forms;
using ScratchLogBrowser;

namespace ArcdpsLogManager.Sections
{
	public class DebugData : Panel
	{
		private LogData logData;

		public LogData LogData
		{
			get => logData;
			set
			{
				logData = value;
				var layout = new DynamicLayout();

				var browserButton = new Button {Text = "Open in Scratch EVTC Browser"};

				layout.BeginVertical(new Padding(5), new Size(5, 5), yscale: true);
				{
					layout.BeginHorizontal();
					{
						layout.Add(null);
						layout.Add(null, true);
						layout.Add(null);
					}
					layout.EndHorizontal();
					layout.AddRow("File name", logData.FileInfo.Name);
					layout.AddRow("File size", $"{logData.FileInfo.Length / 1000f / 1000f:0.000} MB");
					layout.AddRow("File creation", $"{logData.FileInfo.CreationTime}");
					layout.AddRow(null);
					layout.AddRow("Parsing status", $"{logData.ParsingStatus}");
					layout.AddRow("Parsing time", $"{logData.ParseMilliseconds} ms");
					layout.AddRow("Parsed at date", $"{logData.ParseTime}");
					layout.AddRow("dps.report upload state", logData.DpsReportEIUpload.UploadState.ToString());
					layout.AddRow("dps.report upload time", logData.DpsReportEIUpload.UploadTime?.ToString(CultureInfo.InvariantCulture));
					layout.AddRow("dps.report url", logData.DpsReportEIUpload.Url);
					if (logData.ParsingStatus == ParsingStatus.Failed)
					{
						layout.EndVertical();
						layout.BeginVertical();
						layout.AddRow("Parsing exception");
						layout.AddRow(new TextArea {Text = $"{logData.ParsingException}", ReadOnly = true});
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
					var browserForm = new BrowserForm();
					browserForm.SelectLog(logData.FileInfo.FullName);
					browserForm.Show();
				};

				Content = layout;
			}
		}
	}
}