namespace GW2Scratch.EVTCAnalytics.Model;

/// <summary>
/// Represents an error logged by arcdps.
/// </summary>
/// <remarks>
/// Added in arcdps 20200513.
/// </remarks>
public class LogError
{
	public string Error { get; }

	public LogError(string error)
	{
		Error = error;
	}

	public override string ToString()
	{
		return Error;
	}
}