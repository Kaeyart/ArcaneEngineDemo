# Install Arcane Engine v1.0.6 in place

Close Unity first. This updates the existing project and creates a backup; it does not create a new Unity Hub project.

Put `ArcaneEngineDemo_v1.0.6_PERFORMANCE_AIM_UI_HOTFIX.zip` in `/home/kaey/Downloads`, then run this entire command:

```bash
rm -rf /tmp/arcane_engine_v106_install && mkdir -p /tmp/arcane_engine_v106_install "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.6" && rsync -a --delete --exclude Library --exclude Temp --exclude Logs --exclude obj --exclude Builds "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.6/" && unzip -q "/home/kaey/Downloads/ArcaneEngineDemo_v1.0.6_PERFORMANCE_AIM_UI_HOTFIX.zip" -d /tmp/arcane_engine_v106_install && rsync -a /tmp/arcane_engine_v106_install/ArcaneEngineDemo/ "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" && rm -rf /tmp/arcane_engine_v106_install
```

Reopen the same project with Unity 6000.5.2f1, let it compile, open `Assets/ArcaneEngine/Scenes/Main.unity`, clear the Console, and press Play. The title footer should read `VERSION 1.0.6-DEMO`.
