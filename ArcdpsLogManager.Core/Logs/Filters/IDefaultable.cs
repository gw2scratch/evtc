namespace GW2Scratch.ArcdpsLogManager.Logs.Filters;

public interface IDefaultable
{
	/// <summary>
	/// Indicates whether this is set to the default state.
	/// </summary>
	/// <returns>A value indicating whether the object is in its default state.</returns>
	bool IsDefault { get; }

	/// <summary>
	/// Resets the state to the default.
	/// </summary>
	void ResetToDefault();
}