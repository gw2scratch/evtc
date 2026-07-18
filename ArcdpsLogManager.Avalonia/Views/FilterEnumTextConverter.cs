using System;
using System.Globalization;
using Avalonia.Data.Converters;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>
	/// Renders the small enum-based filter-mode selectors (composition player-count comparisons,
	/// and the All/Any/None instability and player filter types) as display text, matching the
	/// Eto <c>EnumDropDown</c>/<c>EnumRadioButtonList</c> <c>GetText</c> callbacks.
	/// </summary>
	public sealed class FilterEnumTextConverter : IValueConverter
	{
		public static readonly FilterEnumTextConverter Instance = new();

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value switch
			{
				PlayerCountFilterType.GreaterOrEqual => "≥",
				PlayerCountFilterType.Equal => "=",
				PlayerCountFilterType.LessOrEqual => "≤",
				InstabilityFilters.FilterType.All => "All of these",
				InstabilityFilters.FilterType.Any => "Any of these",
				InstabilityFilters.FilterType.None => "None of these",
				PlayerFilters.FilterType.All => "All of these",
				PlayerFilters.FilterType.Any => "Any of these",
				PlayerFilters.FilterType.None => "None of these",
				_ => value?.ToString() ?? "",
			};
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
