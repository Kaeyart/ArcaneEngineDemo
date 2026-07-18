# Install Arcane Engine 1.0 Demo in place

Close Unity before installing. The patch is designed for the existing project at:

`/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo`

Run this complete command after the ZIP is in `/home/kaey/Downloads`:

```bash
rm -rf /tmp/arcane_engine_v1_install && mkdir -p /tmp/arcane_engine_v1_install "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1" && rsync -a --exclude Library --exclude Temp --exclude Logs --exclude obj --exclude Builds "/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo/" "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1/" && unzip -q "/home/kaey/Downloads/ArcaneEngineDemo_v1.0.0_DEMO_PATCH.zip" -d /tmp/arcane_engine_v1_install && rsync -a /tmp/arcane_engine_v1_install/ArcaneEngineDemo/ "/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo/" && rm -rf /tmp/arcane_engine_v1_install
```

Open the same project in Unity Hub with Unity 6000.5.2f1. Open `Assets/ArcaneEngine/Scenes/Main.unity`, wait for compilation, confirm there are no red Console errors, and press Play.

To build the standalone Linux version after Unity has compiled the project:

```bash
chmod +x "/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo/BUILD_LINUX_v1.0.sh" && UNITY_BIN="$HOME/Unity/Hub/Editor/6000.5.2f1/Editor/Unity" "/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo/BUILD_LINUX_v1.0.sh" "/home/kaey/Desktop/ArcaneEngineDemo_v0.2.0_MEGA_UPDATE/ArcaneEngineDemo/Builds/Linux"
```

The build log will be written to `Builds/Linux/build.log`.
