using System;
using Eto.Forms;
using Newtonsoft.Json;

namespace ScratchLogBrowser
{
	public class JsonSerializationControl
	{
		public object Object
		{
			get => obj;
			set
			{
				obj = value;
				jsonData.Text = JsonConvert.SerializeObject(obj, Formatting.Indented);
			}
		}

		public Control Control { get; }

		private readonly Label jsonData;
		private Object obj;

		public JsonSerializationControl()
		{
			var layout = new DynamicLayout();
			Control = layout;

			jsonData = new Label {Text = ""};
			layout.Add("Serialized data:");
			layout.Add(jsonData);
		}
	}
}