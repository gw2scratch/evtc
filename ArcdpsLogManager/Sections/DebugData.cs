using ArcdpsLogManager.Logs;
using Eto.Forms;

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

				layout.BeginVertical();
				layout.AddRow("File name", logData.FileInfo.Name);
				layout.AddRow("File size", $"{logData.FileInfo.Length / 1000f / 1000f:0.000} MB");
				layout.AddRow("File creation", $"{logData.FileInfo.CreationTime}");
				layout.AddRow(null);
				layout.AddRow("Parsing status", $"{logData.ParsingStatus}");
				layout.AddRow("Parsing time", $"{logData.ParseMilliseconds} ms");
				layout.AddRow("Parsed at date", $"{logData.ParseTime}");
				if (logData.ParsingStatus == ParsingStatus.Failed)
				{
					layout.AddRow("Parsing exception");
					layout.EndVertical();
                    layout.BeginVertical();
					layout.AddRow(new TextArea {Text = $"{logData.ParsingException}", ReadOnly = true});
				}

                layout.EndVertical();

				Content = layout;
			}
		}
	}
}