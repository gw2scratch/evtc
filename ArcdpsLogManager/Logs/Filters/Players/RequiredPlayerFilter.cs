using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Players;

public class RequiredPlayerFilter : ILogFilter
{
	public string AccountName { get; }
	public string CharacterName { get; }
	public Profession? Profession { get; }

	/// <summary>
	/// Constructs a new filter requiring a player to appear within a log.
	/// </summary>
	/// <param name="accountName">Account name, with leading <c>:</c>.</param>
	/// <param name="characterName">Optional character name. If <see langword='null'/>, any character is allowed.</param>
	/// <param name="profession">Optional profession. If <see langword='null'/>, any profession is allowed.</param>
	public RequiredPlayerFilter(string accountName, string characterName = null, Profession? profession = null)
	{
		ArgumentNullException.ThrowIfNull(accountName);

		AccountName = accountName;
		CharacterName = characterName;
		Profession = profession;
	}

	public bool FilterLog(LogData log)
	{
		if (log.Players == null)
		{
			return false;
		}

		foreach (var player in log.Players)
		{
			bool accountNameMatches = AccountName == player.AccountName;
			bool characterNameMatches = CharacterName == null || (player.Name == CharacterName);
			bool professionMatches = Profession == null || (player.Profession == Profession);
			if (accountNameMatches && characterNameMatches && professionMatches)
			{
				return true;
			}
		}

		return false;
	}
}