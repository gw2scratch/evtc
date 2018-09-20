using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchLogBrowser
{
	public class AgentControl
	{
		public Agent Agent
		{
			get => agent;
			set
			{
				agent = value;
				nameLabel.Text = $"Name: {agent.Name}";
				agentJsonControl.Object = agent;
				UpdateEventList();
			}
		}

		public Control Control { get; }

		public ICollection<Event> Events
		{
			get => events;
			set
			{
				events = value;
				UpdateEventList();
			}
		}

		private Agent agent;

		private readonly JsonSerializationControl agentJsonControl;
		private readonly Label nameLabel = new Label();
		private readonly GridView<Event> eventGridView;
		private ICollection<Event> events;
		private readonly JsonSerializationControl eventJsonControl = new JsonSerializationControl();

		public AgentControl()
		{
			var layout = new DynamicLayout();
			Control = layout;
			agentJsonControl = new JsonSerializationControl();
			agentJsonControl.Control.Height = 200;

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

			UpdateEventList();

			layout.BeginGroup("Agent data", new Padding(5));
			layout.BeginVertical();
			layout.AddRow(nameLabel);
			layout.AddRow(agentJsonControl.Control);
			layout.EndVertical();
			layout.EndGroup();

			layout.BeginGroup("Events featuring the agent", new Padding(5));
			layout.BeginHorizontal();
			layout.BeginVertical();
			layout.Add(eventGridView);
			layout.EndVertical();
			layout.BeginVertical();
			layout.BeginGroup("Event data", new Padding(5));
			layout.Add(eventJsonControl.Control);
			layout.EndGroup();
			layout.EndVertical();
			layout.EndHorizontal();
			layout.EndGroup();
		}

		private void UpdateEventList()
		{
			eventGridView.DataStore =
				Events?.Where(x => EventFilters.IsAgentInEvent(x, agent)).ToArray() ?? Enumerable.Empty<Event>();
		}
	}
}