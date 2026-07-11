using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GW2Scratch.ArcdpsLogManager.Configuration;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Services
{
	/// <summary>
	/// Default <see cref="ISettingsService"/> implementation backed by the static
	/// <see cref="Settings"/> persistence layer.
	/// </summary>
	public sealed class SettingsService : ISettingsService
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		public SettingsService()
		{
			// Mirror the underlying static change events as INotifyPropertyChanged notifications.
			Settings.ThemeChanged += (_, _) => Raise(nameof(Theme));
			Settings.CompactUiChanged += (_, _) => Raise(nameof(CompactUi));
			Settings.ShowFilterSidebarChanged += (_, _) => Raise(nameof(ShowFilterSidebar));
			Settings.ShowDebugDataChanged += (_, _) => Raise(nameof(ShowDebugData));
			Settings.ShowGuildTagsInLogDetailChanged += (_, _) => Raise(nameof(ShowGuildTagsInLogDetail));
			Settings.ShowFailurePercentagesInLogListChanged += (_, _) => Raise(nameof(ShowFailurePercentagesInLogList));
			Settings.UseGW2ApiChanged += (_, _) => Raise(nameof(UseGW2Api));
			Settings.CheckForUpdatesChanged += (_, _) => Raise(nameof(CheckForUpdates));
			Settings.DpsReportUserTokenChanged += (_, _) => Raise(nameof(DpsReportUserToken));
			Settings.DpsReportAutoUploadChanged += (_, _) => Raise(nameof(DpsReportAutoUpload));
			Settings.DpsReportAutoUploadApplyFiltersChanged += (_, _) => Raise(nameof(DpsReportAutoUploadApplyFilters));
			Settings.DpsReportUploadDetailedWvwChanged += (_, _) => Raise(nameof(DpsReportUploadDetailedWvw));
			Settings.DpsReportDomainChanged += (_, _) => Raise(nameof(DpsReportDomain));
			Settings.MinimumLogDurationSecondsChanged += (_, _) => Raise(nameof(MinimumLogDurationSeconds));
			Settings.LogRootPathChanged += (_, _) => Raise(nameof(LogRootPaths));
		}

		public IReadOnlyList<string> LogRootPaths
		{
			get => Settings.LogRootPaths;
			set => Settings.LogRootPaths = value;
		}

		public ApplicationTheme Theme
		{
			get => Settings.Theme;
			set => Settings.Theme = value;
		}

		public bool CompactUi
		{
			get => Settings.CompactUi;
			set => Settings.CompactUi = value;
		}

		public bool ShowFilterSidebar
		{
			get => Settings.ShowFilterSidebar;
			set => Settings.ShowFilterSidebar = value;
		}

		public bool ShowDebugData
		{
			get => Settings.ShowDebugData;
			set => Settings.ShowDebugData = value;
		}

		public bool ShowGuildTagsInLogDetail
		{
			get => Settings.ShowGuildTagsInLogDetail;
			set => Settings.ShowGuildTagsInLogDetail = value;
		}

		public bool ShowFailurePercentagesInLogList
		{
			get => Settings.ShowFailurePercentagesInLogList;
			set => Settings.ShowFailurePercentagesInLogList = value;
		}

		public bool UseGW2Api
		{
			get => Settings.UseGW2Api;
			set => Settings.UseGW2Api = value;
		}

		public bool CheckForUpdates
		{
			get => Settings.CheckForUpdates;
			set => Settings.CheckForUpdates = value;
		}

		public string DpsReportUserToken
		{
			get => Settings.DpsReportUserToken;
			set => Settings.DpsReportUserToken = value;
		}

		public bool DpsReportAutoUpload
		{
			get => Settings.DpsReportAutoUpload;
			set => Settings.DpsReportAutoUpload = value;
		}

		public bool DpsReportAutoUploadApplyFilters
		{
			get => Settings.DpsReportAutoUploadApplyFilters;
			set => Settings.DpsReportAutoUploadApplyFilters = value;
		}

		public bool DpsReportUploadDetailedWvw
		{
			get => Settings.DpsReportUploadDetailedWvw;
			set => Settings.DpsReportUploadDetailedWvw = value;
		}

		public string DpsReportDomain
		{
			get => Settings.DpsReportDomain;
			set => Settings.DpsReportDomain = value;
		}

		public int? MinimumLogDurationSeconds
		{
			get => Settings.MinimumLogDurationSeconds;
			set => Settings.MinimumLogDurationSeconds = value;
		}

		private void Raise([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
