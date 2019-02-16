namespace RotationComparison.Rotations
{
	public class PlayerData
	{
		public string Name { get; }
		public string IconUrl { get; }
		public string LogName { get; }
		public string EncounterName { get; }

		public PlayerData(string name, string iconUrl, string logName, string encounterName)
		{
			Name = name;
			IconUrl = iconUrl;
			LogName = logName;
			EncounterName = encounterName;
		}
	}
}