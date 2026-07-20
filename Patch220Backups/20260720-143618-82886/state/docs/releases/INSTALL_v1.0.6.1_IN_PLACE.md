# Install Arcane Engine v1.0.6.1 in place

Close Unity. Put `ArcaneEngineDemo_v1.0.6.1_COMPILE_FIX.zip` in `/home/kaey/Downloads`, then run the complete command below:

```bash
rm -rf /tmp/arcane_engine_v1061_install && mkdir -p /tmp/arcane_engine_v1061_install "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.6.1" && rsync -a --delete --exclude Library --exclude Temp --exclude Logs --exclude obj --exclude Builds "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" "/home/kaey/Desktop/ArcaneEngineDemo_backup_before_v1.0.6.1/" && unzip -q "/home/kaey/Downloads/ArcaneEngineDemo_v1.0.6.1_COMPILE_FIX.zip" -d /tmp/arcane_engine_v1061_install && rsync -a /tmp/arcane_engine_v1061_install/ArcaneEngineDemo/ "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/" && grep -Fq 'DemoCatalog.AllCores.Any()' "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Assets/ArcaneEngine/Scripts/Core/CombatMath.cs" && ! grep -Fq 'FirstOrDefault(i => i.instanceId == saved.itemInstanceId); string ignored' "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Assets/ArcaneEngine/Scripts/UI/DemoUI.cs" && rm -rf "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Library/ScriptAssemblies" "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Library/Bee" "/home/kaey/Desktop/ArcaneEngineDemo_v1.0.0_DEMO_PATCH/ArcaneEngineDemo/Temp" /tmp/arcane_engine_v1061_install && printf '%s\n' 'Arcane Engine v1.0.6.1 installed and source checks passed.'
```

Reopen the same project in Unity 6000.5.2f1 and wait for the forced script recompile. The title footer should read `VERSION 1.0.6.1-DEMO`.
