# Development Workflow

## Branch model

Arcane Engine is currently best served by a lightweight branch model.

- `main`: maintenance and releasable state.
- `feature/3.0-endgame-first-arpg`: persistent ARPG development.
- `feature/<scope>`: isolated feature work.
- `hotfix/2.2.x-<scope>`: defects against the maintenance baseline.
- `release/<version>`: short-lived stabilization branch.

Avoid a permanent `develop` branch until concurrent contributors make it necessary.

## Initial branch setup

From a clean 2.2.5 repository:

```bash
git switch main
git pull --ff-only
git tag -a v2.2.5 -m "Arcane Engine 2.2.5 maintenance baseline"
git switch -c feature/3.0-endgame-first-arpg
```

Install the 3.0.0-alpha.1 patch only on the 3.0 branch, validate, then commit the resulting source and documents.

## Change sequence

1. Establish a clean working tree.
2. Create a scoped branch.
3. Apply or implement the change.
4. Run repository hygiene.
5. Open Unity 6000.5.2f1.
6. Record compilation and validator results.
7. Complete the relevant Play Mode checklist.
8. Commit code, assets, `.meta` files, migration notes, and validation records together.
9. Open a pull request or perform an equivalent self-review.
10. Merge without rewriting shared history.

## Repository cleanup sequence

```bash
bash tools/cleanup_git_artifacts.sh --dry-run
bash tools/cleanup_git_artifacts.sh --apply
python3 tools/repo_hygiene.py --project .
git status --short
git add .gitignore .gitattributes README.md CHANGELOG.md CONTRIBUTING.md SECURITY.md .github docs tools
git commit -m "chore(repo): establish repository governance and hygiene"
```

The cleanup script removes generated artifacts from the Git index while preserving them locally.
