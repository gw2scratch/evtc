using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class AdvancedFilters : DynamicLayout
	{
		public AdvancedFilters(ImageProvider imageProvider)
		{
			BeginVertical(new Padding(5));
			{
				Add(new Label {Text = "Not implemented yet, doesn't do anything."});
			}
			EndVertical();

			BeginVertical(new Padding(5));
			{
				BeginGroup("Group composition", new Padding(5));
				{
					BeginVertical();
					{
						BeginHorizontal();
						{
							Add(new Label {Text = "Required classes: "});
							Add(new ImageView());
						}
						EndHorizontal();
						BeginHorizontal();
						{
							// Core professions
							BeginVertical(spacing: new Size(2, 2));
							{
								foreach (var profession in GameData.Professions.Select(x => x.profession))
								{
									BeginHorizontal();
									{
										Add(imageProvider.GetTinyProfessionIcon(profession));
										Add(new Label
										{
											Text = profession.ToString(),
											VerticalAlignment = VerticalAlignment.Center,
										});
										Add(new TextStepper() {Width = 50});
										Add(null);
									}
									EndHorizontal();
								}
							}
							EndVertical();
							// HoT elite specializations
							BeginVertical(spacing: new Size(2, 2));
							{
								foreach (var specialization in GameData.Professions.Select(x => x.hot))
								{
									BeginHorizontal();
									{
										Add(imageProvider.GetTinyProfessionIcon(specialization));
										Add(new Label
										{
											Text = specialization.ToString(),
											VerticalAlignment = VerticalAlignment.Center,
										});
										Add(new TextStepper() {Width = 50});
										Add(null);
									}
									EndHorizontal();
								}
							}
							EndVertical();
							// PoF elite specializations
							BeginVertical(spacing: new Size(2, 2));
							{
								foreach (var specialization in GameData.Professions.Select(x => x.pof))
								{
									BeginHorizontal();
									{
										Add(imageProvider.GetTinyProfessionIcon(specialization));
										Add(new Label
										{
											Text = specialization.ToString(),
											VerticalAlignment = VerticalAlignment.Center,
										});
										Add(new TextStepper() {Width = 50});
										Add(null);
									}
									EndHorizontal();
								}
							}
							EndVertical();
						}
						EndHorizontal();
					}
					EndVertical();
				}
				EndGroup();
			}
			EndVertical();
		}
	}
}