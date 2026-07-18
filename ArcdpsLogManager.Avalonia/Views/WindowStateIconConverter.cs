using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>
	/// Renders the custom title bar's maximize/restore caption button glyph based on the window's
	/// current <see cref="WindowState"/>, so it reflects OS maximize state instead of staying static.
	/// </summary>
	public sealed class WindowStateIconConverter : IValueConverter
	{
		public static readonly WindowStateIconConverter Instance = new();

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value is WindowState.Maximized ? "🗗" : "🗖";
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
