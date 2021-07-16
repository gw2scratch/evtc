---
uid: EVTCAnalytics.Contributing.NewEncounter
title: Adding support for a new encounter
---
# Adding support for a new encounter

Introducing support for a new encounter requires multiple steps:

- Add a new value to the Encounter enum in `GameData/Encounters/Encounter.cs`
- Specify a Category for this Encounter in `GameData/Encounters/EncounterCategories.cs`
- Specify an English name for this Encounter in `GameData/Encounters/EncounterNames.cs`

The presence of category and name is checked running tests defined
in `EVTCAnalytics.Tests`, run these to verify this has been done this correctly.

After doing this, add a way to identify the newly added encounter
in the `IdentifyEncounter()` method of the `Processing/DefaultEncounterIdentifier.cs`.
This will be done by checking the species id of the main target in almost all cases.
The most common exception is fights that may register a different boss in the
same instance if the recording player is too close to them.

In case you do not have the species id of the NPC, you can use arcdps to show a window with details
that includes it if you select the NPC. The default shortcut for this window is Alt+Shift+S.

The last step missing is to provide custom `EncounterData` if needed.
`EncounterData` contains definitions that are used to determine whether
the encounter was successful, whether it was in Challenge Mode and others.
You might need to implement new Determiners in case the encounter is not very
typical. If you do, try to make them generic enough to be reusable in case
similar encounters are introduced in the future.

The EVTC Inspector is very useful for finding reliable ways to check results
and to figure out what is happening in the logs.

After specifying all this, gather enough logs to verify your `EncounterData`
works as expected and you are done.
