using GW2Scratch.EVTCAnalytics.Model;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Instability;

public class InstabilityFilter : ILogFilter
{
	private readonly MistlockInstability instability;

	public InstabilityFilter(MistlockInstability instability)
	{
		this.instability = instability;
	}

	public bool FilterLog(LogData log)
	{
		return log.LogExtras?.FractalExtras?.MistlockInstabilities.Contains(instability) ?? false;
	}
}