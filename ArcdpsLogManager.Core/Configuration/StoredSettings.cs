using GW2Scratch.ArcdpsLogManager.Sections.Clears;
using GW2Scratch.ArcdpsLogManager.Uploads;
using System;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Configuration;

/// <summary>
/// A class that is serialized to store the settings. Also used to define default values.
/// </summary>
public class StoredSettings
{
	public Version ManagerVersion { get; set; } = null;
	public List<string> LogRootPaths { get; set; } = new List<string>();
	public bool ShowDebugData { get; set; } = false;
	public bool ShowGuildTagsInLogDetail { get; set; } = false;
	public bool ShowFailurePercentagesInLogList { get; set; } = true;
	public bool ShowFilterSidebar { get; set; } = true;
	public bool UseGW2Api { get; set; } = true;
	public bool CheckForUpdates { get; set; } = true;
	public string DpsReportUserToken { get; set; } = string.Empty;
	public string DpsReportDomain { get; set; } = DpsReportUploader.DefaultDomain.Domain;
	public bool DpsReportAutoUpload { get; set; } = false;
	public bool DpsReportUploadDetailedWvw { get; set; } = false;
	public bool DpsReportAutoUploadApplyFilters { get; set; } = false;
	public int? MinimumLogDurationSeconds { get; set; } = null;

	/// <summary>
	/// The preferred application color theme. Used by the Avalonia UI; ignored by the legacy Eto UI.
	/// </summary>
	public ApplicationTheme Theme { get; set; } = ApplicationTheme.System;

	/// <summary>
	/// Whether to use FluentTheme's compact control density. Used by the Avalonia UI; ignored by
	/// the legacy Eto UI.
	/// </summary>
	public bool CompactUi { get; set; } = false;

	/// <summary>
	/// Sidebar filter section visibility toggles. Used by the Avalonia UI; ignored by the legacy
	/// Eto UI, which has no equivalent per-section visibility control.
	/// </summary>
	public bool ShowFavoritesFilter { get; set; } = true;
	public bool ShowPlayerCountFilter { get; set; } = true;
	public bool ShowDateRangeFilter { get; set; } = true;

	/// <summary>
	/// Whether tag-related UI (sidebar filter, log detail panel, multi-selection panel) is shown
	/// at all. Used by the Avalonia UI; ignored by the legacy Eto UI.
	/// </summary>
	public bool ShowTags { get; set; } = true;

	/// <summary>
	/// Persisted main-window placement. Used by the Avalonia UI; ignored by the legacy Eto UI.
	/// </summary>
	public WindowStateInfo WindowState { get; set; } = new WindowStateInfo();

	public List<string> HiddenLogListColumns { get; set; } =
	[
		"Character",
		"Account",
		"Map ID",
		"Game Version",
		"arcdps Version",
		"Instabilities",
		"Scale",
	];

	public List<string> IgnoredUpdateVersions { get; set; } = [];
	public List<string> PlayerAccountNames { get; set; } = [];

	public List<EncounterCategory> WeeklyClearGroups { get; set; } =
	[
		EncounterCategory.Raids, 
		EncounterCategory.RaidEncountersEndOfDragons, 
		EncounterCategory.RaidEncountersSecretsOfTheObscure,
		EncounterCategory.RaidEncountersVisionsOfEternity,
	];
}