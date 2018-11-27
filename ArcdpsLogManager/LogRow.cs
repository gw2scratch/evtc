using System;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using Eto.Forms;
using ScratchEVTCParser.Model.Encounters;

namespace ArcdpsLogManager
{
	public class LogRow : Panel
	{
		private static Size Spacing = new Size(5, 2);

		public LogRow(LogData logData)
		{
			var layout = new DynamicLayout();
			Content = layout;

			layout.BeginHorizontal();
			layout.BeginVertical(spacing: Spacing, xscale: true);
			// File data
			layout.Add(new Label {Text = logData.Boss.Name});
			layout.Add(new Label {Text = "filename-actually-rather-long, 440 KiB"});
			layout.EndVertical();

			layout.BeginVertical(spacing: Spacing, xscale: true);
			// Fight data
			layout.Add(new Label {Text = GetResultText(logData)});
			layout.Add(new Label {Text = GetDurationText(logData)});
			layout.EndVertical();

			layout.BeginVertical(spacing: Spacing, xscale: true);
			// Group data
			layout.Add("There will be");
			layout.Add("specialization images here");
			layout.EndVertical();

			layout.BeginVertical(spacing: Spacing);
			// Buttons
			layout.Add(new Button {Text = "Upload to dps.report (EI)"});
			layout.Add(new Button {Text = "Upload to gw2raidar"});
			layout.EndVertical();

			layout.EndHorizontal();
		}

		private string GetResultText(LogData logData)
		{
			switch (logData.EncounterResult)
			{
				case EncounterResult.Success:
					return "Success";
				case EncounterResult.Failure:
                    return "Failure";
				case EncounterResult.Unknown:
					return "Unknown";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private string GetDurationText(LogData logData)
		{
			var seconds = logData.EncounterDuration.TotalSeconds;
			return $"{seconds / 60:0}m {seconds % 60f:0.0}s";
		}
	}
}