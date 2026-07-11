using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Uploads;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the Settings window (Avalonia counterpart of the Eto <c>SettingsForm</c> +
	/// its pages). Boolean/simple settings apply immediately through <see cref="ISettingsService"/>;
	/// the log directory list is applied via <see cref="ApplyDirectories"/>.
	/// </summary>
	public partial class SettingsWindowViewModel : ObservableObject
	{
		private const string MaskedUserToken = "************";

		public ISettingsService Settings { get; }

		public ApplicationTheme[] Themes { get; } =
			{ ApplicationTheme.System, ApplicationTheme.Light, ApplicationTheme.Dark };

		public ObservableCollection<string> LogDirectories { get; }

		[ObservableProperty] private bool excludeShortLogs;
		[ObservableProperty] private string minimumLogDurationText;

		/// <summary>Available dps.report upload domains, matching the Eto
		/// <c>DpsReportUploadSettingsPage</c>'s radio list. Includes the currently configured domain
		/// as an extra option if it no longer matches any known domain.</summary>
		public IReadOnlyList<DpsReportDomainOption> DpsReportDomainOptions { get; }

		[ObservableProperty] private string dpsReportDomainDescription = "";

		// The dps.report user token is masked by default; "Show" reveals it read-only, "Change"
		// makes it editable, matching the Eto page's show/change button flow exactly.
		private bool editingUserToken;
		[ObservableProperty] private string userTokenText = MaskedUserToken;
		[ObservableProperty] private bool isUserTokenEnabled;
		[ObservableProperty] private bool isUserTokenEditable;
		[ObservableProperty] private string userTokenButtonText = "Change";
		[ObservableProperty] private bool isShowUserTokenButtonVisible = true;

		public SettingsWindowViewModel(ISettingsService settings)
		{
			Settings = settings;
			LogDirectories = new ObservableCollection<string>(settings.LogRootPaths);
			excludeShortLogs = settings.MinimumLogDurationSeconds.HasValue;
			minimumLogDurationText = (settings.MinimumLogDurationSeconds ?? 5).ToString();

			var domains = DpsReportUploader.AvailableDomains;
			var currentDomain = domains.FirstOrDefault(x => x.Domain == settings.DpsReportDomain);
			if (currentDomain == null)
			{
				currentDomain = new DpsReportDomain(settings.DpsReportDomain, "");
				domains = domains.Append(currentDomain).ToList();
			}

			DpsReportDomainOptions = domains
				.Select(d => new DpsReportDomainOption(d, d == currentDomain))
				.ToList();
			DpsReportDomainDescription = currentDomain.Description;
			foreach (var option in DpsReportDomainOptions)
			{
				option.PropertyChanged += OnDpsReportDomainOptionPropertyChanged;
			}
		}

		private void OnDpsReportDomainOptionPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(DpsReportDomainOption.IsSelected) || sender is not DpsReportDomainOption option)
			{
				return;
			}

			if (!option.IsSelected)
			{
				return;
			}

			foreach (var other in DpsReportDomainOptions)
			{
				if (!ReferenceEquals(other, option))
				{
					other.IsSelected = false;
				}
			}

			DpsReportDomainDescription = option.Description;
			Settings.DpsReportDomain = option.Domain.Domain;
		}

		public void AddDirectory(string path)
		{
			if (!string.IsNullOrWhiteSpace(path) && !LogDirectories.Contains(path))
			{
				LogDirectories.Add(path);
				ApplyDirectories();
			}
		}

		[RelayCommand]
		private void RemoveDirectory(string? path)
		{
			if (path != null && LogDirectories.Remove(path))
			{
				ApplyDirectories();
			}
		}

		private void ApplyDirectories()
		{
			Settings.LogRootPaths = LogDirectories.ToList();
		}

		partial void OnExcludeShortLogsChanged(bool value)
		{
			if (!value)
			{
				Settings.MinimumLogDurationSeconds = null;
			}
			else if (int.TryParse(MinimumLogDurationText, out int seconds) && seconds >= 0)
			{
				Settings.MinimumLogDurationSeconds = seconds;
			}
		}

		partial void OnMinimumLogDurationTextChanged(string value)
		{
			if (ExcludeShortLogs && int.TryParse(value, out int seconds) && seconds >= 0)
			{
				Settings.MinimumLogDurationSeconds = seconds;
			}
		}

		[RelayCommand]
		private void ShowUserToken()
		{
			UserTokenText = Settings.DpsReportUserToken;
			IsUserTokenEnabled = true;
		}

		[RelayCommand]
		private void ToggleEditUserToken()
		{
			if (editingUserToken)
			{
				editingUserToken = false;
				Settings.DpsReportUserToken = UserTokenText;

				IsUserTokenEditable = false;
				UserTokenText = MaskedUserToken;
				IsUserTokenEnabled = false;
				UserTokenButtonText = "Change";
				IsShowUserTokenButtonVisible = true;
			}
			else
			{
				editingUserToken = true;

				IsUserTokenEditable = true;
				UserTokenText = Settings.DpsReportUserToken;
				IsUserTokenEnabled = true;
				UserTokenButtonText = "Save";
				IsShowUserTokenButtonVisible = false;
			}
		}
	}
}
