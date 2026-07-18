#!/usr/bin/env bash
set -Eeuo pipefail
PROJECT="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo"
BACKUP="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/SpellForgeCastingBackups/20260718-232037"

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

echo "Restored SpellForge/casting backup:"
echo "  $BACKUP"
