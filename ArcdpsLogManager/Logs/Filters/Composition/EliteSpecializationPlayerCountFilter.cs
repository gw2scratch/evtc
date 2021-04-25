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
	}
}