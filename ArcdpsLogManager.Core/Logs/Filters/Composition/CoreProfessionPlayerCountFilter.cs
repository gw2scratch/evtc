using System;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition
{
	public class CoreProfessionPlayerCountFilter : PlayerCountFilter
	{
		public Profession Profession { get; }

		public CoreProfessionPlayerCountFilter(Profession profession)
		{
			Profession = profession;
		}

		protected override int GetPlayerCount(LogData log)
		{
			// This is not LINQ for performance purposes.
			if (log.Players == null)
			{
				return 0;
			}

			int count = 0;
			foreach (var player in log.Players)
			{
				if (player.Profession == Profession && player.EliteSpecialization == EliteSpecialization.None)
				{
					count++;
				}
			}

			return count;
		}

		public CoreProfessionPlayerCountFilter DeepClone()
		{
			var filter = new CoreProfessionPlayerCountFilter(Profession) {
				PlayerCount = PlayerCount,
				FilterType = FilterType
			};

			return filter;
		}

		protected bool Equals(CoreProfessionPlayerCountFilter other)
		{
			return Profession == other.Profession &&
			       PlayerCount == other.PlayerCount &&
			       FilterType == other.FilterType;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CoreProfessionPlayerCountFilter) obj);
		}
		
		public override int GetHashCode()
		{
			return HashCode.Combine(Profession, PlayerCount, FilterType);
		}
	}
}