#!/usr/bin/env bash
set -Eeuo pipefail
PROJECT="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo"
BACKUP="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch210Backups/20260719-215956"
STATE="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch210Backups/20260719-215956/state"
NEW_200_BACKUP=""
OLD_200_LAST="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch200Backups/20260719-202230"
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
if [[ -n "$NEW_200_BACKUP" && "$NEW_200_BACKUP" != "$OLD_200_LAST" && "$NEW_200_BACKUP" == "$PROJECT"/Patch200Backups/* ]]; then
    rm -rf "$NEW_200_BACKUP"
fi
echo "Restored the pre-2.1.0 project state from:"
echo "  $BACKUP"
