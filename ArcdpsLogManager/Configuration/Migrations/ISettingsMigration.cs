using System;

namespace GW2Scratch.ArcdpsLogManager.Configuration.Migrations;

public interface ISettingsMigration
{
	bool Applies(Version version);
	void Apply(StoredSettings settings);
}