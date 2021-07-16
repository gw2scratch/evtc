using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GW2Scratch.RotationComparison.GW2Api;
using GW2Scratch.RotationComparison.GW2Api.V2;
using GW2Scratch.RotationComparison.Logs;
using GW2Scratch.RotationComparison.Statistics;

namespace GW2Scratch.RotationComparison
{
	internal class Program
	{
		public static async Task Main(string[] args)
		{
			var logSources = new List<ILogSource>();

			IApiDataSource apiDataSource = new ApiApiDataSource();

			try
			{
				if (args.Contains("--help"))
				{
					// Now this is just a dirty hack
					throw new Exception("Help");
				}

				if (args.Contains("--saveapifile"))
				{
					int index = Array.IndexOf(args, "--saveapifile");

                    if (index + 1 >= args.Length)
                    {
                        throw new Exception("--saveapifile was not followed by a filename");
                    }

                    string filename = args[index + 1];

                   try
                   {
	                    var data = await GW2ApiData.LoadFromApiAsync(new ApiSkillRepository());
	                    await data.SaveToFileAsync(filename);
                   }
                   catch (Exception e)
                   {
                       Console.WriteLine($"An error occured while saving api data: {e.Message}");
                       return;
                   }

                    Console.Error.WriteLine($"Saved api data to {filename}");

					return;
				}

				for (var i = 0; i < args.Length; i++)
				{
					if (args[i] == "--file")
					{
						i++;
						if (i >= args.Length)
						{
							throw new Exception("--file was not followed by a filename");
						}

						logSources.Add(new FileLogSource(args[i]));
					}
					else if (args[i] == "--report")
					{
						i++;
						if (i >= args.Length)
						{
							throw new Exception("--report was not followed by an url");
						}

						logSources.Add(new EliteInsightsUrlLogSource(args[i]));
					}
					else if (args[i] == "--name")
					{
						var source = logSources.LastOrDefault() ??
						             throw new Exception("--name was specified, but there was no log mentioned before to apply it to");

						i++;
						if (i >= args.Length)
						{
							throw new Exception("--name was not followed by a character name");
						}

						source.SetCharacterNameFilter(args[i]);
					}
					else if (args[i] == "--apifile")
					{
						i++;
						if (i >= args.Length)
						{
							throw new Exception("--apifile was not followed by a filename");
						}

						apiDataSource = new FileApiDataSource(args[i]);
					}
					else
					{
						throw new Exception($"Unrecognized argument: \"{args[i]}\"");
					}
				}

				if (!logSources.Any())
				{
					throw new Exception("No log sources provided");
				}
			}
			catch (Exception e)
			{
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine($@"Usage examples:
	{AppDomain.CurrentDomain.FriendlyName} --file 20190121-181420.zevtc --file 20190122-141235.zevtc
	{AppDomain.CurrentDomain.FriendlyName} --file 20190121-181420.zevtc --report https://dps.report/example
	{AppDomain.CurrentDomain.FriendlyName} --file 20190121-181420.zevtc --report dps.report/example
	{AppDomain.CurrentDomain.FriendlyName} --file 20190121-181420.zevtc --name ""Character Name"" --report dps.report/example
	{AppDomain.CurrentDomain.FriendlyName} --file 20190121-181420.zevtc --name ""Character Name"" --report dps.report/example --name ""Character Name""
	{AppDomain.CurrentDomain.FriendlyName} --file 20190121-181420.zevtc --file 20190122-141235.zevtc --report https://dps.report/example

	{AppDomain.CurrentDomain.FriendlyName} --apifile apidata.json --file 20190121-181420.zevtc --file 20190122-141235.zevtc
	{AppDomain.CurrentDomain.FriendlyName} --saveapifile apidata.json

	--name is used to specify which character's rotation to use if multiple are present in the log
	--file is used to provide a path to an arcdps EVTC log to be used in the comparison
	--report is used to provide an url to an Elite Insights report to be used in the comparison

	API Data:
		Some data from the official Guild Wars 2 API is used to show icons and similar
		If no related option is specified, the data is downloaded from https://api.guildwars2.com/
		To avoid downloading this every time, you can run the program like this:
			{AppDomain.CurrentDomain.FriendlyName} --saveapifile <filename>
		All other options are ignored in this case and a file with the API data is produced.

		--apifile <filename> can then be used to use api data from a file with cached data from --saveapifile

		Keep in mind that the file needs to be updated from time to time with new GW2 releases or some data may be missing.

	Other notes:
		If no name is specified and multiple characters are present, all of their rotations are shown
		There is no limit on the amount of logs to compare
		You can use any encounter, although extra data is only available only for training golem logs");
                return;
			}

			GW2ApiData apiData;
			try
			{
				apiData = await apiDataSource.GetApiDataAsync();
			}
			catch (Exception e)
			{
				Console.Error.WriteLine($"Failed to get API data: {e.Message}");
				return;
			}

			try
			{
				var generator = new RotationComparisonGenerator(apiData);
				generator.WriteHtmlOutput(logSources, Console.Out);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine($"Failed to generate rotation: {e.Message}");
				return;
			}
		}
	}
}