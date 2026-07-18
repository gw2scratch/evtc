using System;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Services
{
	/// <summary>
	/// A free-text <see cref="ILogFilter"/> that matches the encounter name, the point-of-view
	/// character/account, or any participating player's character/account name.
	/// </summary>
	public sealed class TextSearchFilter : ILogFilter
	{
		private readonly ILogNameProvider nameProvider;

		public string Query { get; set; } = "";

		public TextSearchFilter(ILogNameProvider nameProvider)
		{
			this.nameProvider = nameProvider;
		}

		public bool FilterLog(LogData log)
		{
			if (string.IsNullOrWhiteSpace(Query))
			{
				return true;
			}

			var q = Query;
			bool Contains(string? s) => s != null && s.IndexOf(q, StringComparison.CurrentCultureIgnoreCase) >= 0;

			if (Contains(nameProvider.GetName(log)) ||
			    Contains(log.PointOfView?.CharacterName) ||
			    Contains(log.PointOfView?.AccountName))
			{
				return true;
			}

			return log.Players != null && log.Players.Any(p => Contains(p.Name) || Contains(p.AccountName));
		}
	}
}
