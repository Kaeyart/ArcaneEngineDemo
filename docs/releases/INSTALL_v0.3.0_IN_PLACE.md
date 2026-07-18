# Install v0.3.0 into the existing project

Close Unity and Unity Hub before installing. The patch is designed for this existing project:

`/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo`

Place the downloaded `ArcaneEngineDemo_v0.3.0_IN_PLACE_PATCH.zip` anywhere in `/home/kaey/Downloads`, then run this single command:

```bash
PROJECT='/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo'; PATCH="$(find '/home/kaey/Downloads' -maxdepth 1 -type f -name 'ArcaneEngineDemo_v0.3.0_IN_PLACE_PATCH*.zip' -printf '%T@ %p\n' | sort -nr | head -n1 | cut -d' ' -f2-)"; test -d "$PROJECT/Assets" && test -n "$PATCH" && test -f "$PATCH" || { echo 'ERROR: Project or patch ZIP was not found.'; exit 1; }; BACKUP="${PROJECT}_backup_before_v0.3.0_$(date +%Y%m%d_%H%M%S)"; cp -a "$PROJECT" "$BACKUP" && unzip -o "$PATCH" -d "$PROJECT" && rm -rf "$PROJECT/Library" "$PROJECT/Temp" "$PROJECT/obj" && echo "PATCH INSTALLED. Backup: $BACKUP" && echo "Reopen this same project in Unity Hub: $PROJECT"
```

The command:

1. Finds the newest matching patch ZIP in Downloads.
2. Verifies the existing Unity project.
3. Creates a timestamped backup beside it.
4. Overwrites only patch files in the same project.
5. Clears generated Unity caches so the scripts recompile cleanly.
6. Leaves the existing Unity Hub project path unchanged.
