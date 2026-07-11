using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Logs
{
	public class PointOfView
	{
		[JsonProperty]
		public string CharacterName { get; set; }

		[JsonProperty]
		public string AccountName { get; set; }
	}
}