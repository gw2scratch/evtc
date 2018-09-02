using Eto.Forms;
using Newtonsoft.Json;
using ScratchEVTCParser.Events;

namespace ScratchLogBrowser
{
	public class EventControl
	{
		public Event Event
		{
			get => @event;
			set
			{
				@event = value;
				jsonData.Text = JsonConvert.SerializeObject(@event, Formatting.Indented);
			}
		}

		public Control Control { get; }

		private readonly Label jsonData;
		private Event @event;

		public EventControl()
		{
			var layout = new DynamicLayout();
			Control = layout;

			jsonData = new Label {Text = ""};
			layout.Add("All data in event:");
			layout.Add(jsonData);
		}
	}
}