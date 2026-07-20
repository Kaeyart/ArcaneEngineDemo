#!/usr/bin/env bash
set -Eeuo pipefail

MODE="${1:---dry-run}"

if [[ "$MODE" != "--dry-run" && "$MODE" != "--apply" ]]; then
    echo "Usage: $0 [--dry-run|--apply]" >&2
    exit 2
fi

command -v git >/dev/null 2>&1 || {
    echo "Error: git is required." >&2
    exit 2
}

ROOT="$(git rev-parse --show-toplevel 2>/dev/null)" || {
    echo "Error: run this command inside the ArcaneEngineDemo Git working copy." >&2
    exit 2
}

cd "$ROOT"

declare -A TARGETS=()

while IFS= read -r tracked; do
    [[ -n "$tracked" ]] || continue
    first="${tracked%%/*}"

    if [[ "$first" == *Backups ]]; then
        TARGETS["$first"]=1
    fi

    if [[ "$tracked" == .arcane-*-last-backup ]]; then
        TARGETS["$tracked"]=1
    fi

    if [[ "$first" == ".repo-cleanup-backups" ]]; then
        TARGETS["$first"]=1
    fi
done < <(git ls-files)

if [[ "${#TARGETS[@]}" -eq 0 ]]; then
    echo "No tracked backup directories or root patch-state pointers were found."
    exit 0
fi

mapfile -t SORTED < <(printf '%s\n' "${!TARGETS[@]}" | sort)

echo "The following paths will be removed from Git tracking:"
printf '  %s\n' "${SORTED[@]}"
echo
echo "Local files will be preserved because the operation uses git rm --cached."

if [[ "$MODE" == "--dry-run" ]]; then
    echo
    echo "Dry run only. Apply with:"
    echo "  bash tools/cleanup_git_artifacts.sh --apply"
    exit 0
fi

for target in "${SORTED[@]}"; do
    if [[ -d "$target" ]]; then
        git rm -r --cached --ignore-unmatch -- "$target"
    else
        git rm --cached --ignore-unmatch -- "$target"
    fi
done

echo
echo "Git index cleanup staged. Review with:"
echo "  git status --short"
echo
echo "Then validate:"
echo "  python3 tools/repo_hygiene.py --project ."
