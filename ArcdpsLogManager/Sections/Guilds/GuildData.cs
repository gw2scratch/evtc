using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Sections.Guilds
{
	public class GuildData
	{
        public string Guid { get; }

        public IReadOnlyList<LogData> Logs { get; }
        public IReadOnlyList<GuildMember> Accounts { get; }
        public IReadOnlyList<GuildCharacter> Characters { get; }

        public GuildData(string guid, IEnumerable<LogData> logs, IEnumerable<LogPlayer> logMembers)
        {
	        Guid = guid;
	        Logs = logs.ToArray();

	        var accountList = new List<GuildMember>();
	        var characterList = new List<GuildCharacter>();

	        var members = logMembers.ToArray();
	        var accounts = members.GroupBy(x => x.AccountName);
	        foreach (var accountGrouping in accounts)
	        {
		        string accountName = accountGrouping.Key;

		        var accountLogs = Logs
			        .Where(l => l.Players.Any(p => p.AccountName == accountName))
			        .Distinct()
			        .ToArray();
		        var account = new GuildMember(accountName, accountLogs);

		        var accountCharacters = new List<GuildCharacter>();

		        var characters = accountGrouping.GroupBy(x => x.Name);
		        foreach (var characterGrouping in characters)
		        {
			        string characterName = characterGrouping.Key;
			        var characterLogs = accountLogs.Where(l => l.Players.Any(p => p.Name == characterName)).Distinct();
					var character = new GuildCharacter(account, characterName, characterLogs);
					accountCharacters.Add(character);
		        }

		        account.Characters = accountCharacters;

		        accountList.Add(account);
		        characterList.AddRange(account.Characters);
	        }

	        Accounts = accountList;
	        Characters = characterList;
        }
	}
}