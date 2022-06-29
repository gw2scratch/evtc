using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;

/// <summary>
/// A mode determiner that tries a determiner and fallbacks to another one. If the mode is not identified
/// and the default (<see langword="null"/>) is returned a fallback determiner is used instead.
/// </summary>
public class FallbackModeDeterminer : IModeDeterminer
{
	private readonly IModeDeterminer firstDeterminer;
	private readonly IModeDeterminer fallbackDeterminer;

	public FallbackModeDeterminer(IModeDeterminer firstDeterminer, IModeDeterminer fallbackDeterminer)
	{
		this.firstDeterminer = firstDeterminer;
		this.fallbackDeterminer = fallbackDeterminer;
	}
	public EncounterMode? GetMode(Log log)
	{
		return firstDeterminer.GetMode(log) ?? fallbackDeterminer.GetMode(log) ?? EncounterMode.Unknown;
	}
}