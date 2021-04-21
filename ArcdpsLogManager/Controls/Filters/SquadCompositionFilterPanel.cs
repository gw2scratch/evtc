using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition;

namespace GW2Scratch.ArcdpsLogManager.Controls.Filters
{
	public class SquadCompositionFilterPanel : DynamicLayout
	{
		public SquadCompositionFilterPanel(ImageProvider imageProvider, LogFilters filters)
		{
			BeginVertical(spacing: new Size(5, 5));
			{
				BeginHorizontal();
				{
					// Core professions
					BeginVertical(spacing: new Size(2, 2));
					{
						foreach (var filter in filters.CompositionFilters.CoreProfessionFilters)
						{
							BeginHorizontal();
							{
								var image = new ImageView {
									Image = imageProvider.GetTinyProfessionIcon(filter.Profession),
									ToolTip = $"Core {filter.Profession}"
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
						filters.CompositionFilters.HeartOfThornsSpecializationFilters,
						filters.CompositionFilters.PathOfFireSpecializationFilters
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
										ToolTip = filter.EliteSpecialization.ToString()
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
			
			BeginVertical(new Padding(5, 5));
			AddRow(null, ConstructResetButton(filters));
			EndVertical();
		}

		private Button ConstructResetButton(LogFilters filters)
		{
			var resetButton = new Button {Text = "Reset composition"};
			resetButton.Click += (_, _) => {
				var filterGroups = new IReadOnlyList<PlayerCountFilter>[] {
					filters.CompositionFilters.CoreProfessionFilters,
					filters.CompositionFilters.HeartOfThornsSpecializationFilters,
					filters.CompositionFilters.PathOfFireSpecializationFilters,
				};
				foreach (var group in filterGroups)
				{
					foreach (var filter in group)
					{
						filter.PlayerCount = 0;
						filter.FilterType = PlayerCountFilterType.GreaterOrEqual;
					}
				}
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