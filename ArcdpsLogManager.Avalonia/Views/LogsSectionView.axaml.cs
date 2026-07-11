using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;
using GW2Scratch.ArcdpsLogManager.Configuration;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class LogsSectionView : UserControl
	{
		// Friendly expansions shown in the column show/hide menu (mirrors the Eto LogList).
		private static readonly Dictionary<string, string> Abbreviations = new()
		{
			{ "★", "Favorite" },
			{ "Instabilities", "Fractals of the Mists" },
			{ "Scale", "Fractals of the Mists" },
		};

		private DataGrid? logGrid;
		private readonly List<(string Header, CheckBox Check)> menuChecks = new();
		private EventHandler<EventArgs>? hiddenColumnsHandler;

		public LogsSectionView()
		{
			InitializeComponent();

			logGrid = this.FindControl<DataGrid>("LogGrid");
			if (logGrid == null)
			{
				return;
			}

			ApplyColumnVisibility();
			logGrid.ContextMenu = BuildColumnMenu();
			logGrid.KeyDown += OnGridKeyDown;
			logGrid.SelectionChanged += OnGridSelectionChanged;

			// Keep column visibility and menu checkmarks in sync with the setting.
			hiddenColumnsHandler = (_, _) => Dispatcher.UIThread.Post(() =>
			{
				ApplyColumnVisibility();
				RefreshMenuChecks();
			});
			Settings.HiddenLogListColumnsChanged += hiddenColumnsHandler;
			DetachedFromVisualTree += (_, _) =>
			{
				if (hiddenColumnsHandler != null)
				{
					Settings.HiddenLogListColumnsChanged -= hiddenColumnsHandler;
				}
			};
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		// Delete-key deletion (Avalonia counterpart of the Eto LogList Delete handler).
		private async void OnGridKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key != Key.Delete || logGrid == null)
			{
				return;
			}

			var rows = logGrid.SelectedItems.OfType<LogRow>().ToList();
			if (rows.Count == 0)
			{
				return;
			}

			// The shell view model owns the shared log collection and performs the deletion.
			if (TopLevel.GetTopLevel(this) is Window window && window.DataContext is MainWindowViewModel shell)
			{
				await shell.DeleteLogsAsync(rows, window, async logsToDelete =>
				{
					var dialog = new UnreliableLogsWindow
					{
						DataContext = new UnreliableLogsWindowViewModel(logsToDelete, shell.NameProvider),
					};
					return await dialog.ShowDialog<bool>(window);
				});
			}
		}

		// Multi-selection wiring (Avalonia counterpart of the Eto LogList's
		// GridView.SelectionChanged -> RefreshSelectionForDetailPanels). The DataGrid's
		// SelectedItem/SelectedLog binding continues to drive the single-log detail panel; this
		// additionally tells the view model how many rows are selected so it can show the
		// multi-log aggregate panel instead once there are 2+.
		private void OnGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
		{
			if (logGrid == null || DataContext is not LogsSectionViewModel viewModel)
			{
				return;
			}

			var selected = logGrid.SelectedItems.OfType<LogRow>().ToList();
			viewModel.UpdateSelection(selected);
		}

		private static string HeaderText(DataGridColumn column) => column.Header?.ToString() ?? "";

		private void ApplyColumnVisibility()
		{
			if (logGrid == null)
			{
				return;
			}

			var hidden = Settings.HiddenLogListColumns;
			foreach (var column in logGrid.Columns)
			{
				column.IsVisible = !hidden.Contains(HeaderText(column));
			}
		}

		private void RefreshMenuChecks()
		{
			foreach (var (header, check) in menuChecks)
			{
				check.IsChecked = !Settings.HiddenLogListColumns.Contains(header);
			}
		}

		private ContextMenu BuildColumnMenu()
		{
			var menu = new ContextMenu();
			var items = new List<object>();

			foreach (var column in logGrid!.Columns)
			{
				string header = HeaderText(column);

				string text = header;
				if (Abbreviations.TryGetValue(header, out var full))
				{
					text = string.IsNullOrWhiteSpace(header) ? full : $"{header} ({full})";
				}

				var check = new CheckBox
				{
					IsChecked = !Settings.HiddenLogListColumns.Contains(header),
					IsHitTestVisible = false,
				};
				menuChecks.Add((header, check));

				var item = new MenuItem { Header = text, Icon = check };
				item.Click += (_, _) =>
				{
					bool currentlyShown = !Settings.HiddenLogListColumns.Contains(header);
					Settings.HiddenLogListColumns = currentlyShown
						? Settings.HiddenLogListColumns.Append(header).Distinct().ToList()
						: Settings.HiddenLogListColumns.Where(x => x != header).ToList();
				};

				items.Add(item);
			}

			menu.ItemsSource = items;
			return menu;
		}
	}
}
