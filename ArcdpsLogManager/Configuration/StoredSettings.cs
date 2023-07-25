using GW2Scratch.ArcdpsLogManager.Uploads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

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
	public bool DpsReportUploadDetailedWvw { get; set; } = false;
	public int? MinimumLogDurationSeconds { get; set; } = null;
	public List<string> HiddenLogListColumns { get; set; } = new List<string>() {"Character", "Map ID", "Game Version", "arcdps Version", "Instabilities", "Scale"};
	public List<string> IgnoredUpdateVersions { get; set; } = new List<string>();
}