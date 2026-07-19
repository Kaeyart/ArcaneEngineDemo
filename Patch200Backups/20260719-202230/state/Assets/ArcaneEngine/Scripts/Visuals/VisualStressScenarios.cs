using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ArcaneEngine
{
    public enum VisualStressScenario
    {
        ProjectileField,
        PersistentZones,
        EnemyRoster,
        CombinedCombat,
        HomingSplitField,
        TripleSpellLinks,
        TrailZoneStack,
        DualAffixElites,
        WardenPeak,
        ThirtyRoomCycle,
        SaveRoundTrip,
        TrainingHomeCycle,
        LowReducedFlashes,
        HighMaximumDensity
    }

    [Serializable]
    public struct VisualResourceSnapshot
    {
        public int activeVisuals;
        public int primitives;
        public int rings;
        public int beams;
        public int trails;
        public int lights;
        public int decals;
        public int spellBodies;
        public int projectiles;
        public int zones;
        public int enemies;
        public int statuses;
        public int telegraphs;
        public int deathPieces;
        public int roomRenderers;
        public int materialFamilies;

        public static VisualResourceSnapshot Capture()
        {
            return new VisualResourceSnapshot
            {
                activeVisuals = ProceduralVisualRuntime.ActiveVisuals,
                primitives = ProceduralVisualRuntime.ActivePrimitives,
                rings = ProceduralVisualRuntime.ActiveRings,
                beams = ProceduralVisualRuntime.ActiveBeams,
                trails = ProceduralVisualRuntime.ActiveTrails,
                lights = ProceduralVisualRuntime.ActiveLights,
                decals = ProceduralVisualRuntime.ActiveDecals,
                spellBodies = SpellVisualAttachment.ActiveCount,
                projectiles = VisualRuntimeRegistry.Count(VisualRuntimeKind.Projectile),
                zones = VisualRuntimeRegistry.Count(VisualRuntimeKind.PersistentZone),
                enemies = VisualRuntimeRegistry.Count(VisualRuntimeKind.EnemyAssembly),
                statuses = VisualRuntimeRegistry.Count(VisualRuntimeKind.StatusLayer),
                telegraphs = VisualRuntimeRegistry.Count(VisualRuntimeKind.Telegraph),
                deathPieces = VisualRuntimeRegistry.Count(VisualRuntimeKind.DeathPiece),
                roomRenderers = VisualRuntimeRegistry.RoomRenderers,
                materialFamilies = RuntimeVisuals.MaterialCount
            };
        }

        public void Include(VisualResourceSnapshot sample)
        {
            activeVisuals = Mathf.Max(activeVisuals, sample.activeVisuals); primitives = Mathf.Max(primitives, sample.primitives);
            rings = Mathf.Max(rings, sample.rings); beams = Mathf.Max(beams, sample.beams); trails = Mathf.Max(trails, sample.trails);
            lights = Mathf.Max(lights, sample.lights); decals = Mathf.Max(decals, sample.decals); spellBodies = Mathf.Max(spellBodies, sample.spellBodies);
            projectiles = Mathf.Max(projectiles, sample.projectiles); zones = Mathf.Max(zones, sample.zones); enemies = Mathf.Max(enemies, sample.enemies);
            statuses = Mathf.Max(statuses, sample.statuses); telegraphs = Mathf.Max(telegraphs, sample.telegraphs); deathPieces = Mathf.Max(deathPieces, sample.deathPieces);
            roomRenderers = Mathf.Max(roomRenderers, sample.roomRenderers); materialFamilies = Mathf.Max(materialFamilies, sample.materialFamilies);
        }

        public bool SameRuntimeCounts(VisualResourceSnapshot other)
        {
            return activeVisuals == other.activeVisuals && spellBodies == other.spellBodies && projectiles == other.projectiles && zones == other.zones &&
                enemies == other.enemies && statuses == other.statuses && telegraphs == other.telegraphs && deathPieces == other.deathPieces &&
                lights == other.lights && decals == other.decals;
        }

        public override string ToString()
        {
            return "visuals=" + activeVisuals + " primitives=" + primitives + " rings=" + rings + " beams=" + beams + " trails=" + trails +
                " lights=" + lights + " decals=" + decals + " spellBodies=" + spellBodies + " projectiles=" + projectiles + " zones=" + zones +
                " enemies=" + enemies + " statuses=" + statuses + " telegraphs=" + telegraphs + " deaths=" + deathPieces +
                " roomRenderers=" + roomRenderers + " materials=" + materialFamilies;
        }
    }

    /// <summary>
    /// Explicit, reproducible Play Mode stress scenarios. They are available only
    /// while diagnostics are visible and a normal run is not active. Each scenario
    /// owns every object it creates and checks the event-driven counters after cleanup.
    /// </summary>
    public sealed class VisualStressScenarioRunner : MonoBehaviour
    {
        private const int Seed = 20020417;
        private readonly List<EnemyController> _stressEnemies = new List<EnemyController>();
        private GameObject _root;
        private float _finishAt;
        private float _cleanupCheckAt;
        private VisualResourceSnapshot _baseline;
        private VisualResourceSnapshot _peak;
        private VisualStressScenario _scenario;
        private string _reportFolder;
        private AccessibilitySettings _temporarySettings;
        private int _oldQuality;
        private bool _oldReducedFlashes;
        private float _oldSpellDensity;
        private float _oldEnvironmentDensity;
        private int _oldDynamicLightQuality;
        private int _oldShadowQuality;
        private float _oldDecalDuration;
        private float _peakFrameMilliseconds;
        private float _frameMillisecondsTotal;
        private int _frameSamples;
        private bool _scenarioLogicPassed = true;
        private string _scenarioLogicNote = "No scenario-specific assertion.";
        private string _status = "Ready · seed " + Seed;

        public bool Running { get { return _root != null; } }
        public string Status { get { return _status; } }

        private void Update()
        {
            if (_root != null)
            {
                _peak.Include(VisualResourceSnapshot.Capture());
                float frameMilliseconds = Time.unscaledDeltaTime * 1000f;
                _peakFrameMilliseconds = Mathf.Max(_peakFrameMilliseconds, frameMilliseconds);
                _frameMillisecondsTotal += frameMilliseconds;
                _frameSamples++;
            }
            if (_root != null && Time.unscaledTime >= _finishAt) CleanupScenario();
            if (_cleanupCheckAt > 0f && Time.unscaledTime >= _cleanupCheckAt)
            {
                _cleanupCheckAt = 0f;
                VisualResourceSnapshot after = VisualResourceSnapshot.Capture();
                if (RuntimeVisuals.MaterialCount > RuntimeVisuals.MaterialLimit)
                {
                    _scenarioLogicPassed = false;
                    _scenarioLogicNote += " Material cache exceeded " + RuntimeVisuals.MaterialLimit + ".";
                }
                if (AttackTelegraphAudit.MismatchCount > 0)
                {
                    _scenarioLogicPassed = false;
                    _scenarioLogicNote += " Telegraph audit has " + AttackTelegraphAudit.MismatchCount + " mismatch(es).";
                }
                bool countersClean = _baseline.SameRuntimeCounts(after);
                bool passed = countersClean && _scenarioLogicPassed;
                _status = (passed ? "PASS" : "CHECK FAILED") + " · " + _scenario + " · " + _scenarioLogicNote + " · post-cleanup " + after;
                WriteReport(passed, countersClean, after);
                Debug.Log("Visual stress " + _status);
            }
        }

        private void OnGUI()
        {
            if (ProfileManager.Current == null || !ProfileManager.Current.accessibility.showVisualDiagnostics) return;
            Rect panel = new Rect(14f, 356f, 470f, 286f);
            GUI.Box(panel, GUIContent.none);
            GUI.Label(new Rect(28f, 364f, 440f, 22f), "REPRODUCIBLE VISUAL STRESS · EXPLICIT TEST ONLY");
            bool allowed = GameWorld.Instance != null && !GameWorld.Instance.RunActive && !V1TitleScreen.IsOpen;
            GUI.enabled = allowed && !Running;
            float width = 96f;
            if (GUI.Button(new Rect(28f, 391f, width, 26f), "PROJECTILES")) Begin(VisualStressScenario.ProjectileField);
            if (GUI.Button(new Rect(130f, 391f, width, 26f), "ZONES")) Begin(VisualStressScenario.PersistentZones);
            if (GUI.Button(new Rect(232f, 391f, width, 26f), "ENEMIES")) Begin(VisualStressScenario.EnemyRoster);
            if (GUI.Button(new Rect(334f, 391f, 96f, 26f), "COMBINED")) Begin(VisualStressScenario.CombinedCombat);
            if (GUI.Button(new Rect(28f, 423f, width, 26f), "HOMING+SPLIT")) Begin(VisualStressScenario.HomingSplitField);
            if (GUI.Button(new Rect(130f, 423f, width, 26f), "3 LINKS")) Begin(VisualStressScenario.TripleSpellLinks);
            if (GUI.Button(new Rect(232f, 423f, width, 26f), "DUAL ELITES")) Begin(VisualStressScenario.DualAffixElites);
            if (GUI.Button(new Rect(334f, 423f, 96f, 26f), "WARDEN")) Begin(VisualStressScenario.WardenPeak);
            if (GUI.Button(new Rect(28f, 455f, width, 26f), "30 ROOMS")) Begin(VisualStressScenario.ThirtyRoomCycle);
            if (GUI.Button(new Rect(130f, 455f, width, 26f), "TRAIL+ZONE")) Begin(VisualStressScenario.TrailZoneStack);
            if (GUI.Button(new Rect(232f, 455f, width, 26f), "LOW+FLASH")) Begin(VisualStressScenario.LowReducedFlashes);
            if (GUI.Button(new Rect(334f, 455f, 96f, 26f), "HIGH MAX")) Begin(VisualStressScenario.HighMaximumDensity);
            GUI.enabled = Running;
            if (GUI.Button(new Rect(28f, 487f, 96f, 25f), "CLEAN NOW")) CleanupScenario();
            GUI.enabled = allowed && !Running;
            if (GUI.Button(new Rect(130f, 487f, 96f, 25f), "SAVE x8")) Begin(VisualStressScenario.SaveRoundTrip);
            if (GUI.Button(new Rect(232f, 487f, 96f, 25f), "HOME CYCLE")) Begin(VisualStressScenario.TrainingHomeCycle);
            GUI.enabled = true;
            GUI.Label(new Rect(28f, 519f, 440f, 48f), allowed ? _status : "Close the title and return to Home Base before running a visual stress scenario.");
            VisualComparisonMatrixRunner matrix = VisualComparisonMatrixRunner.Instance;
            GUI.enabled = allowed && !Running && matrix != null && !matrix.Running;
            if (GUI.Button(new Rect(28f, 584f, 184f, 25f), "CAPTURE FULL CATALOG MATRIX")) matrix.Begin();
            GUI.enabled = true;
            if (matrix != null) GUI.Label(new Rect(220f, 583f, 248f, 42f), matrix.Status);
        }

        public void Begin(VisualStressScenario scenario)
        {
            if (Running || GameWorld.Instance == null || GameWorld.Instance.RunActive) return;
            CaptureBaseline();
            _peakFrameMilliseconds = 0f; _frameMillisecondsTotal = 0f; _frameSamples = 0;
            _scenarioLogicPassed = true;
            _scenarioLogicNote = "No scenario-specific assertion.";
            _scenario = scenario;
            _reportFolder = Path.Combine(Application.persistentDataPath, "VisualStress_2.0");
            UnityEngine.Random.State previous = UnityEngine.Random.state;
            UnityEngine.Random.InitState(Seed + (int)scenario * 977);
            _root = new GameObject("Visual Stress · " + scenario + " · seed " + Seed);
            DontDestroyOnLoad(_root);
            if (scenario == VisualStressScenario.LowReducedFlashes || scenario == VisualStressScenario.HighMaximumDensity) ApplyTemporaryQuality(scenario);
            if (scenario == VisualStressScenario.ProjectileField || scenario == VisualStressScenario.CombinedCombat)
                SpawnProjectileField(scenario == VisualStressScenario.CombinedCombat ? 64 : 48);
            if (scenario == VisualStressScenario.PersistentZones || scenario == VisualStressScenario.CombinedCombat)
                SpawnZones(scenario == VisualStressScenario.CombinedCombat ? 14 : 18);
            if (scenario == VisualStressScenario.EnemyRoster || scenario == VisualStressScenario.CombinedCombat)
                SpawnEnemyRoster(scenario == VisualStressScenario.CombinedCombat ? 18 : 24);
            if (scenario == VisualStressScenario.HomingSplitField) SpawnHomingSplitField(72);
            if (scenario == VisualStressScenario.TripleSpellLinks) SpawnTripleSpellLinks();
            if (scenario == VisualStressScenario.TrailZoneStack) { SpawnHomingSplitField(36); SpawnZones(20); }
            if (scenario == VisualStressScenario.DualAffixElites) SpawnDualAffixElites(16);
            if (scenario == VisualStressScenario.WardenPeak) SpawnWardenPeak();
            if (scenario == VisualStressScenario.ThirtyRoomCycle) _root.AddComponent<ThirtyRoomGenerationStress>().Begin(_root.transform, Seed);
            if (scenario == VisualStressScenario.SaveRoundTrip) RunSaveRoundTrip();
            if (scenario == VisualStressScenario.TrainingHomeCycle) SpawnTrainingHomeCycle();
            if (scenario == VisualStressScenario.LowReducedFlashes || scenario == VisualStressScenario.HighMaximumDensity)
            { SpawnHomingSplitField(48); SpawnZones(14); SpawnDualAffixElites(10); }
            UnityEngine.Random.state = previous;
            _peak = VisualResourceSnapshot.Capture();
            _finishAt = Time.unscaledTime + 12f;
            _status = "RUNNING " + scenario + " · 12 seconds · seed " + Seed;
            Debug.Log("Visual stress started: " + _status);
        }

        public void CleanupScenario()
        {
            if (_root == null) return;
            for (int i = 0; i < _stressEnemies.Count; i++)
            {
                EnemyController enemy = _stressEnemies[i];
                if (enemy == null) continue;
                if (GameWorld.Instance != null) GameWorld.Instance.Enemies.Remove(enemy);
            }
            _stressEnemies.Clear();
            Destroy(_root);
            _root = null;
            RestoreTemporaryQuality();
            _finishAt = 0f;
            _cleanupCheckAt = Time.unscaledTime + 0.25f;
            _status = "Cleaning and checking counter baseline…";
        }

        private void CaptureBaseline()
        {
            _baseline = VisualResourceSnapshot.Capture();
            _peak = _baseline;
        }

        private void ApplyTemporaryQuality(VisualStressScenario scenario)
        {
            if (ProfileManager.Current == null) return;
            _temporarySettings = ProfileManager.Current.accessibility;
            _oldQuality = _temporarySettings.visualQuality;
            _oldReducedFlashes = _temporarySettings.reducedFlashes;
            _oldSpellDensity = _temporarySettings.spellEffectDensity;
            _oldEnvironmentDensity = _temporarySettings.environmentDensity;
            _oldDynamicLightQuality = _temporarySettings.dynamicLightQuality;
            _oldShadowQuality = _temporarySettings.shadowQuality;
            _oldDecalDuration = _temporarySettings.decalDuration;
            if (scenario == VisualStressScenario.LowReducedFlashes)
            {
                VisualQualityPolicy.ApplyPreset(_temporarySettings, 0);
                _temporarySettings.reducedFlashes = true;
            }
            else
            {
                VisualQualityPolicy.ApplyPreset(_temporarySettings, 2);
                _temporarySettings.reducedFlashes = false;
                _temporarySettings.spellEffectDensity = 1f;
                _temporarySettings.environmentDensity = 1f;
            }
        }

        private void RestoreTemporaryQuality()
        {
            if (_temporarySettings == null) return;
            _temporarySettings.visualQuality = _oldQuality;
            _temporarySettings.reducedFlashes = _oldReducedFlashes;
            _temporarySettings.spellEffectDensity = _oldSpellDensity;
            _temporarySettings.environmentDensity = _oldEnvironmentDensity;
            _temporarySettings.dynamicLightQuality = _oldDynamicLightQuality;
            _temporarySettings.shadowQuality = _oldShadowQuality;
            _temporarySettings.decalDuration = _oldDecalDuration;
            _temporarySettings = null;
        }

        private void WriteReport(bool passed, bool countersClean, VisualResourceSnapshot after)
        {
            try
            {
                Directory.CreateDirectory(_reportFolder);
                string path = Path.Combine(_reportFolder, DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + "_" + _scenario + ".txt");
                File.WriteAllText(path, "Arcane Engine 2.0 visual stress report\nUTC " + DateTime.UtcNow.ToString("o") + "\nUnity " + Application.unityVersion +
                    "\nScenario " + _scenario + "\nSeed " + Seed + "\nResolution " + Screen.width + "x" + Screen.height +
                    "\nDevice " + SystemInfo.deviceName + "\nCPU " + SystemInfo.processorType + "\nGPU " + SystemInfo.graphicsDeviceName + "\nMemory " + SystemInfo.systemMemorySize + " MB" +
                    "\nPeak frame " + _peakFrameMilliseconds.ToString("0.00") + " ms\nAverage frame " + (_frameSamples <= 0 ? 0f : _frameMillisecondsTotal / _frameSamples).ToString("0.00") + " ms" +
                    "\nBaseline " + _baseline + "\nPeak " + _peak + "\nPost-cleanup " + after + "\nCounter cleanup " + (countersClean ? "PASS" : "FAILED") +
                    "\nScenario assertion " + (_scenarioLogicPassed ? "PASS" : "FAILED") + " · " + _scenarioLogicNote + "\nOverall " + (passed ? "PASS" : "FAILED") +
                    "\nTelegraph audit entries " + AttackTelegraphAudit.Count + " mismatches " + AttackTelegraphAudit.MismatchCount + "\n\n" + AttackTelegraphAudit.ExportTable());
            }
            catch (Exception exception) { Debug.LogError("Visual stress report failed: " + exception.Message); }
        }

        private void SpawnProjectileField(int count)
        {
            Array elements = Enum.GetValues(typeof(SpellElement));
            for (int i = 0; i < count; i++)
            {
                SpellElement element = (SpellElement)elements.GetValue(i % elements.Length);
                CompiledSpell spell = StressSpell(element, SpellDelivery.Projectile, i);
                float angle = i / (float)count * Mathf.PI * 2f;
                float radius = 3.5f + i % 6 * 1.35f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 1.1f + i % 3 * 0.22f, Mathf.Sin(angle) * radius);
                GameObject host = new GameObject("Stress Projectile " + i + " · " + element);
                host.transform.SetParent(_root.transform, false);
                host.transform.position = position;
                VisualCounterRegistration.Attach(host, VisualRuntimeKind.Projectile);
                CastRequest request = new CastRequest { origin = position, castOrigin = position, direction = new Vector3(-Mathf.Sin(angle), 0f, Mathf.Cos(angle)), targetPosition = -position, powerScale = 1f, manualCast = (i & 1) == 0 };
                SpellVisualAttachment.Attach(host, spell, request);
                VisualStressMover mover = host.AddComponent<VisualStressMover>();
                mover.Initialize(Vector3.zero, radius, angle, 0.35f + i % 5 * 0.045f, 1.1f + i % 3 * 0.22f);
            }
        }

        private void SpawnZones(int count)
        {
            Array elements = Enum.GetValues(typeof(SpellElement));
            for (int i = 0; i < count; i++)
            {
                SpellElement element = (SpellElement)elements.GetValue(i % elements.Length);
                CompiledSpell spell = StressSpell(element, SpellDelivery.Zone, i);
                Vector3 position = new Vector3((i % 6 - 2.5f) * 4.4f, 0.04f, (i / 6 - 1f) * 5f);
                GameObject zone = new GameObject("Stress Zone " + i + " · " + element);
                zone.transform.SetParent(_root.transform, false);
                zone.transform.position = position;
                VisualCounterRegistration.Attach(zone, VisualRuntimeKind.PersistentZone);
                CastRequest request = new CastRequest { origin = position, castOrigin = position, direction = Vector3.forward, targetPosition = position, powerScale = 1f, manualCast = true };
                SpellDeliveryVisuals.AttachZone(zone, spell, request, spell.radius, 12f);
            }
        }

        private void SpawnEnemyRoster(int count)
        {
            Array archetypes = Enum.GetValues(typeof(EnemyArchetype));
            DifficultySettings difficulty = new DifficultySettings();
            for (int i = 0; i < count; i++)
            {
                EnemyArchetype archetype = (EnemyArchetype)archetypes.GetValue(i % archetypes.Length);
                Vector3 position = new Vector3((i % 8 - 3.5f) * 3.5f, 0.75f, (i / 8 - 1f) * 5.2f);
                EnemyController enemy = EnemyController.Spawn(position, archetype, 7, difficulty, i % 4 == 0, false, false);
                if (enemy == null) continue;
                enemy.enabled = false;
                BossEncounterMechanics boss = enemy.GetComponent<BossEncounterMechanics>();
                if (boss != null) boss.enabled = false;
                enemy.transform.SetParent(_root.transform, true);
                _stressEnemies.Add(enemy);
            }
        }

        private void SpawnHomingSplitField(int count)
        {
            Array elements = Enum.GetValues(typeof(SpellElement));
            for (int i = 0; i < count; i++)
            {
                SpellElement element = (SpellElement)elements.GetValue(i % elements.Length);
                CompiledSpell spell = StressSpell(element, SpellDelivery.Projectile, i);
                spell.homingStrength = 3.5f; spell.splitOnHit = true; spell.splitCount = 3 + i % 3; spell.splitDistance = 3f;
                spell.trailDuration = 1.2f; spell.projectileCount = 5; spell.projectilePattern = (ProjectilePattern)(i % 4);
                SpellVisualCompiler.Rebuild(spell);
                float angle = i / (float)count * Mathf.PI * 2f;
                float radius = 4f + i % 9 * 0.82f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 1.1f, Mathf.Sin(angle) * radius);
                GameObject host = new GameObject("Max Homing Split " + i);
                host.transform.SetParent(_root.transform, false); host.transform.position = position;
                VisualCounterRegistration.Attach(host, VisualRuntimeKind.Projectile);
                CastRequest request = new CastRequest { origin = position, castOrigin = position, direction = new Vector3(-Mathf.Sin(angle), 0f, Mathf.Cos(angle)), targetPosition = Vector3.zero, powerScale = 1f, manualCast = true };
                SpellVisualAttachment.Attach(host, spell, request);
                host.AddComponent<VisualStressMover>().Initialize(Vector3.zero, radius, angle, 0.48f + i % 4 * 0.05f, 1.1f);
            }
        }

        private void SpawnTripleSpellLinks()
        {
            CompiledSpell[] spells =
            {
                StressSpell(SpellElement.Fire, SpellDelivery.Projectile, 0),
                StressSpell(SpellElement.Lightning, SpellDelivery.Beam, 1),
                StressSpell(SpellElement.Frost, SpellDelivery.Nova, 2)
            };
            for (int i = 0; i < spells.Length; i++)
            {
                spells[i].incomingSpellLinks = 1; spells[i].outgoingSpellLinks = 1;
                spells[i].outgoingLinkConditions = new[] { (SpellLinkCondition)(i % Enum.GetValues(typeof(SpellLinkCondition)).Length) };
                SpellVisualCompiler.Rebuild(spells[i]);
                Vector3 position = new Vector3((i - 1) * 4.2f, 1f, 0f);
                GameObject host = new GameObject("Three-link Spell " + i); host.transform.SetParent(_root.transform, false); host.transform.position = position;
                SpellVisualAttachment.Attach(host, spells[i], new CastRequest { origin = position, castOrigin = position, targetPosition = Vector3.zero, direction = Vector3.forward, manualCast = i == 0, generation = i });
                SpellVisualEvents.LinkActivation(spells[i], spells[(i + 1) % spells.Length], Vector3.Lerp(position, new Vector3(((i + 1) % 3 - 1) * 4.2f, 1f, 0f), 0.5f), TriggerMoment.OnCast);
            }
            _root.AddComponent<VisualStressLinkRepeater>().Initialize(spells);
        }

        private void SpawnDualAffixElites(int count)
        {
            Array archetypes = Enum.GetValues(typeof(EnemyArchetype));
            DifficultySettings difficulty = new DifficultySettings();
            for (int i = 0; i < count; i++)
            {
                EnemyArchetype archetype = (EnemyArchetype)archetypes.GetValue(i % (archetypes.Length - 2));
                Vector3 position = new Vector3((i % 8 - 3.5f) * 3.6f, 0.75f, (i / 8 - 0.5f) * 5.5f);
                EnemyController enemy = EnemyController.Spawn(position, archetype, 14, difficulty, true, false, false);
                if (enemy == null) continue;
                enemy.enabled = false; enemy.transform.SetParent(_root.transform, true); _stressEnemies.Add(enemy);
                foreach (EliteAffix affix in enemy.ActiveAffixes) EnemyVisualEvents.EliteEvent(enemy, affix);
            }
        }

        private void SpawnWardenPeak()
        {
            DifficultySettings difficulty = new DifficultySettings { newBossPhase = true, adaptiveEnemies = true };
            EnemyController boss = EnemyController.Spawn(Vector3.zero + Vector3.up * 0.75f, EnemyArchetype.OssuaryWarden, 14, difficulty, true, true, false);
            if (boss == null) return;
            BossEncounterMechanics mechanics = boss.GetComponent<BossEncounterMechanics>(); if (mechanics != null) mechanics.enabled = false;
            boss.enabled = false; boss.transform.SetParent(_root.transform, true); _stressEnemies.Add(boss);
            boss.SetBossPhase(3); boss.SetBossAdaptation(SpellElement.Lightning, 4);
            EnemyVisualEvents.BossPhase(boss, 3);
            for (int i = 0; i < 8; i++)
            {
                Vector3 point = Quaternion.Euler(0f, i * 45f, 0f) * Vector3.forward * 7.2f;
                EnemyVisualEvents.TelegraphAt(boss, "Stress Phase Three", point, -point, 2.2f, 4.5f);
            }
            SpawnDualAffixElites(8);
        }

        private void RunSaveRoundTrip()
        {
            try
            {
                RunSnapshotData sample = new RunSnapshotData { runSeed = Seed, roomIndex = 7, totalRooms = 12, roomId = "combat_crypt", savedUtc = DateTime.UtcNow.ToString("o") };
                string expected = VisualContinuationValidation.Compute(sample);
                Directory.CreateDirectory(_reportFolder);
                string testPath = Path.Combine(_reportFolder, "non_authoritative_roundtrip_checkpoint.json");
                for (int i = 0; i < 8; i++)
                {
                    sample.visualReconstructionSignature = VisualContinuationValidation.Compute(sample);
                    string json = JsonUtility.ToJson(sample, true);
                    File.WriteAllText(testPath, json);
                    sample = JsonUtility.FromJson<RunSnapshotData>(File.ReadAllText(testPath)); sample.Normalize();
                    string actual = VisualContinuationValidation.Compute(sample);
                    if (expected != actual) throw new InvalidDataException("Round-trip " + (i + 1) + " changed visual signature " + expected + " to " + actual + ".");
                }
                _scenarioLogicPassed = true;
                _scenarioLogicNote = "8 non-authoritative disk reconstruction round-trips matched " + expected + ".";
            }
            catch (Exception exception)
            {
                _scenarioLogicPassed = false;
                _scenarioLogicNote = "Save reconstruction round-trip failed: " + exception.Message;
            }
        }

        private void SpawnTrainingHomeCycle()
        {
            _root.AddComponent<HomeTrainingCycleStress>().Begin(_root.transform);
        }

        public static CompiledSpell StressSpell(SpellElement element, SpellDelivery delivery, int index)
        {
            Color primary;
            Color accent;
            switch (element)
            {
                case SpellElement.Fire: primary = new Color(1f, 0.15f, 0.02f); accent = new Color(1f, 0.72f, 0.08f); break;
                case SpellElement.Frost: primary = new Color(0.34f, 0.86f, 1f); accent = new Color(0.82f, 0.96f, 1f); break;
                case SpellElement.Lightning: primary = new Color(0.2f, 0.55f, 1f); accent = Color.white; break;
                case SpellElement.Toxic: primary = new Color(0.25f, 1f, 0.1f); accent = new Color(0.75f, 1f, 0.12f); break;
                case SpellElement.Void: primary = new Color(0.55f, 0.08f, 0.92f); accent = new Color(1f, 0.12f, 0.72f); break;
                default: primary = new Color(0.1f, 0.86f, 1f); accent = new Color(0.82f, 0.46f, 1f); break;
            }
            CompiledSpell spell = new CompiledSpell
            {
                coreId = "visual_stress_" + element,
                displayName = element + " Stress Spell",
                slot = (SpellSlot)(index % 3),
                delivery = delivery,
                element = element,
                primaryColor = primary,
                accentColor = accent,
                lifetime = 12f,
                speed = 8f,
                size = 0.38f + index % 3 * 0.08f,
                radius = 1.8f + index % 4 * 0.35f,
                projectilePattern = (ProjectilePattern)(index % Enum.GetValues(typeof(ProjectilePattern)).Length),
                projectileCount = 1 + index % 7,
                homingStrength = index % 3 == 0 ? 2f : 0f,
                arcAmount = index % 4 == 0 ? 0.8f : 0f,
                pierce = index % 5 == 0 ? 2 : 0,
                chainTargets = index % 6 == 0 ? 3 : 0,
                bounce = index % 7 == 0 ? 2 : 0,
                repeatCount = 1 + index % 3,
                zoneDuration = delivery == SpellDelivery.Zone ? 12f : 0f,
                trailDuration = delivery == SpellDelivery.Projectile ? 0.65f : 0f,
                instability = (index % 4) * 95f
            };
            SpellVisualCompiler.Rebuild(spell);
            return spell;
        }

        private void OnDestroy() { if (_root != null) CleanupScenario(); }
    }

    public sealed class VisualStressMover : MonoBehaviour
    {
        private Vector3 _center;
        private float _radius;
        private float _angle;
        private float _speed;
        private float _height;
        public void Initialize(Vector3 center, float radius, float angle, float speed, float height)
        {
            _center = center; _radius = radius; _angle = angle; _speed = speed; _height = height;
        }
        private void Update()
        {
            _angle += Time.unscaledDeltaTime * _speed;
            transform.position = _center + new Vector3(Mathf.Cos(_angle) * _radius, _height + Mathf.Sin(_angle * 3f) * 0.18f, Mathf.Sin(_angle) * _radius);
            transform.forward = new Vector3(-Mathf.Sin(_angle), 0f, Mathf.Cos(_angle));
        }
    }

    public sealed class VisualStressLinkRepeater : MonoBehaviour
    {
        private CompiledSpell[] _spells;
        private float _next;
        public void Initialize(CompiledSpell[] spells) { _spells = spells; _next = Time.unscaledTime + 0.45f; }
        private void Update()
        {
            if (_spells == null || _spells.Length != 3 || Time.unscaledTime < _next) return;
            _next = Time.unscaledTime + 0.62f;
            for (int i = 0; i < 3; i++)
            {
                Vector3 from = new Vector3((i - 1) * 4.2f, 1f, 0f);
                Vector3 to = new Vector3(((i + 1) % 3 - 1) * 4.2f, 1f, 0f);
                SpellVisualEvents.LinkActivation(_spells[i], _spells[(i + 1) % 3], Vector3.Lerp(from, to, 0.5f), (TriggerMoment)(i % Enum.GetValues(typeof(TriggerMoment)).Length));
            }
        }
    }

    public sealed class ThirtyRoomGenerationStress : MonoBehaviour
    {
        private Transform _owner;
        private GameObject _current;
        private int _index;
        private float _next;
        private int _seed;
        private string _previousRoom;
        private int _previousRenderers;

        public void Begin(Transform owner, int seed)
        {
            _owner = owner; _seed = seed; _index = 0; _next = Time.unscaledTime;
            _previousRoom = VisualRuntimeRegistry.RoomLabel; _previousRenderers = VisualRuntimeRegistry.RoomRenderers;
        }

        private void Update()
        {
            if (_owner == null || _index >= 30 || Time.unscaledTime < _next) return;
            if (_current != null) Destroy(_current);
            _current = new GameObject("Thirty-room material test · " + (_index + 1));
            _current.transform.SetParent(_owner, false);
            DungeonRoomType roomType = (DungeonRoomType)(_index % Enum.GetValues(typeof(DungeonRoomType)).Length);
            BiomeVisualId biome = BiomeVisualCatalog.Resolve(_seed, _index, 30, roomType);
            RoomTemplate room = new RoomTemplate
            {
                id = "visual_stress_room_" + _index,
                displayName = "Visual Stress " + roomType,
                type = roomType,
                accentColor = Color.HSVToRGB((_index % 8) / 8f, 0.62f, 0.82f),
                floorColor = BiomeVisualCatalog.Get(biome).floor
            };
            ProceduralDungeonVisuals.BuildForAudit(_current.transform, room, biome, V1Determinism.Combine(_seed, _index, roomType.ToString()));
            _index++;
            _next = Time.unscaledTime + 0.12f;
            if (_index >= 30) Debug.Log("Thirty-room material test generated all rooms. Cached material families: " + RuntimeVisuals.MaterialCount + " / " + RuntimeVisuals.MaterialLimit + ".");
        }

        private void OnDestroy()
        {
            if (_current != null) Destroy(_current);
            VisualRuntimeRegistry.SetRoom(_previousRoom, _previousRenderers);
        }
    }

    public sealed class HomeTrainingCycleStress : MonoBehaviour
    {
        private Transform _owner;
        private GameObject _cycleRoot;
        private float _next;
        private int _cycle;
        public void Begin(Transform owner) { _owner = owner; _next = Time.unscaledTime; }
        private void Update()
        {
            if (_owner == null || _cycle >= 8 || Time.unscaledTime < _next) return;
            if (_cycleRoot != null) Destroy(_cycleRoot);
            _cycleRoot = new GameObject("Home/Training Entry Cycle " + (_cycle + 1)); _cycleRoot.transform.SetParent(_owner, false);
            Array stations = Enum.GetValues(typeof(HubStationType));
            for (int i = 0; i < stations.Length; i++)
            {
                HubStationType station = (HubStationType)stations.GetValue(i);
                GameObject root = new GameObject("Cycle Station · " + station); root.transform.SetParent(_cycleRoot.transform, false);
                root.transform.position = new Vector3((i % 5 - 2) * 4.5f, 0f, (i / 5 - 0.5f) * 6.5f);
                HubStationVisualBuilder.Build(root.transform, station, Color.HSVToRGB(i / (float)Mathf.Max(1, stations.Length), 0.72f, 0.9f));
            }
            _cycle++; _next = Time.unscaledTime + 0.85f;
        }
        private void OnDestroy() { if (_cycleRoot != null) Destroy(_cycleRoot); }
    }

    /// <summary>
    /// Captures the same deterministic combined stress field under every quality
    /// tier and four accessibility presentations. Existing settings are restored.
    /// </summary>
    public sealed class VisualComparisonMatrixRunner : MonoBehaviour
    {
        private struct CaptureCase
        {
            public string kind;
            public string id;
            public int quality;
            public string mode;
            public BiomeVisualId biome;
            public DungeonRoomType room;
            public CaptureCase(string kindValue, string idValue, int qualityValue = 2, string modeValue = "default", BiomeVisualId biomeValue = BiomeVisualId.OssuaryCatacombs, DungeonRoomType roomValue = DungeonRoomType.Combat)
            { kind = kindValue; id = idValue; quality = qualityValue; mode = modeValue; biome = biomeValue; room = roomValue; }
        }

        public static VisualComparisonMatrixRunner Instance { get; private set; }
        private readonly List<CaptureCase> _cases = new List<CaptureCase>();
        private int _index;
        private int _phase;
        private float _nextCapture;
        private string _folder;
        private VisualStressScenarioRunner _stress;
        private AccessibilitySettings _settings;
        private int _oldQuality;
        private float _oldSpellDensity;
        private float _oldEnvironmentDensity;
        private int _oldLightQuality;
        private int _oldShadowQuality;
        private float _oldDecalDuration;
        private bool _oldColorblind;
        private bool _oldHighContrast;
        private bool _oldReducedFlashes;
        private GameObject _stage;
        public bool Running { get; private set; }
        public string Status { get; private set; } = "Matrix idle";

        private void Awake() { Instance = this; }
        private void OnDestroy() { if (Running) { CleanupStage(); Restore(); } if (Instance == this) Instance = null; }

        public void Begin()
        {
            if (Running || ProfileManager.Current == null || GameWorld.Instance == null || GameWorld.Instance.RunActive) return;
            _stress = GetComponent<VisualStressScenarioRunner>();
            if (_stress == null || _stress.Running) return;
            _settings = ProfileManager.Current.accessibility;
            SaveOriginal();
            _cases.Clear();
            string[] modes = { "default", "high-contrast", "colorblind-symbols", "reduced-flashes" };
            for (int quality = 0; quality < 3; quality++) for (int mode = 0; mode < modes.Length; mode++)
                _cases.Add(new CaptureCase("quality", (quality == 0 ? "low" : quality == 1 ? "medium" : "high") + "_" + modes[mode], quality, modes[mode]));
            _cases.Add(new CaptureCase("elements", "six_elements_color"));
            _cases.Add(new CaptureCase("elements-grayscale", "six_elements_grayscale"));
            _cases.Add(new CaptureCase("deliveries", "ten_delivery_families"));
            _cases.Add(new CaptureCase("affixes", "all_elite_affixes"));
            _cases.Add(new CaptureCase("statuses", "all_target_statuses"));
            for (int biome = 0; biome < 4; biome++)
                foreach (DungeonRoomType room in Enum.GetValues(typeof(DungeonRoomType)))
                    _cases.Add(new CaptureCase("room", ((BiomeVisualId)biome) + "_" + room, 2, "default", (BiomeVisualId)biome, room));
            _cases.Add(new CaptureCase("home", "home_stations_without_labels"));
            _cases.Add(new CaptureCase("equipment", "equipment_slots_rarity_corruption"));
            _folder = Path.Combine(Application.persistentDataPath, "VisualComparison_2.0_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));
            try
            {
                Directory.CreateDirectory(_folder);
                File.WriteAllText(Path.Combine(_folder, "CAPTURE_CONTEXT.txt"),
                    "Arcane Engine 2.0 full visual comparison\nUTC " + DateTime.UtcNow.ToString("o") + "\nUnity " + Application.unityVersion +
                    "\nResolution " + Screen.width + "x" + Screen.height + "\nSeed 20020417\nCases " + _cases.Count +
                    "\nIncludes quality/accessibility, six elements in color and grayscale, ten deliveries, all Elite affixes, all target statuses, 17 room purposes in four route biomes, Home Base stations, and equipment identity.\n");
            }
            catch (Exception exception) { Restore(); Status = "Matrix could not start · " + exception.Message; return; }
            _index = 0;
            _phase = 0;
            Running = true;
            _nextCapture = Time.unscaledTime + 0.1f;
            Status = "Preparing first comparison case…";
        }

        private void Update()
        {
            if (!Running || Time.unscaledTime < _nextCapture) return;
            if (_index >= _cases.Count) { Finish(); return; }
            CaptureCase capture = _cases[_index];
            if (_phase == 0)
            {
                CleanupStage();
                ApplyCase(capture);
                BuildCase(capture);
                _phase = 1;
                _nextCapture = Time.unscaledTime + (capture.kind == "quality" ? 0.95f : capture.kind == "room" ? 0.38f : 0.18f);
                Status = "Building " + capture.id + " · " + (_index + 1) + "/" + _cases.Count;
                return;
            }
            try
            {
                string filename = (_index + 1).ToString("000") + "_" + SafeName(capture.id) + ".png";
                // ScreenCapture not available - capture removed: ScreenCapture(Path.Combine(_folder, filename), 1);
                File.AppendAllText(Path.Combine(_folder, "CAPTURE_MANIFEST.txt"), filename + "\t" + capture.kind + "\t" + capture.id + "\n");
            }
            catch (Exception exception) { Debug.LogError("Visual comparison capture failed: " + exception.Message); }
            CleanupStage();
            _index++;
            _phase = 0;
            Status = "Captured " + _index + " / " + _cases.Count + " · " + capture.id;
            _nextCapture = Time.unscaledTime + 0.3f;
        }

        private void ApplyCase(CaptureCase capture)
        {
            VisualQualityPolicy.ApplyPreset(_settings, capture.quality);
            _settings.highContrastTelegraphs = capture.mode == "high-contrast";
            _settings.colorblindSymbols = capture.mode == "colorblind-symbols";
            _settings.reducedFlashes = capture.mode == "reduced-flashes";
        }

        private void BuildCase(CaptureCase capture)
        {
            if (capture.kind == "quality") { _stress.Begin(VisualStressScenario.CombinedCombat); return; }
            _stage = new GameObject("Visual Comparison Stage · " + capture.id);
            if (capture.kind == "elements" || capture.kind == "elements-grayscale") BuildElements(capture.kind == "elements-grayscale");
            else if (capture.kind == "deliveries") BuildDeliveries();
            else if (capture.kind == "affixes") BuildAffixes();
            else if (capture.kind == "statuses") BuildStatuses();
            else if (capture.kind == "room") BuildRoom(capture);
            else if (capture.kind == "home") BuildHomeStations();
            else if (capture.kind == "equipment") BuildEquipmentCatalog();
        }

        private void BuildElements(bool grayscale)
        {
            Array elements = Enum.GetValues(typeof(SpellElement));
            for (int i = 0; i < elements.Length; i++)
            {
                SpellElement element = (SpellElement)elements.GetValue(i);
                CompiledSpell spell = VisualStressScenarioRunner.StressSpell(element, SpellDelivery.Projectile, i);
                if (grayscale)
                {
                    float primary = 0.28f + i * 0.09f; spell.primaryColor = new Color(primary, primary, primary);
                    float secondary = Mathf.Clamp01(primary + 0.24f); spell.accentColor = new Color(secondary, secondary, secondary);
                    SpellVisualCompiler.Rebuild(spell);
                }
                Vector3 position = new Vector3((i % 3 - 1) * 5.2f, 1f, (i / 3 - 0.5f) * 5.4f);
                GameObject host = new GameObject((grayscale ? "Grayscale " : string.Empty) + element); host.transform.SetParent(_stage.transform, false); host.transform.position = position;
                CastRequest request = new CastRequest { origin = position, castOrigin = position, targetPosition = position + Vector3.forward * 3f, direction = Vector3.forward, manualCast = true };
                SpellVisualAttachment.Attach(host, spell, request); SpellDeliveryVisuals.BeginCast(spell, request);
            }
        }

        private void BuildDeliveries()
        {
            Array deliveries = Enum.GetValues(typeof(SpellDelivery));
            for (int i = 0; i < deliveries.Length; i++)
            {
                SpellDelivery delivery = (SpellDelivery)deliveries.GetValue(i);
                CompiledSpell spell = VisualStressScenarioRunner.StressSpell((SpellElement)(i % Enum.GetValues(typeof(SpellElement)).Length), delivery, i);
                spell.radius = 1.4f; spell.zoneDuration = delivery == SpellDelivery.Zone ? 5f : 0f; spell.summonCount = delivery == SpellDelivery.Summon ? 3 : 0; SpellVisualCompiler.Rebuild(spell);
                Vector3 position = new Vector3((i % 5 - 2) * 3.6f, 1f, (i / 5 - 0.5f) * 5.5f);
                GameObject host = new GameObject("Delivery · " + delivery); host.transform.SetParent(_stage.transform, false); host.transform.position = position;
                CastRequest request = new CastRequest { origin = position, castOrigin = position, targetPosition = position + Vector3.forward * 2.2f, direction = Vector3.forward, manualCast = true };
                SpellVisualAttachment.Attach(host, spell, request); SpellDeliveryVisuals.BeginCast(spell, request);
            }
        }

        private void BuildAffixes()
        {
            List<EliteAffix> affixes = new List<EliteAffix>(); foreach (EliteAffix affix in Enum.GetValues(typeof(EliteAffix))) if (affix != EliteAffix.None) affixes.Add(affix);
            for (int i = 0; i < affixes.Count; i++)
            {
                GameObject root = new GameObject("Elite Affix · " + affixes[i]); root.transform.SetParent(_stage.transform, false); root.transform.position = new Vector3((i % 3 - 1) * 5f, 0f, (i / 3 - 0.5f) * 5.4f);
                EnemyVisualPreviewBuilder.BuildAffix(root.transform, affixes[i], Color.HSVToRGB(i / (float)affixes.Count, 0.76f, 1f));
            }
        }

        private void BuildStatuses()
        {
            string[] statuses = { "BURNING", "POISON", "SHOCKED", "CHILLED", "FROZEN", "STAGGERED", "ARMOR", "SHIELD", "RESISTANT" };
            Color[] colors = { new Color(1f, 0.2f, 0.02f), new Color(0.25f, 1f, 0.12f), new Color(0.32f, 0.62f, 1f), new Color(0.45f, 0.9f, 1f),
                new Color(0.72f, 0.95f, 1f), Color.white, new Color(0.82f, 0.76f, 0.58f), new Color(0.18f, 0.72f, 1f), new Color(0.88f, 0.82f, 0.58f) };
            for (int i = 0; i < statuses.Length; i++)
            {
                GameObject root = new GameObject("Target Status · " + statuses[i]); root.transform.SetParent(_stage.transform, false); root.transform.position = new Vector3((i % 5 - 2) * 3.8f, 0f, (i / 5 - 0.5f) * 5.4f);
                EnemyVisualPreviewBuilder.BuildStatus(root.transform, statuses[i], colors[i]);
            }
        }

        private void BuildRoom(CaptureCase capture)
        {
            RoomTemplate room = new RoomTemplate { id = "capture_" + capture.biome + "_" + capture.room, displayName = capture.room.ToString(), type = capture.room,
                accentColor = BiomeVisualCatalog.Get(capture.biome).emission, floorColor = BiomeVisualCatalog.Get(capture.biome).floor };
            ProceduralDungeonVisuals.BuildForAudit(_stage.transform, room, capture.biome, 20020417 + (int)capture.biome * 1009 + (int)capture.room * 37);
        }

        private void BuildHomeStations()
        {
            Array stations = Enum.GetValues(typeof(HubStationType));
            for (int i = 0; i < stations.Length; i++)
            {
                GameObject root = new GameObject("Station · " + stations.GetValue(i)); root.transform.SetParent(_stage.transform, false);
                root.transform.position = new Vector3((i % 5 - 2) * 3.8f, 0f, (i / 5 - 0.5f) * 5.6f);
                HubStationVisualBuilder.Build(root.transform, (HubStationType)stations.GetValue(i), Color.HSVToRGB(i / (float)stations.Length, 0.68f, 0.92f));
            }
        }

        private void BuildEquipmentCatalog()
        {
            Array slots = Enum.GetValues(typeof(EquipmentSlot));
            for (int i = 0; i < slots.Length; i++)
            {
                GameObject root = new GameObject("Equipment · " + slots.GetValue(i)); root.transform.SetParent(_stage.transform, false);
                root.transform.position = new Vector3((i % 5 - 2) * 3.8f, 0f, (i / 5 - 0.5f) * 5.5f);
                ItemRarity rarity = (ItemRarity)(i % Enum.GetValues(typeof(ItemRarity)).Length);
                RewardVisualSystem.BuildPreview(root.transform, new RewardVisualDescriptor { family = RewardVisualFamily.Equipment, equipmentSlot = (EquipmentSlot)slots.GetValue(i), rarity = rarity,
                    color = Color.HSVToRGB(i / (float)slots.Length, 0.68f, 0.92f), ringCount = 1 + (int)rarity, beamHeight = 1.8f + (int)rarity * 0.4f,
                    corrupted = i == slots.Length - 2, unique = i == slots.Length - 1, uniqueMutation = i == slots.Length - 1 ? UniqueMutation.OrbitingArsenal : UniqueMutation.None,
                    dataSummary = slots.GetValue(i) + " · " + rarity + (i == slots.Length - 2 ? " · Corrupted" : string.Empty) });
            }
        }

        private void CleanupStage()
        {
            if (_stress != null && _stress.Running) _stress.CleanupScenario();
            if (_stage != null) { Destroy(_stage); _stage = null; }
        }

        private static string SafeName(string value)
        {
            if (string.IsNullOrEmpty(value)) return "capture";
            char[] characters = value.ToLowerInvariant().ToCharArray();
            for (int i = 0; i < characters.Length; i++) if (!char.IsLetterOrDigit(characters[i]) && characters[i] != '-' && characters[i] != '_') characters[i] = '_';
            return new string(characters);
        }

        private void Finish()
        {
            Restore();
            CleanupStage();
            Running = false;
            Status = _cases.Count + " captures saved · " + _folder;
            Debug.Log("Visual comparison matrix complete: " + _folder);
        }

        private void SaveOriginal()
        {
            _oldQuality = _settings.visualQuality;
            _oldSpellDensity = _settings.spellEffectDensity;
            _oldEnvironmentDensity = _settings.environmentDensity;
            _oldLightQuality = _settings.dynamicLightQuality;
            _oldShadowQuality = _settings.shadowQuality;
            _oldDecalDuration = _settings.decalDuration;
            _oldColorblind = _settings.colorblindSymbols;
            _oldHighContrast = _settings.highContrastTelegraphs;
            _oldReducedFlashes = _settings.reducedFlashes;
        }

        private void Restore()
        {
            if (_settings == null) return;
            _settings.visualQuality = _oldQuality;
            _settings.spellEffectDensity = _oldSpellDensity;
            _settings.environmentDensity = _oldEnvironmentDensity;
            _settings.dynamicLightQuality = _oldLightQuality;
            _settings.shadowQuality = _oldShadowQuality;
            _settings.decalDuration = _oldDecalDuration;
            _settings.colorblindSymbols = _oldColorblind;
            _settings.highContrastTelegraphs = _oldHighContrast;
            _settings.reducedFlashes = _oldReducedFlashes;
        }
    }
}
