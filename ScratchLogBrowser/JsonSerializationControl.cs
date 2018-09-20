using System;
using System.IO;
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
				var serializer = new JsonSerializer();
				serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
				serializer.Formatting = Formatting.Indented;
				serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

				var writer = new StringWriter();
                serializer.Serialize(writer, obj);
				jsonData.Text = writer.ToString();
			}
		}

		public Control Control { get; }

		private readonly TextArea jsonData;
		private Object obj;

		public JsonSerializationControl()
		{
			var layout = new DynamicLayout();
			Control = layout;

			jsonData = new TextArea {Text = "", ReadOnly = true};
			layout.Add("Serialized data:");
			layout.Add(jsonData);
		}
	}
}