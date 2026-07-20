# Contributing to Arcane Engine Demo

## Working principles

- Preserve SpellForge and the seven-element reaction identity.
- Keep persistent schemas versioned and migration-aware.
- Separate design intent, static validation, Unity compilation, and Play Mode validation.
- Prefer cohesive, reversible changes over broad unvalidated rewrites.
- Never commit Unity-generated folders or installer backup directories.

## Branches

- `main` contains the maintenance/releasable state.
- Use `feature/<scope>` for product work.
- Use `hotfix/<version>-<scope>` for focused defects.
- Use `release/<version>` only for release preparation.

Arcane Engine 3.0 work belongs on:

```text
feature/3.0-endgame-first-arpg
```

## Commit format

Use an imperative subject with a clear scope:

```text
feat(atlas): persist first-completion sustain
fix(spellforge): prevent disconnected Rune activation
docs(release): add 3.0.0-alpha.1 validation status
chore(repo): remove tracked installer backups
```

Keep generated content, code changes, and documentation updates in the same commit when they form one atomic change.

## Unity asset rules

- Keep `.meta` files with their assets.
- Do not regenerate GUIDs unnecessarily.
- Close Unity before applying source-transforming patches.
- Use text serialization for scenes, prefabs, and assets.
- Use Git LFS for binary art and audio covered by `.gitattributes`.
- Do not commit `Library`, `Temp`, `Obj`, `Logs`, `UserSettings`, builds, recordings, or memory captures.

## Persistent data changes

A change to profiles, maps, items, classes, Constellations, or discoveries must include:

- Stable IDs.
- Schema version change when required.
- Migration or repair behavior.
- Failure and backup behavior.
- Validation steps.
- Compatibility statement for old saves.

## Pull requests

A pull request should contain:

- Purpose and scope.
- Files and systems changed.
- Migration impact.
- Static validation performed.
- Unity version used.
- Console result.
- Play Mode result.
- Known limitations.
- Screenshots or captures for visible changes when useful.

Do not report Unity success unless the target editor actually compiled and ran the change.

## Release artifacts

Do not commit release ZIPs or local backup directories into the project tree. Attach release ZIPs and checksums to
GitHub Releases.
