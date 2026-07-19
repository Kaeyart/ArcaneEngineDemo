#!/usr/bin/env bash
set -Eeuo pipefail
PROJECT="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo"
BACKUP="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch211Backups/20260719-231220"
STATE="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch211Backups/20260719-231220/state"
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
echo "Restored the project state from before Patch 2.1.1."
