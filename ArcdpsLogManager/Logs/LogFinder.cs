using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using System.Text;

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
				if (logCache != null)
				{
					if (logCache.TryGetLogData(file, out var cachedLog))
					{
						return cachedLog;
					}
				}

				return new LogData(file);
			});
		}

		/// <summary>
		/// Checks if the file is put into consideration as a evtc log. 
		/// It will intentionally not check if the file actually exists to reduce filesystem calls.
		/// </summary>
		/// <param name="filename">The fullpath to the file to check.</param>
		/// <returns>Returns <see langword="true"/> if the file can be considered as a evtc log.</returns>
		public bool IsLikelyEvtcLog(string filename)
		{
			if (filename.EndsWith(".evtc") || filename.EndsWith(".evtc.zip") || filename.EndsWith(".zevtc"))
			{
				return true;
			}
			
			if (Path.GetExtension(filename) == "")
			{
				// If arcdps compression fails, the file is created with no extension. If we find any files without
				// extension, we check the magic string at the start of the file to determine whether they are an EVTC log.
				try
				{
					Span<byte> buffer = stackalloc byte[4];
					var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4);
					stream.Read(buffer);

					return buffer[0] == 'E' && buffer[1] == 'V' && buffer[2] == 'T' && buffer[3] == 'C';
				}
				catch
				{
					return false;
				}
			}

			return false;
		}

		/// <summary>
		/// Arcdps creates a file and then adds into an archive. The file is then deleted. Checks if this is possibly one of these files.
		/// It is not guaranteed, however, and this may return true for files that will not be removed.
		/// </summary>
		public bool IsLikelyTemporary(string filename) => !filename.Contains('.');

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