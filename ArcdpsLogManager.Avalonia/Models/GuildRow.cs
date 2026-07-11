using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Sections.Guilds;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// A display-ready projection of a guild (Avalonia counterpart of the Eto <c>GuildList</c>
	/// grid rows), backed by <see cref="Sections.Guilds.GuildData"/>. Guild name/tag come from the
	/// API data cache (unknown until the guild is fetched from the GW2 API).
	/// </summary>
	public sealed class GuildRow
	{
		public GuildData Data { get; }

		public string Tag { get; }
		public string Name { get; }
		public int LogCount { get; }
		public int PlayerCount { get; }
		public int CharacterCount { get; }

		/// <summary>Member account names (for the detail list).</summary>
		public IReadOnlyList<string> Members { get; }

		public GuildRow(GuildData data, ApiData apiData)
		{
			Data = data;
			Tag = data.Guid == null ? "null" : apiData.GetGuildTag(data.Guid);
			Name = data.Guid == null ? "null" : apiData.GetGuildName(data.Guid);
			LogCount = data.Logs.Count;
			PlayerCount = data.Accounts.Count;
			CharacterCount = data.Characters.Count;

			Members = data.Accounts
				.OrderByDescending(a => a.Logs.Count)
				.Select(a => $"{a.Name.TrimStart(':')} ({a.Logs.Count})")
				.ToList();
		}
	}
}
