#!/usr/bin/env bash
set -Eeuo pipefail
PROJECT="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo"
BACKUP="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch225Backups/20260720-165051-110200"
STATE="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch225Backups/20260720-165051-110200/state"
if [[ -f "$BACKUP/existed.txt" ]]; then
    while IFS= read -r relative; do
        [[ -n "$relative" ]] || continue
        rm -rf "$PROJECT/$relative"
        mkdir -p "$PROJECT/$(dirname "$relative")"
        cp -a "$STATE/$relative" "$PROJECT/$relative"
    done < "$BACKUP/existed.txt"
fi
if [[ -f "$BACKUP/created.txt" ]]; then
    while IFS= read -r relative; do
        [[ -n "$relative" ]] || continue
        rm -rf "$PROJECT/$relative"
    done < "$BACKUP/created.txt"
fi
if [[ -f "$BACKUP/created_dirs.txt" ]]; then
    while IFS= read -r relative_dir; do
        [[ -n "$relative_dir" ]] || continue
        rmdir "$PROJECT/$relative_dir" 2>/dev/null || true
    done < "$BACKUP/created_dirs.txt"
fi
echo "Restored the project state from before Patch 2.2.5."
