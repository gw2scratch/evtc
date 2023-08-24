# Log Manager changelog

This is the full changelog of the arcdps Log Manager.

## Log Manager v1.10

#### New features
- Added support for Secrets of the Obscure strike missions
- Added support for Escort
- Added total duration and success/fail counts to the multiple log selection panel
- Added a button to set the dps.report user token in the settings
- Added max parallel threads setting to the *Game data gathering* tab (View -> Debug Data must be enabled to see this tab).

#### Performance improvements
- Improved log processing performance by ~25% for 10man logs, even more in WvW logs.

#### Fixes
- Fixed logs with Emboldened appearing mid-fight failing to be processed.
- Fixed log detail pane not appearing for selected logs processed with Log Manager versions 1.3 and older.
- Fixed a stray debug data section appearing when multiple logs were selected even if debug data wasn't enabled

#### EVTC Inspector notes
- Fixed buff extension events not being distinguished from normal buff applies
- Added stack ids to buff applies / extends
- Added "is stack active" to buff applies / extends

## Log Manager v1.9
#### New features
- Added support for Silent Surf CM and NM
- Added fractal scale as a column to the log list; right-click to hide/show (in logs since 2023-07-16)
- Added fractal scale to the log detail panel (in logs since 2023-07-16)
- *EVTC Inspector*: Added rudimentary support for the reworked effect events (since 2023-07-16).
- Added ability/buff column for skills to the Game data gathering tab (View -> Debug Data must be enabled to see this tab).
#### Changes
- The game data collecting tab (requires Show -> Debug data to be enabled) now scans multiple files at once. This
  provides a nice performance benefit on SSDs, but may result in slowdowns with HDDs (not tested on a HDD).
#### Fixes
- Fixed API error caused by adding a workaround for a GW2 API internal error when searching for unknown guilds (thank you, @Denrage!)

## Log Manager v1.8
#### New features
- Added support for Old Lion's Court CM
- Added rudimentary support for map (full instance) logs – they are correctly identified, have proper map names, and a filtering category.
- *EVTC Inspector*: Added skill and buff info from logs to Processed skills tab (no skill timings and buff formulas yet)

## Log Manager v1.7

#### New features
- Added support for Old Lion's Court (categorized under EoD strikes for now; might change later)
- Slightly improved health display for Harvest Temple (16.66% per phase), a better phase display might come in the future.
- *EVTC Inspector*: Added an option to show times since start of log (enabled by default)
- *EVTC Inspector*: Added more log information (map id, game build, ...) to statistics tab

#### Changes
- Linux only (already worked on Windows): Clicking on the filename in the log detail panel now selects the file (requires dbus).

#### Fixes
- Fixed Ai logs always being categorized as both phases.

*We now also have an automatic log test suite (134 logs) to avoid issues similar to what happened to Ai in the last release.*

## Log Manager v1.6

#### New features
- Added Emboldened (easy) mode detection and filtering.
- Added CM detection for Kaineng Overlook for real this time.
- Added CM detection for Harvest Temple.
- Added an option to generate detailed WvW reports when uploading (thanks, Somec!)
- Added filtering by players to *Advanced filters*.
- Added *None of these* as an option to instability filters.
- Added new event types to the Inspector.
- Added processed skill and effect tabs to the Inspector.

#### Changes
- *Advanced filters* are now split into tabs to provide space for more filters.
- *Advanced filters* button text now changes to show when advanced filters are active.

#### Fixes
- Fixed Elite Specializations statistics not fitting into their box with default window size on Windows
- Fixed blurry specialization icons on Windows with DPI scaling enabled

## Log Manager v1.5

#### New features
- Added CM detection for Aetherblade Hideout
- Added CM detection for Xunlai Jade Junkyard
- Added CM detection for Kaineng Overlook (we are assuming the main foe will have more health in CM)

#### Fixes
- Added End of Dragons Elite Specializations to the Statistics tab.

## Log Manager v1.4

#### New features
- Added support for all 4 End of Dragons strike missions.
- Added an option to have multiple log directories
- Mistlock Instabilities are now detected for Fractal logs and shown in the details pane.
- Added a new column for Mistlock Instabilities to the log list (right-click the column header to enable).
- Added filters for Mistlock Instabilities to *Advanced Filters*.
- Added an option to permanently delete logs by pressing delete when logs are selected – yes, there is a confirmation window (thanks, @Linkaaaaa and @therealketchup!)

