using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>Maps a favorite boolean to a filled (★) or empty (☆) star glyph.</summary>
	public sealed class FavoriteStarConverter : IValueConverter
	{
		public static readonly FavoriteStarConverter Instance = new();

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value is true ? "★" : "☆";
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
