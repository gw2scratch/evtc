using System.Linq;
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
			return log.Players?.Count(x => x.EliteSpecialization == EliteSpecialization) ?? 0;
		}
	}
}