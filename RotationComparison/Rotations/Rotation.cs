using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.RotationComparison.Rotations
{
	public class Rotation
	{
		public string PlayerName { get; }
		public Profession Profession { get; }
		public EliteSpecialization Specialization { get; }
		public IEnumerable<RotationItem> Items { get; }

		public Rotation(string playerName, Profession profession, EliteSpecialization specialization,
			IEnumerable<RotationItem> items)
		{
			PlayerName = playerName;
			Profession = profession;
			Specialization = specialization;
			Items = items.ToArray();
		}
	}
}