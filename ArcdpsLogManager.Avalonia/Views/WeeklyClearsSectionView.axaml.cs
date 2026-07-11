using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Sections.Clears;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	public partial class WeeklyClearsSectionView : UserControl
	{
		// Maps each week-grid column's header prefix to the encounter category it belongs to, so its
		// visibility can mirror Settings.WeeklyClearGroups (Avalonia counterpart of the Eto
		// WeeklyClears' per-column GridColumn.Visible flag, driven by the same setting). DataGridColumn
		// is not part of the visual tree, so this is done in code-behind rather than a XAML binding,
		// matching the existing LogsSectionView column show/hide pattern.
		private static readonly (string HeaderPrefix, EncounterCategory Category)[] CategoryColumnPrefixes =
		{
			("Raid ", EncounterCategory.Raids),
			("IBS ", EncounterCategory.RaidEncountersIcebroodSaga),
			("EoD ", EncounterCategory.RaidEncountersEndOfDragons),
			("SotO ", EncounterCategory.RaidEncountersSecretsOfTheObscure),
			("VoE ", EncounterCategory.RaidEncountersVisionsOfEternity),
		};

		private readonly DataGrid? weekGrid;
		private EventHandler<EventArgs>? groupsChangedHandler;

		public WeeklyClearsSectionView()
		{
			InitializeComponent();

			weekGrid = this.FindControl<DataGrid>("WeekGrid");
			if (weekGrid == null)
			{
				return;
			}

			ApplyColumnVisibility();

			groupsChangedHandler = (_, _) => Dispatcher.UIThread.Post(ApplyColumnVisibility);
			Settings.WeeklyClearGroupsChanged += groupsChangedHandler;
			DetachedFromVisualTree += (_, _) =>
			{
				if (groupsChangedHandler != null)
				{
					Settings.WeeklyClearGroupsChanged -= groupsChangedHandler;
				}
			};
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		// Opens the player-picker dialog (Avalonia counterpart of the Eto WeeklyClears'
		// addNewAccountButton constructing a PlayerSelectDialog) instead of typing a raw account
		// name; the window itself is constructed here rather than in the view model, matching this
		// project's existing convention for dialogs that need constructing from a Click handler.
		private async void OnAddAccountClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is not WeeklyClearsSectionViewModel vm || TopLevel.GetTopLevel(this) is not Window window)
			{
				return;
			}

			var dialog = new PlayerSelectWindow
			{
				DataContext = new PlayerSelectWindowViewModel(vm.LoadedLogs, vm.Images),
			};
			var selectedAccountName = await dialog.ShowDialog<string?>(window);
			if (selectedAccountName != null)
			{
				vm.AddAccountByName(selectedAccountName);
			}
		}

		private void ApplyColumnVisibility()
		{
			if (weekGrid == null)
			{
				return;
			}

			var enabledGroups = Settings.WeeklyClearGroups;
			foreach (var column in weekGrid.Columns)
			{
				string header = column.Header?.ToString() ?? "";
				foreach (var (prefix, category) in CategoryColumnPrefixes)
				{
					if (header.StartsWith(prefix, StringComparison.Ordinal))
					{
						column.IsVisible = enabledGroups.Contains(category);
						break;
					}
				}
			}
		}
	}
}
