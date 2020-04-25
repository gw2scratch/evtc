using System;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Sections;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public class LogFilterPanel : DynamicLayout
	{
		public event EventHandler FiltersUpdated;

		private LogFilters Filters { get; }

		public LogFilterPanel(ImageProvider imageProvider, LogFilters filters)
		{
			Filters = filters;

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
				endDateTimePicker.Value = DateTime.Now.Date.AddDays(1);
			};

			var allTimeButton = new Button {Text = "All time"};
			allTimeButton.Click += (sender, args) =>
			{
				startDateTimePicker.Value = LogFilters.GuildWars2ReleaseDate;
				endDateTimePicker.Value = DateTime.Now.Date.AddDays(1);
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

			BeginGroup("Filters", new Padding(5, 0, 5, 5));
			{
				BeginHorizontal();
				{
					BeginVertical();
					{
						BeginVertical(spacing: new Size(4, 0));
						{
							BeginHorizontal();
							{
								BeginGroup("Result", new Padding(4, 0, 4, 2), spacing: new Size(4, 0));
								{
									BeginHorizontal();
									{
										Add(successCheckBox);
										Add(failureCheckBox);
										Add(unknownCheckBox);
									}
									EndHorizontal();
								}
								EndGroup();
								BeginGroup("Mode", new Padding(4, 0, 4, 2), spacing: new Size(4, 0));
								{
									BeginHorizontal();
									{
										Add(normalModeCheckBox);
										Add(challengeModeCheckBox);
									}
									EndHorizontal();
								}
								EndGroup();
								Add(null);
							}
							EndHorizontal();
						}
						EndBeginVertical(spacing: new Size(4, 0));
						{
							BeginGroup("Time", new Padding(4, 0, 4, 2), spacing: new Size(4, 0));
							{
								BeginHorizontal();
								{
									Add(new Label
									{
										Text = "Between",
										VerticalAlignment = VerticalAlignment.Center
									});
									Add(startDateTimePicker);
									Add(new Label {Text = "and", VerticalAlignment = VerticalAlignment.Center});
									Add(endDateTimePicker);
									Add(lastDayButton);
									Add(allTimeButton);
								}
								EndHorizontal();
							}
							EndGroup();
							Add(null);
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

		private void OnFiltersUpdated()
		{
			FiltersUpdated?.Invoke(this, EventArgs.Empty);
		}
	}
}