# Install v0.5.0 into the existing Unity project

Close Unity and Unity Hub. Put `ArcaneEngineDemo_v0.5.0_DEMO_PATCH.zip` in `/home/kaey/Downloads`, then run this complete command:

```bash
PROJECT='/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo'; PATCH="$(find '/home/kaey/Downloads' -maxdepth 1 -type f -name 'ArcaneEngineDemo_v0.5.0_DEMO_PATCH*.zip' -printf '%T@ %p\n' | sort -nr | head -n1 | cut -d' ' -f2-)"; test -d "$PROJECT/Assets" && test -d "$PROJECT/Packages" && test -d "$PROJECT/ProjectSettings" && test -n "$PATCH" && test -f "$PATCH" || { echo 'ERROR: Existing Unity project or v0.5.0 patch ZIP was not found.'; exit 1; }; BACKUP="${PROJECT}_backup_before_v0.5.0_$(date +%Y%m%d_%H%M%S)"; cp -a "$PROJECT" "$BACKUP" && unzip -o "$PATCH" -d "$PROJECT" && rm -rf "$PROJECT/Library" "$PROJECT/Temp" "$PROJECT/obj" && echo "V0.5.0 DEMO PATCH INSTALLED. Backup: $BACKUP" && echo "Reopen this SAME project with Unity 6000.5.2f1: $PROJECT"
```

Do not create a new Unity project. Reopen the same project path after the command finishes and allow Unity to reimport it.
