# Install Arcane Engine v1.0.3 in place

Close Unity before installing. This hotfix targets:

`/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo`

Put `ArcaneEngineDemo_v1.0.3_MENU_FLOW_HOTFIX.zip` in `/home/kaey/Downloads`, then run:

```bash
rm -rf /tmp/arcane_engine_v103_install && mkdir -p /tmp/arcane_engine_v103_install "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.3" && rsync -a --delete --exclude Library --exclude Temp --exclude Logs --exclude obj --exclude Builds "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.3/" && unzip -q "/home/kaey/Downloads/ArcaneEngineDemo_v1.0.3_MENU_FLOW_HOTFIX.zip" -d /tmp/arcane_engine_v103_install && rsync -a /tmp/arcane_engine_v103_install/ArcaneEngineDemo/ "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" && rm -rf /tmp/arcane_engine_v103_install
```

Reopen the same project with Unity 6000.5.2f1, wait for compilation, open `Assets/ArcaneEngine/Scenes/Main.unity`, clear the Console, and press Play. On the title screen, clicking the buttons works through both normal Toolkit input and the direct fallback; Enter/1 starts preparation and 2/Escape enters Home Base.
