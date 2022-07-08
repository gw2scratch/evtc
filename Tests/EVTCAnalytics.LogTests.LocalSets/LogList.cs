using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GW2Scratch.EVTCAnalytics.LogTests.LocalSets;

public class LogList
{
	[JsonPropertyName("log")]
	public List<LogDefinition> Logs { get; set; }
}