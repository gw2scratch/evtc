using System.IO;
using ScratchEVTCParser.Statistics;
using ScratchLogHTMLGenerator.Parts;

namespace ScratchLogHTMLGenerator.Sections.General
{
	public class BossPage : Page
	{
		private readonly TargetSquadDamageData damageData;

		public BossPage(TargetSquadDamageData damageData) : base(true, damageData.Target.Name)
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

			new DamageTable(damageData).WriteHtml(writer);

			writer.WriteLine($@"
			</div>");
		}
	}
}