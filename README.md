# Arcane Engine Demo

Arcane Engine is a Unity action-RPG prototype centered on **SpellForge**: constructed spells assembled from
Spell Cores, Support Runes, board topology, delivery mechanics, elements, and Spell Links.

The current product direction is an **endgame-first persistent ARPG**. A new character starts at Level 0 with
one minimal Core and must build equipment, currency, Runes, map completion, Constellations, and class
specialization through a forty-tier map structure. The earlier roguelite structure remains available as
optional side content.

## Project status

| Line | Purpose | Status |
|---|---|---|
| `2.2.x` | Maintenance baseline for SpellForge, reactions, lifecycle, audio, and rewards | Intended baseline: 2.2.5 |
| `3.0.0-alpha.1` | Persistent profile, classes, maps, item/currency foundations, Constellations, Ascendancies | Architecture alpha; target-editor validation required |

Unity compilation and runtime behavior are not considered verified until tested in the target editor.

## Requirements

- Unity **6000.5.2f1**
- Git
- Git LFS

## Clone and open

```bash
git clone <repository-url>
cd ArcaneEngineDemo
git lfs install
git lfs pull
```

Open the repository root as a Unity project.

## Repository layout

```text
Assets/             Unity source, scenes, generated content, and project assets
Packages/           Unity package manifest and lock data
ProjectSettings/    Unity project configuration
docs/               Architecture, release records, validation, roadmap, and workflows
Tools/              Existing project-specific utilities
tools/              Repository-maintenance and validation utilities
.github/            CI, issue forms, pull-request template, and ownership rules
```

Generated Unity folders, local installer backups, and patch-state pointer files must not be committed.

## Development branches

- `main`: maintenance and releasable state.
- `feature/3.0-endgame-first-arpg`: active 3.0 development.
- `hotfix/2.2.x-*`: focused maintenance work.
- `release/<version>`: short-lived release preparation.

See [Development Workflow](docs/DEVELOPMENT_WORKFLOW.md).

## Validation

Repository-only checks:

```bash
python3 tools/repo_hygiene.py --project .
```

Unity validation for a gameplay release must include compilation, the supplied Editor validator, and its
version-specific Play Mode checklist.

## Core identity

- SpellForge construction using Cores, Runes, topology, and Links.
- Seven elements: Fire, Cold, Lightning, Physical, Blood, Toxic, and Void.
- Ailments and controlled multi-element reactions.
- Procedural combat rooms and spell morphology.
- Persistent map, item, crafting, class, and Constellation progression in 3.0.
- Optional roguelite/Fracture Run content.

## Releases

Release artifacts belong in GitHub Releases, not in the tracked Unity project tree. Each release should include:

- Versioned ZIP.
- SHA-256 checksum.
- Patch notes.
- Implementation manifest.
- Migration notes.
- Validation checklist.
- Honest Unity-validation status.

See [Release Process](docs/RELEASE_PROCESS.md).

## Contributing

Read [CONTRIBUTING.md](CONTRIBUTING.md) before changing Unity assets, persistent schemas, or release tooling.

## License

No open-source license is currently declared by this repository cleanup. Public access alone does not grant
permission to reuse, redistribute, or create derivative works. Kaey should choose and add a license deliberately.
