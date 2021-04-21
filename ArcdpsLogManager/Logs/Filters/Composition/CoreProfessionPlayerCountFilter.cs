using System.Linq;
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
			return log.Players?.Count(x => x.Profession == Profession && x.EliteSpecialization == EliteSpecialization.None) ?? 0;
		}
	}
}