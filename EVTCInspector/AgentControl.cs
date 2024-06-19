using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCInspector
{
	public class AgentControl : Panel
	{
		public Agent Agent
		{
			get => agent;
			set
			{
				agent = value;
				nameLabel.Text = agent == null ? "No agent selected." : $"Name: {agent.Name}";
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

		public bool ShowTimeSinceFirstEvent
		{
			get => eventListControl.ShowTimeSinceFirstEvent;
			set => eventListControl.ShowTimeSinceFirstEvent = true;
		}

		private Agent agent;

		private readonly JsonSerializationControl agentJsonControl;
		private readonly Label nameLabel = new Label();
		private ICollection<Event> events;
		private readonly EventListControl eventListControl;
		private readonly CheckBox eventFilterSourceCheckBox;
		private readonly CheckBox eventFilterTargetCheckBox;

		public AgentControl()
		{
			var tabControl = new TabControl();
			Content = tabControl;

			var agentDataLayout = new DynamicLayout();
			var eventLayout = new DynamicLayout();
			tabControl.Pages.Add(new TabPage(agentDataLayout) {Text = "Agent data"});
			tabControl.Pages.Add(new TabPage(eventLayout) {Text = "Agent events"});

			agentJsonControl = new JsonSerializationControl {Height = 200};
			eventListControl = new EventListControl();

			eventFilterSourceCheckBox = new CheckBox() {Text = "Events where agent is source (attacker, buff applier, caused effect)", Checked = true};
			eventFilterTargetCheckBox = new CheckBox() {Text = "Events where agent is target (defender, receiving buffs, affected by effect)", Checked = true};
			eventFilterSourceCheckBox.CheckedChanged += (s, e) => UpdateEventList();
			eventFilterTargetCheckBox.CheckedChanged += (s, e) => UpdateEventList();

			agentDataLayout.BeginVertical(new Padding(5));
			agentDataLayout.AddRow(nameLabel);
			agentDataLayout.AddRow(agentJsonControl);
			agentDataLayout.EndVertical();

			eventLayout.BeginVertical(new Padding(5));
			eventLayout.AddRow(eventFilterSourceCheckBox);
			eventLayout.AddRow(eventFilterTargetCheckBox);
			eventLayout.EndVertical();
			eventLayout.BeginGroup("Events featuring the agent", new Padding(5));
			eventLayout.Add(eventListControl);
			eventLayout.EndGroup();
		}

		private void UpdateEventList()
		{
			if (agent == null)
			{
				eventListControl.Events = new List<Event>();
			}
			else
			{
				eventListControl.Events = events.Where(x => EventFilters.IsAgentInEvent(x, agent, eventFilterSourceCheckBox.Checked.Value, eventFilterTargetCheckBox.Checked.Value)).ToArray();
			}
			eventListControl.TimeOfOldestEvent = events.Where(x => x is not UnknownEvent).Select(x => x.Time).DefaultIfEmpty().Min();
		}
	}
}