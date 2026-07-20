# Release Process

## Versioning

- `X.0.0`: major product or architectural release.
- `X.Y.0`: meaningful feature expansion or system rework.
- `X.Y.Z`: focused bug or compatibility fix.
- Pre-release identifiers follow semantic-version order, for example `3.0.0-alpha.1`.

## Required release evidence

Every release must state separately:

- Design status.
- Package/static validation.
- Unity compilation result.
- Editor-validator result.
- Play Mode result.
- Known limitations.

## Release contents

- Versioned patch ZIP.
- SHA-256 checksum.
- Patch notes.
- Implementation manifest.
- Migration notes.
- Validation checklist.
- Rollback instructions.
- Target Unity version.

## GitHub Release procedure

1. Stabilize on a `release/<version>` branch.
2. Complete repository, Unity, and Play Mode validation.
3. Merge to the intended release branch.
4. Create an annotated tag:

   ```bash
   git tag -a v3.0.0-alpha.1 -m "Arcane Engine 3.0.0-alpha.1"
   git push origin v3.0.0-alpha.1
   ```

5. Create a GitHub Release from the tag.
6. Mark alpha and beta releases as pre-releases.
7. Attach the ZIP and checksum.
8. Paste a concise validation-status summary.
9. Do not commit the release ZIP into the Unity project repository.

## Current release recommendations

- Publish `v2.2.5` as the maintenance baseline once its target-editor status is documented.
- Publish `v3.0.0-alpha.1` as a pre-release only after the installed source compiles and the supplied checklist has
  at least a documented first pass.
