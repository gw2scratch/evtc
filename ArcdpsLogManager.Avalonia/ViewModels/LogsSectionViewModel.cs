using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the Logs section (Avalonia counterpart of the Eto <c>Sections/LogList.cs</c>):
	/// the log grid plus the single-log detail panel (and the multi-selection aggregate panel).
	/// Rows are supplied by the shell (<see cref="MainWindowViewModel"/>), which owns the shared
	/// cache/services.
	/// </summary>
	public partial class LogsSectionViewModel : ObservableObject
	{
		private readonly LogCacheService cacheService;

		[ObservableProperty] private string statusText = "Loading log cache…";

		[ObservableProperty] private LogRow? selectedLog;

		/// <summary>True when 2+ logs are selected in the grid — the view swaps <see cref="Detail"/>
		/// for <see cref="MultipleDetail"/> (Avalonia counterpart of how the Eto <c>LogList</c> swaps
		/// its <c>LogDetailPanel</c>/<c>MultipleLogPanel</c> visibility).</summary>
		[ObservableProperty] private bool hasMultipleSelection;

		public ObservableCollection<LogRow> Logs { get; } = new();

		public LogDetailPanelViewModel Detail { get; }

		/// <summary>The aggregate panel shown instead of <see cref="Detail"/> when multiple logs are
		/// selected (Avalonia counterpart of the Eto <c>Controls/MultipleLogPanel.cs</c>).</summary>
		public MultipleLogPanelViewModel MultipleDetail { get; }

		public LogsSectionViewModel(ImageProvider images, ILogNameProvider nameProvider, LogCacheService cacheService,
			ApiData apiData)
		{
			this.cacheService = cacheService;
			Detail = new LogDetailPanelViewModel(images, nameProvider, cacheService, apiData);
			MultipleDetail = new MultipleLogPanelViewModel(cacheService, images);
		}

		/// <summary>
		/// Replaces the grid contents with the given rows (called on the UI thread). Synchronizes
		/// <see cref="Logs"/> in place (Add/Remove/Move) rather than clearing and re-adding
		/// everything: a full Clear() resets the DataGrid's selection, which would otherwise make
		/// every filter change or reparse drop whatever the user had selected.
		/// </summary>
		public void SetLogs(IReadOnlyList<LogRow> rows)
		{
			if (Logs.Count == 0)
			{
				foreach (var row in rows)
				{
					Logs.Add(row);
				}
				return;
			}

			var desired = new HashSet<LogRow>(rows);
			for (int i = Logs.Count - 1; i >= 0; i--)
			{
				if (!desired.Contains(Logs[i]))
				{
					Logs.RemoveAt(i);
				}
			}

			for (int i = 0; i < rows.Count; i++)
			{
				var row = rows[i];
				if (i < Logs.Count && ReferenceEquals(Logs[i], row))
				{
					continue;
				}

				int currentIndex = Logs.IndexOf(row);
				if (currentIndex < 0)
				{
					Logs.Insert(i, row);
				}
				else
				{
					Logs.Move(currentIndex, i);
				}
			}
		}

		partial void OnSelectedLogChanged(LogRow? value)
		{
			Detail.Show(value);
		}

		/// <summary>
		/// Called by the view whenever the grid's multi-selection changes (the grid uses
		/// <c>SelectionMode="Extended"</c>). Zero or one selected logs behave as before (the single
		/// <see cref="SelectedLog"/> binding drives <see cref="Detail"/>); two or more show
		/// <see cref="MultipleDetail"/> instead.
		/// </summary>
		public void UpdateSelection(IReadOnlyList<LogRow> selectedRows)
		{
			HasMultipleSelection = selectedRows.Count >= 2;
			if (HasMultipleSelection)
			{
				MultipleDetail.Show(selectedRows.Select(r => r.Log).ToList());
			}
		}

		/// <summary>Toggles the favorite state of a log and persists it through the cache.</summary>
		[RelayCommand]
		private void ToggleFavorite(LogRow? row)
		{
			if (row is null)
			{
				return;
			}

			row.Favorite = !row.Favorite;
			row.Log.IsFavorite = row.Favorite;
			cacheService.NotifyChanged(row.Log);
		}
	}
}
