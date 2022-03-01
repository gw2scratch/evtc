using System;

namespace GW2Scratch.ArcdpsLogManager.Configuration.Migrations;

public class MaxVersionSettingsMigration : ISettingsMigration
{
	private readonly Version maxVersion;
	private readonly Action<StoredSettings> migration;

	public MaxVersionSettingsMigration(Version minVersion, Action<StoredSettings> migration)
	{
		this.maxVersion = minVersion;
		this.migration = migration;
	}

	public bool Applies(Version version)
	{
		return version < maxVersion;
	}

	public void Apply(StoredSettings settings)
	{
		migration(settings);
	}
}