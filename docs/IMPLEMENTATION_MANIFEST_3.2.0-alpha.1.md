# Implementation Manifest — 3.2.0-alpha.1

## Modified runtime files

- `ArpgContent30.cs`: registers the expanded 3.2 catalogues and updates legacy validation for expanded counts.
- `ArpgData30.cs`: advances the profile schema to 32000 and enables grid-managed capacity.
- `ArpgFoundation30.cs`: integrates map-affix danger, account-wide map consumption, protected completion rewards, and boss-exclusive Unique target farming.
- `ArpgItems30.cs`: routes item generation through the 3.2 factory and applies extended stats/tooltips.
- `ArpgLoot31.cs`: integrates grid-aware pickup, shared currency, loot filters, and Unique/Exceptional presentation.
- `ArpgFrontend31.cs`: carries forward mouse-first frontend and compile repairs.
- `ArpgRefuge31.cs`: adds the physical Arsenal & Exchange station.

## New runtime files

- `ArpgArsenalContent32.cs`
- `ArpgArsenalData32.cs`
- `ArpgArsenalSystems32.cs`
- `ArpgArsenalUI32.cs`

## New Editor tooling

- `ArpgArsenalValidator32.cs`

## Persistent files

- `arcane_arsenal_320_account.json`
- `arcane_arsenal_320_account.backup.json`

The 3.2 account save owns variable-size grid positions, account-wide equipment stash items, shared maps, shared Core/Rune/Link discoveries, account currency, filters, vendor state, Unique/Exceptional metadata, corruption, sealed modifiers, and craft history. Existing character profiles remain authoritative for character identity, progression, and equipped/carried item instances.
