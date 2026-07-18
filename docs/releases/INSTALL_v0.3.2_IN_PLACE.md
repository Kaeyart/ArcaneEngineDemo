# Install v0.3.2 into the existing project

Close Unity and Unity Hub, place `ArcaneEngineDemo_v0.3.2_WORLD_ROOM_FLOW_PATCH.zip` in `/home/kaey/Downloads`, and run:

```bash
PROJECT='/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo'; PATCH="$(find '/home/kaey/Downloads' -maxdepth 1 -type f -name 'ArcaneEngineDemo_v0.3.2_WORLD_ROOM_FLOW_PATCH*.zip' -printf '%T@ %p\n' | sort -nr | head -n1 | cut -d' ' -f2-)"; test -d "$PROJECT/Assets" && test -n "$PATCH" && test -f "$PATCH" || { echo 'ERROR: Existing Unity project or v0.3.2 patch ZIP was not found.'; exit 1; }; BACKUP="${PROJECT}_backup_before_v0.3.2_$(date +%Y%m%d_%H%M%S)"; cp -a "$PROJECT" "$BACKUP" && unzip -o "$PATCH" -d "$PROJECT" && rm -rf "$PROJECT/Library" "$PROJECT/Temp" "$PROJECT/obj" && echo "WORLD ROOM FLOW PATCH INSTALLED. Backup: $BACKUP" && echo "Reopen this SAME project with Unity 6000.5.2f1: $PROJECT"
```
