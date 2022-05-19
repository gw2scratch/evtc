using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager.Sections.Statistics.Tabs
{
	public class SpecializationStatistics : DynamicLayout
	{
		private readonly Dictionary<Profession, Label> professionCountLabels = new Dictionary<Profession, Label>();

		private readonly Dictionary<Profession, Label> coreCountLabels =
			new Dictionary<Profession, Label>();

		private readonly Dictionary<EliteSpecialization, Label> eliteCountLabels =
			new Dictionary<EliteSpecialization, Label>();

		public SpecializationStatistics(ImageProvider imageProvider)
		{
			BeginVertical(spacing: new Size(10, 10));
			{
				BeginHorizontal();
				{
					BeginGroup("Profession counts", new Padding(5), new Size(5, 5), xscale: true);
					{
						foreach (var profession in ProfessionData.Professions.Select(x => x.Profession))
						{
							professionCountLabels[profession] = new Label {Text = "Unknown"};

							BeginHorizontal();
							{
								var imageView = new ImageView
								{
									Image = imageProvider.GetTinyProfessionIcon(profession),
									ToolTip = GameNames.GetName(profession)
								};
								Add(imageView);
								Add(professionCountLabels[profession]);
							}
							EndHorizontal();
						}

						Add(null);
					}
					EndGroup();

					BeginGroup("Specialization counts", xscale: true);
					{
						BeginHorizontal();
						{
							BeginVertical(new Padding(5), new Size(5, 5));
							{
								foreach (var profession in ProfessionData.Professions)
								{
									BeginHorizontal();
									{
										coreCountLabels[profession.Profession] = new Label {Text = "Unknown", Width = 60};
										var coreImage = new ImageView
										{
											Image = imageProvider.GetTinyProfessionIcon(profession.Profession),
											ToolTip = $"Core {GameNames.GetName(profession.Profession)}"
										};
										Add(coreImage);
										Add(coreCountLabels[profession.Profession]);

										foreach (var specialization in new[] {profession.HoT, profession.PoF, profession.EoD})
										{
											eliteCountLabels[specialization] = new Label {Text = "Unknown", Width = 60};
											var image = new ImageView
											{
												Image = imageProvider.GetTinyProfessionIcon(specialization),
												ToolTip = GameNames.GetName(specialization)
											};
											Add(image);
											Add(eliteCountLabels[specialization]);
										}
									}
									EndHorizontal();
								}

								Add(null);
							}
							EndVertical();
						}
						EndHorizontal();
					}
					EndGroup();
				}
				EndHorizontal();

				Add(null, yscale: true);
			}
			EndVertical();
			AddSeparateRow(null);
		}

		public void UpdateDataFromLogs(IEnumerable<LogData> logs)
		{
			var professionCounts = new Dictionary<Profession, int>();

			var specializationCoreCounts = new Dictionary<Profession, int>();
			var specializationEliteCounts = new Dictionary<EliteSpecialization, int>();
			int logCount = 0;

			foreach (Profession profession in Enum.GetValues(typeof(Profession)))
			{
				professionCounts[profession] = 0;
				specializationCoreCounts[profession] = 0;
			}

			foreach (EliteSpecialization specialization in Enum.GetValues(typeof(EliteSpecialization)))
			{
				specializationEliteCounts[specialization] = 0;
			}

			foreach (var log in logs)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed) continue;

				foreach (var player in log.Players)
				{
					professionCounts[player.Profession]++;
					if (player.EliteSpecialization == EliteSpecialization.None)
					{
						specializationCoreCounts[player.Profession]++;
					}
					else
					{
						specializationEliteCounts[player.EliteSpecialization]++;
					}
				}

				logCount++;
			}

			foreach (var pair in professionCountLabels)
			{
				var profession = pair.Key;
				var label = pair.Value;
				var count = professionCounts[profession];
				label.Text = $"{count} ({count / (float) logCount:0.00} on average)";
			}

			foreach (var pair in coreCountLabels)
			{
				var profession = pair.Key;
				var label = pair.Value;
				var count = specializationCoreCounts[profession];
				label.Text = $"{count}";
			}

			foreach (var pair in eliteCountLabels)
			{
				var specialization = pair.Key;
				var label = pair.Value;
				var count = specializationEliteCounts[specialization];
				label.Text = $"{count}";
			}

			SuspendLayout();
			ResumeLayout();
		}
	}
}