namespace GW2Scratch.ArcdpsLogManager.Logs.Naming
{
	public interface ILogNameProvider
	{
		string GetName(LogData logData);
	}
}