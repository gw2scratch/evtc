using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Sections;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public class LogFilterPanel : DynamicLayout
	{
		private LogFilters Filters { get; }

		private readonly LogEncounterFilterTree encounterTree;

		public LogFilterPanel(ImageProvider imageProvider, LogFilters filters)
		{
			Filters = filters;

			encounterTree = new LogEncounterFilterTree(imageProvider, Filters);

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
			startDateTimePicker.ValueBinding.Bind(this, x => x.Filters.MinDateTime);
			var endDateTimePicker = new DateTimePicker {Mode = DateTimePickerMode.DateTime};
			endDateTimePicker.ValueBinding.Bind(this, x => x.Filters.MaxDateTime);

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

			var advancedFiltersButton = new Button {Text = "Advanced filters"};
			advancedFiltersButton.Click += (sender, args) =>
			{
				var form = new Form
				{
					Title = "Advanced filters - arcdps Log Manager",
					Content = new AdvancedFilterPanel(imageProvider, filters)
				};
				form.Show();
			};

			//BeginGroup("Filters", new Padding(5, 0, 5, 5));
			{
				BeginVertical(spacing: new Size(4, 4));
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
					BeginGroup("Time", new Padding(4, 0, 4, 2), spacing: new Size(4, 2));
					{
						BeginVertical(spacing: new Size(4, 2));
						{
							BeginHorizontal();
							{
								Add(new Label
								{
									Text = "Between",
									VerticalAlignment = VerticalAlignment.Center
								});
								Add(startDateTimePicker);
								Add(null, xscale: true);
							}
							EndHorizontal();
							BeginHorizontal();
							{
								Add(new Label {Text = "and", VerticalAlignment = VerticalAlignment.Center});
								Add(endDateTimePicker);
								Add(null, xscale: true);
							}
							EndHorizontal();
						}
						EndVertical();
						BeginVertical(spacing: new Size(4, 0));
						{
							BeginHorizontal();
							{
								Add(allTimeButton, xscale: false);
								Add(lastDayButton, xscale: false);
								Add(null, xscale: true);
							}
							EndHorizontal();
						}
						EndVertical();
					}
					EndGroup();
					Add(encounterTree, yscale: true);
					Add(advancedFiltersButton);
				}
				EndVertical();
			}
			//EndGroup();
		}

		/// <summary>
		/// Needs to be called to update selections that depend on the available logs, such as
		/// filtering by encounters.
		/// </summary>
		/// <param name="logs">Logs to be available for filtering.</param>
		public void UpdateLogs(IReadOnlyList<LogData> logs)
		{
			encounterTree.UpdateLogs(logs);
		}
	}
}