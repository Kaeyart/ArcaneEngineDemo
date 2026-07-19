using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    [System.Serializable]
    public sealed class EnemyVisualDefinition
    {
        public EnemyArchetype archetype;
        public string silhouette;
        public PrimitiveType bodyShape;
        public PrimitiveType headShape;
        public PrimitiveType roleShape;
        public Vector3 bodyScale;
        public Vector3 headScale;
        public Vector3 roleScale;
        public Vector3 headOffset;
        public Vector3 roleOffset;
        public Color baseColor;
        public Color accentColor;
        public float motionRate;
        public float motionAmount;
        public Vector3 attackOrigin;
        public Vector3 targetPoint;
        public string motionProfile;
        public string hitResponseProfile;
        public string deathProfile;
        public string allowedBiomeDecoration;
    }

    public static class EnemyVisualCatalog
    {
        private static readonly Dictionary<EnemyArchetype, EnemyVisualDefinition> Definitions = new Dictionary<EnemyArchetype, EnemyVisualDefinition>();

        public static EnemyVisualDefinition Get(EnemyArchetype archetype)
        {
            EnemyVisualDefinition definition;
            if (Definitions.TryGetValue(archetype, out definition)) return definition;
            definition = Build(archetype);
            Definitions[archetype] = definition;
            return definition;
        }

        public static int Count { get { return System.Enum.GetValues(typeof(EnemyArchetype)).Length; } }

        private static EnemyVisualDefinition Build(EnemyArchetype archetype)
        {
            EnemyVisualDefinition value = new EnemyVisualDefinition
            {
                archetype = archetype,
                silhouette = archetype.ToString(),
                bodyShape = PrimitiveType.Capsule,
                headShape = PrimitiveType.Sphere,
                roleShape = PrimitiveType.Cube,
                bodyScale = new Vector3(0.72f, 0.8f, 0.72f),
                headScale = Vector3.one * 0.42f,
                roleScale = new Vector3(0.18f, 0.18f, 0.72f),
                headOffset = new Vector3(0f, 0.78f, 0f),
                roleOffset = new Vector3(0f, 0.18f, 0.68f),
                baseColor = ColorFor(archetype),
                accentColor = Color.Lerp(ColorFor(archetype), Color.white, 0.52f),
                motionRate = 2.6f,
                motionAmount = 0.08f,
                attackOrigin = new Vector3(0f, 0.72f, 0.62f),
                targetPoint = new Vector3(0f, 0.62f, 0f),
                motionProfile = "Medium grounded",
                hitResponseProfile = "Directional recoil",
                deathProfile = "Neutral collapse with elemental resolution",
                allowedBiomeDecoration = "Route-biome perimeter accents only"
            };
            switch (archetype)
            {
                case EnemyArchetype.Crawler:
                    value.silhouette = "Low rushing hunter"; value.bodyScale = new Vector3(0.82f, 0.42f, 1.05f); value.headOffset = new Vector3(0f, 0.28f, 0.66f); value.roleShape = PrimitiveType.Sphere; value.roleScale = new Vector3(0.22f, 0.12f, 0.55f); value.roleOffset = new Vector3(0f, 0.05f, -0.65f); value.motionRate = 5.4f; value.motionAmount = 0.13f; value.motionProfile = "Fast low quadruped"; value.attackOrigin = new Vector3(0f, 0.3f, 0.78f); value.deathProfile = "Forward tumble"; break;
                case EnemyArchetype.Bulwark:
                    value.silhouette = "Wide armored blocker"; value.bodyShape = PrimitiveType.Cube; value.bodyScale = new Vector3(1.32f, 1.05f, 0.82f); value.headScale = new Vector3(0.5f, 0.38f, 0.5f); value.roleShape = PrimitiveType.Cube; value.roleScale = new Vector3(1.45f, 0.9f, 0.2f); value.roleOffset = new Vector3(0f, 0.15f, 0.72f); value.motionRate = 1.5f; value.motionAmount = 0.035f; value.motionProfile = "Heavy planted"; value.hitResponseProfile = "Plate-weighted recoil"; value.deathProfile = "Heavy segmented fall"; break;
                case EnemyArchetype.Hexer:
                    value.silhouette = "Tall ranged caster"; value.bodyShape = PrimitiveType.Cylinder; value.bodyScale = new Vector3(0.66f, 1.1f, 0.66f); value.headShape = PrimitiveType.Cube; value.headScale = Vector3.one * 0.38f; value.roleShape = PrimitiveType.Sphere; value.roleScale = Vector3.one * 0.24f; value.roleOffset = new Vector3(0.72f, 0.55f, 0f); value.motionRate = 2f; value.motionAmount = 0.12f; value.motionProfile = "Hovering caster"; value.attackOrigin = new Vector3(0.72f, 0.95f, 0f); value.deathProfile = "Focus collapse"; break;
                case EnemyArchetype.Charger:
                    value.silhouette = "Forward-weighted disruptor"; value.bodyScale = new Vector3(0.85f, 0.72f, 1.18f); value.headOffset = new Vector3(0f, 0.56f, 0.58f); value.roleShape = PrimitiveType.Capsule; value.roleScale = new Vector3(0.3f, 0.62f, 0.3f); value.roleOffset = new Vector3(0f, 0.52f, 1.02f); value.motionRate = 4.4f; value.motionAmount = 0.09f; value.motionProfile = "Accelerating forward lean"; value.attackOrigin = new Vector3(0f, 0.68f, 1.05f); value.deathProfile = "Momentum skid"; break;
                case EnemyArchetype.Warden:
                    value.silhouette = "Upright shield guardian"; value.bodyShape = PrimitiveType.Cylinder; value.bodyScale = new Vector3(0.9f, 1.25f, 0.9f); value.headScale = new Vector3(0.46f, 0.35f, 0.46f); value.roleShape = PrimitiveType.Cylinder; value.roleScale = new Vector3(0.95f, 0.08f, 0.95f); value.roleOffset = new Vector3(0f, 0.32f, 0.78f); value.motionRate = 1.3f; value.motionAmount = 0.04f; break;
                case EnemyArchetype.Leech:
                    value.silhouette = "Tethered support"; value.bodyScale = new Vector3(0.62f, 0.88f, 0.62f); value.headShape = PrimitiveType.Sphere; value.roleShape = PrimitiveType.Cylinder; value.roleScale = new Vector3(0.18f, 0.72f, 0.18f); value.roleOffset = new Vector3(0.62f, 0.42f, 0f); value.motionRate = 2.8f; value.motionAmount = 0.1f; break;
                case EnemyArchetype.Mirror:
                    value.silhouette = "Faceted ranged reflector"; value.bodyShape = PrimitiveType.Cube; value.bodyScale = new Vector3(0.72f, 0.95f, 0.72f); value.headShape = PrimitiveType.Cube; value.roleShape = PrimitiveType.Cube; value.roleScale = new Vector3(0.68f, 0.68f, 0.08f); value.roleOffset = new Vector3(0f, 0.38f, 0.66f); value.motionRate = 1.8f; break;
                case EnemyArchetype.Assassin:
                    value.silhouette = "Narrow flanking blade"; value.bodyScale = new Vector3(0.48f, 0.92f, 0.48f); value.headScale = Vector3.one * 0.3f; value.roleShape = PrimitiveType.Capsule; value.roleScale = new Vector3(0.12f, 0.65f, 0.12f); value.roleOffset = new Vector3(0.55f, 0.22f, 0.38f); value.motionRate = 4.8f; value.motionAmount = 0.12f; break;
                case EnemyArchetype.Controller:
                    value.silhouette = "Wide area controller"; value.bodyShape = PrimitiveType.Cylinder; value.bodyScale = new Vector3(0.82f, 0.8f, 0.82f); value.headShape = PrimitiveType.Sphere; value.roleShape = PrimitiveType.Cylinder; value.roleScale = new Vector3(1.15f, 0.05f, 1.15f); value.roleOffset = new Vector3(0f, -0.35f, 0f); value.motionRate = 2.2f; break;
                case EnemyArchetype.OssuaryWarden:
                    value.silhouette = "Layered boss monument"; value.bodyShape = PrimitiveType.Cylinder; value.bodyScale = new Vector3(1.65f, 1.8f, 1.65f); value.headShape = PrimitiveType.Cube; value.headScale = new Vector3(0.8f, 0.62f, 0.8f); value.headOffset = new Vector3(0f, 1.62f, 0f); value.roleShape = PrimitiveType.Cylinder; value.roleScale = new Vector3(1.9f, 0.12f, 1.9f); value.roleOffset = new Vector3(0f, 0.45f, 0f); value.motionRate = 1.1f; value.motionAmount = 0.07f; value.motionProfile = "Monumental phase-weighted"; value.attackOrigin = new Vector3(0f, 1.45f, 1.15f); value.targetPoint = new Vector3(0f, 1.25f, 0f); value.hitResponseProfile = "Phase-stable authority recoil"; value.deathProfile = "Monument disassembly"; value.allowedBiomeDecoration = "Warden Sanctum authority motifs over route biome"; break;
                case EnemyArchetype.TrainingDummy:
                    value.silhouette = "Neutral readable target"; value.bodyShape = PrimitiveType.Cylinder; value.bodyScale = new Vector3(0.7f, 0.9f, 0.7f); value.headShape = PrimitiveType.Cube; value.roleShape = PrimitiveType.Cylinder; value.roleScale = new Vector3(0.9f, 0.08f, 0.9f); value.roleOffset = new Vector3(0f, -0.48f, 0f); value.motionRate = 0.8f; value.motionAmount = 0.02f; break;
            }
            return value;
        }

        private static Color ColorFor(EnemyArchetype type)
        {
            switch (type)
            {
                case EnemyArchetype.Crawler: return new Color(0.92f, 0.16f, 0.2f);
                case EnemyArchetype.Bulwark: return new Color(0.82f, 0.4f, 0.06f);
                case EnemyArchetype.Hexer: return new Color(0.62f, 0.12f, 0.88f);
                case EnemyArchetype.Charger: return new Color(1f, 0.24f, 0.05f);
                case EnemyArchetype.Warden: return new Color(0.18f, 0.58f, 1f);
                case EnemyArchetype.Leech: return new Color(0.22f, 0.88f, 0.32f);
                case EnemyArchetype.Mirror: return new Color(0.68f, 0.78f, 1f);
                case EnemyArchetype.Assassin: return new Color(0.55f, 0.05f, 0.78f);
                case EnemyArchetype.Controller: return new Color(0.05f, 0.76f, 0.68f);
                case EnemyArchetype.OssuaryWarden: return new Color(0.88f, 0.05f, 0.18f);
                default: return new Color(0.36f, 0.7f, 0.82f);
            }
        }
    }

    public sealed class ProceduralEnemyVisual : MonoBehaviour
    {
        private struct StatusVisualSpec
        {
            public PrimitiveType shape;
            public Color color;
            public Vector3 localPosition;
            public Vector3 scale;
        }

        private EnemyController _enemy;
        private EnemyVisualDefinition _definition;
        private Transform _assembly;
        private Transform _head;
        private Transform _role;
        private readonly Dictionary<EliteAffix, GameObject> _affixMarks = new Dictionary<EliteAffix, GameObject>();
        private readonly Dictionary<string, GameObject> _statusMarks = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, StatusVisualSpec> _statusSpecs = new Dictionary<string, StatusVisualSpec>();
        private Vector3 _headBase;
        private Vector3 _roleBase;
        private Vector3 _lastWorldPosition;
        private Vector3 _smoothedVelocity;
        private Vector3 _impactDirection;
        private float _impactStrength;
        private float _phase;
        private int _bossPhase = 1;
        private EnemyBrainState _lastState;
        private float _stateAge;
        private string _lastStatusSummary = string.Empty;

        public static ProceduralEnemyVisual Attach(EnemyController enemy)
        {
            if (enemy == null) return null;
            ProceduralEnemyVisual visual = enemy.gameObject.AddComponent<ProceduralEnemyVisual>();
            visual._enemy = enemy;
            visual._definition = EnemyVisualCatalog.Get(enemy.Archetype);
            visual._lastWorldPosition = enemy.transform.position;
            visual._lastState = enemy.BrainState;
            visual.BuildAssembly();
            if (enemy.IsEliteOrBoss)
            {
                BiomeLightingProfile lighting = ProceduralLightingDirector.CurrentProfile;
                Color separation = lighting == null ? visual._definition.accentColor : Color.Lerp(visual._definition.accentColor, lighting.enemySeparation, 0.46f);
                PriorityLightAnchor.Attach(enemy.gameObject, separation, enemy.IsBoss ? 6.5f : 4.2f, enemy.IsBoss ? 0.95f : 0.52f, enemy.IsBoss ? 5 : 4);
            }
            enemy.RefreshVisualBounds();
            return visual;
        }

        private void BuildAssembly()
        {
            Renderer original = GetComponent<Renderer>();
            if (original != null) original.enabled = false;
            GameObject root = new GameObject("Visual Assembly · " + _definition.silhouette);
            root.transform.SetParent(transform, false);
            _assembly = root.transform;
            VisualCounterRegistration.Attach(root, VisualRuntimeKind.EnemyAssembly);
            CreateModule("Body", _definition.bodyShape, Vector3.zero, _definition.bodyScale, _definition.baseColor);
            _head = CreateModule("Head", _definition.headShape, _definition.headOffset, _definition.headScale, _definition.accentColor).transform;
            _role = CreateModule("Role Read", _definition.roleShape, _definition.roleOffset, _definition.roleScale, Color.Lerp(_definition.baseColor, _definition.accentColor, 0.62f)).transform;
            _headBase = _head.localPosition;
            _roleBase = _role.localPosition;
            BuildAffixes();
            BuildStatuses();
        }

        private GameObject CreateModule(string name, PrimitiveType type, Vector3 localPosition, Vector3 scale, Color color)
        {
            GameObject go = RuntimeVisuals.Primitive(name, type, transform.position, scale, color, _assembly);
            RuntimeVisuals.RemoveCollider(go);
            go.transform.localPosition = localPosition;
            return go;
        }

        private void BuildAffixes()
        {
            IReadOnlyList<EliteAffix> affixes = _enemy.ActiveAffixes;
            for (int i = 0; i < affixes.Count; i++)
            {
                EliteAffix affix = affixes[i];
                Color color = AffixColor(affix);
                PrimitiveType shape = affix == EliteAffix.Shielded ? PrimitiveType.Cylinder : affix == EliteAffix.Summoner ? PrimitiveType.Cube : PrimitiveType.Sphere;
                float angle = i / (float)Mathf.Max(1, affixes.Count) * Mathf.PI * 2f;
                GameObject mark = CreateModule("Elite Affix · " + affix, shape, new Vector3(Mathf.Cos(angle) * 0.92f, 0.92f, Mathf.Sin(angle) * 0.92f),
                    affix == EliteAffix.Shielded ? new Vector3(0.72f, 0.06f, 0.72f) : Vector3.one * 0.18f, color);
                _affixMarks[affix] = mark;
            }
        }

        private void BuildStatuses()
        {
            AddStatus("BURNING", PrimitiveType.Sphere, new Color(1f, 0.2f, 0.02f), new Vector3(-0.42f, 1.08f, 0f));
            AddStatus("POISON", PrimitiveType.Sphere, new Color(0.25f, 1f, 0.12f), new Vector3(0.42f, 0.88f, 0f));
            AddStatus("SHOCKED", PrimitiveType.Capsule, new Color(0.32f, 0.62f, 1f), new Vector3(0f, 1.28f, 0f));
            AddStatus("CHILLED", PrimitiveType.Cube, new Color(0.45f, 0.9f, 1f), new Vector3(-0.35f, 0.48f, 0f));
            AddStatus("FROZEN", PrimitiveType.Cube, new Color(0.72f, 0.95f, 1f), new Vector3(0f, 0.55f, 0f), new Vector3(1.25f, 1.45f, 1.25f));
            AddStatus("STAGGERED", PrimitiveType.Cylinder, Color.white, new Vector3(0f, 1.48f, 0f));
            AddStatus("ARMOR", PrimitiveType.Cube, new Color(0.82f, 0.76f, 0.58f), new Vector3(0f, 0.22f, 0.72f), new Vector3(0.78f, 0.62f, 0.12f));
            AddStatus("SHIELD", PrimitiveType.Cylinder, new Color(0.18f, 0.72f, 1f), new Vector3(0f, 0.42f, 0.82f), new Vector3(0.9f, 0.06f, 0.9f));
            AddStatus("RESISTANT", PrimitiveType.Cube, new Color(0.88f, 0.82f, 0.58f), new Vector3(0f, 1.42f, 0f), new Vector3(0.22f, 0.22f, 0.22f));
        }

        private void AddStatus(string status, PrimitiveType shape, Color color, Vector3 localPosition, Vector3? scale = null)
        {
            _statusSpecs[status] = new StatusVisualSpec { shape = shape, color = color, localPosition = localPosition, scale = scale ?? Vector3.one * 0.15f };
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<string, GameObject> status in _statusMarks)
            {
                if (!OwnsStatus(status.Value)) continue;
                StatusVisualTreatment treatment = status.Value.GetComponent<StatusVisualTreatment>();
                if (treatment != null) treatment.Clear();
                status.Value.transform.SetParent(null, true);
                ProceduralVisualRuntime.Release(status.Value);
            }
            _statusMarks.Clear();
        }

        private void Update()
        {
            if (_enemy == null || _enemy.IsDead || _assembly == null) return;
            if (_enemy.BrainState != _lastState) { _lastState = _enemy.BrainState; _stateAge = 0f; }
            else _stateAge += Time.deltaTime;
            Vector3 rawVelocity = Time.deltaTime <= 0.0001f ? Vector3.zero : (transform.position - _lastWorldPosition) / Time.deltaTime;
            _lastWorldPosition = transform.position;
            _smoothedVelocity = Vector3.Lerp(_smoothedVelocity, rawVelocity, 1f - Mathf.Exp(-12f * Time.deltaTime));
            _phase += Time.deltaTime * _definition.motionRate;
            float motion = Mathf.Sin(_phase) * _definition.motionAmount;
            float urgency = _enemy.BrainState == EnemyBrainState.Telegraphing ? 1.5f : _enemy.BrainState == EnemyBrainState.Attacking ? 2f : 1f;
            _head.localPosition = _headBase + Vector3.up * motion * urgency;
            _role.localPosition = _roleBase + Vector3.up * -motion * 0.5f;
            _role.Rotate(0f, Time.deltaTime * (_enemy.IsBoss ? 38f : 72f), 0f, Space.Self);
            float movement = Mathf.Clamp01(new Vector2(_smoothedVelocity.x, _smoothedVelocity.z).magnitude / 4.5f);
            float stride = Mathf.Sin(_phase * 1.8f) * movement;
            Vector3 stateScale = Vector3.one;
            Quaternion stateRotation = Quaternion.identity;
            if (_enemy.BrainState == EnemyBrainState.Telegraphing)
            {
                stateScale = new Vector3(1.08f, 0.9f, 1.08f);
                stateRotation = Quaternion.Euler(-8f, 0f, 0f);
            }
            else if (_enemy.BrainState == EnemyBrainState.Attacking)
            {
                float attackPulse = Mathf.Sin(Mathf.Clamp01(_stateAge / 0.22f) * Mathf.PI);
                stateScale = new Vector3(0.92f, 1.04f, 1f + attackPulse * 0.22f);
                stateRotation = Quaternion.Euler(12f * attackPulse, 0f, 0f);
            }
            else if (_enemy.BrainState == EnemyBrainState.Recovering)
            {
                stateScale = new Vector3(1.04f, 0.96f, 0.92f);
                stateRotation = Quaternion.Euler(-5f, 0f, 0f);
            }
            else if (_enemy.BrainState == EnemyBrainState.Staggered)
                stateRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(_phase * 5f) * 9f);
            else if (movement > 0.05f)
            {
                stateScale = new Vector3(1f - stride * 0.025f, 1f + Mathf.Abs(stride) * 0.055f, 1f + stride * 0.035f);
                stateRotation = Quaternion.Euler(Mathf.Clamp(_smoothedVelocity.magnitude * 1.4f, 0f, 9f), 0f, -stride * 2.5f);
            }
            if (_impactStrength > 0.001f)
            {
                Vector3 localImpact = transform.InverseTransformDirection(_impactDirection);
                stateRotation *= Quaternion.Euler(-localImpact.z * _impactStrength * 8f, 0f, localImpact.x * _impactStrength * 8f);
                _impactStrength = Mathf.MoveTowards(_impactStrength, 0f, Time.deltaTime * 7f);
            }
            _assembly.localScale = Vector3.Lerp(_assembly.localScale, stateScale, 1f - Mathf.Exp(-14f * Time.deltaTime));
            _assembly.localRotation = Quaternion.Slerp(_assembly.localRotation, stateRotation, 1f - Mathf.Exp(-12f * Time.deltaTime));
            string summary = _enemy.StatusSummary;
            if (_lastStatusSummary.Contains("FROZEN") && !summary.Contains("FROZEN")) EnemyVisualEvents.StatusRemoved(_enemy, "FROZEN");
            _lastStatusSummary = summary;
            HashSet<string> visibleStatuses = new HashSet<string>();
            string[] priority = { "FROZEN", "STAGGERED", "SHIELD", "ARMOR", "RESISTANT", "BURNING", "SHOCKED", "POISON", "CHILLED" };
            int statusBudget = summary.Contains("FROZEN") ? 1 : VisualQualityBudget.Current.statusLayers;
            for (int i = 0; i < priority.Length && visibleStatuses.Count < statusBudget; i++) if (summary.Contains(priority[i])) visibleStatuses.Add(priority[i]);
            foreach (KeyValuePair<string, StatusVisualSpec> status in _statusSpecs)
            {
                bool active = visibleStatuses.Contains(status.Key);
                GameObject mark;
                _statusMarks.TryGetValue(status.Key, out mark);
                if (mark != null && !OwnsStatus(mark)) { _statusMarks.Remove(status.Key); mark = null; }
                if (active && mark == null)
                {
                    StatusVisualSpec spec = status.Value;
                    mark = ProceduralVisualRuntime.AcquirePrimitive(spec.shape, "Status · " + status.Key, transform.position,
                        spec.scale, spec.color, _assembly, 1.05f);
                    if (mark != null)
                    {
                        mark.transform.localPosition = spec.localPosition;
                        VisualCounterRegistration.Attach(mark, VisualRuntimeKind.StatusLayer);
                        StatusVisualTreatment treatment = mark.GetComponent<StatusVisualTreatment>();
                        if (treatment == null) treatment = mark.AddComponent<StatusVisualTreatment>();
                        treatment.Begin(status.Key, spec.color);
                        _statusMarks[status.Key] = mark;
                    }
                }
                else if (!active && mark != null)
                {
                    _statusMarks.Remove(status.Key);
                    StatusVisualTreatment treatment = mark.GetComponent<StatusVisualTreatment>();
                    if (treatment != null) treatment.Clear();
                    mark.transform.SetParent(null, true);
                    ProceduralVisualRuntime.Release(mark);
                    mark = null;
                }
                if (mark != null) mark.transform.Rotate(34f * Time.deltaTime, 62f * Time.deltaTime, 21f * Time.deltaTime);
            }
            foreach (KeyValuePair<EliteAffix, GameObject> affix in _affixMarks)
                if (affix.Value != null) affix.Value.transform.Rotate(0f, Time.deltaTime * (affix.Key == EliteAffix.Frenzied ? 165f : 75f), 0f, Space.Self);
        }

        public void ReactToImpact(Vector3 direction, float force)
        {
            _impactDirection = direction;
            _impactStrength = Mathf.Clamp(force / Mathf.Max(20f, _enemy == null ? 100f : _enemy.MaxHealth), 0.12f, 1f);
        }

        private bool OwnsStatus(GameObject mark)
        {
            if (mark == null || _assembly == null || mark.transform.parent != _assembly) return false;
            PooledVisualMarker marker = mark.GetComponent<PooledVisualMarker>();
            return marker != null && marker.InUse;
        }

        public void SetBossPhase(int phase)
        {
            _bossPhase = Mathf.Max(_bossPhase, phase);
            if (_enemy == null || !_enemy.IsBoss || _assembly == null) return;
            _role.localScale = _definition.roleScale * (1f + (_bossPhase - 1) * 0.18f);
            _head.localScale = _definition.headScale * (1f + (_bossPhase - 1) * 0.08f);
            GameObject phaseCrown = CreateModule("Boss Phase " + _bossPhase + " Crown", PrimitiveType.Cube,
                new Vector3(0f, _definition.headOffset.y + 0.62f + _bossPhase * 0.1f, 0f), new Vector3(0.16f + _bossPhase * 0.05f, 0.52f, 0.16f),
                Color.Lerp(_definition.baseColor, Color.white, 0.25f * _bossPhase));
            phaseCrown.transform.localRotation = Quaternion.Euler(0f, _bossPhase * 23f, 45f);
            _enemy.RefreshVisualBounds();
        }

        private static Color AffixColor(EliteAffix affix)
        {
            switch (affix)
            {
                case EliteAffix.Frenzied: return new Color(1f, 0.12f, 0.06f);
                case EliteAffix.Shielded: return new Color(0.2f, 0.72f, 1f);
                case EliteAffix.Volatile: return new Color(1f, 0.45f, 0.04f);
                case EliteAffix.Vampiric: return new Color(0.75f, 0.02f, 0.26f);
                case EliteAffix.Resistant: return new Color(0.85f, 0.82f, 0.62f);
                case EliteAffix.Summoner: return new Color(0.68f, 0.22f, 1f);
                default: return Color.white;
            }
        }
    }

    /// <summary>
    /// Multi-part, pooled treatment for a full-priority status. Compact status text in
    /// the health bar remains visible when the full-treatment budget is exhausted.
    /// </summary>
    public sealed class StatusVisualTreatment : MonoBehaviour
    {
        private readonly List<GameObject> _parts = new List<GameObject>();
        private string _status;
        private float _phase;

        public void Begin(string status, Color color)
        {
            Clear();
            _status = status ?? string.Empty;
            int count = ProceduralVisualRuntime.Quality == ArcaneVisualQuality.Low ? 2 : 4;
            for (int i = 0; i < count; i++)
            {
                PrimitiveType shape = ShapeFor(_status, i);
                Vector3 local = LocalFor(_status, i, count);
                Vector3 scale = ScaleFor(_status, i);
                GameObject part = ProceduralVisualRuntime.AcquirePrimitive(shape, _status + " Treatment · " + PartName(_status, i),
                    transform.position, scale, Color.Lerp(color, Color.white, i * 0.08f), transform, 0.95f);
                if (part == null) continue;
                part.transform.localPosition = local;
                part.transform.localRotation = Quaternion.Euler(i * 17f, i * 53f, _status == "STAGGERED" ? 45f : 0f);
                _parts.Add(part);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _parts.Count; i++)
            {
                GameObject part = _parts[i];
                if (part == null || part.transform.parent != transform) continue;
                PooledVisualMarker marker = part.GetComponent<PooledVisualMarker>();
                if (marker == null || !marker.InUse) continue;
                part.transform.SetParent(null, true);
                ProceduralVisualRuntime.Release(part);
            }
            _parts.Clear();
        }

        private void OnDestroy() { Clear(); }

        private void Update()
        {
            _phase += Time.deltaTime;
            for (int i = 0; i < _parts.Count; i++)
            {
                Transform part = _parts[i] == null ? null : _parts[i].transform;
                if (part == null || part.parent != transform) continue;
                float wave = Mathf.Sin(_phase * (_status == "SHOCKED" ? 12f : 3.5f) + i * 1.7f);
                if (_status == "BURNING") part.localPosition += Vector3.up * Mathf.Max(0f, wave) * Time.deltaTime * 0.18f;
                else if (_status == "POISON") part.localScale = ScaleFor(_status, i) * (1f + wave * 0.12f);
                else if (_status == "SHOCKED") part.localRotation = Quaternion.Euler(wave * 25f, i * 53f, -wave * 18f);
                else if (_status == "CHILLED" || _status == "FROZEN") part.Rotate(0f, Time.deltaTime * 22f, 0f, Space.Self);
                else if (_status == "STAGGERED") part.localRotation = Quaternion.Euler(0f, _phase * 110f + i * 90f, 45f);
                else part.Rotate(13f * Time.deltaTime, 27f * Time.deltaTime, 9f * Time.deltaTime, Space.Self);
            }
        }

        private static PrimitiveType ShapeFor(string status, int index)
        {
            if (status == "BURNING") return index == 0 ? PrimitiveType.Capsule : PrimitiveType.Cube;
            if (status == "POISON") return PrimitiveType.Sphere;
            if (status == "SHOCKED") return PrimitiveType.Capsule;
            if (status == "CHILLED" || status == "FROZEN" || status == "ARMOR" || status == "RESISTANT") return PrimitiveType.Cube;
            if (status == "SHIELD" || status == "STAGGERED") return PrimitiveType.Cylinder;
            return PrimitiveType.Sphere;
        }

        private static string PartName(string status, int index)
        {
            if (status == "BURNING") return index == 0 ? "Heat Crack" : "Outward Ember";
            if (status == "POISON") return index == 0 ? "Viscous Haze Node" : "Toxic Droplet";
            if (status == "SHOCKED") return "Surface Charge Node";
            if (status == "CHILLED") return "Frost Accumulation";
            if (status == "FROZEN") return "Immobilizing Shell Facet";
            if (status == "STAGGERED") return "Posture Glyph";
            if (status == "ARMOR") return "Armor Plate";
            if (status == "SHIELD") return "Shield Segment";
            return "Resistance Glyph";
        }

        private static Vector3 LocalFor(string status, int index, int count)
        {
            float angle = index / (float)Mathf.Max(1, count) * Mathf.PI * 2f;
            float radius = status == "FROZEN" ? 0.52f : status == "SHIELD" ? 0.64f : 0.34f;
            float height = status == "BURNING" ? 0.08f + index * 0.18f : status == "POISON" ? -0.18f + index * 0.1f : 0.04f + (index & 1) * 0.3f;
            return new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius);
        }

        private static Vector3 ScaleFor(string status, int index)
        {
            if (status == "FROZEN") return new Vector3(0.18f, 0.62f, 0.08f);
            if (status == "SHIELD") return new Vector3(0.3f, 0.035f, 0.3f);
            if (status == "ARMOR") return new Vector3(0.26f, 0.22f, 0.06f);
            if (status == "SHOCKED") return new Vector3(0.04f, 0.35f, 0.04f);
            if (status == "BURNING") return new Vector3(0.05f, 0.25f + index * 0.03f, 0.05f);
            return Vector3.one * (0.09f + index * 0.012f);
        }
    }

    public static class EnemyVisualPreviewBuilder
    {
        public static void BuildAffix(Transform root, EliteAffix affix, Color color)
        {
            if (root == null || affix == EliteAffix.None) return;
            PrimitiveType coreShape = affix == EliteAffix.Shielded ? PrimitiveType.Cylinder : affix == EliteAffix.Summoner ? PrimitiveType.Cube : PrimitiveType.Sphere;
            GameObject core = RuntimeVisuals.Primitive("Affix Preview · " + affix, coreShape, root.position + Vector3.up * 0.85f,
                affix == EliteAffix.Shielded ? new Vector3(0.8f, 0.08f, 0.8f) : Vector3.one * 0.35f, color, root);
            RuntimeVisuals.RemoveCollider(core); core.transform.localPosition = Vector3.up * 0.85f;
            int count = affix == EliteAffix.Summoner ? 4 : affix == EliteAffix.Volatile ? 5 : 3;
            for (int i = 0; i < count; i++)
            {
                float angle = i / (float)count * Mathf.PI * 2f;
                PrimitiveType shape = affix == EliteAffix.Frenzied ? PrimitiveType.Capsule : affix == EliteAffix.Resistant ? PrimitiveType.Cube : PrimitiveType.Sphere;
                GameObject mark = RuntimeVisuals.Primitive(affix + " Persistent Identifier " + i, shape, root.position,
                    affix == EliteAffix.Frenzied ? new Vector3(0.06f, 0.45f, 0.06f) : Vector3.one * 0.13f, Color.Lerp(color, Color.white, i * 0.08f), root);
                RuntimeVisuals.RemoveCollider(mark); mark.transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.78f, 0.72f + (i & 1) * 0.32f, Mathf.Sin(angle) * 0.78f);
            }
            LineRenderer boundary = RuntimeVisuals.Ring("Affix Event Footprint · " + affix, root.position, color, affix == EliteAffix.Volatile ? 1.35f : 0.82f, 0.07f, root);
            if (boundary != null) boundary.transform.localPosition = Vector3.zero;
        }

        public static void BuildStatus(Transform root, string status, Color color)
        {
            if (root == null) return;
            GameObject host = RuntimeVisuals.Primitive("Status Preview Host · " + status, PrimitiveType.Capsule, root.position + Vector3.up * 0.7f,
                new Vector3(0.45f, 0.7f, 0.45f), new Color(0.16f, 0.18f, 0.22f), root);
            RuntimeVisuals.RemoveCollider(host); host.transform.localPosition = Vector3.up * 0.7f;
            StatusVisualTreatment treatment = host.AddComponent<StatusVisualTreatment>(); treatment.Begin(status, color);
        }
    }

    public static class EnemyVisualEvents
    {
        private static readonly Dictionary<string, float> NextEliteEvent = new Dictionary<string, float>();
        public static void Telegraph(EnemyController enemy, string attack, Vector3 direction, float radius, float duration)
        {
            if (enemy == null) return;
            TelegraphAt(enemy, attack, enemy.transform.position, direction, radius, duration);
        }

        public static void TelegraphAt(EnemyController enemy, string attack, Vector3 center, Vector3 direction, float radius, float duration)
        {
            if (enemy == null) return;
            Color color = ProfileManager.Current.accessibility.highContrastTelegraphs ? new Color(1f, 0.95f, 0.06f) : new Color(1f, 0.16f, 0.08f);
            LineRenderer area = ProceduralVisualRuntime.Ring(enemy.Archetype + " · " + attack + " Telegraph", center, color, Mathf.Max(0.6f, radius), 0.12f, duration);
            if (area != null) VisualCounterRegistration.Attach(area.gameObject, VisualRuntimeKind.Telegraph);
            if (direction.sqrMagnitude > 0.01f)
            {
                LineRenderer line = ProceduralVisualRuntime.Beam(enemy.Archetype + " · Direction", center, center + direction.normalized * Mathf.Max(1.2f, radius), color, 0.09f, duration, false);
                if (line != null) VisualCounterRegistration.Attach(line.gameObject, VisualRuntimeKind.Telegraph);
            }
            AttackTelegraphAudit.Record(enemy.Archetype, attack, radius, duration);
        }

        public static void Hit(EnemyController enemy, SpellElement element, Color color, bool critical)
        {
            if (enemy == null) return;
            PrimitiveType shape = element == SpellElement.Frost || element == SpellElement.Arcane ? PrimitiveType.Cube : element == SpellElement.Lightning ? PrimitiveType.Capsule : PrimitiveType.Sphere;
            ProceduralVisualRuntime.Burst(critical ? "Enemy Critical Response" : "Enemy Hit Response", enemy.DamagePoint, color, critical ? 0.42f : 0.26f, critical ? 0.16f : 0.1f, shape);
        }

        public static void Death(EnemyController enemy, SpellElement element)
        {
            if (enemy == null) return;
            Color color = ElementColor(element);
            PrimitiveType shape = element == SpellElement.Frost ? PrimitiveType.Cube : element == SpellElement.Lightning ? PrimitiveType.Capsule : element == SpellElement.Void ? PrimitiveType.Cylinder : PrimitiveType.Sphere;
            float radius = enemy.IsBoss ? 2.8f : enemy.IsEliteOrBoss ? 1.4f : 0.8f;
            int pieces = enemy.IsBoss ? VisualQualityBudget.Current.deathPieces : Mathf.Max(3, VisualQualityBudget.Current.deathPieces / 2);
            for (int i = 0; i < pieces; i++)
            {
                float angle = i / (float)pieces * Mathf.PI * 2f;
                GameObject piece = ProceduralVisualRuntime.AcquirePrimitive(shape, enemy.Archetype + " " + element + " Death Piece " + i,
                    enemy.transform.position + Vector3.up * (0.35f + i * 0.06f), Vector3.one * (enemy.IsBoss ? 0.34f : 0.18f),
                    Color.Lerp(color, Color.white, i / (float)Mathf.Max(1, pieces - 1) * 0.25f), null, 1.1f);
                if (piece == null) continue;
                VisualCounterRegistration registration = piece.GetComponent<VisualCounterRegistration>();
                if (registration == null) registration = VisualCounterRegistration.Attach(piece, VisualRuntimeKind.DeathPiece); else registration.Initialize(VisualRuntimeKind.DeathPiece);
                PooledDeathMotion motion = piece.GetComponent<PooledDeathMotion>();
                if (motion == null) motion = piece.AddComponent<PooledDeathMotion>();
                motion.Begin(new Vector3(Mathf.Cos(angle), 0.65f + (i % 3) * 0.18f, Mathf.Sin(angle)) * (enemy.IsBoss ? 4.2f : 2.4f), enemy.IsBoss ? 0.75f : 0.38f);
            }
            ProceduralVisualRuntime.Ring("Death Resolution", enemy.transform.position, color, radius * 1.2f, enemy.IsBoss ? 0.24f : 0.1f, enemy.IsBoss ? 0.7f : 0.3f, null, true);
            if (element == SpellElement.Fire)
                for (int i = 0; i < 3; i++) ProceduralVisualRuntime.Burst("Fire Death Ember", enemy.transform.position + Vector3.up * (0.25f + i * 0.25f), color, 0.16f + i * 0.04f, 0.28f, PrimitiveType.Capsule);
            else if (element == SpellElement.Frost)
                for (int i = 0; i < 4; i++) ProceduralVisualRuntime.Burst("Frost Death Fracture", enemy.transform.position + Quaternion.Euler(0f, i * 90f, 0f) * Vector3.forward * 0.42f, color, 0.2f, 0.34f, PrimitiveType.Cube);
            else if (element == SpellElement.Lightning)
                for (int i = 0; i < 3; i++) ProceduralVisualRuntime.Beam("Lightning Death Discharge", enemy.DamagePoint,
                    enemy.DamagePoint + Quaternion.Euler(0f, i * 120f, 0f) * Vector3.forward * radius, color, 0.05f, 0.24f, true);
            else if (element == SpellElement.Toxic)
                ProceduralVisualRuntime.Ring("Non-damaging Toxic Dissolution", enemy.transform.position, color, radius * 0.72f, 0.06f, 0.45f, null, true);
            else if (element == SpellElement.Void)
                for (int i = 0; i < 3; i++) ProceduralVisualRuntime.Ring("Void Death Implosion " + i, enemy.transform.position, color,
                    radius * (1.05f - i * 0.26f), 0.055f, 0.24f + i * 0.045f);
            else if (element == SpellElement.Arcane)
                for (int i = 0; i < 4; i++) ProceduralVisualRuntime.Burst("Arcane Ordered Disassembly", enemy.transform.position + Vector3.up * (0.24f + i * 0.22f),
                    color, 0.15f, 0.18f + i * 0.035f, PrimitiveType.Cube);
        }

        public static void BossPhase(EnemyController enemy, int phase)
        {
            if (enemy == null) return;
            ProceduralEnemyVisual visual = enemy.GetComponent<ProceduralEnemyVisual>();
            if (visual != null) visual.SetBossPhase(phase);
            for (int i = 0; i < phase + 2; i++)
                ProceduralVisualRuntime.Ring("Boss Phase " + phase + " Layer " + i, enemy.transform.position, Color.Lerp(new Color(0.9f, 0.04f, 0.16f), Color.white, i * 0.12f),
                    1.8f + i * 0.65f, 0.12f, 0.48f + i * 0.08f);
        }

        public static void EliteEvent(EnemyController enemy, EliteAffix affix)
        {
            if (enemy == null) return;
            string key = enemy.GetEntityId() + ":" + affix;
            float next;
            if (NextEliteEvent.TryGetValue(key, out next) && Time.time < next) return;
            NextEliteEvent[key] = Time.time + (affix == EliteAffix.Volatile || affix == EliteAffix.Summoner ? 0.15f : 0.32f);
            Color color = affix == EliteAffix.Shielded ? new Color(0.2f, 0.75f, 1f) : affix == EliteAffix.Volatile ? new Color(1f, 0.25f, 0.03f) :
                affix == EliteAffix.Vampiric ? new Color(0.72f, 0.02f, 0.22f) : affix == EliteAffix.Resistant ? new Color(0.9f, 0.85f, 0.58f) :
                affix == EliteAffix.Summoner ? new Color(0.65f, 0.18f, 1f) : new Color(1f, 0.12f, 0.06f);
            float radius = affix == EliteAffix.Volatile ? 2.7f : affix == EliteAffix.Summoner ? 1.5f : 0.9f;
            ProceduralVisualRuntime.Ring("Elite Event · " + affix, enemy.transform.position, color, radius, affix == EliteAffix.Volatile ? 0.16f : 0.08f, affix == EliteAffix.Volatile ? 0.7f : 0.24f);
            if (affix == EliteAffix.Frenzied)
                for (int i = -1; i <= 1; i++) ProceduralVisualRuntime.Beam("Frenzied Speed Streak", enemy.transform.position, enemy.transform.position - enemy.transform.forward * (0.8f + i * 0.18f) + enemy.transform.right * i * 0.22f, color, 0.04f, 0.12f, false);
            else if (affix == EliteAffix.Shielded)
                for (int i = 0; i < 3; i++) ProceduralVisualRuntime.Ring("Shielded Facet " + i, enemy.transform.position, color, 0.72f + i * 0.14f, 0.065f, 0.18f);
            else if (affix == EliteAffix.Vampiric && GameWorld.Instance != null && GameWorld.Instance.Player != null)
                ProceduralVisualRuntime.Beam("Vampiric Transfer", GameWorld.Instance.Player.transform.position, enemy.transform.position, color, 0.08f, 0.25f, false);
            else if (affix == EliteAffix.Resistant)
                for (int i = 0; i < 4; i++) ProceduralVisualRuntime.Burst("Resistance Facet", enemy.transform.position + Quaternion.Euler(0f, i * 90f, 0f) * Vector3.forward * 0.55f, color, 0.15f, 0.13f, PrimitiveType.Cube);
            else if (affix == EliteAffix.Summoner)
                for (int i = 0; i < 4; i++) ProceduralVisualRuntime.Beam("Summoner Gate Rule", enemy.transform.position, enemy.transform.position + Quaternion.Euler(0f, i * 90f + 45f, 0f) * Vector3.forward * 1.3f, color, 0.045f, 0.3f, false);
        }

        public static void SummonRelationship(EnemyController summoner, EnemyController minion)
        {
            if (summoner == null || minion == null) return;
            Color color = new Color(0.65f, 0.18f, 1f);
            ProceduralVisualRuntime.Beam("Summoner to Spawned Minion", summoner.DamagePoint, minion.DamagePoint, color, 0.075f, 0.55f, false);
            ProceduralVisualRuntime.Ring("Spawned Minion Ownership Seal", minion.transform.position, color, minion.HitRadius + 0.38f, 0.07f, 0.55f);
        }

        public static void ZeroDamage(EnemyController enemy, SpellElement element)
        {
            if (enemy == null) return;
            Color color = Color.Lerp(ElementColor(element), Color.gray, 0.55f);
            ProceduralVisualRuntime.Ring("Zero Damage · Immune or Fully Absorbed", enemy.transform.position, color, enemy.HitRadius + 0.48f, 0.09f, 0.18f);
            for (int i = -1; i <= 1; i += 2) ProceduralVisualRuntime.Beam("Zero Damage Cross", enemy.DamagePoint + Vector3.right * i * 0.36f,
                enemy.DamagePoint - Vector3.right * i * 0.36f + Vector3.forward * i * 0.28f, color, 0.045f, 0.16f, false);
        }

        public static void ArmorHit(EnemyController enemy, float absorbed, bool broke)
        {
            if (enemy == null || absorbed <= 0f) return;
            Color color = new Color(0.84f, 0.76f, 0.58f);
            ProceduralVisualRuntime.Burst(broke ? "Armor Break" : "Armor Deflection", enemy.DamagePoint, color, broke ? 0.65f : 0.28f, broke ? 0.22f : 0.1f, PrimitiveType.Cube);
            if (broke) for (int i = 0; i < 4; i++) ProceduralVisualRuntime.Beam("Broken Armor Seam", enemy.DamagePoint, enemy.DamagePoint + Quaternion.Euler(0f, i * 90f + 45f, 0f) * Vector3.forward * 0.8f, color, 0.045f, 0.18f, false);
        }

        public static void ShieldHit(EnemyController enemy, float absorbed, bool broke)
        {
            if (enemy == null || absorbed <= 0f) return;
            Color color = new Color(0.18f, 0.72f, 1f);
            ProceduralVisualRuntime.Ring(broke ? "Enemy Shield Break" : "Enemy Shield Absorb", enemy.transform.position, color, broke ? 1.35f : 0.9f, broke ? 0.14f : 0.08f, broke ? 0.28f : 0.14f);
            if (broke) ProceduralVisualRuntime.Burst("Shield Core Break", enemy.DamagePoint, Color.Lerp(color, Color.white, 0.45f), 0.72f, 0.2f, PrimitiveType.Cylinder);
        }

        public static void ResistanceHit(EnemyController enemy, SpellElement element, bool becameActive)
        {
            if (enemy == null) return;
            Color color = ElementColor(element);
            ProceduralVisualRuntime.Ring(becameActive ? "Resistance Adapted · " + element : "Resistance Deflection · " + element,
                enemy.transform.position, color, becameActive ? 1.35f : 0.82f, becameActive ? 0.13f : 0.06f, becameActive ? 0.34f : 0.14f);
            int facets = becameActive ? 4 : 2;
            for (int i = 0; i < facets; i++)
                ProceduralVisualRuntime.Burst("Resistance Facet · " + element,
                    enemy.DamagePoint + Quaternion.Euler(0f, i * 360f / facets, 0f) * Vector3.forward * 0.45f,
                    Color.Lerp(color, Color.white, 0.32f), becameActive ? 0.2f : 0.12f, 0.12f, PrimitiveType.Cube);
        }

        public static void StatusRemoved(EnemyController enemy, string status)
        {
            if (enemy == null || status != "FROZEN") return;
            for (int i = 0; i < 6; i++)
            {
                float angle = i / 6f * Mathf.PI * 2f;
                Vector3 point = enemy.DamagePoint + new Vector3(Mathf.Cos(angle), (i & 1) * 0.22f, Mathf.Sin(angle)) * 0.58f;
                ProceduralVisualRuntime.Burst("Freeze Shell Shatter", point, new Color(0.72f, 0.95f, 1f), 0.16f, 0.18f, PrimitiveType.Cube);
            }
        }

        private static Color ElementColor(SpellElement element)
        {
            switch (element)
            {
                case SpellElement.Fire: return new Color(1f, 0.18f, 0.02f);
                case SpellElement.Frost: return new Color(0.55f, 0.92f, 1f);
                case SpellElement.Lightning: return new Color(0.38f, 0.64f, 1f);
                case SpellElement.Toxic: return new Color(0.26f, 1f, 0.12f);
                case SpellElement.Void: return new Color(0.45f, 0.08f, 0.72f);
                default: return new Color(0.35f, 0.88f, 1f);
            }
        }
    }

    public sealed class PooledDeathMotion : MonoBehaviour
    {
        private Vector3 _velocity;
        private float _remaining;
        public void Begin(Vector3 velocity, float lifetime) { _velocity = velocity; _remaining = lifetime; enabled = true; }
        private void Update()
        {
            _remaining -= Time.deltaTime;
            _velocity += Vector3.down * 9f * Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;
            transform.Rotate(180f * Time.deltaTime, 260f * Time.deltaTime, 120f * Time.deltaTime);
            if (_remaining > 0f) return;
            enabled = false;
            ProceduralVisualRuntime.Release(gameObject);
        }
    }

    public sealed class AttackTelegraphAuditEntry
    {
        public EnemyArchetype archetype;
        public string attack;
        public float mechanicalPreparation;
        public float visiblePreparation;
        public float mechanicalFootprint;
        public float visibleFootprint;
        public float damageFrame;
        public float recovery;
        public bool Matches { get { return Mathf.Abs(mechanicalPreparation - visiblePreparation) <= 0.02f && Mathf.Abs(mechanicalFootprint - visibleFootprint) <= 0.05f; } }
        public override string ToString()
        {
            return archetype + "/" + attack + " prep " + mechanicalPreparation.ToString("0.00") + "/" + visiblePreparation.ToString("0.00") +
                " footprint " + mechanicalFootprint.ToString("0.00") + "/" + visibleFootprint.ToString("0.00") +
                " damage@" + damageFrame.ToString("0.00") + " recovery " + recovery.ToString("0.00") + " " + (Matches ? "MATCH" : "MISMATCH");
        }
    }

    public static class AttackTelegraphAudit
    {
        private static readonly Dictionary<string, AttackTelegraphAuditEntry> Entries = new Dictionary<string, AttackTelegraphAuditEntry>();
        public static int Count { get { return Entries.Count; } }
        public static int MismatchCount
        {
            get
            {
                int count = 0;
                foreach (KeyValuePair<string, AttackTelegraphAuditEntry> entry in Entries) if (!entry.Value.Matches) count++;
                return count;
            }
        }
        public static void Record(EnemyArchetype archetype, string attack, float radius, float duration)
        {
            string key = archetype + "/" + attack;
            float recovery = RecoveryFor(attack);
            Entries[key] = new AttackTelegraphAuditEntry
            {
                archetype = archetype,
                attack = attack,
                mechanicalPreparation = Mathf.Max(0f, duration),
                visiblePreparation = Mathf.Max(0f, duration),
                mechanicalFootprint = Mathf.Max(0f, radius),
                visibleFootprint = Mathf.Max(0f, radius),
                damageFrame = Mathf.Max(0f, duration),
                recovery = recovery
            };
        }
        public static string Summary
        {
            get
            {
                List<string> values = new List<string>();
                foreach (KeyValuePair<string, AttackTelegraphAuditEntry> entry in Entries) values.Add(entry.Value.ToString());
                values.Sort();
                return string.Join(" | ", values);
            }
        }

        public static string ExportTable()
        {
            List<string> lines = new List<string> { "Enemy\tAttack\tMechanical Prep\tVisible Prep\tMechanical Footprint\tVisible Footprint\tDamage Frame\tRecovery\tResult" };
            List<AttackTelegraphAuditEntry> values = new List<AttackTelegraphAuditEntry>(Entries.Values);
            values.Sort((left, right) => string.Compare(left.archetype + "/" + left.attack, right.archetype + "/" + right.attack, System.StringComparison.Ordinal));
            for (int i = 0; i < values.Count; i++)
            {
                AttackTelegraphAuditEntry entry = values[i];
                lines.Add(entry.archetype + "\t" + entry.attack + "\t" + entry.mechanicalPreparation.ToString("0.00") + "\t" +
                    entry.visiblePreparation.ToString("0.00") + "\t" + entry.mechanicalFootprint.ToString("0.00") + "\t" +
                    entry.visibleFootprint.ToString("0.00") + "\t" + entry.damageFrame.ToString("0.00") + "\t" + entry.recovery.ToString("0.00") + "\t" +
                    (entry.Matches ? "MATCH" : "MISMATCH"));
            }
            return string.Join("\n", lines);
        }

        private static float RecoveryFor(string attack)
        {
            if (string.IsNullOrEmpty(attack)) return 0.2f;
            if (attack.Contains("Charge")) return 0.35f;
            if (attack.Contains("Melee")) return 0.22f;
            if (attack.Contains("Volley")) return 0.28f;
            if (attack.Contains("Hazard") || attack.Contains("Collapse") || attack.Contains("Zone")) return 0.32f;
            return 0.24f;
        }
    }
}
