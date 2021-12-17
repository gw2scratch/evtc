using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Logs.Extras;

/// <summary>
/// Extra data that is only relevant for some logs.
/// </summary>
public class LogExtras
{
	/// <summary>
	/// Extra data for fractal encounters.
	/// </summary>
	[JsonProperty]
	public FractalExtras FractalExtras { get; set; }
}