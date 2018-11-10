using System.IO;
using ScratchEVTCParser.Statistics;
using ScratchLogHTMLGenerator.Parts;

namespace ScratchLogHTMLGenerator.Sections.Phases
{
	public class PhaseTargetPage : Page
	{
		private readonly PhaseStats phaseStats;
		private readonly TargetSquadDamageData damageData;

		public PhaseTargetPage(PhaseStats phaseStats, TargetSquadDamageData damageData, ITheme theme)
			: base(damageData.Target.Name, true, theme)
		{
			this.phaseStats = phaseStats;
			this.damageData = damageData;
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
            <div class='title is-4'>Phase: {phaseStats.PhaseName}</div>
            <div class='subtitle is-6'>Duration: {MillisecondsToReadableFormat(damageData.TimeMs)}</div>

			<div>
                <div class='title is-5'>Target damage to {damageData.Target.Name}</div>");

			new DamageTable(damageData, Theme).WriteHtml(writer);

			writer.WriteLine($@"
			</div>");
		}
	}
}