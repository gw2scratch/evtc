using System.IO;
using System.Linq;
using ScratchEVTCParser.Statistics;
using ScratchLogHTMLGenerator.Parts;

namespace ScratchLogHTMLGenerator.Sections.Phases
{
	public class PhasePage : Page
	{
		private readonly PhaseStats phaseStats;

		public PhasePage(PhaseStats phaseStats, ITheme theme) : base(phaseStats.PhaseName, true, theme)
		{
			this.phaseStats = phaseStats;

			Subpages = phaseStats.TargetDamageData.Select(x => new PhaseTargetPage(this.phaseStats, x, theme)).ToArray();
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
            <div class='title is-4'>Phase: {phaseStats.PhaseName}</div>
            <div class='subtitle is-6'>Duration: {MillisecondsToReadableFormat(phaseStats.PhaseDuration)}</div>

			<div>
                <div class='title is-5'>Total damage in phase</div>
                <div class='subtitle is-7'>May include damage to insignificant enemies.</div>");

				new DamageTable(phaseStats.TotalDamageData, Theme).WriteHtml(writer);

				writer.WriteLine($@"
			</div>");
		}
	}
}