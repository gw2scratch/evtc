using GW2Scratch.EVTCAnalytics.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Logs.Extras;

public class FractalExtras
{
	[JsonProperty]
	public List<MistlockInstability> MistlockInstabilities { get; set; } = new List<MistlockInstability>();
}