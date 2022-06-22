using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition;

namespace GW2Scratch.ArcdpsLogManager.Controls.Filters
{
	public class SquadCompositionFilterPanel : DynamicLayout
	{
		public SquadCompositionFilterPanel(ImageProvider imageProvider, LogFilters filters)
		{
			var filterSnapshot = filters.CompositionFilters.DeepClone();
			BeginVertical(spacing: new Size(5, 5));
			{
				BeginHorizontal();
				{
					// Core professions
					BeginVertical(spacing: new Size(2, 2));
					{
						foreach (var filter in filterSnapshot.CoreProfessionFilters)
						{
							BeginHorizontal();
							{
								var image = new ImageView {
									Image = imageProvider.GetTinyProfessionIcon(filter.Profession),
									ToolTip = $"Core {GameNames.GetName(filter.Profession)}"
								};
								Add(image);
								Add(ConstructDropDown(filter));
								Add(ConstructStepper(filter));
								Add(null);
							}
							EndHorizontal();
						}
					}
					EndVertical();

					// Elite specializations
					var specializationFilterGroups = new[] {
						filterSnapshot.HeartOfThornsSpecializationFilters,
						filterSnapshot.PathOfFireSpecializationFilters,
						filterSnapshot.EndOfDragonsSpecializationFilters
					};
					foreach (var specializationFilters in specializationFilterGroups)
					{
						BeginVertical(spacing: new Size(2, 2));
						{
							foreach (var filter in specializationFilters)
							{
								BeginHorizontal();
								{
									var image = new ImageView {
										Image = imageProvider.GetTinyProfessionIcon(filter.EliteSpecialization),
										ToolTip = GameNames.GetName(filter.EliteSpecialization)
									};
									Add(image);
									Add(ConstructDropDown(filter));
									Add(ConstructStepper(filter));
									Add(null);
								}
								EndHorizontal();
							}
						}
						EndVertical();
					}
				}
				EndHorizontal();
			}
			EndVertical();

			BeginVertical(new Padding(5, 5), new Size(5, 5));
			{
				AddRow(null, ConstructResetButton(filterSnapshot), ConstructApplyButton(filters, filterSnapshot));
				Add(null);
			}
			EndVertical();
		}

		private Control ConstructApplyButton(LogFilters filters, CompositionFilters filterSnapshot)
		{
			var applyButton = new Button {
				Text = "Apply",
				Enabled = !Equals(filters.CompositionFilters, filterSnapshot)
			};

			applyButton.Click += (_, _) => {
				filters.CompositionFilters = filterSnapshot.DeepClone();
				applyButton.Enabled = !Equals(filters.CompositionFilters, filterSnapshot);
			};

			filterSnapshot.PropertyChanged += (_, _) => {
				applyButton.Enabled = !Equals(filters.CompositionFilters, filterSnapshot);
			};

			return applyButton;
		}

		private Button ConstructResetButton(CompositionFilters filterSnapshot)
		{
			var resetButton = new Button {
				Text = "Reset composition",
				Enabled = !Equals(new CompositionFilters(), filterSnapshot)
			};
			
			resetButton.Click += (_, _) => {
				filterSnapshot.ResetToDefault();
			};

			filterSnapshot.PropertyChanged += (_, _) => {
				resetButton.Enabled = !Equals(new CompositionFilters(), filterSnapshot);
			};

			return resetButton;
		}

		private DropDown ConstructDropDown(PlayerCountFilter filter)
		{
			var dropdown = new EnumDropDown<PlayerCountFilterType> {
				GetText = type => type switch {
					PlayerCountFilterType.GreaterOrEqual => "≥",
					PlayerCountFilterType.Equal => "=",
					PlayerCountFilterType.LessOrEqual => "≤",
					_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
				}
			};
			dropdown.SelectedValueBinding.Bind(filter, x => x.FilterType);

			return dropdown;
		}

		private NumericStepper ConstructStepper(PlayerCountFilter filter)
		{
			var numericStepper = new NumericStepper {Value = 0, MinValue = 0};
			numericStepper.ValueBinding
				.BindDataContext(
					Binding.Property((PlayerCountFilter x) => x.PlayerCount)
						.Convert(r => (double) r, v => (int) v)
				);
			numericStepper.DataContext = filter;

			return numericStepper;
		}
	}
}