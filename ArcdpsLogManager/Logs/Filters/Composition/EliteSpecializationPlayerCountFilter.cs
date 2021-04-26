using System;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition
{
	public class EliteSpecializationPlayerCountFilter : PlayerCountFilter
	{
		public EliteSpecialization EliteSpecialization { get; }
		
		public EliteSpecializationPlayerCountFilter(EliteSpecialization eliteSpecialization)
		{
			EliteSpecialization = eliteSpecialization;
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
				if (player.EliteSpecialization == EliteSpecialization)
				{
					count++;
				}
			}

			return count;
		}
		
		public EliteSpecializationPlayerCountFilter DeepClone()
		{
			var filter = new EliteSpecializationPlayerCountFilter(EliteSpecialization) {
				PlayerCount = PlayerCount,
				FilterType = FilterType
			};

			return filter;
		}
		
		protected bool Equals(EliteSpecializationPlayerCountFilter other)
		{
			return EliteSpecialization == other.EliteSpecialization &&
			       PlayerCount == other.PlayerCount &&
			       FilterType == other.FilterType;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((EliteSpecializationPlayerCountFilter) obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(EliteSpecialization, PlayerCount, FilterType);
		}
	}
}