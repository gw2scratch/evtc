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

			var compositionTab = new TabPage
			{
				Text = "Squad composition",
				Content = ConstructSquadComposition(imageProvider, filters),
				Padding = new Padding(5),
			};
			var instabilityTab = new TabPage
			{
				Text = "Mistlock Instabilities",
				Content = ConstructMistlockInstabilities(imageProvider, filters),
				Padding = new Padding(5),
			};
			var processingTab = new TabPage
			{
				Text = "Processing status", Content = ConstructProcessingStatus(), Padding = new Padding(5),
			};

			void UpdateTabNames()
			{
				compositionTab.Text = "Squad composition" + (filters.CompositionFilters.IsDefault ? "" : " •");
				instabilityTab.Text = "Mistlock Instabilities" + (filters.InstabilityFilters.IsDefault ? "" : " •");
				processingTab.Text = "Processing status" + (AreProcessingFiltersDefault(filters) ? "" : " •");
			}
			
			var tabs = new TabControl();
			tabs.Pages.Add(compositionTab);
			tabs.Pages.Add(instabilityTab);
			tabs.Pages.Add(processingTab);
			UpdateTabNames();
			
			filters.PropertyChanged += (_, _) => UpdateTabNames();

			Add(tabs);
		}

		private Control ConstructSquadComposition(ImageProvider imageProvider, LogFilters filters)
		{
			return new SquadCompositionFilterPanel(imageProvider, filters);
		}

		private Control ConstructMistlockInstabilities(ImageProvider imageProvider, LogFilters filters)
		{
			return new InstabilityFilterPanel(imageProvider, filters);
		}

		private Control ConstructProcessingStatus()
		{
			var unparsedCheckBox = new CheckBox { Text = "Unprocessed" };
			unparsedCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseUnparsedLogs);
			var parsingCheckBox = new CheckBox { Text = "Processing" };
			parsingCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseParsingLogs);
			var parsedCheckBox = new CheckBox { Text = "Processed" };
			parsedCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseParsedLogs);
			var failedCheckBox = new CheckBox { Text = "Failed" };
			failedCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowParseFailedLogs);
			var layout = new DynamicLayout();
			layout.BeginVertical(spacing: new Size(5, 5));
			{
				layout.Add(unparsedCheckBox);
				layout.Add(parsingCheckBox);
				layout.Add(parsedCheckBox);
				layout.Add(failedCheckBox);
				layout.Add(null);
			}
			layout.EndVertical();
			return layout;
		}

		private static bool AreProcessingFiltersDefault(LogFilters filters)
		{
			return filters.ShowParseUnparsedLogs && filters.ShowParseParsingLogs && filters.ShowParseParsedLogs &&
			       filters.ShowParseFailedLogs;
		}

		public static int CountNonDefaultAdvancedFilters(LogFilters filters)
		{
			int count = 0;
			if (!filters.CompositionFilters.IsDefault) { count += 1; }

			if (!filters.InstabilityFilters.IsDefault) { count += 1; }

			if (!AreProcessingFiltersDefault(filters))
			{
				// We count the whole processing status section as one.
				count += 1;
			}

			return count;
		}
	}
}