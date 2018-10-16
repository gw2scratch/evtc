using System.IO;
using System.Linq;
using ScratchEVTCParser.Statistics;
using ScratchLogHTMLGenerator.Parts;

namespace ScratchLogHTMLGenerator.Sections.Phases
{
	public class PhasePage : Page
	{
		private readonly PhaseStats phaseStats;

		public PhasePage(PhaseStats phaseStats) : base(true, phaseStats.PhaseName)
		{
			this.phaseStats = phaseStats;

			Subpages = phaseStats.TargetDamageData.Select(x => new PhaseTargetPage(this.phaseStats, x)).ToArray();
		}

		public override void WriteHtml(TextWriter writer)
		{
				writer.WriteLine($@"
            <div class='title is-4'>Phase: {phaseStats.PhaseName}</div>
            <div class='subtitle is-6'>Duration: {MillisecondsToReadableFormat(phaseStats.PhaseDuration)}</div>

			<div>
                <div class='title is-5'>Total damage in phase</div>");

				new DamageTable(phaseStats.TotalDamageData).WriteHtml(writer);

				writer.WriteLine($@"
			</div>");
		}
	}
}