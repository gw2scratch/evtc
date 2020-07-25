using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Logs.Caching
{
	public class VersionConverter : JsonConverter
	{
		private readonly Newtonsoft.Json.Converters.VersionConverter defaultConverter = new Newtonsoft.Json.Converters.VersionConverter();

		// .NET Core 3.1 introduced an undocumented breaking change when it added a VersionConverter for the System.Version type:
		// https://github.com/dotnet/corefx/pull/28516/files
		// This converter counteracts that, restoring the previous behavior for serialization

		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				var dict = serializer.Deserialize<Dictionary<string, int>>(reader);
				return new Version(dict["Major"], dict["Minor"], dict["Build"], dict["Revision"]);
			}

			return defaultConverter.ReadJson(reader, objectType, existingValue, serializer);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Version);
		}

		public override bool CanWrite => false;
	}
}