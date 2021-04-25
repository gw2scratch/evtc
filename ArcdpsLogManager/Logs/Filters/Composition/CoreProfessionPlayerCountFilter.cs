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
	}
}