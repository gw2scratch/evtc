using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;

namespace GW2Scratch.ArcdpsLogManager.Controls.Filters
{
	public class AdvancedFilterPanel : DynamicLayout
	{
		private LogFilters Filters { get; }

		public AdvancedFilterPanel(ImageProvider imageProvider, LogFilters filters)
		{
			Filters = filters;

			BeginVertical(new Padding(5));
			{
				BeginGroup("Squad composition", new Padding(5));
				{
					Add(new SquadCompositionFilterPanel(imageProvider, filters));
				}
				EndGroup();
			}
			EndVertical();

			var unparsedCheckBox = new CheckBox {Text = "Unprocessed"};
			unparsedCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseUnparsedLogs);
			var parsingCheckBox = new CheckBox {Text = "Processing"};
			parsingCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseParsingLogs);
			var parsedCheckBox = new CheckBox {Text = "Processed"};
			parsedCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseParsedLogs);
			var failedCheckBox = new CheckBox {Text = "Failed"};
			failedCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseFailedLogs);
			BeginVertical(new Padding(5));
			{
				BeginGroup("Processing status", new Padding(5), new Size(6, 0));
				{
					AddRow(unparsedCheckBox, parsingCheckBox, parsedCheckBox, failedCheckBox);
				}
				EndGroup();
			}
			EndVertical();
		}

	}
}