#### Changes
- Strike Missions are now split into *Icebrood Saga* and *End of Dragons* subcategories.
- The Festival category (currently only Freezie and hidden by default) is moved under Strike Missions

#### Fixes
- Added missing icons for Catalyst and Specter

## Log Manager v1.3.1

#### Fixes
- Fixes single-file published versions by embedding resources within the executable
- Mordremoth now properly appears in the encounter list

## Log Manager v1.3.0
This version of Log Manager requires [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0/runtime) to run.

#### New features
- Added a dialog to compress all uncompressed logs (Data → Compress logs) (thanks, @DBPhoenix!)
- Added support for Bladesworn, Vindicator, Catalyst, Specter, Mechanist, and Untamed.
- Added support for logs of the Mordremoth fight (Hearts and Minds, may require adding id 15884 to custom logging).

#### Changes
- The program now uses [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0/runtime); .NET Core 3.1 is no longer needed.
- Significantly reduced time spent loading the cache and searching for logs (big thanks to @Denrage!)
- Significantly improved log processing performance
- The challenge mode column is now next to the encounter name column in the log list
- The status panel at the bottom now shows filtered log counts and log processing is shown separately.
- Columns of the log list now show sort directions (only on Windows)

#### Fixes
- Fixed log detail panel showing previously selected log after changing filters

## Log Manager v1.2.1

#### Changes
- Significantly improved performance of searching for log files
- Removed white pixels in Scourge icon (relevant for dark themes on Linux)

#### Fixes
- Fixed a crash caused by having a log without player data processed with an older version of the program (issue #134)
- Fixed blurry Harbinger, Virtuoso and Willbender icons in log list when DPI Scaling was enabled on Windows (issue #132)

## Log Manager v1.2.0

#### New features
- Added support for Virtuoso, Harbinger and Willbender
- Added support for EoD specializations in squad composition filters

#### Fixes
- Fixed broken profession detection for Core Guardians.
- Fixed broken event filters in the EVTC Inspector on Windows

## Log Manager v1.1.1

#### New features
- Added average times to encounter statistics (thanks, @robinwils!)
- Added tooltips to profession and specialization icons

#### Changes
- Added an exe icon for the Windows version
- The Windows exe file now has the correct version numbers
- Improved labels and button text for date and time filters
- Date and time filter buttons now round time to the nearest minute

#### Fixes
- Fixed a crash that occurs when saving logs in case arcdps 2021-03-24 generated directory names with invalid Unicode characters (logs in these directories will be processed again on every Log Manager launch, you should move them to directories with normal names)
- Fixed handling of logs that contain no profession data for a player
- Fixed encounter filters sometimes unselecting everything (it still resets sometimes, working on a proper fix)

## Log Manager v1.1

#### New features
- Added CM detection for Keep Construct
- Added squad composition filters to *Advanced filters* (issue #5)
- Added a last week filter button for convenience (issue #100)
- Added categories for filtering individual WvW maps (issue #68)
- Added icons to all encounters (thanks, @Linkaaaaa and @therealketchup!) (issue #79)

#### Changes
- Significantly improved performance of filtering
- Filtering by time can have no bound, which fixes confusion with logs not appearing if the program has been open for too long
- It is no longer possible to open multiple instances of the log manager. This prevents data loss issues in some cases.

#### Fixes
- Fixed an issue with the last processed log not updating the UI correctly on Windows (status stuck on "Processing 51/52...")
- Fixed a rare issue with Twisted Castle logs being identified as Xera logs if the player stood too close to Xera's platform (issue #110)
- Fixed some controls looking very old on Windows
- Fixed character log list including irrelevant logs if a character was deleted, the character name reused by a different account, and a log contained both accounts (issue #119).
- Fixed the mysterious disappearance of the program icon on Windows (issue #102)
- Fixed some crashes in the EVTC Inspector

## Log Manager v1.0.2

#### Fixes
- Fixed a crash when tagging logs in the window opened by "Show all logs with this guild"
- Fixed a long UI freeze when scheduling a lot of logs for reprocessing due to an update

## Log Manager v1.0.1

#### Fixes
- Fixed update notifications

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
