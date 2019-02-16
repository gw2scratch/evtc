using System.Collections.Generic;
using RotationComparison.Rotations;

namespace RotationComparison.Logs
{
	public interface ILogSource
	{
		void SetCharacterNameFilter(params string[] names);
		IEnumerable<Rotation> GetRotations();
		string GetEncounterName();
		string GetLogName();
	}
}