[![Build and test (.NET Core)](https://img.shields.io/github/workflow/status/gw2scratch/evtc/Build%20and%20test%20(.NET%20Core)?logo=github)](https://github.com/gw2scratch/evtc/actions?query=workflow%3A%22Build+and+test+%28.NET+Core%29%22)
[![Discord](https://img.shields.io/discord/543804828808249374?label=discord&logo=discord&logoColor=white&)](https://discord.gg/TnHpN34)

# GW2Scratch EVTC Tools
This repository hosts multiple programs for analysis and management of EVTC logs generated by the [arcdps](https://www.deltaconnected.com/arcdps/) addon for Guild Wars 2. All programs are available for Windows/Linux.

#### Dependencies
  - Running programs: .NET 6
  - Development: .NET 6 SDK or newer

#### Programs
  - [arcdps Log Manager](#arcdps-log-manager)
  - [EVTC Inspector](#evtc-inspector)

#### Libraries
  - [GW2Scratch.EVTCAnalytics](#evtc-analytics)

## [arcdps Log Manager](ArcdpsLogManager)

A manager for arcdps EVTC logs. Filter logs, upload them with one click, find fastest kills and interesting statistics.

- Filter logs by time, encounter, success/failure
- Quickly see the composition, players, encounter duration for any log
- Upload to dps.report with one click, batch uploads
- A table of all players in your currently filtered logs
- A table of all guilds within your currently filtered logs

## [EVTC Inspector](EVTCInspector)
A program for exploring EVTC logs. Handy when developing anything that analyzes logs.

- browse through raw combat items
- browse through raw agent data
- browse through processed events, with filtering
- browse through agents and events they are involved in

## [EVTC Analytics](EVTCAnalytics)
The core library for parsing and analyzing EVTC logs. Built with integration
in other projects in mind. Currently has a somewhat API, changes are to be expected.

- get raw agent, skill and combat item data from logs
- get agents and processed events with structured data
- get encounter results
- calculate statistics such as DPS, buff uptimes and similar (very work-in-progress)

There is [documentation](https://gw2scratch.github.io/evtc/master/) available.

## Contributing
Reporting bugs is the most important way of contributing, it's hard to fix things you are
not aware about. Either create an issue on GitHub or let us know in the `#bug-reports`
channel in our [Discord server](https://discord.gg/rNXRS6ZkYe).

Tiny fixes are always welcome, however do please discuss bigger changes first. Currently,
most of the projects are very much in early development and significant changes are planned.

In the future, this repository may get split up into smaller ones for the individual projects.
