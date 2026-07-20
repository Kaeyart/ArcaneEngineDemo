#!/usr/bin/env bash
set -Eeuo pipefail
PROJECT="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo"
BACKUP="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch220Backups/20260720-143618-82886"
STATE="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch220Backups/20260720-143618-82886/state"
OLD_200_LAST="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch200Backups/20260719-202230"
OLD_210_LAST="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch210Backups/20260719-215956"
OLD_211_LAST="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch211Backups/20260719-231220"
NEW_200_LAST="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch200Backups/20260719-202230"
NEW_210_LAST="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch210Backups/20260719-215956"
NEW_211_LAST="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Patch211Backups/20260719-231220"
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
if [[ -n "$NEW_211_LAST" && "$NEW_211_LAST" != "$OLD_211_LAST" && "$NEW_211_LAST" == "$PROJECT"/Patch211Backups/* ]]; then rm -rf "$NEW_211_LAST"; fi
if [[ -n "$NEW_210_LAST" && "$NEW_210_LAST" != "$OLD_210_LAST" && "$NEW_210_LAST" == "$PROJECT"/Patch210Backups/* ]]; then rm -rf "$NEW_210_LAST"; fi
if [[ -n "$NEW_200_LAST" && "$NEW_200_LAST" != "$OLD_200_LAST" && "$NEW_200_LAST" == "$PROJECT"/Patch200Backups/* ]]; then rm -rf "$NEW_200_LAST"; fi
echo "Restored the project state from before Patch 2.2.0."
