using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Sections;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public class LogFilterPanel : DynamicLayout
	{
		private static readonly DateTime GuildWars2ReleaseDate = new DateTime(2012, 8, 28);
		private const string EncounterFilterAll = "All";
		private string EncounterFilter { get; set; } = EncounterFilterAll;

		private bool ShowSuccessfulLogs { get; set; } = true;
		private bool ShowFailedLogs { get; set; } = true;
		private bool ShowUnknownLogs { get; set; } = true;

		private DateTime? MinDateTimeFilter { get; set; } = GuildWars2ReleaseDate;
		private DateTime? MaxDateTimeFilter { get; set; } = DateTime.Now.Date.AddDays(1);

		private readonly DropDown encounterFilterDropDown;
		private readonly AdvancedFilters advancedFilters;

		public event EventHandler FiltersUpdated;

		public LogFilterPanel(ImageProvider imageProvider)
		{
			advancedFilters = new AdvancedFilters(this, imageProvider);

			encounterFilterDropDown = new DropDown {SelectedKey = EncounterFilterAll};
			encounterFilterDropDown.SelectedKeyBinding.Bind(this, x => x.EncounterFilter);

			var successCheckBox = new CheckBox {Text = "Success"};
			successCheckBox.CheckedBinding.Bind(this, x => x.ShowSuccessfulLogs);
			var failureCheckBox = new CheckBox {Text = "Failure"};
			failureCheckBox.CheckedBinding.Bind(this, x => x.ShowFailedLogs);
			var unknownCheckBox = new CheckBox {Text = "Unknown"};
			unknownCheckBox.CheckedBinding.Bind(this, x => x.ShowUnknownLogs);

			var startDateTimePicker = new DateTimePicker {Mode = DateTimePickerMode.DateTime};
			startDateTimePicker.ValueBinding.Bind(this, x => x.MinDateTimeFilter);
			var endDateTimePicker = new DateTimePicker {Mode = DateTimePickerMode.DateTime};
			endDateTimePicker.ValueBinding.Bind(this, x => x.MaxDateTimeFilter);

			var lastDayButton = new Button {Text = "Last day"};
			lastDayButton.Click += (sender, args) =>
			{
				startDateTimePicker.Value = DateTime.Now - TimeSpan.FromDays(1);
				endDateTimePicker.Value = DateTime.Now;
			};

			var allTimeButton = new Button {Text = "All time"};
			allTimeButton.Click += (sender, args) =>
			{
				startDateTimePicker.Value = GuildWars2ReleaseDate;
				endDateTimePicker.Value = DateTime.Now;
			};

			var advancedFiltersButton = new Button {Text = "Advanced"};
			advancedFiltersButton.Click += (sender, args) =>
			{
				var form = new Form
				{
					Title = "Advanced filters - arcdps Log Manager",
					Content = advancedFilters
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

		public bool FilterLog(LogData log)
		{
			if (EncounterFilter != EncounterFilterAll)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed)
				{
					return false;
				}

				if (log.EncounterName != EncounterFilter)
				{
					return false;
				}
			}

			if (!ShowFailedLogs && log.EncounterResult == EncounterResult.Failure ||
			    !ShowUnknownLogs && log.EncounterResult == EncounterResult.Unknown ||
			    !ShowSuccessfulLogs && log.EncounterResult == EncounterResult.Success)
			{
				return false;
			}


			if (log.EncounterStartTime.LocalDateTime < MinDateTimeFilter ||
			    log.EncounterStartTime.LocalDateTime > MaxDateTimeFilter)
			{
				return false;
			}

			if (!advancedFilters.FilterLog(log))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Needs to be called to update selections that depend on the available logs, such as
		/// filtering by encounter name.
		/// </summary>
		/// <param name="logs">A logs which will be filtered in the future.</param>
		public void UpdateLogs(IEnumerable<LogData> logs)
		{
			var previousKey = encounterFilterDropDown.SelectedKey;

			var encounterNames = new[] {EncounterFilterAll}
				.Concat(logs
					.Where(x => x.ParsingStatus == ParsingStatus.Parsed)
					.Select(x => x.EncounterName)
					.Distinct()
					.OrderBy(x => x)
					.ToArray()
				);

			if (encounterNames.Contains(previousKey))
			{
				encounterFilterDropDown.SelectedKey = previousKey;
			}
			else
			{
				encounterFilterDropDown.SelectedKey = EncounterFilterAll;
			}
		}

		private void OnFiltersUpdated()
		{
			FiltersUpdated?.Invoke(this, EventArgs.Empty);
		}
	}
}