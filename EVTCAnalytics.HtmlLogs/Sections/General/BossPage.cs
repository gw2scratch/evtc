using System.IO;
using GW2Scratch.EVTCAnalytics.Statistics;
using ScratchLogHTMLGenerator.Parts;

namespace ScratchLogHTMLGenerator.Sections.General
{
	public class BossPage : Page
	{
		private readonly TargetSquadDamageData damageData;

		public BossPage(TargetSquadDamageData damageData, ITheme theme) : base(damageData.Target.Name, true, theme)
		{
			this.damageData = damageData;
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
            <div class='title is-4'>Full Encounter</div>
            <div class='subtitle is-6'>Duration: {MillisecondsToReadableFormat(damageData.TimeMs)}</div>

			<div>
                <div class='title is-5'>Target damage to {damageData.Target.Name}</div>");

			new DamageTable(damageData, Theme).WriteHtml(writer);

			writer.WriteLine($@"
			</div>");
		}
	}
}