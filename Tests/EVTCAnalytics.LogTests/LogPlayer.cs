using System;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.LogTests
{
	public class LogPlayer : IEquatable<LogPlayer>
	{
		public string CharacterName { get; set; }
		public string AccountName { get; set; }
		public Profession Profession { get; set; }
		public EliteSpecialization EliteSpecialization { get; set; }
		public int Subgroup { get; set; }

		public bool Equals(LogPlayer other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return CharacterName == other.CharacterName && AccountName == other.AccountName && Profession == other.Profession &&
			       EliteSpecialization == other.EliteSpecialization && Subgroup == other.Subgroup;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((LogPlayer) obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(CharacterName, AccountName, (int) Profession, (int) EliteSpecialization, Subgroup);
		}

		public static bool operator ==(LogPlayer left, LogPlayer right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(LogPlayer left, LogPlayer right)
		{
			return !Equals(left, right);
		}
	}
}