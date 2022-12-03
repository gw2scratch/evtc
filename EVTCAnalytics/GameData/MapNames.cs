using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.GameData
{
	/// <summary>
	/// Provides names for maps.
	/// </summary>
	public static class MapNames
	{
		public static IReadOnlyDictionary<int, string> EnglishNames { get; } = new Dictionary<int, string>
		{
			{MapIds.RaidWing1, "Spirit Vale"},
			{MapIds.RaidWing2, "Salvation Pass"},
			{MapIds.RaidWing3, "Stronghold of the Faithful"},
			{MapIds.RaidWing4, "Stronghold of the Faithful"},
			{MapIds.OldLionsCourt, "Old Lion's Court"},
		};

		/// <summary>
		/// Provides a dictionary of names for maps in the specified language if it is available.
		/// </summary>
		/// <param name="language">The language of the map name.</param>
		/// <param name="namesByMap">When this value returns, contains the name dictionary; otherwise, <see langword="null"/>.</param>
		/// <returns><see langword="true" /> if there are names available; otherwise, <see langword="false" />.</returns>
		public static bool TryGetNamesForLanguage(GameLanguage language, out IReadOnlyDictionary<int, string> namesByMap)
		{
			// TODO: Add translated names for other languages as well
			switch (language)
			{
				case GameLanguage.English:
					namesByMap = EnglishNames;
					return true;
				case GameLanguage.French:
				case GameLanguage.German:
				case GameLanguage.Spanish:
				case GameLanguage.Chinese:
				case GameLanguage.Other:
					namesByMap = null;
					return false;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Provides a name for an map in the specified language if it is available.
		/// </summary>
		/// <param name="language">The language of the map name.</param>
		/// <param name="map">The map.</param>
		/// <param name="mapName">When this value returns, contains the name; otherwise, <see langword="null"/>.</param>
		/// <returns><see langword="true" /> if there is a name available; otherwise, <see langword="false" />.</returns>
		public static bool TryGetMapNameForLanguage(GameLanguage language, int map, out string mapName)
		{
			if (TryGetNamesForLanguage(language, out var names) && names.TryGetValue(map, out string name))
			{
				mapName = name;
				return true;
			}

			mapName = null;
			return false;
		}
	}
}