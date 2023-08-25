using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Parsing;

public class UnboundedArrayFilter : IIdFilter
{
	private readonly bool[] filter;
	
	public UnboundedArrayFilter(IReadOnlyList<uint> keptIds)
	{
		var max = keptIds.DefaultIfEmpty().Max();
		filter = new bool[max + 1];

		foreach (var id in keptIds)
		{
			filter[id] = true;
		}
	}
	
	public bool IsKept(uint id)
	{
		return id < filter.Length && filter[id];
	}
}