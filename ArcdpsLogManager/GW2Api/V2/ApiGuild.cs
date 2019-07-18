using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.GW2Api.V2
{
	public class ApiGuild
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Tag { get; set; }
		public Emblem Emblem { get; set; }
	}

	public class Emblem
	{
		public EmblemPart Background { get; set; }
		public EmblemPart Foreground { get; set; }

		public List<string> Flags { get; set; }
	}

	public class EmblemPart
	{
		public int Id { get; set; }
		public List<int> Colors { get; set; }
	}
}