using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class AdvancedFilters : DynamicLayout
	{
		private bool ShowParseUnparsedLogs { get; set; } = true;
		private bool ShowParseParsingLogs { get; set; } = true;
		private bool ShowParseParsedLogs { get; set; } = true;
		private bool ShowParseFailedLogs { get; set; } = true;

		public AdvancedFilters(LogFilterPanel filter, ImageProvider imageProvider)
		{
			BeginVertical(new Padding(5));
			{
				BeginGroup("Group composition", new Padding(5));
				{
					AddSeparateRow(new Label {Text = "Not implemented yet, doesn't do anything."});
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

			var unparsedCheckBox = new CheckBox {Text = "Unparsed"};
			unparsedCheckBox.CheckedBinding.Bind(this, x => x.ShowParseUnparsedLogs);
			var parsingCheckBox = new CheckBox {Text = "Parsing"};
			parsingCheckBox.CheckedBinding.Bind(this, x => x.ShowParseParsingLogs);
			var parsedCheckBox = new CheckBox {Text = "Parsed"};
			parsedCheckBox.CheckedBinding.Bind(this, x => x.ShowParseParsedLogs);
			var failedCheckBox = new CheckBox {Text = "Failed"};
			failedCheckBox.CheckedBinding.Bind(this, x => x.ShowParseFailedLogs);
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

		public bool FilterLog(LogData log)
		{
			if (!ShowParseUnparsedLogs && log.ParsingStatus == ParsingStatus.Unparsed ||
			    !ShowParseParsedLogs && log.ParsingStatus == ParsingStatus.Parsed ||
			    !ShowParseParsingLogs && log.ParsingStatus == ParsingStatus.Parsing ||
			    !ShowParseFailedLogs && log.ParsingStatus == ParsingStatus.Failed)
			{
				return false;
			}

			return true;
		}
	}
}