using System.Collections.Generic;
using System.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Configuration;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Services
{
	/// <summary>
	/// Provides access to the application settings as observable properties so that views and
	/// view models can bind to them directly instead of subscribing to the many individual
	/// change events on the static <see cref="Settings"/> class.
	/// </summary>
	/// <remarks>
	/// The underlying persistence still happens through the static <see cref="Settings"/> class
	/// (JSON file on disk). This service is a thin, bindable wrapper over it. Property setters
	/// write through to <see cref="Settings"/>, which persists and raises its own change events;
	/// those events are mirrored here as <see cref="INotifyPropertyChanged"/> notifications.
	/// </remarks>
	public interface ISettingsService : INotifyPropertyChanged
	{
		ApplicationTheme Theme { get; set; }
		bool CompactUi { get; set; }
		bool ShowFilterSidebar { get; set; }
		bool ShowDebugData { get; set; }
		bool ShowGuildTagsInLogDetail { get; set; }
		bool ShowFailurePercentagesInLogList { get; set; }
		bool UseGW2Api { get; set; }
		bool CheckForUpdates { get; set; }
		string DpsReportUserToken { get; set; }
		bool DpsReportAutoUpload { get; set; }
		bool DpsReportAutoUploadApplyFilters { get; set; }
		bool DpsReportUploadDetailedWvw { get; set; }
		string DpsReportDomain { get; set; }
		int? MinimumLogDurationSeconds { get; set; }
		IReadOnlyList<string> LogRootPaths { get; set; }
	}
}
