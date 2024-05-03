using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using Image = Eto.Drawing.Image;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public class LogEncounterFilterTree : TreeGridView
	{
		private sealed class GroupFilterTreeItem : TreeGridItem
		{
			public LogGroup LogGroup { get; }
			public Image Icon { get; set; }
			public int LogCount { get; set; }

			public GroupFilterTreeItem(LogGroup group)
			{
				LogGroup = group;

				if (LogGroup is RootLogGroup or RaidLogGroup or StrikeMissionLogGroup or FractalLogGroup)
				{
					Expanded = true;
				}

				foreach (var subgroup in group.Subgroups)
				{
					Children.Add(new GroupFilterTreeItem(subgroup));
				}
			}
		}

		/// <summary>
		/// Encounters hidden by default unless there is at least one log with this encounter.
		/// </summary>
		private IReadOnlyList<Encounter> HiddenEncounters { get; } = new[]
		{
			// An obsolete Encounter type, replaced by specialized ones.
#pragma warning disable 618
			Encounter.AiKeeperOfThePeak,
#pragma warning restore 618
			// An encounter that is not logged by default.
			Encounter.VariniaStormsounder,
			// An encounter that is not logged by default and has no proper support.
			Encounter.Escort,
			// The only kitty golem we show by default is the standard one.
			Encounter.MediumKittyGolem,
			Encounter.LargeKittyGolem,
			Encounter.MassiveKittyGolem,
			Encounter.Mordremoth,
		};

		/// <summary>
		/// Encounter categories hidden by default unless there is at least one log in the category.
		/// </summary>
		private IReadOnlyList<EncounterCategory> HiddenCategories { get; } = new[]
		{
			// An obsolete category.
			EncounterCategory.Festival,
			// An obscure category. May be confusing to users if they have not done any of the fights within.
			EncounterCategory.StrikeMissionFestival,
			// Not supported by all tools and not enabled by all users.
			EncounterCategory.Map,
			// No reason showing this if there is nothing to select inside.
			EncounterCategory.Other
		};

		private readonly ImageProvider imageProvider;
		private readonly ILogNameProvider logNameProvider;
		private LogFilters Filters { get; }

		public LogEncounterFilterTree(ImageProvider imageProvider, LogFilters filters, ILogNameProvider logNameProvider)
		{
			this.imageProvider = imageProvider;
			this.logNameProvider = logNameProvider;
			Filters = filters;

			AllowMultipleSelection = true;

			Columns.Add(new GridColumn()
			{
				HeaderText = "Category",
				DataCell = new ImageTextCell
				{
					TextBinding = Binding.Property<GroupFilterTreeItem, string>(x => x.LogGroup.Name),
					ImageBinding = Binding.Property<GroupFilterTreeItem, Image>(x => x.Icon),
				},
			});

			Columns.Add(new GridColumn()
			{
				HeaderText = "Count",
				DataCell = new ImageTextCell
				{
					TextBinding = Binding.Property<GroupFilterTreeItem, string>(x => x.LogCount.ToString())
				},
			});

			RowHeight = 22;

			SelectionChanged += (sender, args) =>
			{
				Filters.LogGroups = SelectedItems.Select(x => ((GroupFilterTreeItem) x).LogGroup).ToList();
			};

			var expandAll = new ButtonMenuItem {Text = "Expand all"};
			expandAll.Click += (sender, args) =>
			{
				DoForAllItems(item =>
				{
					item.Expanded = true;
				});
				ReloadData();
			};
			var collapseAll = new ButtonMenuItem {Text = "Collapse all"};
			collapseAll.Click += (sender, args) =>
			{
				DoForAllItems(item =>
				{
					item.Expanded = false;
				});
				ReloadData();
			};
			ContextMenu = new ContextMenu();
			ContextMenu.Items.Add(expandAll);
			ContextMenu.Items.Add(collapseAll);
		}

		private void DoForAllItems(Action<GroupFilterTreeItem> action)
		{
			if (DataStore.Count == 0)
			{
				return;
			}

			void DoForAllChildren(GroupFilterTreeItem item)
			{
				action(item);
				foreach (var child in item.Children)
				{
					DoForAllChildren((GroupFilterTreeItem) child);
				}
			}

			var root = (GroupFilterTreeItem) DataStore[0];
			DoForAllChildren(root);
		}

		/// <summary>
		/// Needs to be called to update selections that depend on the available logs, such as
		/// filtering by encounter name.
		/// </summary>
		/// <param name="logs">Logs to be available for filtering.</param>
		public void UpdateLogs(IReadOnlyList<LogData> logs)
		{
			// We construct a tree of groups. Everything is under an "All logs" node that can be used to easily select all.
			// All raid wings which have separate categories are within a meta group "Raids". Other than that, only
			// Encounter.Other logs have special handling - each unique target getting its own group.

			var rootGroup = new RootLogGroup(logs, logNameProvider);
			var rootItem = new GroupFilterTreeItem(rootGroup);

			void UpdateIcons(GroupFilterTreeItem item)
			{
				item.Icon = item.LogGroup switch
				{
					RootLogGroup => imageProvider.GetTinyLogIcon(),
					StrikeMissionLogGroup => imageProvider.GetTinyStrikeIcon(),
					RaidLogGroup => imageProvider.GetTinyRaidIcon(),
					FractalLogGroup => imageProvider.GetTinyFractalsIcon(),
					CategoryLogGroup categoryGroup => GetCategoryIcon(categoryGroup.Category),
					EncounterLogGroup encounterGroup => GetEncounterIcon(encounterGroup.Encounter),
					MapLogGroup mapGroup => imageProvider.GetWvWMapIcon(mapGroup.MapId),
					_ => item.Icon
				};

				foreach (var child in item.Children)
				{
					UpdateIcons((GroupFilterTreeItem) child);
				}
			}

			void UpdateCounts(GroupFilterTreeItem item)
			{
				item.LogCount = logs.Count(log => item.LogGroup.IsInGroup(log));

				foreach (var child in item.Children)
				{
					UpdateCounts((GroupFilterTreeItem) child);
				}
			}

			void PruneEmptyHiddenItems(GroupFilterTreeItem item)
			{
				// Removes items that have no logs associated with them and are undesirable for some reason.

				var toRemove = new List<GroupFilterTreeItem>();

				foreach (var child in item.Children)
				{
					var groupTreeItem = (GroupFilterTreeItem) child;
					bool removed = false;

					if (groupTreeItem.LogGroup is EncounterLogGroup encounterLogGroup)
					{
						if (HiddenEncounters.Contains(encounterLogGroup.Encounter) && groupTreeItem.LogCount == 0)
						{
							toRemove.Add(groupTreeItem);
							removed = true;
						}
					}
					else if (groupTreeItem.LogGroup is CategoryLogGroup categoryLogGroup)
					{
						if (HiddenCategories.Contains(categoryLogGroup.Category) && groupTreeItem.LogCount == 0)
						{
							toRemove.Add(groupTreeItem);
							removed = true;
						}
					}

					if (!removed)
					{
						PruneEmptyHiddenItems(groupTreeItem);
					}
				}

				foreach (var childToRemove in toRemove)
				{
					item.Children.Remove(childToRemove);
				}
			}

			UpdateIcons(rootItem);
			UpdateCounts(rootItem);
			PruneEmptyHiddenItems(rootItem);

			DataStore = new TreeGridItemCollection {rootItem};
		}

		private Image GetCategoryIcon(EncounterCategory category)
		{
			return category switch {
				EncounterCategory.Other => imageProvider.GetTinyUncategorizedIcon(),
				EncounterCategory.WorldVersusWorld => imageProvider.GetTinyWorldVersusWorldIcon(),
				EncounterCategory.Festival => imageProvider.GetTinyFestivalIcon(),
				EncounterCategory.Fractal => imageProvider.GetTinyFractalsIcon(),
				EncounterCategory.StrikeMissionIcebroodSaga => imageProvider.GetTinyIcebroodSagaIcon(),
				EncounterCategory.StrikeMissionEndOfDragons => imageProvider.GetTinyEndOfDragonsIcon(),
				EncounterCategory.StrikeMissionSecretsOfTheObscure => imageProvider.GetTinySecretsOfTheObscureIcon(),
				EncounterCategory.StrikeMissionFestival => imageProvider.GetTinyFestivalIcon(),
				EncounterCategory.SpecialForcesTrainingArea => imageProvider.GetTinyTrainingAreaIcon(),
				EncounterCategory.RaidWing1 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing2 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing3 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing4 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing5 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing6 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.RaidWing7 => imageProvider.GetRaidWingIcon(),
				EncounterCategory.FractalNightmare => imageProvider.GetFractalMapIcon(),
				EncounterCategory.FractalShatteredObservatory => imageProvider.GetFractalMapIcon(),
				EncounterCategory.FractalSunquaPeak => imageProvider.GetFractalMapIcon(),
				EncounterCategory.FractalSilentSurf => imageProvider.GetFractalMapIcon(),
				EncounterCategory.Map => imageProvider.GetTinyInstanceIcon(),
				_ => null
			};
		}

		private Image GetEncounterIcon(Encounter encounter)
		{
			return imageProvider.GetTinyEncounterIcon(encounter);
		}
	}
}