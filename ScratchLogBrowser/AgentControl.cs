using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Eto.Drawing;
using Eto.Forms;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchLogBrowser
{
	public class AgentControl : Panel
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
		private ICollection<Event> events;
		private readonly EventListControl eventListControl;
		private readonly CheckBox eventFilterAttackerCheckBox;
		private readonly CheckBox eventFilterDefenderCheckBox;

		public AgentControl()
		{
			var layout = new DynamicLayout();
			Content = layout;

			agentJsonControl = new JsonSerializationControl {Height = 200};
			eventListControl = new EventListControl();

			eventFilterAttackerCheckBox = new CheckBox() {Text = "Damage events where agent is attacker", Checked = true};
			eventFilterDefenderCheckBox = new CheckBox() {Text = "Damage events where agent is defender", Checked = true};
			eventFilterAttackerCheckBox.CheckedChanged += (s, e) => UpdateEventList();
			eventFilterDefenderCheckBox.CheckedChanged += (s, e) => UpdateEventList();

			layout.BeginGroup("Agent data", new Padding(5));
			layout.BeginVertical();
			layout.AddRow(nameLabel);
			layout.AddRow(agentJsonControl);
			layout.EndVertical();
			layout.BeginVertical();
			layout.AddRow(eventFilterAttackerCheckBox, eventFilterDefenderCheckBox);
			layout.EndVertical();
			layout.EndGroup();

			layout.BeginGroup("Events featuring the agent", new Padding(5));
			layout.Add(eventListControl);
			layout.EndGroup();
		}

		private void UpdateEventList()
		{
			eventListControl.Events = events.Where(x => EventFilters.IsAgentInEvent(x, agent,
				eventFilterAttackerCheckBox.Checked.Value, eventFilterDefenderCheckBox.Checked.Value)).ToArray();
		}
	}
}