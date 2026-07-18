using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Updates
{
	public class ReleaseFeed
	{
		/// <summary>
		/// URL of source code. May be null.
		/// </summary>
		public string SourceUrl { get; set; }
		public List<Release> Releases { get; set; }
	}
}