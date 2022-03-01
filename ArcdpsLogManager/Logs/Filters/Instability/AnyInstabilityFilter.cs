using GW2Scratch.EVTCAnalytics.Model;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Instability;

public class AnyInstabilityFilter : ILogFilter
{
	private readonly List<MistlockInstability> instabilities;

	public AnyInstabilityFilter(IEnumerable<MistlockInstability> instabilities)
	{
		this.instabilities = instabilities.ToList();
	}

	public bool FilterLog(LogData log)
	{
		foreach (var instability in instabilities)
		{
			if ((log.LogExtras?.FractalExtras?.MistlockInstabilities.Contains(instability) ?? false))
			{
				return true;
			}
		}

		return false;
	}
}