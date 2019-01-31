using System.Collections.Generic;
using ScratchEVTCParser.Statistics.PlayerDataParts;

namespace RotationComparison.Logs
{
	public interface ILogSource
	{
		void SetCharacterNameFilter(params string[] names);
		IEnumerable<PlayerRotation> GetRotations();
		string GetEncounterName();
		string GetLogName();
	}
}