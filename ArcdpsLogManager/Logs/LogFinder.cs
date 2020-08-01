using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Logs
{
	public class LogFinder
	{
		/// <summary>
		/// Creates a new log collection that contains all logs contained in a directory including subdirectories
		/// </summary>
		/// <param name="directoryPath">A path to a dictionary that should be searched.</param>
		/// <param name="logCache">An optional log cache that will be used to retrieve log data if available.</param>
		/// <returns>Logs that were found.</returns>
		public IEnumerable<LogData> GetFromDirectory(string directoryPath, LogCache logCache = null)
		{
			var files = Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
				.Where(IsLikelyEvtcLog);

			return files.Select(file =>
			{
				var fileInfo = new FileInfo(file);

				if (logCache != null)
				{
					if (logCache.TryGetLogData(fileInfo, out var cachedLog))
					{
						return cachedLog;
					}
				}

				return new LogData(fileInfo);
			});
		}

		public bool IsLikelyEvtcLog(string filename)
		{
			if (filename.EndsWith(".evtc") || filename.EndsWith(".evtc.zip") || filename.EndsWith(".zevtc"))
			{
				return true;
			}

			try
			{
				using var reader = new StreamReader(filename);
				var buffer = new char[4];
				reader.Read(buffer, 0, buffer.Length);

				// TODO: Check contents of zips as well. Not as important because arcdps has never created those without a specified extension.

				return buffer[0] == 'E' && buffer[1] == 'V' && buffer[2] == 'T' && buffer[3] == 'C';
			}
			catch
			{
				return false;
			}
		}

		public IEnumerable<LogData> GetTesting()
		{
			var player1 = new LogPlayer("Testing player", ":Testing account.1234", 1, Profession.Thief, EliteSpecialization.Daredevil, "01D1DADF-751E-E411-ADEE-AC162DC0070D");
			var player2 = new LogPlayer("Testing player 2", ":Another account.1235", 1, Profession.Ranger, EliteSpecialization.Druid, "01D1DADF-751E-E411-ADEE-000000000000");
			var player3 = new LogPlayer("Testing player 3", ":ChronoGoddess.1236", 2, Profession.Mesmer, EliteSpecialization.Chronomancer, null);
			var players = new[] {player1, player2, player3};

			var dateStart1 = new DateTimeOffset(2018, 06, 15, 12, 13, 14, TimeSpan.Zero);
			var dateStart2 = new DateTimeOffset(2018, 06, 15, 13, 14, 15, TimeSpan.Zero);
			var dateStart3 = new DateTimeOffset(2018, 06, 15, 14, 15, 16, TimeSpan.Zero);

			var duration1 = new TimeSpan(0, 1, 0);
			var duration2 = new TimeSpan(0, 2, 0);
			var duration3 = new TimeSpan(0, 6, 0);

			var log1 = new LogData(null, players, Encounter.Sabetha, "Sabetha the Saboteur", EncounterResult.Failure, dateStart1,
				duration1);
			var log2 = new LogData(null, players, Encounter.Sabetha, "Sabetha the Saboteur", EncounterResult.Failure, dateStart2,
				duration2);
			var log3 = new LogData(null, players, Encounter.Sabetha, "Sabetha the Saboteur", EncounterResult.Success, dateStart3,
				duration3);

			return Enumerable.Repeat(new[] {log1, log2, log3}, 8).SelectMany(x => x);
		}
	}
}