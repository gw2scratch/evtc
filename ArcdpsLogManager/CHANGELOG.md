# Log Manager changelog

This is the full changelog of the arcdps Log Manager.

## Log Manager v1.0.0

#### Important

The Log Manager is now a .NET Core program. It is available in two versions:
- standard (smaller file size, requires .NET Core 3.1 runtime installed)
- self-contained (bigger file size, does not require .NET Core installed)

.NET Core 3.1 runtime is available [here](https://dotnet.microsoft.com/download/dotnet-core/3.1/runtime).

#### New features
- Added better filtering UI for encounters that allows selecting multiple bosses (issue #66)
- Commander tags are shown in log details for logs from arcdps version 2020-06-09 and newer (issue #72)
- Added support for Varinia Stormsounder, the main boss in Strike Mission: Cold War (issue #77)
- Added support for Ai, Keeper of the Peak, the CM boss in the Sunqua Peak fractal (issue #89)
- Added boss health percentage to failures, both in log list and log details (issue #63)
- Added an option to tag and favorite logs (thanks, [@jcogilvie](https://github.com/jcogilvie)!; issue #70)
- Added a Log Manager update check on launch (can be disabled)

#### Changes
- The manager now requires .NET Core 3.1 or newer
- Moved the filtering UI to the left side, can be collapsed and moved to a tab instead (issue #66)
- Increased the default width of the manager window
- Changed all filters to change shown logs instantly
- Improved encounter durations to closely match Elite Insights times
- EVTC files are detected correctly if they do not have an extension
- Uncompressed logs are automatically compressed when uploading to dps.report
- Kitty Golems (Special Forces Training Area) now have somewhat reliable success detection
- Significantly improved loading time of the manager (issue #16)

#### Fixes
- Fixed Voice and Claw success detection
- Fixed some very rare Xera false successes
- Fixed short Twin Largos failures being classified as Unknown result
- Fixed uploading logs without an extension to dps.report
- Fixed log uploads failing after 100 seconds (issue #65)
- Fixed saving hidden log list columns
- Fixed newly generated logs sometimes not appearing properly (issue #40)


## Log Manager v0.7.2

__**Important**__: See the changelog for Log Manager v0.7 if you are updating from an older version than that.

_**Changes**_
- Slightly improved how the filters look, more changes soon.
- Both time filter buttons now round the same way, more changes soon.
- Reverted forced sorting order for log links when multiple logs are selected

_**Fixes**_
- Logs generated with the 2020-05-06 version of arcdps now get correctly processed (added a workaround for an arcdps bug)
- Fixed CM detection for Skorvald the Shattered CM.
- Fixed text for the log location setting being cut off.

## Log Manager v0.7.1

__**Important**__: See the changelog for Log Manager v0.7 if you are updating from an older version.

_**New features**_
- When a processing issue is fixed (such as success/mode detection), you will get a dialog with an option to update data from affected logs. This means you won't have to process logs again unless needed.

_**Fixes**_
- Fixed Twin Largos success detection when only one of the Largos dies.
- Fixed Deimos success detection in some rare cases when a group wipes in the final 10% phase.
- Fixed CM detection for 2018 logs with random big maximum health values.

## Log Manager v0.7

__**Important**__:  Saved data from processed logs will be lost with this update because of a rework in how it is saved. You will get an error message that says the log cache data could not be loaded. Select the option to delete it. This should not be ever needed in the future anymore (I hope). All logs will be processed again, although you'd want to do that for CM detection anyway.

_**New features**_
- Added CM detection, filtering
- Added support for Whisper of Jormag
- Added Twisted Castle success detection
- Added a button to get to a selected log file quickly (bottom right)
- Added a button to open multiple uploaded logs in a browser at once
- Added more columns to the log list - CM (shown by default), character name, game version, arcdps version, map id (hidden by default, maps will eventually get names)
- Columns can be hidden/shown if you right click the log list
- Added an option to change which endpoint for dps.report is used (some work better in certain parts of the world)
- Added a way to see the user token used for uploading to dps.report

_**Changes**_
- Encounter names are always in English (possible support for more languages later, would just need translations)
- Settings are now split into tabs
- Filtering by last day allows new logs to be shown
- Reworked how processed log data is stored
- Removed some unfinished buttons related to arcdps settings and updates
- Removed a button that would one day be able to edit arcdps build templates...

_**Fixes**_
- Fixed Deimos success detections (again, 3 times)
- Fixed quite a few obscure bugs with weird logs
- Errors while loading are now shown properly
- Fixed logs sometimes being in wrong order in the panel for batch uploading

## Log Manager v0.6

**Changes**
Encounter names are now always in English, even if the enemy name is not available (You won't be seeing names such as ch-17126-542 anymore).

The encounter names for already processed logs do not update automatically. To update them, you can process all logs again (Data -> Log cache -> Delete the cache) or only selected ones if you enable Settings -> Show Debug Data and use the Reparse button.

This will be reworked in the future and there will be at least one time when all logs will have to be processed again before the full release.

**Bug fixes**
- Fixed wrong encounter durations for new logs. Logs have to be processed again, see the paragraph above for instructions.

## Log Manager v0.5

- Added per-encounter stats (log counts, time spent, success rate...)
- Added a new setting for minimum log duration
- Automatically adds logs when they are created (needs testing)
- Uploads and GW2 API requests are now shown in the status bar
- Minor changes to the layout of the player and guild lists
- Fixed Xera success detection
- Fixed a crash when trying to upload from a log from a log list window

## Log Manager v0.4

- All tables are now sortable
- Fixes issues with sorting not persisting

## Log Manager v0.3.1

- Fixes more issues in 2017 logs (duplicated skill definitions)

## Log Manager v0.3

- Initial support for WvW
- Hopefully all 2017 logs work now
- Fixed the batch upload progressbar
- Reworked how settings are saved (they will reset)
- Packed all the random looking files into the .exe file

## Log Manager v0.2

Fixed sorting issues and huge icons in guild details.

## Log Manager v0.1

The first alpha version of the Log Manager. Likely to contain bugs, make sure to let us know about them.
