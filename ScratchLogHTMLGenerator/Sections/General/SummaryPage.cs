using System.IO;
using System.Linq;
using ScratchEVTCParser.Statistics;
using ScratchLogHTMLGenerator.Parts;

namespace ScratchLogHTMLGenerator.Sections.General
{
	public class SummaryPage : Page
	{
		private readonly LogStatistics stats;

		public SummaryPage(LogStatistics stats, ITheme theme) : base("Summary", true, theme)
		{
			this.stats = stats;
		}

		public override void WriteStyleHtml(TextWriter writer)
		{
			writer.WriteLine(@"
.summary-boxes {
	margin: 1rem 0;
}
.summary-boxes .subtitle {
	color: #8E98A2;
	font-weight: 300;
}
");
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
            <section class='summary-boxes'>
				<div class='tile is-ancestor has-text-centered'>
					<div class='tile is-parent'>
						<article class='tile is-child box'>
							<p class='title'>{stats.FightTimeMs / 1000 / 60}m {stats.FightTimeMs / 1000f % 60:0.0}s</p>
							<p class='subtitle'>Fight time</p>
						</article>
					</div>
					<div class='tile is-parent'>
						<article class='tile is-child box'>
							<p class='title'>TODO</p>
							<p class='subtitle'>Squad DPS</p>
						</article>
					</div>
					<div class='tile is-parent'>
						<article class='tile is-child box'>
							<p class='title'>{stats.PlayerData.Sum(x => x.DownCount)}</p>
							<p class='subtitle'>Downs</p>
						</article>
					</div>
					<div class='tile is-parent'>
						<article class='tile is-child box'>
							<p class='title'>{stats.PlayerData.Sum(x => x.DeathCount)}</p>
							<p class='subtitle'>Deaths</p>
						</article>
					</div>
				</div>
            </section>
			<div class='columns'>
				<div class='column is-three-quarters'>
                    <div class='title is-5'>Boss damage</div>
            ");

			if (stats.FullFightBossDamageData.Count() == 1)
			{
                new DamageTable(stats.FullFightBossDamageData.First(), Theme).WriteHtml(writer);
			}
			else
			{
                new MultiTargetDamageTable(stats.FullFightBossDamageData, Theme).WriteHtml(writer);
			}

			writer.WriteLine($@"
                </div>");

			if (stats.FullFightBossDamageData.Count() == 1)
			{
				new BoonColumn(stats.FullFightBossDamageData.First(), Theme).WriteHtml(writer);
			}
			else
			{
				// TODO: Implement a multi-target boon column (or, rather, combine damage data)

				writer.WriteLine($@"
				<div class='column'>
					Multi-target boon availability is not implemented yet.
				</div>");
			}

			writer.WriteLine($@"
			</div>
			<br>
            <div>
                <div class='title is-5'>Total damage</div>
                <div class='subtitle is-7'>May include damage to insignificant enemies.</div>
");
			new DamageTable(stats.FullFightSquadDamageData, Theme).WriteHtml(writer);

			writer.WriteLine(@"
			</div>");
		}
	}
}