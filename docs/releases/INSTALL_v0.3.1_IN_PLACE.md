# Install v0.3.1 into the existing project

Close Unity and Unity Hub before installing. Put the downloaded `ArcaneEngineDemo_v0.3.1_AIM_HIT_UI_PATCH.zip` in `/home/kaey/Downloads`, then run the complete command below:

```bash
PROJECT='/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo'; PATCH="$(find '/home/kaey/Downloads' -maxdepth 1 -type f -name 'ArcaneEngineDemo_v0.3.1_AIM_HIT_UI_PATCH*.zip' -printf '%T@ %p\n' | sort -nr | head -n1 | cut -d' ' -f2-)"; test -d "$PROJECT/Assets" && test -n "$PATCH" && test -f "$PATCH" || { echo 'ERROR: Existing Unity project or v0.3.1 patch ZIP was not found.'; exit 1; }; BACKUP="${PROJECT}_backup_before_v0.3.1_$(date +%Y%m%d_%H%M%S)"; cp -a "$PROJECT" "$BACKUP" && unzip -o "$PATCH" -d "$PROJECT" && rm -rf "$PROJECT/Library" "$PROJECT/Temp" "$PROJECT/obj" && echo "PATCH INSTALLED. Backup: $BACKUP" && echo "Reopen this SAME project in Unity Hub with 6000.5.2f1: $PROJECT"
```

This creates a timestamped backup, patches the same project, and clears only generated Unity caches so the new UI module and scripts recompile cleanly.
