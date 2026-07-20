# Repository Structure

## Tracked project roots

- `Assets/`: Unity source and assets.
- `Packages/`: Unity package manifest and lock file.
- `ProjectSettings/`: Unity configuration.
- `docs/`: design, architecture, release, and validation records.
- `Tools/`: existing project utilities.
- `tools/`: repository maintenance and validation.
- `.github/`: automation and contribution templates.

## Never tracked

- Unity `Library`, `Temp`, `Obj`, `Logs`, and `UserSettings`.
- Local builds and test artifacts.
- Installer backup directories ending in `Backups`.
- Root `.arcane-*-last-backup` state pointers.
- IDE-generated solution and project files.
- Release ZIPs intended for GitHub Releases.

## Patch markers

Files such as `Assets/ArcaneEngine/PATCH_2_2_5.txt` are not local residue. They are installation prerequisites and
should remain tracked.

Root state pointers such as `.arcane-patch-2.2.5-last-backup` identify a local backup path and should not be tracked.

## Documentation migration

Historical documents are currently flat under `docs/`. Move them only in a dedicated documentation commit, grouped
by release line, with all internal links updated and checked.
