# Install Arcane Engine v1.0.1 in place

Close Unity before installing. This hotfix targets the project at:

`/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo`

Put `ArcaneEngineDemo_v1.0.1_COMPATIBILITY_HOTFIX.zip` in `/home/kaey/Downloads`, then run:

```bash
rm -rf /tmp/arcane_engine_v101_install && mkdir -p /tmp/arcane_engine_v101_install "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.1" && rsync -a --delete --exclude Library --exclude Temp --exclude Logs --exclude obj --exclude Builds "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.1/" && unzip -q "/home/kaey/Downloads/ArcaneEngineDemo_v1.0.1_COMPATIBILITY_HOTFIX.zip" -d /tmp/arcane_engine_v101_install && rsync -a /tmp/arcane_engine_v101_install/ArcaneEngineDemo/ "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" && rm -rf /tmp/arcane_engine_v101_install
```

Open the same project with Unity 6000.5.2f1. Unity Package Manager will resolve Input System 1.19.0 on the first import. Open `Assets/ArcaneEngine/Scenes/Main.unity`, wait for package resolution and script compilation to finish, clear the Console, and press Play.
