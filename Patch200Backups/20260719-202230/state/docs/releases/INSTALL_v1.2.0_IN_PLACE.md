# Install Arcane Engine v1.2.0 in the current project

Close Unity completely. Put `ArcaneEngineDemo_v1.2.0_ARCANE_SYSTEMS_OVERHAUL.zip` in `/home/kaey/Downloads`, then paste this entire command into Terminal:

```bash
set -euo pipefail; PROJECT='/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo'; ZIP='/home/kaey/Downloads/ArcaneEngineDemo_v1.2.0_ARCANE_SYSTEMS_OVERHAUL.zip'; if pgrep -x Unity >/dev/null 2>&1; then printf '%s\n' 'Close Unity completely, then run this command again.'; exit 1; fi; test -d "$PROJECT/Assets/ArcaneEngine"; test -f "$ZIP"; STAMP="$(date +%Y%m%d_%H%M%S)"; BACKUP="/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.2.0_$STAMP"; TMP="$(mktemp -d /tmp/arcane_engine_v120_install.XXXXXX)"; trap 'rm -rf "$TMP"' EXIT; mkdir -p "$BACKUP"; rsync -a --exclude Library --exclude Temp --exclude Logs --exclude obj --exclude Builds "$PROJECT/" "$BACKUP/"; unzip -q "$ZIP" -d "$TMP"; test -d "$TMP/ArcaneEngineDemo/Assets/ArcaneEngine"; test -f "$TMP/ArcaneEngineDemo/Assets/ArcaneEngine/Scripts/Core/V12Systems.cs"; rsync -a "$TMP/ArcaneEngineDemo/" "$PROJECT/"; grep -Fq 'bundleVersion: 1.2.0' "$PROJECT/ProjectSettings/ProjectSettings.asset"; grep -Fq 'VERSION 1.2.0-DEMO' "$PROJECT/Assets/ArcaneEngine/Scripts/UI/V1FrontEndUI.cs"; test -f "$PROJECT/Assets/ArcaneEngine/Scripts/Core/V12Systems.cs"; printf '%s\n' "Arcane Engine v1.2.0 installed in the current project. Backup: $BACKUP" 'Reopen the same project in Unity 6000.5.2f1, wait for compilation, open Assets/ArcaneEngine/Scenes/Main.unity, clear the Console, then press Play.'
```

This overlays the same Unity project, creates a timestamped source backup, and deliberately leaves Unity's `Library` and `Temp` folders alone.
