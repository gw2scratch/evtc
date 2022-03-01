using System;

namespace GW2Scratch.ArcdpsLogManager.Configuration.Migrations;

public class NullVersionSettingsMigration : ISettingsMigration
{
	private readonly Action<StoredSettings> migration;

	public NullVersionSettingsMigration(Action<StoredSettings> migration)
	{
		this.migration = migration;
	}

	public bool Applies(Version version)
	{
		return version == null;
	}

	public void Apply(StoredSettings settings)
	{
		migration(settings);
	}
}