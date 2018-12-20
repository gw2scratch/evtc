using System;
using System.Collections.Generic;
using System.Linq;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using Eto.Forms;
using ScratchEVTCParser.Model.Agents;

namespace ArcdpsLogManager.Sections
{
	public class Statistics : DynamicLayout
	{
		private readonly Dictionary<Profession, Label> professionCountLabels = new Dictionary<Profession, Label>();

		private readonly Dictionary<Profession, Label> specializationCoreCountLabels =
			new Dictionary<Profession, Label>();

		private readonly Dictionary<EliteSpecialization, Label> specializationEliteCountLabels =
			new Dictionary<EliteSpecialization, Label>();

		public Statistics(LogList logList, ImageProvider imageProvider)
		{
			var refreshButton = new Button {Text = "Refresh statistics for current filter"};
			BeginVertical(new Padding(5));
			{
				BeginHorizontal();
				{
					Add(null, true);
					Add(refreshButton);
					Add(null, true);
				}
				EndHorizontal();
			}
			EndVertical();

			BeginVertical();
			{
				BeginHorizontal();
				{
					BeginGroup("Profession counts", new Padding(5), new Size(5, 5), xscale: true);
					{
						foreach (var profession in GameData.Professions.Select(x => x.profession))
						{
							professionCountLabels[profession] = new Label {Text = "Unknown"};

							BeginHorizontal();
							{
								var imageView = new ImageView {Image = imageProvider.GetTinyProfessionIcon(profession)};
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
						// Core specializations (no elite)
						BeginHorizontal();
						{
							BeginVertical(new Padding(5), new Size(5, 5));
							{
								foreach (var profession in GameData.Professions.Select(x => x.profession))
								{
									specializationCoreCountLabels[profession] = new Label {Text = "Unknown"};

									BeginHorizontal();
									{
										var imageView = new ImageView
											{Image = imageProvider.GetTinyProfessionIcon(profession)};
										Add(imageView);
										Add(specializationCoreCountLabels[profession]);
									}
									EndHorizontal();
								}

								Add(null);
							}
							EndVertical();

							// Heart of Thorns elite specializations
							BeginVertical(new Padding(5), new Size(5, 5));
							{
								foreach (var specialization in GameData.Professions.Select(x => x.hot))
								{
									specializationEliteCountLabels[specialization] = new Label {Text = "Unknown"};

									BeginHorizontal();
									{
										var imageView = new ImageView
											{Image = imageProvider.GetTinyProfessionIcon(specialization)};
										Add(imageView);
										Add(specializationEliteCountLabels[specialization]);
									}
									EndHorizontal();
								}

								Add(null);
							}
							EndVertical();

							// Path of Fire elite specializations
							BeginVertical(new Padding(5), new Size(5, 5));
							{
								foreach (var specialization in GameData.Professions.Select(x => x.pof))
								{
									specializationEliteCountLabels[specialization] = new Label {Text = "Unknown"};

									BeginHorizontal();
									{
										var imageView = new ImageView
											{Image = imageProvider.GetTinyProfessionIcon(specialization)};
										Add(imageView);
										Add(specializationEliteCountLabels[specialization]);
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
			}
			EndVertical();
			AddSeparateRow(null);

			refreshButton.Click += (sender, args) =>
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

				foreach (var log in logList.DataStore)
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

				foreach (var pair in specializationCoreCountLabels)
				{
					var profession = pair.Key;
					var label = pair.Value;
					var count = specializationCoreCounts[profession];
					label.Text = $"{count}";
				}

				foreach (var pair in specializationEliteCountLabels)
				{
					var specialization = pair.Key;
					var label = pair.Value;
					var count = specializationEliteCounts[specialization];
					label.Text = $"{count}";
				}
			};
		}
	}
}