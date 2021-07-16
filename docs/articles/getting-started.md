---
uid: EVTCAnalytics.GettingStarted
title: Getting Started
---
# Getting started

## Processing steps
The library uses three distinct steps to read EVTC logs and access data.
1. Parsing (file → raw values in [`ParsedLog`](../api/GW2Scratch.EVTCAnalytics.Parsed.ParsedLog.html)) – [`EVTCParser`](../api/GW2Scratch.EVTCAnalytics.EVTCParser.html)
2. Processing (raw values in [`ParsedLog`](../api/GW2Scratch.EVTCAnalytics.Parsed.ParsedLog.html) → processed objects in [`Log`](../api/GW2Scratch.EVTCAnalytics.Model.Log.html)) – [`LogProcessor`](../api/GW2Scratch.EVTCAnalytics.Processing.LogProcessor.html)
3. Analysis (processed objects in [`Log`](../api/GW2Scratch.EVTCAnalytics.Model.Log.html) → results) – [`LogAnalyzer`](../api/GW2Scratch.EVTCAnalytics.LogAnalyzer.html) and custom analysis

## Step 1: Parsing
This first step reads raw data from EVTC logs into *Parsed* objects, which contain raw values
in a format very similar to the original structs used.
The [`EVTCParser`](../api/GW2Scratch.EVTCAnalytics.EVTCParser.html) class is used for this, and the result is [`ParsedLog`](../api/GW2Scratch.EVTCAnalytics.Parsed.ParsedLog.html).

This is the last step at which data may be modified. The [`EVTCWriter`](../api/GW2Scratch.EVTCAnalytics.EVTCWriter.html) may be used to write a (potentially
modified) [`ParsedLog`](../api/GW2Scratch.EVTCAnalytics.Parsed.ParsedLog.html) as an EVTC file.

## Step 2: Processing

This step converts raw data from the parsing step into *processed* objects which only contain relevant values.
The [`LogProcessor`](../api/GW2Scratch.EVTCAnalytics.Processing.LogProcessor.html) accepts a [`ParsedLog`](../api/GW2Scratch.EVTCAnalytics.Parsed.ParsedLog.html) and produces an immutable [`Log`](../api/GW2Scratch.EVTCAnalytics.Model.Log.html) with processed
[events](../api/GW2Scratch.EVTCAnalytics.Events.html),
[agents](../api/GW2Scratch.EVTCAnalytics.Model.Agents.Agent.html),
and [skills](../api/GW2Scratch.EVTCAnalytics.Model.Skills.Skill.html).

This is also the step at which the encounter is identified, but no analysis, such as success detection is made.

## Step 3: Analysis

This step includes all actions done with the immutable [`Log`](../api/GW2Scratch.EVTCAnalytics.Model.Log.html) object.
The [`LogAnalyzer`](../api/GW2Scratch.EVTCAnalytics.LogAnalyzer.html) allows easy access to common statistics.

## Example code
This is a short example of using the library. There is also a sample project available in the repository.
```cs
string filename = "example.zevtc";

var parser = new EVTCParser();      // Used to read a log file and get raw data out of it
var processor = new LogProcessor(); // Used to process the raw data

// The parsed log contains raw data from the EVTC file
ParsedLog parsedLog = parser.ParseLog(filename);

// The log after processing the raw data into structured events and agents.
Log log = processor.ProcessLog(parsedLog);

// At this point, we can do anything with the processed data, and use the LogAnalyzer
// for easy access to most common results with caching.
var analyzer = new LogAnalyzer(log);

Encounter encounter = analyzer.GetEncounter();

// Encounter names are available for some languages, we use the target name if it's not.
if (EncounterNames.TryGetEncounterNameForLanguage(GameLanguage.English, encounter, out string name))
    Console.WriteLine($"Encounter: {name}");
else
    Console.WriteLine($"Encounter: {log.MainTarget?.Name ?? "unknown target"}");

Console.WriteLine($"Result: {analyzer.GetResult()}");
Console.WriteLine($"Mode: {analyzer.GetMode()}");
Console.WriteLine($"Duration: {analyzer.GetEncounterDuration()}");

// The processed log allows easy access to data about agents
foreach (var player in log.Agents.OfType<Player>())
{
    Console.WriteLine($"{player.Name} - {player.AccountName} - {player.Profession} - {player.EliteSpecialization}");
}

// Events may be accessed as well
foreach (var deadEvent in log.Events.OfType<AgentDeadEvent>())
{
    if (deadEvent.Agent is Player player)
        Console.WriteLine($"{player.Name} died at {deadEvent.Time}.");
}
```
