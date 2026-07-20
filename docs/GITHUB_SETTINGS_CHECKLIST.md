# GitHub Settings Checklist

These settings require repository-owner access and are not changed by the cleanup package.

## About panel

Recommended description:

```text
Unity ARPG prototype centered on modular SpellForge construction, elemental reactions, and persistent map progression.
```

Recommended topics:

```text
unity
unity3d
csharp
arpg
action-rpg
procedural-generation
spellcrafting
game-development
```

## General

- Default branch: `main`.
- Enable Issues.
- Disable wiki unless it will be actively maintained.
- Enable Discussions only when community feedback is desired.
- Enable private vulnerability reporting.
- Automatically delete merged branches.
- Use squash or rebase merging for small scoped changes; keep merge commits for release integration when useful.

## Branch protection for `main`

- Require status check: `Validate repository structure`.
- Block force pushes.
- Block branch deletion.
- Require conversation resolution.
- Require linear history when it does not conflict with the chosen release merge policy.
- For a solo repository, do not require an external approval that cannot be satisfied.

## Unity CI

The licensed GameCI jobs are disabled unless the repository variable below is set:

```text
ENABLE_UNITY_CI=true
```

Configure the required Unity license secrets before enabling it:

- `UNITY_EMAIL`
- `UNITY_PASSWORD`
- `UNITY_SERIAL`

## Labels

Suggested labels:

- `bug`
- `enhancement`
- `needs-triage`
- `priority: critical`
- `priority: high`
- `system: spellforge`
- `system: reactions`
- `system: atlas`
- `system: items`
- `system: persistence`
- `system: ui`
- `release`
- `validation`

## Releases

- Create a `v2.2.5` release after documenting target-editor status.
- Create `v3.0.0-alpha.1` as a pre-release after Unity compilation and initial Play Mode validation.
- Attach patch ZIPs and SHA-256 files to Releases rather than committing them.
