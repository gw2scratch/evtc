using System.IO;
using ScratchEVTCParser.Statistics;
using ScratchLogHTMLGenerator.Parts;

namespace ScratchLogHTMLGenerator.Sections.General
{
	public class SummaryPage : Page
	{
		private readonly LogStatistics stats;

		public SummaryPage(LogStatistics stats) : base(true, "Summary")
		{
			this.stats = stats;
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
            <div class='title is-4'>Full Encounter</div>
            <div class='subtitle is-6'>Duration: {MillisecondsToReadableFormat(stats.FullFightSquadDamageData.TimeMs)}</div>

			<div>
                <div class='title is-5'>Total damage in encounter</div>");

			new DamageTable(stats.FullFightSquadDamageData).WriteHtml(writer);

			writer.WriteLine(@"
			</div>");
		}
	}
}