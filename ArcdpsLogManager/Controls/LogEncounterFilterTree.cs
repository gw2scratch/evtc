using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Gtk;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups;
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

				if (LogGroup is RootLogGroup || LogGroup is RaidLogGroup)
				{
					Expanded = true;
				}

				foreach (var subgroup in group.Subgroups)
				{
					Children.Add(new GroupFilterTreeItem(subgroup));
				}
			}
		}

		private readonly ImageProvider imageProvider;
		private LogFilters Filters { get; }

		public LogEncounterFilterTree(ImageProvider imageProvider, LogFilters filters)
		{
			this.imageProvider = imageProvider;
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

			var rootGroup = new RootLogGroup(logs);
			var rootItem = new GroupFilterTreeItem(rootGroup);

			void UpdateIcons(GroupFilterTreeItem item)
			{
				item.Icon = item.LogGroup switch
				{
					RootLogGroup _ => imageProvider.GetTinyLogIcon(),
					RaidLogGroup _ => imageProvider.GetTinyRaidIcon(),
					CategoryLogGroup categoryGroup => GetCategoryIcon(categoryGroup.Category),
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

			UpdateIcons(rootItem);
			UpdateCounts(rootItem);

			DataStore = new TreeGridItemCollection {rootItem};
		}

		private Image GetCategoryIcon(EncounterCategory category)
		{
			if (category == EncounterCategory.Fractal)
			{
				return imageProvider.GetTinyFractalsIcon();
			}

			return null;
		}
	}
}