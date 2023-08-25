using GW2Scratch.EVTCAnalytics.Parsed;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Parsing;

public class CombatItemFilterCache
{
	private readonly Dictionary<ParsedBossData, ICombatItemFilters> filtersByBossData = new Dictionary<ParsedBossData, ICombatItemFilters>();
	
	public bool TryGetFilters(ParsedBossData bossData, out ICombatItemFilters filters)
	{
		return filtersByBossData.TryGetValue(bossData, out filters);
	}

	public void CacheFilters(ParsedBossData bossData, ICombatItemFilters filters)
	{
		filtersByBossData[bossData] = filters;
	}

	public void Clear()
	{
		filtersByBossData.Clear();
	}
}