using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCInspector
{
	public class EventListControl : Panel
	{
		public class TypeFilterItem : TreeGridItem
		{
			public Type Type { get; }

			private bool? shown;
			private int settingRecursionLevel = 0;

			public bool? Checked
			{
				get => shown;
				set
				{
					if (shown == value) return;

					shown = value;

					if (shown.HasValue)
					{
						foreach (var item in Children)
						{
							var child = (TypeFilterItem) item;
							child.settingRecursionLevel += 1;
							child.Checked = shown.Value;
							child.settingRecursionLevel -= 1;
						}
					}

					if (Parent != null)
					{
						var parent = (TypeFilterItem) Parent;
						bool? newValue;
						
						bool isAbstract = parent.Type.IsAbstract;

						if (parent.Children.All(x => ((TypeFilterItem) x).Checked == true) && isAbstract)
						{
							newValue = true;
						}
						else if (parent.Children.All(x => ((TypeFilterItem) x).Checked == false) && isAbstract)
						{
							newValue = false;
						}
						else
						{
							newValue = null;
						}

						if (newValue != null)
						{
							parent.settingRecursionLevel += 1;
							parent.Checked = newValue;
							parent.settingRecursionLevel -= 1;
						}
					}

					if (settingRecursionLevel == 0)
					{
						UsedDirectly?.Invoke(this, EventArgs.Empty);
					}
				}
			}

			public event EventHandler<EventArgs> UsedDirectly;

			private int count = -1;

			public new int Count
			{
				get
				{
					if (count == -1)
					{
						count = Children.Sum(x => ((TypeFilterItem) x).Count);
					}

					return count;
				}
			}

			// Creates a filter item that is not present in the data, count is taken as sum of children.
			public TypeFilterItem(Type type)
			{
				Type = type;
			}

			public TypeFilterItem(Type type, int count)
			{
				Type = type;
				this.count = count;
			}
		}

		public ICollection<Event> Events
		{
			get => events;
			set
			{
				events.Clear();
				events.AddRange(value);

				eventGridView.DataStore = new FilterCollection<Event>(events) {Filter = GetEventFilter};

				UpdateFilterView();
			}
		}

		public ICollection<Agent> Agents
		{
			get => agents;
			set
			{
				agents.Clear();
				agents.AddRange(value);
			}
		}

		public bool ShowTimeSinceFirstEvent { get; set; } = true;
		public long TimeOfOldestEvent { get; set; } = 0;
		
		private readonly List<Event> events = new List<Event>();
		private readonly List<Agent> agents = new List<Agent>();

		public TypeFilterItem[] TypeFilters { get; private set; }

		private readonly EventContentFilterControl contentFilters = new EventContentFilterControl();
		private readonly GridView<Event> eventGridView;
		private readonly TreeGridView typeFilterTree;
		private readonly PropertyGrid eventPropertyGrid = new PropertyGrid
		{
			ShowCategories = false, ShowDescription = true,
		};
		private readonly TabControl tabs = new TabControl();
		private readonly TabPage filterPage;
		private readonly TabPage eventDataPage;

		public EventListControl()
		{
			eventGridView = new GridView<Event>();
			eventGridView.Columns.Add(new GridColumn() 
			{
				HeaderText = "Time", 
				DataCell = new TextBoxCell("Time") {Binding = new DelegateBinding<Event,string>(x =>
				{
					if (!(x is UnknownEvent) && ShowTimeSinceFirstEvent)
					{
						return $"{((x.Time - TimeOfOldestEvent) / 1000.0):0.000}";
					}

					return x.Time.ToString();
				})}
				
			});
			eventGridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Event Type",
				DataCell = new TextBoxCell() {Binding = new DelegateBinding<object, string>(x => x.GetType().Name)}
			});
			eventGridView.SelectionChanged += (sender, args) => eventPropertyGrid.SelectedObject = eventGridView.SelectedItem;
			eventGridView.Width = 250;

			var filterLayout = new DynamicLayout();

			typeFilterTree = new TreeGridView();
			typeFilterTree.Columns.Add(new GridColumn
			{
				DataCell = new TextBoxCell() {Binding = Binding.Property<TypeFilterItem, string>(x => x.Type.Name)},
				HeaderText = "Event Name",
			});
			typeFilterTree.Columns.Add(new GridColumn
			{
				DataCell = new CheckBoxCell() {Binding = Binding.Property<TypeFilterItem, bool?>(x => x.Checked)},
				HeaderText = "Show",
				Editable = true,
			});
			typeFilterTree.Columns.Add(new GridColumn()
			{
				DataCell = new TextBoxCell()
				{
					Binding = Binding.Property<TypeFilterItem, string>(x => x.Count.ToString())
				},
				HeaderText = "Event Count",
				Editable = false
			});

			var applyContentFilterButton = new Button {Text = "Apply filters"};
			applyContentFilterButton.Click += (sender, args) => ApplyFilters();
			filterLayout.BeginVertical(yscale: true);
			{
				filterLayout.Add(contentFilters);
			}
			filterLayout.EndVertical();
			filterLayout.BeginVertical(yscale: false);
			{
				filterLayout.AddRow(applyContentFilterButton, null);
			}
			filterLayout.EndVertical();

			var filterTabs = new TabControl();
			filterTabs.Pages.Add(new TabPage {Text = "By Event Type", Content = typeFilterTree});
			filterTabs.Pages.Add(new TabPage {Text = "By Content", Content = filterLayout});

			filterPage = new TabPage {Text = "Filters", Content = filterTabs};
			eventDataPage = new TabPage {Text = "Event data", Content = eventPropertyGrid};
			tabs.Pages.Add(filterPage);
			tabs.Pages.Add(eventDataPage);

			var eventsSplitter = new Splitter {Panel1 = eventGridView, Panel2 = tabs, Position = 300};
			Content = eventsSplitter;
		}

		private void UpdateFilterView()
		{
			TypeFilters = events.GroupBy(x => x.GetType()).Select(x => new TypeFilterItem(x.Key, x.Count()))
				.OrderBy(x => x.Type.Name).ToArray();

			var rootType = TypeFilters.FirstOrDefault()?.Type;
			foreach (var type in TypeFilters)
			{
				rootType = GetClosestType(rootType, type.Type);
			}

			var filterByType = TypeFilters.ToDictionary(x => x.Type);

			// Arrange the filters as their tree of dependency
			foreach (var filter in TypeFilters)
			{
				var child = filter;
				Type parent = filter.Type.BaseType;
				while (parent != null)
				{
					bool parentProcessed = false;
					if (!filterByType.ContainsKey(parent))
					{
						filterByType[parent] = new TypeFilterItem(parent, -1) {Expanded = true};
					}
					else
					{
						parentProcessed = true;
					}

					var parentFilter = filterByType[parent];
					parentFilter.Children.Add(child);

					child = parentFilter;
					parent = parent.BaseType;

					if (parentProcessed) break;
				}
			}

			// The following part is somewhat inefficient, this is the simplest way to write it.
			// Performance should not be an issue unless there is a very long chain of dependencies

			// Remove all types that are in a chain of types that are not present,
			// except for the direct parent of a present type
			bool changed;
			do
			{
				changed = false;

				foreach (var pair in filterByType)
				{
					TypeFilterItem filter = pair.Value;

					if (filter.Parent == null) continue; // Keep the root

					if (filter.Children.Count == 1 && filter.Count < 0)
					{
						var childPresent = ((TypeFilterItem) filter.Children.First()).Count > 0;
						if (!childPresent)
						{
							var parent = (TypeFilterItem) filter.Parent;
							var child = (TypeFilterItem) filter.Children.First();
							filter.Children.Clear();
							parent.Children.Clear();

							parent.Children.Add(child);
							changed = true;
						}
					}
				}
			} while (changed);

			TypeFilterItem rootItem;
			if (rootType != null)
			{
				rootItem = filterByType[rootType];
				rootItem.Checked = true;
			}
			else
			{
				rootItem = new TypeFilterItem(typeof(object)) {Checked = true};
			}

			typeFilterTree.DataStore = rootItem;
			AddDirectUseHandlerRecursively(rootItem);

			void AddDirectUseHandlerRecursively(TypeFilterItem item)
			{
				item.UsedDirectly += (sender, args) =>
				{
					InvalidateTree();
					ApplyFilters();
				};
				foreach (var child in item.Children)
				{
					if (child is TypeFilterItem filterChild)
					{
						AddDirectUseHandlerRecursively(filterChild);
					}
				}
			}
		}

		private void InvalidateTree()
		{
			typeFilterTree.Invalidate();
			// HACK: Invalidate doesn't redraw the tree on the Gtk3 platform
			// issue: https://github.com/picoe/Eto/issues/1982
			// TODO: Disable on other platforms if not necessary, may look significantly worse there
			
			tabs.SelectedPage = eventDataPage;
			tabs.SelectedPage = filterPage;
		}

		private void ApplyFilters()
		{
			((FilterCollection<Event>) eventGridView.DataStore).Refresh();

			contentFilters.UpdateContent(this);
		}

		private bool FilterByEventType(Event e)
		{
			var dataStore = typeFilterTree.DataStore;
			if (dataStore == null) return true;
			return TypeFilters.FirstOrDefault(x => x.Type == e.GetType())?.Checked ?? true;
		}

		private bool GetEventFilter(Event e)
		{
			return FilterByEventType(e) && contentFilters.FilterEvent(e);
		}

		private Type GetClosestType(Type a, Type b)
		{
			while (a != null)
			{
				if (a.IsAssignableFrom(b))
					return a;

				a = a.BaseType;
			}

			return null;
		}
	}
}