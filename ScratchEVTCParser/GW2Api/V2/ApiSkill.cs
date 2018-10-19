using System.Collections.Generic;

namespace ScratchEVTCParser.GW2Api.V2
{
	public class ApiSkill
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; } // Optional
		public string Icon { get; set; }
		public string ChatLink { get; set; }
		public string Type { get; set; } // Optional
		public string WeaponType { get; set; } // Optional
		public List<string> Professions { get; set; }
		public string Slot { get; set; }
		public List<string> Categories { get; set; } // Optional
		public string Attunement { get; set; } // Optional
		public string DualWield { get; set; } // Optional
		public int PrevChain { get; set; } // Optional
		public int NextChain { get; set; } // Optional

		// Ignored fields: facts, traited_facts, cost, flip_skill, initiative,
		// transform_skills, bundle_skills, toolbelt_skill
	}
}