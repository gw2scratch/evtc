using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	public interface IModeDeterminer
	{
		EncounterMode GetMode(Log log);
	}
}