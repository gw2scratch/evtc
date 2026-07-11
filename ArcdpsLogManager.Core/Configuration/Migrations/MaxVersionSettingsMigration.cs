using System;

namespace GW2Scratch.ArcdpsLogManager.Configuration.Migrations;

public class MaxVersionSettingsMigration(Version maxVersion, Action<StoredSettings> migration) : ISettingsMigration
{
	public bool Applies(Version version)
	{
		return version < maxVersion;
	}

	public void Apply(StoredSettings settings)
	{
		migration(settings);
	}
}