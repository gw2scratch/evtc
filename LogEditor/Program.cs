using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Compression;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Parsed.Editing;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;

namespace LogEditor
{
	class Program
	{
		static void Main(string[] args)
		{
			var command = new RootCommand
			{
				new Option<bool>(
					"--anonymize",
					getDefaultValue: () => false,
					description: "Anonymize the character and account names of players in the log"),
				new Option<bool>(
					"--remove-rewards",
					getDefaultValue: () => false,
					description: "Remove all reward events from the log"),
				new Option<bool>(
					"--zevtc",
					getDefaultValue: () => true,
					description: "Compress output as a .zevtc file"),
				new Option<string>(
					"--input",
					description: "The path to the input file")
				{
					Required = true,
				},
				new Option<string>(
					"--output",
					description: "The name of the output file. Replaces file if it exists.")
				{
					Required = true,
				}
			};

			command.Description = "Edit arcdps EVTC Logs";

			command.Handler = CommandHandler.Create<bool, bool, bool, string, string>(
				(anonymize, removeRewards, zevtc, input, output) =>
				{
					var parser = new EVTCParser();
					var editor = new ParsedLogEditor();
					var writer = new EVTCWriter();

					var log = parser.ParseLog(input);

					if (anonymize)
					{
						Console.WriteLine("Anonymizing...");
						editor.AnonymizePlayers(log);
					}

					if (removeRewards)
					{
						editor.RemoveStateChanges(log, StateChange.Reward);
						Console.Write("Removing rewards...");
					}

					if (zevtc)
					{
						using var zip = ZipFile.Open(output, ZipArchiveMode.Create);
						var entry = zip.CreateEntry("1");
						using var binaryWriter = new BinaryWriter(entry.Open());
						writer.WriteLog(log, binaryWriter);
					}
					else
					{
						using var file = File.OpenWrite(output);
						using var binaryWriter = new BinaryWriter(file);
						writer.WriteLog(log, binaryWriter);
					}
				}
			);

			command.Invoke(args);
		}
	}
}