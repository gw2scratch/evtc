# Log Editor
This is a simple command-line program for editing arcdps EVTC logs.

Requires .NET 6.

## Supported features
 - Anonymizing account and character names
 - Removing reward events. This is useful for testing success detection for kills that are repeated within a reward period. As of GW2 build 97235, reward chests are not awarded at all if a raid boss is repeated twice in a week.

## Example of usage
The following command will replace players character and account with anonymous names:
```
./LogEditor --anonymized --output-zevtc --input input.zevtc --output output.zevtc
```

Use the following command to see all options:
```
./LogEditor --help
```
