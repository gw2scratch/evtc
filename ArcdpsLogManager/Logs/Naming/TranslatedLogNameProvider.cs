using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GW2Scratch.ArcdpsLogManager.Logs.Naming
{
	public class TranslatedLogNameProvider : ILogNameProvider
	{
		private readonly GameLanguage language;
		private Dictionary<int, string> EnglishMapNames { get; set; }

		public TranslatedLogNameProvider(GameLanguage language)
		{
			this.language = language;
			if (Settings.UseGW2Api)
			{
				Task.Run(GetMapNames);
			}
		}

		private async Task GetMapNames()
		{
			var locale = language switch
			{
				GameLanguage.English => Locale.English,
				GameLanguage.French => Locale.French,
				GameLanguage.German => Locale.German,
				GameLanguage.Spanish => Locale.Spanish,
				GameLanguage.Chinese => Locale.Chinese,
				GameLanguage.Other => Locale.English,
				_ => throw new ArgumentOutOfRangeException()
			};

			var client = new Gw2Client(new Connection(locale));
			var maps = await client.WebApi.V2.Maps.AllAsync();
			await Application.Instance.InvokeAsync(() =>
			{
				EnglishMapNames = maps.ToDictionary(x => x.Id, x => x.Name);
				MapNamesUpdated?.Invoke(this, EventArgs.Empty);
			});
		}

		public string GetName(LogData logData)
		{
			if (logData.Encounter != Encounter.Other && logData.Encounter != Encounter.Map)
			{
				EncounterNames.TryGetEncounterNameForLanguage(out string encounterName, language, logData.Encounter, logData.EncounterMode);
				return encounterName;
			}

			if (logData.Encounter == Encounter.Map)
			{
				return GetMapName(logData.MapId);
			}

			// We default to the name of the main target in case a translated name
			// for the encounter is not available or we don't know the encounter.
			return logData.MainTargetName;
		}

		public string GetMapName(int? mapId)
		{
			if (mapId == null)
			{
				return "Unknown map";
			}

			if (EnglishMapNames != null && EnglishMapNames.TryGetValue(mapId.Value, out var name))
			{
				return name;
			}

			return $"Map {mapId} (GW2 API unavailable?)";
		}

		public event EventHandler MapNamesUpdated;
	}
}