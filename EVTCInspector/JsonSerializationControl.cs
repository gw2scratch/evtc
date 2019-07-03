using System;
using System.IO;
using Eto.Forms;
using Newtonsoft.Json;

namespace GW2Scratch.EVTCInspector
{
	public class JsonSerializationControl : Panel
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

		private readonly TextArea jsonData;
		private Object obj;

		public JsonSerializationControl()
		{
			var layout = new DynamicLayout();
			Content = layout;

			jsonData = new TextArea {Text = "", ReadOnly = true};
			layout.Add("Serialized data:");
			layout.Add(jsonData);
		}
	}
}