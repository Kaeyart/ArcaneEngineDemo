#!/usr/bin/env bash
set -Eeuo pipefail
PROJECT="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo"
BACKUP="/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/SmallPatchBackups/20260718-225919"

cp -a "$BACKUP/Assets/ArcaneEngine/Scripts/Core/PlayerController.cs"       "$PROJECT/Assets/ArcaneEngine/Scripts/Core/PlayerController.cs"

rm -rf "$PROJECT/Assets/ArcaneEngine/Resources/V21Content"
rm -f "$PROJECT/Assets/ArcaneEngine/Resources/V21Content.meta"

if [[ -e "$BACKUP/Assets/ArcaneEngine/Resources/V21Content" ]]; then
    cp -a "$BACKUP/Assets/ArcaneEngine/Resources/V21Content"           "$PROJECT/Assets/ArcaneEngine/Resources/V21Content"
fi

if [[ -e "$BACKUP/Assets/ArcaneEngine/Resources/V21Content.meta" ]]; then
    cp -a "$BACKUP/Assets/ArcaneEngine/Resources/V21Content.meta"           "$PROJECT/Assets/ArcaneEngine/Resources/V21Content.meta"
fi

echo "Restored small-patch backup: $BACKUP"
