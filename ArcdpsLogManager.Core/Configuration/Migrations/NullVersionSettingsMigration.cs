using System;

namespace GW2Scratch.ArcdpsLogManager.Configuration.Migrations;

public class NullVersionSettingsMigration(Action<StoredSettings> migration) : ISettingsMigration
{
	public bool Applies(Version version)
	{
		return version == null;
	}

	public void Apply(StoredSettings settings)
	{
		migration(settings);
	}
}