using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Parsing;

public class HashSetFilter : IIdFilter
{
	private readonly HashSet<uint> filter;
	
	public HashSetFilter(IEnumerable<uint> keptIds)
	{
		filter = new HashSet<uint>();

		foreach (var id in keptIds)
		{
			filter.Add(id);
		}
	}
	
	public bool IsKept(uint id)
	{
		return filter.Contains(id);
	}
}