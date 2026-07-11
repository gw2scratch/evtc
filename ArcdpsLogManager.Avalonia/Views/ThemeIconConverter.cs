using System;
using System.Globalization;
using Avalonia.Data.Converters;
using GW2Scratch.ArcdpsLogManager.Configuration;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>
	/// Renders the header's theme-picker button as a compact glyph reflecting the current
	/// <see cref="ApplicationTheme"/>, instead of a "Theme: Dark" text label.
	/// </summary>
	public sealed class ThemeIconConverter : IValueConverter
	{
		public static readonly ThemeIconConverter Instance = new();

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value switch
			{
				ApplicationTheme.Light => "☀",
				ApplicationTheme.Dark => "🌙",
				_ => "🖥",
			};
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
