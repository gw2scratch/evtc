using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchLogInspector
{
	public class EventListControl : Panel
	{
		private class TypeFilterItem : TreeGridItem
		{
			public Type Type { get; }

			private bool? shown;

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
							child.Checked = shown.Value;
						}
					}

					if (Parent != null)
					{
						var parent = (TypeFilterItem) Parent;
						bool? newValue;
						if (parent.Children.All(x => ((TypeFilterItem) x.Parent).Checked == true))
						{
							newValue = true;
						}
						else if (parent.Children.All(x => ((TypeFilterItem) x.Parent).Checked == false))
						{
							newValue = false;
						}
						else
						{
							newValue = null;
						}

						parent.Checked = newValue;
					}
				}
			}

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
				eventGridView.DataStore = new FilterCollection<Event>(events) {Filter = GetEventFilter()};

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

		private readonly List<Event> events = new List<Event>();
		private readonly List<Agent> agents = new List<Agent>();

		private TypeFilterItem[] filters;

		private readonly Panel filterPanel = new Panel();
		private readonly GridView<Event> eventGridView;
		private readonly TreeGridView typeFilterTree;
		private readonly JsonSerializationControl eventJsonControl = new JsonSerializationControl();

		public EventListControl()
		{
			eventGridView = new GridView<Event>();
			eventGridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Time",
				DataCell = new TextBoxCell("Time")
			});
			eventGridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Event Type",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<object, string>(x => x.GetType().Name)
				}
			});
			eventGridView.SelectionChanged += (sender, args) => eventJsonControl.Object = eventGridView.SelectedItem;
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
					{Binding = Binding.Property<TypeFilterItem, string>(x => x.Count.ToString())},
				HeaderText = "Event Count",
				Editable = false
			});

			filterLayout.BeginHorizontal();
			filterLayout.BeginVertical();
			filterLayout.Add(filterPanel);
			filterLayout.EndVertical();
			filterLayout.EndHorizontal();

			var filterTabs = new TabControl();
			filterTabs.Pages.Add(new TabPage {Text = "By Event Type", Content = typeFilterTree});
			filterTabs.Pages.Add(new TabPage {Text = "By Content", Content = filterLayout});

			var tabs = new TabControl();
			var filterPage = new TabPage {Text = "Filters", Content = filterTabs};
			var eventDataPage = new TabPage {Text = "Event data", Content = eventJsonControl};
			tabs.Pages.Add(filterPage);
			tabs.Pages.Add(eventDataPage);

			typeFilterTree.CellEdited += (sender, args) =>
			{
				typeFilterTree.Invalidate();

				// HACK: Invalidate doesn't redraw the tree on the Gtk3 platform
				// TODO: Disable on other platforms if not necessary, may look significantly worse there
				tabs.SelectedPage = eventDataPage;
				tabs.SelectedPage = filterPage;

				ApplyFilters();
			};

			var eventsSplitter = new Splitter {Panel1 = eventGridView, Panel2 = tabs, Position = 300};
			Content = eventsSplitter;
		}

		private void UpdateFilterView()
		{
			filters = events.GroupBy(x => x.GetType()).Select(x => new TypeFilterItem(x.Key, x.Count()))
				.OrderBy(x => x.Type.Name).ToArray();

			var rootType = filters.First().Type;
			foreach (var type in filters)
			{
				rootType = GetClosestType(rootType, type.Type);
			}

			var filterByType = filters.ToDictionary(x => x.Type);

			// Arrange the filters as their tree of dependency
			foreach (var filter in filters)
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

			var rootItem = filterByType[typeof(object)];
			rootItem.Checked = true;

			typeFilterTree.DataStore = rootItem;
		}

		private void ApplyFilters()
		{
            ((FilterCollection<Event>) eventGridView.DataStore).Refresh();

			/*
			var selectedTypes = filters.Where(x => x.Checked.HasValue && x.Checked.Value).ToArray();
			if (selectedTypes.Length == 0)
			{
				return;
			}

			var commonAncestor = selectedTypes.First().Type;
			foreach (var type in selectedTypes)
			{
				commonAncestor = GetClosestType(commonAncestor, type.Type);
			}

			if (typeof(AgentEvent).IsAssignableFrom(commonAncestor))
			{
				var filterAgentRadioButton = new RadioButton {Text = "Exact Agent"};
				var filterAgentDataRadioButton = new RadioButton(filterAgentRadioButton) {Text = "Agent data"};
				var agentsDropDown = new DropDown
				{
					DataStore = agents,
					ItemTextBinding = new DelegateBinding<Agent, string>(x => $"{x.Name} ({x.Address})")
				};

				var filterLayout = new DynamicLayout();
				filterLayout.BeginHorizontal();
				filterLayout.BeginGroup("Agent filter");
				filterLayout.BeginVertical(spacing: new Size(5, 5));
				filterLayout.AddRow(filterAgentRadioButton, null, agentsDropDown);
				filterLayout.AddRow(filterAgentDataRadioButton, new CheckBox {Text = "Name"}, new TextBox {Text = ""});
				filterLayout.AddRow(null, new CheckBox {Text = "Type"},
					new DropDown {DataStore = new[] {"Player", "NPC", "Gadget"}});
				filterLayout.AddRow(null);
				filterLayout.EndVertical();
				filterLayout.EndGroup();
				filterLayout.EndHorizontal();
				filterPanel.Content = filterLayout;
			}
			else
			{
				filterPanel.Content = null;
			}
			*/
		}

		private Func<Event, bool> GetEventFilter()
		{
			return e =>
			{
				var dataStore = typeFilterTree.DataStore;
				if (dataStore == null) return true;
				return filters.FirstOrDefault(x => x.Type == e.GetType())?.Checked ?? true;
			};
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