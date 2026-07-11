using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the "unreliable success detection" warning shown before permanently deleting
	/// logs for encounters where success detection is known to occasionally be wrong (Avalonia
	/// counterpart of the Eto <c>UnreliableLogsFoundDialog</c>).
	/// </summary>
	public class UnreliableLogsWindowViewModel
	{
		// This is a bit of a hack, ideally we would have reliability information within
		// EVTCAnalytics (or, even better, no unreliable detections).
		private static readonly Encounter[] UnreliableEncounters =
		{
			Encounter.BanditTrio, Encounter.Artsariiv, Encounter.Arkk, Encounter.Other
		};

		public IReadOnlyList<string> EncounterNames { get; }

		public UnreliableLogsWindowViewModel(IEnumerable<LogData> logs, ILogNameProvider nameProvider)
		{
			EncounterNames = GetUnreliableEncounterNames(logs, nameProvider).ToList();
		}

		/// <summary>Whether any of the given logs have unreliable success detection.</summary>
		public static bool IsApplicable(IEnumerable<LogData> logs) => GetPresentUnreliableLogs(logs).Any();

		private static IEnumerable<LogData> GetPresentUnreliableLogs(IEnumerable<LogData> logs)
		{
			return logs.Where(x => UnreliableEncounters.Contains(x.Encounter));
		}

		private static IEnumerable<string> GetUnreliableEncounterNames(IEnumerable<LogData> logs,
			ILogNameProvider nameProvider)
		{
			var unreliableLogs = GetPresentUnreliableLogs(logs);
			return unreliableLogs.Select(nameProvider.GetName).Distinct().OrderBy(x => x);
		}
	}
}
