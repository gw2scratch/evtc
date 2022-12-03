using System;

namespace GW2Scratch.ArcdpsLogManager.Logs.Naming
{
	public interface ILogNameProvider
	{
		string GetName(LogData logData);
		string GetMapName(int? mapId);

		event EventHandler MapNamesUpdated;
	}
}