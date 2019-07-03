using System.Collections.Generic;
using GW2Scratch.RotationComparison.Rotations;

namespace GW2Scratch.RotationComparison.Logs
{
	public interface ILogSource
	{
		void SetCharacterNameFilter(params string[] names);
		IEnumerable<Rotation> GetRotations();
		string GetEncounterName();
		string GetLogName();
	}
}