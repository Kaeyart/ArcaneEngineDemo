# Migration Notes — 3.1.0-alpha.1

## Source prerequisite

Install on the `feature/3.0-endgame-first-arpg` line after the 3.0 foundation. The existing persistent-stat hook in `GameWorld.cs` must be present.

## Save migration

- Schema advances to `31000`.
- A previous single 3.0 profile is imported into the new multi-character roster once.
- Missing IDs, timestamps, collections, active Core, item/map instance IDs, affix lists, equipment references, currencies, and ranges are repaired.
- Duplicate items/maps/currencies and invalid equipment references are normalized.
- Existing profile data is copied before rewrite.
- New writes use an atomic temporary file and two rotating backups.
- Load failure attempts backup restoration before reporting a damaged profile.

## Deleted characters

Deletion moves the character JSON and backups into the `DeletedCharacters` directory. It is not a one-click permanent filesystem deletion.

## Rollback

The installer prints an exact rollback command and stores its latest backup path in:

```text
.arcane-patch-3.1.0-alpha.1-last-backup
```

Rolling back source does not automatically delete saves already migrated to the 3.1 roster. Preserve the `ArcaneEngine31` persistent-data folder before testing a source rollback.
