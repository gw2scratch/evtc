using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>
	/// Maps a boolean to a pixel <see cref="GridLength"/> (given via <c>ConverterParameter</c>) when
	/// true, or zero when false. Used to collapse a resizable sidebar column entirely when hidden,
	/// rather than just hiding its content, so the GridSplitter next to it collapses too.
	/// </summary>
	public sealed class BoolToGridLengthConverter : IValueConverter
	{
		public static readonly BoolToGridLengthConverter Instance = new();

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is not true) return new GridLength(0);

			double pixels = parameter is string s && double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)
				? d
				: 0;
			return new GridLength(pixels);
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
