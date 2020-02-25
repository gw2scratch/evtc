using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Sections;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public class LogFilterPanel : DynamicLayout
	{
		private readonly ILogNameProvider logNameProvider;
		private readonly DropDown encounterFilterDropDown;
		public event EventHandler FiltersUpdated;

		private LogFilters Filters { get; }

		public LogFilterPanel(ImageProvider imageProvider, ILogNameProvider logNameProvider, LogFilters filters)
		{
			this.logNameProvider = logNameProvider;
			Filters = filters;

			encounterFilterDropDown = new DropDown {SelectedKey = LogFilters.EncounterFilterAll};
			encounterFilterDropDown.SelectedKeyBinding.Bind(this, x => x.Filters.EncounterFilter);

			var successCheckBox = new CheckBox {Text = "Success"};
			successCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowSuccessfulLogs);
			var failureCheckBox = new CheckBox {Text = "Failure"};
			failureCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowFailedLogs);
			var unknownCheckBox = new CheckBox {Text = "Unknown"};
			unknownCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowUnknownLogs);

			var normalModeCheckBox = new CheckBox {Text = "Normal"};
			normalModeCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowNormalModeLogs);
			var challengeModeCheckBox = new CheckBox {Text = "CM"};
			challengeModeCheckBox.CheckedBinding.Bind(this, x => x.Filters.ShowChallengeModeLogs);

			var startDateTimePicker = new DateTimePicker {Mode = DateTimePickerMode.DateTime};
			startDateTimePicker.ValueBinding.Bind(this, x => x.Filters.MinDateTimeFilter);
			var endDateTimePicker = new DateTimePicker {Mode = DateTimePickerMode.DateTime};
			endDateTimePicker.ValueBinding.Bind(this, x => x.Filters.MaxDateTimeFilter);

			var lastDayButton = new Button {Text = "Last day"};
			lastDayButton.Click += (sender, args) =>
			{
				startDateTimePicker.Value = DateTime.Now - TimeSpan.FromDays(1);
				endDateTimePicker.Value = DateTime.Now;
			};

			var allTimeButton = new Button {Text = "All time"};
			allTimeButton.Click += (sender, args) =>
			{
				startDateTimePicker.Value = LogFilters.GuildWars2ReleaseDate;
				endDateTimePicker.Value = DateTime.Now;
			};

			var advancedFiltersButton = new Button {Text = "Advanced"};
			advancedFiltersButton.Click += (sender, args) =>
			{
				var form = new Form
				{
					Title = "Advanced filters - arcdps Log Manager",
					Content = new AdvancedFilterPanel(imageProvider, filters)
				};
				form.Show();
			};

			var applyFilterButton = new Button {Text = "Apply"};
			applyFilterButton.Click += (sender, args) => { OnFiltersUpdated(); };

			BeginGroup("Filters", new Padding(5));
			{
				BeginHorizontal();
				{
					BeginVertical();
					{
						BeginVertical(new Padding(5), new Size(4, 0));
						{
							BeginHorizontal();
							{
								Add(new Label
									{Text = "Encounter", VerticalAlignment = VerticalAlignment.Center});
								Add(encounterFilterDropDown);
								Add(new Label
									{Text = "Result", VerticalAlignment = VerticalAlignment.Center});
								Add(successCheckBox);
								Add(failureCheckBox);
								Add(unknownCheckBox);
								Add(new Label
									{Text = "Mode", VerticalAlignment = VerticalAlignment.Center});
								Add(normalModeCheckBox);
								Add(challengeModeCheckBox);
							}
							EndHorizontal();
						}
						EndBeginVertical(new Padding(5), new Size(4, 0));
						{
							BeginHorizontal();
							{
								Add(
									new Label
									{
										Text = "Encounter time between",
										VerticalAlignment = VerticalAlignment.Center
									});
								Add(startDateTimePicker);
								Add(new Label
									{Text = "and", VerticalAlignment = VerticalAlignment.Center});
								Add(endDateTimePicker);
								Add(lastDayButton);
								Add(allTimeButton);
							}
							EndHorizontal();
						}
						EndVertical();
					}
					EndVertical();

					Add(null, true);

					BeginVertical(new Padding(5), new Size(0, 5));
					{
						Add(advancedFiltersButton);
						Add(null, true);
						Add(applyFilterButton);
					}
					EndVertical();
				}
				EndHorizontal();
			}
			EndGroup();
		}

		/// <summary>
		/// Needs to be called to update selections that depend on the available logs, such as
		/// filtering by encounter name.
		/// </summary>
		/// <param name="logs">A logs which will be filtered in the future.</param>
		public void UpdateLogs(IEnumerable<LogData> logs)
		{
			string previousKey = encounterFilterDropDown.SelectedKey;

			var encounterNames = new[] {LogFilters.EncounterFilterAll}
				.Concat(logs
					.Where(x => x.ParsingStatus == ParsingStatus.Parsed)
					.Select(x => logNameProvider.GetName(x))
					.Distinct()
					.OrderBy(x => x)
					.ToArray()
				).ToArray();

			encounterFilterDropDown.DataStore = encounterNames;

			if (encounterNames.Contains(previousKey))
			{
				encounterFilterDropDown.SelectedKey = previousKey;
			}
			else
			{
				encounterFilterDropDown.SelectedKey = LogFilters.EncounterFilterAll;
			}
		}

		private void OnFiltersUpdated()
		{
			FiltersUpdated?.Invoke(this, EventArgs.Empty);
		}
	}
}