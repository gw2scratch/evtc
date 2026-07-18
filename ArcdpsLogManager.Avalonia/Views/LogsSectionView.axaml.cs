using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Processing;

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

		// Built once and reused for every header right-click; the row menu below is rebuilt per
		// right-click instead, since its contents (Reparse/upload/Delete availability) depend on
		// the row(s) under the pointer at the time.
		private ContextMenu? columnMenu;

		// Click-and-drag row selection (Avalonia's DataGrid only supports click / ctrl+click /
		// shift+click natively; the Eto GridView also supported dragging across rows to select
		// a contiguous range, so we reimplement that gesture here).
		private bool isDragSelecting;
		private LogRow? dragAnchorItem;

		public LogsSectionView()
		{
			InitializeComponent();

			logGrid = this.FindControl<DataGrid>("LogGrid");
			if (logGrid == null)
			{
				return;
			}

			ApplyColumnVisibility();
			columnMenu = BuildColumnMenu();
			// Assigning ContextMenu here (rather than only inside OnGridContextRequested) primes
			// Avalonia's context-menu machinery ahead of time: setting the property for the very
			// first time during the same right-click gesture that's supposed to open it doesn't
			// take effect synchronously, so without this the first right-click of the app silently
			// does nothing and only the second one onward opens a menu.
			logGrid.ContextMenu = columnMenu;
			logGrid.KeyDown += OnGridKeyDown;
			logGrid.SelectionChanged += OnGridSelectionChanged;

			// Tunnel + handledEventsToo so these always run, before/regardless of the DataGrid's
			// own internal pointer handling (which selects the anchor row and may mark the event
			// handled).
			logGrid.AddHandler(InputElement.PointerPressedEvent, OnGridPointerPressedForDrag,
				RoutingStrategies.Tunnel, handledEventsToo: true);
			logGrid.AddHandler(InputElement.PointerMovedEvent, OnGridPointerMovedForDrag,
				RoutingStrategies.Tunnel, handledEventsToo: true);
			logGrid.AddHandler(InputElement.PointerReleasedEvent, OnGridPointerReleasedForDrag,
				RoutingStrategies.Tunnel, handledEventsToo: true);

			// Decide which context menu applies (column show/hide on the header, row actions on a
			// data row) before the built-in bubble-phase handling opens whichever menu is currently
			// assigned to logGrid.ContextMenu.
			logGrid.AddHandler(ContextRequestedEvent, OnGridContextRequested,
				RoutingStrategies.Tunnel, handledEventsToo: true);
			logGrid.PointerCaptureLost += (_, _) =>
			{
				isDragSelecting = false;
				dragAnchorItem = null;
			};

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
			await DeleteRowsAsync(rows);
		}

		// Shared by the Delete key and the row context menu's "Delete" item.
		private async Task DeleteRowsAsync(IReadOnlyList<LogRow> rows)
		{
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

		private MainWindowViewModel? GetShell() =>
			TopLevel.GetTopLevel(this) is Window { DataContext: MainWindowViewModel shell } ? shell : null;

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

		// Starts a potential drag-select: records the row under the pointer as the anchor. The
		// native single-row selection still runs afterward for this same click (we don't mark the
		// event handled), so a plain click keeps working exactly as before; this only adds
		// behavior when the pointer subsequently moves while still pressed.
		private void OnGridPointerPressedForDrag(object? sender, PointerPressedEventArgs e)
		{
			if (logGrid == null)
			{
				return;
			}

			var point = e.GetCurrentPoint(logGrid);
			if (!point.Properties.IsLeftButtonPressed || e.KeyModifiers != KeyModifiers.None)
			{
				// Ctrl/Shift-click keep their native add-to-selection/range-select behavior untouched.
				return;
			}

			var hit = logGrid.InputHitTest(point.Position) as Visual;
			if (hit?.FindAncestorOfType<Button>(includeSelf: true) != null)
			{
				// Don't hijack clicks on the favorite-star toggle button.
				return;
			}

			if (hit?.FindAncestorOfType<DataGridRow>(includeSelf: true)?.DataContext is not LogRow item)
			{
				return;
			}

			dragAnchorItem = item;
			isDragSelecting = true;
			e.Pointer.Capture(logGrid);
		}

		private void OnGridPointerMovedForDrag(object? sender, PointerEventArgs e)
		{
			if (!isDragSelecting || logGrid == null || dragAnchorItem == null)
			{
				return;
			}

			var point = e.GetCurrentPoint(logGrid);
			if (!point.Properties.IsLeftButtonPressed)
			{
				isDragSelecting = false;
				return;
			}

			// All rows currently realized on screen, top to bottom. Virtualized-away rows outside
			// the viewport aren't included; dragging stays within the visible area since there's no
			// auto-scroll, so this is always the complete set relevant to the current drag.
			var rows = logGrid.GetVisualDescendants().OfType<DataGridRow>()
				.OrderBy(r => r.Bounds.Top)
				.ToList();

			int anchorIndex = rows.FindIndex(r => ReferenceEquals(r.DataContext, dragAnchorItem));
			if (anchorIndex < 0)
			{
				// The anchor row scrolled out of view; leave the current selection as-is.
				return;
			}

			var hitRow = (logGrid.InputHitTest(point.Position) as Visual)?.FindAncestorOfType<DataGridRow>(includeSelf: true);
			int currentIndex;
			if (hitRow != null)
			{
				currentIndex = rows.FindIndex(r => ReferenceEquals(r, hitRow));
			}
			else if (rows.Count > 0)
			{
				// Dragged above the first row or below the last one: clamp to that edge so
				// dragging past the visible rows still extends the selection.
				currentIndex = point.Position.Y < rows[0].Bounds.Top ? 0 : rows.Count - 1;
			}
			else
			{
				return;
			}

			int lo = Math.Min(anchorIndex, currentIndex);
			int hi = Math.Max(anchorIndex, currentIndex);

			logGrid.SelectedItems.Clear();
			foreach (var row in rows.Skip(lo).Take(hi - lo + 1))
			{
				if (row.DataContext is LogRow rowItem)
				{
					logGrid.SelectedItems.Add(rowItem);
				}
			}
		}

		private void OnGridPointerReleasedForDrag(object? sender, PointerReleasedEventArgs e)
		{
			isDragSelecting = false;
			dragAnchorItem = null;
			e.Pointer.Capture(null);
		}

		// Decides, ahead of the built-in bubble-phase handling that actually opens the menu,
		// whether a right-click landed on a column header (show the column show/hide menu) or a
		// data row (show row actions: Reparse / upload status / Delete). Elsewhere (e.g. below the
		// last row) shows no menu at all.
		private void OnGridContextRequested(object? sender, ContextRequestedEventArgs e)
		{
			if (logGrid == null || !e.TryGetPosition(logGrid, out var position))
			{
				return;
			}

			var hit = logGrid.InputHitTest(position) as Visual;

			if (hit?.FindAncestorOfType<DataGridColumnHeader>(includeSelf: true) != null)
			{
				logGrid.ContextMenu = columnMenu;
				return;
			}

			if (hit?.FindAncestorOfType<DataGridRow>(includeSelf: true)?.DataContext is LogRow logRow)
			{
				// Right-clicking a row outside the current selection replaces it, matching typical
				// list/grid context-menu behavior; right-clicking within an existing multi-selection
				// keeps it so the menu applies to the whole selection.
				if (!logGrid.SelectedItems.Contains(logRow))
				{
					logGrid.SelectedItems.Clear();
					logGrid.SelectedItems.Add(logRow);
				}

				logGrid.ContextMenu = BuildRowMenu(logGrid.SelectedItems.OfType<LogRow>().ToList());
				return;
			}

			logGrid.ContextMenu = null;
		}

		// Row actions: Reparse (re-processes the local file, keeping any existing upload URL),
		// upload status (initiates an upload when missing, copies the URL once uploaded), and
		// Delete (removes from the view and deletes the file from disk).
		private ContextMenu BuildRowMenu(List<LogRow> rows)
		{
			var shell = GetShell();
			var menu = new ContextMenu();
			var items = new List<object>();

			var processingService = shell?.Processing;
			var reparseItem = new MenuItem { Header = "Reparse", IsEnabled = processingService != null };
			reparseItem.Click += (_, _) =>
			{
				if (processingService == null)
				{
					return;
				}

				foreach (var row in rows)
				{
					row.MarkProcessing();
					processingService.ScheduleReprocessing(row.Log);
				}
			};
			items.Add(reparseItem);

			items.Add(BuildUploadMenuItem(rows, processingService?.UploadProcessor));

			items.Add(new Separator());

			var deleteItem = new MenuItem { Header = "Delete" };
			deleteItem.Click += async (_, _) => await DeleteRowsAsync(rows);
			items.Add(deleteItem);

			menu.ItemsSource = items;
			return menu;
		}

		private MenuItem BuildUploadMenuItem(List<LogRow> rows, UploadProcessor? uploadProcessor)
		{
			var logs = rows.Select(r => r.Log).ToList();
			int missing = logs.Count(x => x.DpsReportEIUpload.UploadState
				is UploadState.NotUploaded or UploadState.UploadError or UploadState.ProcessingError);
			int uploading = logs.Count(x => x.DpsReportEIUpload.UploadState
				is UploadState.Queued or UploadState.Uploading);

			var item = new MenuItem();

			if (missing > 0)
			{
				item.Header = missing == 1 ? "Not uploaded" : $"Not uploaded ({missing})";
				item.IsEnabled = uploadProcessor != null;
				item.Click += (_, _) =>
				{
					if (uploadProcessor == null)
					{
						return;
					}

					foreach (var log in logs)
					{
						var state = log.DpsReportEIUpload.UploadState;
						if (state is UploadState.NotUploaded or UploadState.UploadError or UploadState.ProcessingError)
						{
							uploadProcessor.ScheduleDpsReportEIUpload(log);
						}
					}
				};
			}
			else if (uploading > 0)
			{
				item.Header = "Uploading...";
				item.IsEnabled = false;
			}
			else
			{
				item.Header = rows.Count > 1 ? "Copy URLs" : "Copy URL";
				item.Click += async (_, _) =>
				{
					string text = string.Join(Environment.NewLine,
						logs.Where(x => x.DpsReportEIUpload.Url != null).Select(x => x.DpsReportEIUpload.Url!));
					if (string.IsNullOrEmpty(text))
					{
						return;
					}

					var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
					if (clipboard != null)
					{
						await clipboard.SetTextAsync(text);
					}
				};
			}

			return item;
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
