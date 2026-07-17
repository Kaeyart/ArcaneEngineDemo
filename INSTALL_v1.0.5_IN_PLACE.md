# Install Arcane Engine v1.0.5 in place

Close Unity before installing. This hotfix targets:

`/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo`

Put `ArcaneEngineDemo_v1.0.5_GAMEPLAY_INPUT_AUDIO_HOTFIX.zip` in `/home/kaey/Downloads`, then run:

```bash
rm -rf /tmp/arcane_engine_v105_install && mkdir -p /tmp/arcane_engine_v105_install "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.5" && rsync -a --delete --exclude Library --exclude Temp --exclude Logs --exclude obj --exclude Builds "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.5/" && unzip -q "/home/kaey/Downloads/ArcaneEngineDemo_v1.0.5_GAMEPLAY_INPUT_AUDIO_HOTFIX.zip" -d /tmp/arcane_engine_v105_install && rsync -a /tmp/arcane_engine_v105_install/ArcaneEngineDemo/ "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" && rm -rf /tmp/arcane_engine_v105_install
```

Reopen the same project with Unity 6000.5.2f1, wait for compilation, open `Assets/ArcaneEngine/Scenes/Main.unity`, clear the Console, and press Play. Confirm that the title footer reads `VERSION 1.0.5-DEMO`.
