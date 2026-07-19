# Install Arcane Engine v1.0.4 in place

Close Unity before installing. This hotfix targets:

`/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo`

Put `ArcaneEngineDemo_v1.0.4_TITLE_INPUT_HOTFIX.zip` in `/home/kaey/Downloads`, then run:

```bash
rm -rf /tmp/arcane_engine_v104_install && mkdir -p /tmp/arcane_engine_v104_install "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.4" && rsync -a --delete --exclude Library --exclude Temp --exclude Logs --exclude obj --exclude Builds "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.4/" && unzip -q "/home/kaey/Downloads/ArcaneEngineDemo_v1.0.4_TITLE_INPUT_HOTFIX.zip" -d /tmp/arcane_engine_v104_install && rsync -a /tmp/arcane_engine_v104_install/ArcaneEngineDemo/ "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" && rm -rf /tmp/arcane_engine_v104_install
```

Reopen the same project with Unity 6000.5.2f1, wait for compilation, open `Assets/ArcaneEngine/Scenes/Main.unity`, clear the Console, and press Play. Confirm that the title footer reads `VERSION 1.0.4-DEMO` before testing.
