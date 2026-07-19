#!/usr/bin/env bash
set -Eeuo pipefail
PROJECT="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo"
BACKUP="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/ElementalSynthesisBackups/20260718-235557"

if [[ -f "$BACKUP/existed.txt" ]]; then
    while IFS= read -r relative; do
        mkdir -p "$PROJECT/$(dirname "$relative")"
        cp -a "$BACKUP/$relative" "$PROJECT/$relative"
    done < "$BACKUP/existed.txt"
fi

if [[ -f "$BACKUP/created.txt" ]]; then
    while IFS= read -r relative; do
        rm -f "$PROJECT/$relative"
    done < "$BACKUP/created.txt"
fi

echo "Restored elemental synthesis backup:"
echo "  $BACKUP"
