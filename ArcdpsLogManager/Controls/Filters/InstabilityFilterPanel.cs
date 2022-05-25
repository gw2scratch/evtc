using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.ArcdpsLogManager.Controls.Filters
{
	public class InstabilityFilterPanel : DynamicLayout
	{
		public InstabilityFilterPanel(ImageProvider imageProvider, LogFilters filters)
		{
			BeginVertical(spacing: new Size(5, 25));
			{
				BeginVertical(spacing: new Size(5, 5));
				{
					var typeRadios = new EnumRadioButtonList<InstabilityFilters.FilterType>()
					{
						GetText = type => type switch
						{
							InstabilityFilters.FilterType.All => "All of these",
							InstabilityFilters.FilterType.Any => "Any of these",
							_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
						},
					};
					typeRadios.SelectedValueBinding.Bind(filters.InstabilityFilters,
						nameof(filters.InstabilityFilters.Type));

					AddRow(typeRadios);

					BeginHorizontal();
					{
						foreach (var column in Enum.GetValues(typeof(MistlockInstability)).Cast<MistlockInstability>()
							         .Chunk(5))
						{
							BeginVertical(spacing: new Size(2, 2));
							{
								foreach (var instability in column)
								{
									BeginHorizontal();
									{
										var image = new ImageView
										{
											Image = imageProvider.GetMistlockInstabilityIcon(instability),
											ToolTip = GameNames.GetInstabilityName(instability),
											Size = new Size(20, 20),
										};
										Add(image);
										var checkbox = new CheckBox
										{
											Text = GameNames.GetInstabilityName(instability)
										};
										checkbox.CheckedBinding.Bind(
											() => filters.InstabilityFilters[instability],
											value => filters.InstabilityFilters[instability] = value ?? false
										);
										Add(checkbox);
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
				BeginGroup("Tip", new Padding(10, 5));
				{
					Add(new Label
					{
						Text = "You can enable the Mistlock Instabilities column in the log list by right-clicking " +
						       "the log list header and checking the related checkbox.",
						Wrap = WrapMode.Word,
					});
				}
				EndGroup();
				Add(null);
			}
			EndVertical();
		}
	}
}