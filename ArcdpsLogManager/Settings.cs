using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace ArcdpsLogManager
{
	public class Settings
	{
		private static ISettings AppSettings => CrossSettings.Current;

		public static string LogRootPath
		{
			get => AppSettings.GetValueOrDefault(nameof(LogRootPath), string.Empty);

			set => AppSettings.AddOrUpdateValue(nameof(LogRootPath), value);
		}
	}
}