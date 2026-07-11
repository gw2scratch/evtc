using System;
using System.Collections.Generic;
using System.Linq;
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
			logGrid.ContextMenu = BuildColumnMenu();
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
