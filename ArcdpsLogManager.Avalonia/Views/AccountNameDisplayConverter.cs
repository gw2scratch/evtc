using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>Strips the leading <c>:</c> GW2 API account names are stored with for display,
	/// matching the <c>TrimStart(':')</c> convention used by <c>PlayerRow</c>/<c>PlayerAccountRow</c>.</summary>
	public sealed class AccountNameDisplayConverter : IValueConverter
	{
		public static readonly AccountNameDisplayConverter Instance = new();

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return (value as string)?.TrimStart(':') ?? "";
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
