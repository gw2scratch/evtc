using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class AdvancedFilterPanel : DynamicLayout
	{
		private LogFilters Filters { get; }

		public AdvancedFilterPanel(ImageProvider imageProvider, LogFilters filters)
		{
			Filters = filters;

			// TODO: Implement squad composition filters
			/*
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
								foreach (var profession in GameData.Professions.Select(x => x.Profession))
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
								foreach (var specialization in GameData.Professions.Select(x => x.HoT))
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
								foreach (var specialization in GameData.Professions.Select(x => x.PoF))
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
			*/

			var unparsedCheckBox = new CheckBox {Text = "Unparsed"};
			unparsedCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseUnparsedLogs);
			var parsingCheckBox = new CheckBox {Text = "Parsing"};
			parsingCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseParsingLogs);
			var parsedCheckBox = new CheckBox {Text = "Parsed"};
			parsedCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseParsedLogs);
			var failedCheckBox = new CheckBox {Text = "Failed"};
			failedCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseFailedLogs);
			BeginVertical(new Padding(5));
			{
				BeginGroup("Parsing status", new Padding(5));
				{
					AddRow(unparsedCheckBox, parsingCheckBox, parsedCheckBox, failedCheckBox);
				}
				EndGroup();
			}
			EndVertical();
		}

	}
}