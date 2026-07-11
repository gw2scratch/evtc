using Avalonia;
using Avalonia.Styling;
using GW2Scratch.ArcdpsLogManager.Configuration;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Services
{
	/// <summary>
	/// Applies the configured <see cref="ApplicationTheme"/> to the running Avalonia application.
	/// </summary>
	public static class ThemeManager
	{
		public static ThemeVariant ToThemeVariant(ApplicationTheme theme) => theme switch
		{
			ApplicationTheme.Light => ThemeVariant.Light,
			ApplicationTheme.Dark => ThemeVariant.Dark,
			_ => ThemeVariant.Default,
		};

		/// <summary>
		/// Applies the given theme to the current application. <see cref="ApplicationTheme.System"/>
		/// maps to <see cref="ThemeVariant.Default"/>, which follows the operating system setting.
		/// </summary>
		public static void Apply(ApplicationTheme theme)
		{
			var app = Application.Current;
			if (app is not null)
			{
				app.RequestedThemeVariant = ToThemeVariant(theme);
			}
		}
	}
}
