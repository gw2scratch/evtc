# GW2Scratch.EVTCAnalytics.LogTests.LocalSets

A simple and small program for testing processing EVTC logs in sets with GW2Scratch.EVTCAnalytics.

Allows specifying expected results. Reports how many logs and what does not match expected values.

## Log database
TODO

There will be a database of anonymized logs for testing. It will contain at least one
success and one failure log for each boss and a Challenge Mode log if applicable.

Logs for uncommon edge cases and broken logs will also be included.

## Usage
```
dotnet run --local config.toml
```
Multiple local configs may be specified with multiple `--local filename` arguments.

## Sample config
Filenames are relative to the location of the config.
`null` values are not checked, specified values have to match.

```toml
[[log]]
filename = "encounters/deimos_fail_close.zevtc"
encounter = "Deimos"
result = "Failure"
mode = "Normal"
comment = "Failure; last player died at 0.49% of Deimos' health."

[[log]]
filename = "unusual/sabetha_split_agent.zevtc"
encounter = "Sabetha"
result = "Success"
mode = "Normal"
comment = "Successful Sabetha with split main agent at 25%"

[[log]]
filename = "unusual/sabetha_split_agent2.zevtc"
encounter = "Sabetha"
result = "Success"
mode = "Normal"
comment = "Successful Sabetha with split main agent at 25%"
```

### Sample output
```
OK log-tests/20190807-195107.zevtc Failure; last player died at 0.49% of Deimos' health.
WRONG log-tests/20190807-213308.zevtc Successful Sabetha with split main agent at 25%
        OK Encounter Expected: Sabetha, Actual: Sabetha
        WRONG Result Expected: Success, Actual: Failure
        OK Mode Expected: Normal, Actual: Normal
WRONG log-tests/20190807-153724.zevtc Successful Sabetha with split main agent at 25%
        OK Encounter Expected: Sabetha, Actual: Sabetha
        WRONG Result Expected: Success, Actual: Failure
        OK Mode Expected: Normal, Actual: Normal

1/3 OK
2/3 WRONG
0/3 FAILED
```

- `OK` logs match requirements.
- `WRONG` logs do not match requirements.
- `FAILED` logs are logs for which an exception was thrown.
