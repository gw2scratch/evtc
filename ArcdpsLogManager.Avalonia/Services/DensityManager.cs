using System.Linq;
using Avalonia;
using Avalonia.Themes.Fluent;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Services
{
	/// <summary>
	/// Applies FluentTheme's built-in compact control density (<c>DensityStyle.Compact</c>) to the
	/// running application. This is Avalonia's own answer to "the tabs/buttons/etc look huge" —
	/// FluentTheme's default (<c>DensityStyle.Normal</c>) sizes controls generously; Compact shrinks
	/// padding/min-heights app-wide (TabItem included) without us hand-tuning every control's style.
	/// </summary>
	public static class DensityManager
	{
		public static void Apply(bool compact)
		{
			var app = Application.Current;
			var fluentTheme = app?.Styles.OfType<FluentTheme>().FirstOrDefault();
			if (fluentTheme != null)
			{
				fluentTheme.DensityStyle = compact ? DensityStyle.Compact : DensityStyle.Normal;
			}
		}
	}
}
