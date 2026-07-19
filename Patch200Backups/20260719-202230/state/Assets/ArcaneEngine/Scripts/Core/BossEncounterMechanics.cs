using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    /// <summary>
    /// Runtime boss encounter rules. Each phase changes what the player must do;
    /// the component is attached only to the room boss and owns its arena objects.
    /// </summary>
    public sealed class BossEncounterMechanics : MonoBehaviour
    {
        public int Phase { get; private set; } = 1;
        public string PhaseRule
        {
            get
            {
                if (_livingPillars > 0) return "WARD ACTIVE · BREAK " + _livingPillars + " PILLAR" + (_livingPillars == 1 ? string.Empty : "S");
                if (_boss != null && _boss.Archetype == EnemyArchetype.EmberTitan)
                    return Phase == 2 ? "MOLTEN RING · CROSS BETWEEN ERUPTIONS" : Phase >= 3 ? "CORE MELTDOWN · KEEP ROTATING" : "FURNACE VOLLEY · USE THE GAPS";
                if (_boss != null && _boss.Archetype == EnemyArchetype.ArchiveSeraph)
                    return Phase == 2 ? "MIRROR SCRIPT · CHANGE ANGLES" : Phase >= 3 ? "ERASURE BEAM · LEAVE THE LINE" : "INDEX VOLLEY · READ THE SEQUENCE";
                if (_boss != null && _boss.Archetype == EnemyArchetype.VenomMatriarch)
                    return Phase == 2 ? "BROOD BLOOM · CLEAR THE EGGS" : Phase >= 3 ? "TOXIC COLLAPSE · KEEP MOVING" : "VENOM POOLS · PRESERVE SPACE";
                if (Phase == 2) return "BONE STORM · KEEP MOVING";
                if (Phase >= 3) return "ADAPTIVE ECHO · CHANGE DAMAGE TYPES";
                return "THE WARD IS BROKEN";
            }
        }

        private readonly List<BreakableProp> _pillars = new List<BreakableProp>();
        private EnemyController _boss;
        private DifficultySettings _difficulty;
        private int _livingPillars;
        private float _mechanicCooldown = 2.2f;
        private SpellElement _adaptedElement = SpellElement.Arcane;
        private int _sameElementHits;
        private LineRenderer _arenaRing;
        private LineRenderer _innerRing;
        private float _phaseStartedAt;
        private bool _stallEscalated;

        public void Initialize(EnemyController boss, DifficultySettings difficulty)
        {
            _boss = boss;
            _difficulty = difficulty;
            BuildArena();
            BuildWardPillars(difficulty != null && difficulty.newBossPhase ? 4 : 3);
            _phaseStartedAt = Time.time;
            if (RunStatistics.Instance != null) RunStatistics.Instance.BeginBoss();
            if (DemoV05Director.Instance != null)
                DemoV05Director.Instance.Announce(_boss.DisplayName.ToUpperInvariant() + " · BREAK THE WARD PILLARS", 3.2f);
        }

        public float ModifyIncomingDamage(float damage, SpellElement element)
        {
            if (_livingPillars > 0) return damage * 0.12f;
            if (Phase >= 3)
            {
                if (element == _adaptedElement) _sameElementHits++;
                else { _adaptedElement = element; _sameElementHits = 1; }
                _boss.SetBossAdaptation(_adaptedElement, _sameElementHits);
                if (_sameElementHits >= 3) return damage * 0.48f;
            }
            return damage;
        }

        private void Update()
        {
            if (_boss == null || _boss.IsDead || GameWorld.Instance == null || !GameWorld.Instance.RunActive) return;
            int wanted = _boss.HealthRatio <= 0.34f ? 3 : _boss.HealthRatio <= 0.67f ? 2 : 1;
            if (wanted > Phase) EnterPhase(wanted);
            _mechanicCooldown -= Time.deltaTime;
            if (!_stallEscalated && Time.time - _phaseStartedAt > 38f)
            {
                _stallEscalated = true;
                _mechanicCooldown = 0f;
                if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce(_boss.DisplayName.ToUpperInvariant() + " ENRAGES · THE ARENA IS COLLAPSING", 2.5f);
                Vector3[] collapse = new Vector3[4];
                for (int i = 0; i < collapse.Length; i++)
                {
                    collapse[i] = new Vector3(-7f + i * 4.7f, 0f, 2f);
                    EnemyVisualEvents.TelegraphAt(_boss, "Enrage Collapse", collapse[i], Vector3.zero, 2.2f, 0.72f);
                }
                StartCoroutine(DelayedHazards(collapse, 14f, 0.72f));
            }
            if (_mechanicCooldown > 0f || _livingPillars > 0) return;
            _mechanicCooldown = Phase == 1 ? 4.5f : Phase == 2 ? 3.2f : 2.35f;
            PlayerController player = GameWorld.Instance.Player;
            if (player == null) return;
            if (_boss.Archetype == EnemyArchetype.EmberTitan)
            {
                ExecuteEmberMechanic(player);
            }
            else if (_boss.Archetype == EnemyArchetype.ArchiveSeraph)
            {
                ExecuteArchiveMechanic(player);
            }
            else if (_boss.Archetype == EnemyArchetype.VenomMatriarch)
            {
                ExecuteVenomMechanic(player);
            }
            else if (Phase == 1)
            {
                EnemyVisualEvents.TelegraphAt(_boss, "Ward Radial Volley", transform.position, Vector3.zero, 5.5f, 0.58f);
                StartCoroutine(DelayedRadialVolley(0.58f));
            }
            else if (Phase == 2)
            {
                Vector3[] points = new Vector3[3];
                for (int i = 0; i < 3; i++)
                {
                    points[i] = player.transform.position + new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
                    EnemyVisualEvents.TelegraphAt(_boss, "Bone Storm Hazard", points[i], Vector3.zero, 2.2f, 0.68f);
                }
                StartCoroutine(DelayedHazards(points, 12f, 0.68f));
            }
            else
            {
                Vector3 direction = player.transform.position - transform.position; direction.y = 0f;
                EnemyVisualEvents.TelegraphAt(_boss, "Adaptive Echo Volley", transform.position, direction, 9f, 0.52f);
                StartCoroutine(DelayedAdaptiveVolley(direction.normalized, 0.52f));
            }
        }

        private void ExecuteEmberMechanic(PlayerController player)
        {
            if (Phase == 1)
            {
                EnemyVisualEvents.TelegraphAt(_boss, "Furnace Volley", transform.position, Vector3.zero, 6.5f, 0.62f);
                StartCoroutine(DelayedElementRadialVolley(0.62f, 10, new Color(1f, 0.2f, 0.02f), 13f));
                return;
            }
            Vector3[] points = new Vector3[Phase >= 3 ? 6 : 4];
            for (int i = 0; i < points.Length; i++)
            {
                float angle = i / (float)points.Length * Mathf.PI * 2f + Time.time * 0.2f;
                points[i] = new Vector3(Mathf.Cos(angle) * 7.5f, 0f, Mathf.Sin(angle) * 7.5f + 2f);
                EnemyVisualEvents.TelegraphAt(_boss, "Molten Eruption", points[i], Vector3.zero, 2.5f, 0.72f);
            }
            StartCoroutine(DelayedHazards(points, Phase >= 3 ? 18f : 14f, 0.72f));
        }

        private void ExecuteArchiveMechanic(PlayerController player)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0f;
            if (Phase == 1)
            {
                EnemyVisualEvents.TelegraphAt(_boss, "Index Sequence", transform.position, direction, 10f, 0.58f);
                StartCoroutine(DelayedFanVolley(direction.normalized, 0.58f, 3, 18f, new Color(0.15f, 0.75f, 1f)));
            }
            else if (Phase == 2)
            {
                Vector3[] lines =
                {
                    player.transform.position + Vector3.left * 3f,
                    player.transform.position + Vector3.right * 3f,
                    player.transform.position + Vector3.forward * 3f
                };
                foreach (Vector3 point in lines) EnemyVisualEvents.TelegraphAt(_boss, "Mirror Script", point, Vector3.zero, 1.8f, 0.66f);
                StartCoroutine(DelayedHazards(lines, 13f, 0.66f));
            }
            else
            {
                EnemyVisualEvents.TelegraphAt(_boss, "Erasure Beam", transform.position, direction, 12f, 0.82f);
                StartCoroutine(DelayedFanVolley(direction.normalized, 0.82f, 7, 21f, new Color(0.55f, 0.25f, 1f)));
            }
        }

        private void ExecuteVenomMechanic(PlayerController player)
        {
            int count = Phase >= 3 ? 5 : 3;
            Vector3[] points = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                points[i] = player.transform.position + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
                EnemyVisualEvents.TelegraphAt(_boss, "Venom Pool", points[i], Vector3.zero, 2.4f, 0.7f);
            }
            StartCoroutine(DelayedHazards(points, Phase >= 3 ? 16f : 11f, 0.7f));
            if (Phase >= 2 && GameWorld.Instance.Enemies.Count < 14)
            {
                EnemyController.Spawn(transform.position + new Vector3(-4f, 0f, 2f), EnemyArchetype.Leech, 7, _difficulty, true, false);
                if (Phase >= 3) EnemyController.Spawn(transform.position + new Vector3(4f, 0f, 2f), EnemyArchetype.Assassin, 8, _difficulty, true, false);
            }
        }

        private void EnterPhase(int phase)
        {
            Phase = phase;
            _boss.SetBossPhase(phase);
            _phaseStartedAt = Time.time;
            _stallEscalated = false;
            _mechanicCooldown = 0.8f;
            foreach (EnemyBolt bolt in FindObjectsByType<EnemyBolt>()) Destroy(bolt.gameObject);
            foreach (PersistentEnemyHazard hazard in FindObjectsByType<PersistentEnemyHazard>()) Destroy(hazard.gameObject);
            if (RunStatistics.Instance != null) RunStatistics.Instance.RecordBossPhase(phase);
            string message;
            if (_boss.Archetype == EnemyArchetype.EmberTitan)
                message = phase == 2 ? "PHASE 2 · MOLTEN RING · CROSS BETWEEN ERUPTIONS" : "FINAL PHASE · CORE MELTDOWN · KEEP ROTATING";
            else if (_boss.Archetype == EnemyArchetype.ArchiveSeraph)
                message = phase == 2 ? "PHASE 2 · MIRROR SCRIPT · CHANGE ANGLES" : "FINAL PHASE · ERASURE BEAM · LEAVE THE LINE";
            else if (_boss.Archetype == EnemyArchetype.VenomMatriarch)
                message = phase == 2 ? "PHASE 2 · BROOD BLOOM · CLEAR THE ADDS" : "FINAL PHASE · TOXIC COLLAPSE · KEEP MOVING";
            else message = phase == 2 ? "PHASE 2 · BONE STORM · THE ARENA BECOMES DANGEROUS" :
                "FINAL PHASE · THE WARDEN ADAPTS · ROTATE DAMAGE TYPES";
            if (DemoV05Director.Instance != null) DemoV05Director.Instance.Announce(message, 3f);
            if (phase == 2 && _boss.Archetype == EnemyArchetype.OssuaryWarden)
            {
                EnemyController.Spawn(transform.position + new Vector3(-5f, 0f, 3f), EnemyArchetype.Controller, 7, _difficulty, true, false);
                EnemyController.Spawn(transform.position + new Vector3(5f, 0f, 3f), EnemyArchetype.Leech, 7, _difficulty, true, false);
            }
            GameFeelSystem.Burst(transform.position + Vector3.up, phase == 2 ? Color.red : Color.magenta, 1.8f);
            if (AdaptiveAudioSystem.Instance != null) AdaptiveAudioSystem.PlayBossPhase(phase);
        }

        private System.Collections.IEnumerator DelayedRadialVolley(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_boss == null || _boss.IsDead) yield break;
            for (int i = 0; i < 8; i++)
                EnemyBolt.Create(transform.position + Vector3.up, Quaternion.Euler(0f, i * 45f, 0f) * Vector3.forward, 12f, new Color(0.75f, 0.15f, 1f));
        }

        private System.Collections.IEnumerator DelayedElementRadialVolley(float delay, int count, Color color, float damage)
        {
            yield return new WaitForSeconds(delay);
            if (_boss == null || _boss.IsDead) yield break;
            for (int i = 0; i < count; i++)
                EnemyBolt.Create(transform.position + Vector3.up, Quaternion.Euler(0f, i * (360f / count), 0f) * Vector3.forward, damage, color);
        }

        private System.Collections.IEnumerator DelayedFanVolley(Vector3 direction, float delay, int count, float damage, Color color)
        {
            yield return new WaitForSeconds(delay);
            if (_boss == null || _boss.IsDead) yield break;
            float spread = count <= 1 ? 0f : 54f;
            for (int i = 0; i < count; i++)
            {
                float angle = count <= 1 ? 0f : Mathf.Lerp(-spread * 0.5f, spread * 0.5f, i / (float)(count - 1));
                EnemyBolt.Create(transform.position + Vector3.up, Quaternion.Euler(0f, angle, 0f) * direction, damage, color);
            }
        }

        private System.Collections.IEnumerator DelayedHazards(Vector3[] points, float damage, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_boss == null || _boss.IsDead) yield break;
            for (int i = 0; i < points.Length; i++)
                PersistentEnemyHazard.Create(points[i], 2.2f, damage, new Color(0.9f, 0.12f, 0.18f), 0f);
        }

        private System.Collections.IEnumerator DelayedAdaptiveVolley(Vector3 direction, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_boss == null || _boss.IsDead) yield break;
            for (int i = 0; i < 5; i++)
                EnemyBolt.Create(transform.position + Vector3.up, Quaternion.Euler(0f, -24f + i * 12f, 0f) * direction, 15f, ElementColor(_adaptedElement));
            EnemyController.Spawn(transform.position + new Vector3(Random.value < 0.5f ? -4f : 4f, 0f, 3f),
                EnemyArchetype.Assassin, 8, _difficulty, true, false);
        }

        private void BuildArena()
        {
            _arenaRing = RuntimeVisuals.Ring("Boss Arena Boundary", Vector3.zero, new Color(0.55f, 0.08f, 0.75f), 15.2f, 0.16f);
            _innerRing = RuntimeVisuals.Ring("Boss Arena Sigil", new Vector3(0f, 0.01f, 3f), new Color(0.9f, 0.12f, 0.2f), 4.5f, 0.1f);
        }

        private void BuildWardPillars(int count)
        {
            _livingPillars = count;
            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f;
                string objectName = _boss != null && _boss.Archetype == EnemyArchetype.EmberTitan ? "Furnace Vent" :
                    _boss != null && _boss.Archetype == EnemyArchetype.ArchiveSeraph ? "Archive Seal" :
                    _boss != null && _boss.Archetype == EnemyArchetype.VenomMatriarch ? "Brood Egg" : "Boss Ward Pillar";
                Color objectColor = _boss != null && _boss.Archetype == EnemyArchetype.EmberTitan ? new Color(1f, 0.2f, 0.02f) :
                    _boss != null && _boss.Archetype == EnemyArchetype.ArchiveSeraph ? new Color(0.15f, 0.75f, 1f) :
                    _boss != null && _boss.Archetype == EnemyArchetype.VenomMatriarch ? new Color(0.3f, 1f, 0.08f) :
                    new Color(0.7f, 0.08f, 1f);
                BreakableProp pillar = BreakableProp.Create(new Vector3(Mathf.Cos(angle) * 10f, 0f, Mathf.Sin(angle) * 8f + 2f),
                    objectName, objectColor).SetHealth(65f, false);
                pillar.transform.localScale = new Vector3(1.1f, 2.8f, 1.1f);
                pillar.Destroyed += OnPillarDestroyed;
                _pillars.Add(pillar);
            }
        }

        private void OnPillarDestroyed()
        {
            _livingPillars = Mathf.Max(0, _livingPillars - 1);
            if (RunStatistics.Instance != null) RunStatistics.Instance.RecordBossPillar();
            if (_livingPillars == 0 && DemoV05Director.Instance != null)
                DemoV05Director.Instance.Announce("THE WARD SHATTERS · DAMAGE THE BOSS", 2.5f);
        }

        private void OnDestroy()
        {
            foreach (BreakableProp pillar in _pillars.Where(value => value != null)) Destroy(pillar.gameObject);
            if (_arenaRing != null) Destroy(_arenaRing.gameObject);
            if (_innerRing != null) Destroy(_innerRing.gameObject);
        }

        private static Color ElementColor(SpellElement element)
        {
            if (element == SpellElement.Fire) return new Color(1f, 0.22f, 0.05f);
            if (element == SpellElement.Lightning) return new Color(0.2f, 0.75f, 1f);
            if (element == SpellElement.Frost) return new Color(0.35f, 0.9f, 1f);
            if (element == SpellElement.Toxic) return new Color(0.2f, 1f, 0.3f);
            return new Color(0.75f, 0.2f, 1f);
        }
    }
}
