using System;
using System.IO;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Statistics;

namespace ScratchLogHTMLGenerator.Parts
{
	public class BoonColumn
	{
		private readonly SquadDamageData squadDamageData;
		private readonly ITheme theme;

		public BoonColumn(SquadDamageData squadDamageData, ITheme theme)
		{
			this.squadDamageData = squadDamageData;
			this.theme = theme;
		}

		public void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
				<div class='column'>
                    <div class='title is-5'>Boons</div>
                    <div class='box media'>
						<figure class='media-left'>
							<p class='image is-32x32'>
                                <img src='{theme.GetMightIconUrl()}' alt='Might' title='Might'>
							</p>
						</figure>
						<div class='media-content'>
                            Average on hit");

			if (squadDamageData.PlayerAverageMightOnHit != null)
			{
				writer.WriteLine($@"{squadDamageData.PlayerAverageMightOnHit:0.0}
                            <progress class='progress' value='{squadDamageData.PlayerAverageMightOnHit:0.0}' max='25'>{squadDamageData.PlayerAverageMightOnHit:0.0}</progress>");
			}
			else
			{
				writer.WriteLine("Unknown");
			}

			writer.WriteLine($@"
						</div>
                    </div>
                    <div class='box media'>
						<figure class='media-left'>
							<p class='image is-32x32'>
                                <img src='{theme.GetQuicknessIconUrl()}' alt='Quickness' title='Quickness'>
							</p>
						</figure>
						<div class='media-content'>
							Availability on casts");

			if (squadDamageData.PlayerQuicknessCastRatio != null)
			{
				var percentage = squadDamageData.PlayerQuicknessCastRatio * 100;
				writer.WriteLine($@"{percentage:0.0}%
                            <progress class='progress' value='{percentage:0.0}' max='100'>{percentage:0.0}</progress>");
			}
			else
			{
				writer.WriteLine("Unknown");
			}

			writer.WriteLine($@"
						</div>
                    </div>
                    <div class='title is-5'>Conditions</div>
                    <div class='box media'>
						<figure class='media-left'>
							<p class='image is-32x32'>
                                <img src='{theme.GetVulnerabilityIconUrl()}' alt='Vulnerability' title='Vulnerability'>
							</p>
						</figure>
						<div class='media-content'>
                            Average on hit");

			if (squadDamageData.PlayerAverageTargetVulnerabilityOnHit != null)
			{
				writer.WriteLine($@"{squadDamageData.PlayerAverageTargetVulnerabilityOnHit:0.0}
                            <progress class='progress' value='{squadDamageData.PlayerAverageTargetVulnerabilityOnHit:0.0}' max='25'>{squadDamageData.PlayerAverageTargetVulnerabilityOnHit:0.0}</progress>");
			}
			else
			{
				writer.WriteLine("Unknown");
			}

			writer.WriteLine($@"
						</div>
                    </div>
				</div>");
		}
	}
}