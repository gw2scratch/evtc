using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Sections.Statistics.Tabs;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class StatisticsSection : Panel
	{
		private readonly SpecializationStatistics specializations;

		public StatisticsSection(ImageProvider imageProvider)
		{
			specializations = new SpecializationStatistics(imageProvider);
			var tabs = new TabControl();
			tabs.Pages.Add(new TabPage {Text = "Specializations", Content = specializations, Padding = new Padding(10)});
			Content = tabs;
		}

		public void UpdateDataFromLogs(IEnumerable<LogData> logs)
		{
			specializations.UpdateDataFromLogs(logs);
		}
	}
}