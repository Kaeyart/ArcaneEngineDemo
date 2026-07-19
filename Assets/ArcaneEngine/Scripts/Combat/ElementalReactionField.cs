using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ElementalReactionField : MonoBehaviour
    {
        private const int MaxFields = 40;

        private static readonly List<ElementalReactionField> Active =
            new List<ElementalReactionField>();

        private ReactionElement _signature;
        private float _radius;
        private float _expiresAt;
        private float _damagePerPulse;
        private float _buildupPerPulse;
        private float _nextPulse;
        private float _pulseInterval;
        private Renderer _renderer;
        private Vector3 _baseScale;

        public ReactionElement Signature
        {
            get { return _signature; }
        }

        public static ElementalReactionField Spawn(
            ReactionElement signature,
            Vector3 position,
            float radius,
            float duration,
            float damagePerPulse,
            float buildupPerPulse)
        {
            if (signature == ReactionElement.None)
                return null;

            CleanupList();

            foreach (ElementalReactionField field in Active)
            {
                if (field == null)
                    continue;

                float mergeDistance =
                    Mathf.Max(field._radius, radius) * 0.65f;

                Vector3 delta =
                    field.transform.position - position;

                delta.y = 0f;

                if (delta.sqrMagnitude <=
                    mergeDistance * mergeDistance)
                {
                    field.Merge(
                        signature,
                        radius,
                        duration,
                        damagePerPulse,
                        buildupPerPulse);

                    return field;
                }
            }

            while (Active.Count >= MaxFields)
            {
                ElementalReactionField oldest = Active[0];
                Active.RemoveAt(0);

                if (oldest != null)
                    Destroy(oldest.gameObject);
            }

            GameObject gameObject =
                GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            gameObject.name =
                "Elemental Field " +
                ElementalReactionCodex.SignatureText(signature);

            gameObject.transform.position =
                new Vector3(position.x, position.y + 0.04f, position.z);

            Collider collider = gameObject.GetComponent<Collider>();

            if (collider != null)
                Destroy(collider);

            ElementalReactionField result =
                gameObject.AddComponent<ElementalReactionField>();

            result.Initialize(
                signature,
                radius,
                duration,
                damagePerPulse,
                buildupPerPulse);

            Active.Add(result);
            return result;
        }


        public static void SurgeNear(
            Vector3 position,
            float radius,
            ReactionElement signature,
            float damageScale,
            float durationBonus)
        {
            CleanupList();
            float radiusSquared = radius * radius;
            bool surged = false;

            foreach (ElementalReactionField field in Active)
            {
                if (field == null)
                    continue;

                Vector3 delta = field.transform.position - position;
                delta.y = 0f;

                if (delta.sqrMagnitude > radiusSquared)
                    continue;

                field.Merge(
                    signature,
                    Mathf.Max(field._radius, radius * 0.45f),
                    Mathf.Max(0.5f, durationBonus),
                    field._damagePerPulse * Mathf.Max(0.15f, damageScale - 1f),
                    field._buildupPerPulse * 0.4f);

                field._damagePerPulse *= Mathf.Max(1f, damageScale);
                field._expiresAt += Mathf.Max(0f, durationBonus);
                field._nextPulse = Time.time;
                field.Pulse();
                surged = true;
            }

            if (!surged)
            {
                Spawn(
                    signature,
                    position,
                    Mathf.Max(1f, radius * 0.55f),
                    Mathf.Max(1f, durationBonus),
                    Mathf.Max(0.1f, damageScale),
                    0.3f);
            }
        }

        private void Initialize(
            ReactionElement signature,
            float radius,
            float duration,
            float damagePerPulse,
            float buildupPerPulse)
        {
            _signature = signature;
            _radius = Mathf.Max(0.6f, radius);
            _expiresAt = Time.time + Mathf.Max(0.4f, duration);
            _damagePerPulse = Mathf.Max(0f, damagePerPulse);
            _buildupPerPulse = Mathf.Max(0.05f, buildupPerPulse);

            ElementalReactionDefinition definition =
                ElementalReactionCodex.Get(signature);

            int pulses =
                definition == null
                    ? 5
                    : definition.pulseCount;

            _pulseInterval =
                Mathf.Clamp(
                    Mathf.Max(0.4f, duration) /
                    Mathf.Max(1, pulses),
                    0.28f,
                    0.85f);

            _nextPulse = Time.time + 0.08f;
            _renderer = GetComponent<Renderer>();

            if (_renderer != null)
            {
                _renderer.sharedMaterial =
                    RuntimeVisuals.Material(
                        ElementalReactionCodex.BlendColor(signature),
                        0.42f);
            }

            _baseScale =
                new Vector3(
                    _radius * 2f,
                    0.035f,
                    _radius * 2f);

            transform.localScale = _baseScale;

            ProceduralSpellPresentation.EmitField(
                SpellPresentationEventType.FieldCreated,
                gameObject,
                _signature,
                _radius,
                Mathf.Max(0.4f, duration),
                1f);
        }

        private void Merge(
            ReactionElement signature,
            float radius,
            float duration,
            float damagePerPulse,
            float buildupPerPulse)
        {
            ReactionElement combined = _signature | signature;

            _signature = combined;
            _radius =
                Mathf.Clamp(
                    Mathf.Max(_radius, radius) +
                    ElementalReactionCodex.CountBits(combined) * 0.08f,
                    0.6f,
                    8.5f);

            _expiresAt =
                Mathf.Max(
                    _expiresAt,
                    Time.time + duration);

            _damagePerPulse =
                Mathf.Max(
                    _damagePerPulse,
                    damagePerPulse) * 1.08f;

            _buildupPerPulse =
                Mathf.Max(
                    _buildupPerPulse,
                    buildupPerPulse);

            ElementalReactionDefinition definition =
                ElementalReactionCodex.Get(combined);

            if (definition != null)
            {
                _pulseInterval =
                    Mathf.Clamp(
                        definition.duration /
                        Mathf.Max(1, definition.pulseCount),
                        0.25f,
                        0.75f);
            }

            if (_renderer != null)
            {
                _renderer.sharedMaterial =
                    RuntimeVisuals.Material(
                        ElementalReactionCodex.BlendColor(combined),
                        0.46f);
            }

            _baseScale =
                new Vector3(
                    _radius * 2f,
                    0.035f,
                    _radius * 2f);

            GameFeelSystem.Burst(
                transform.position + Vector3.up * 0.08f,
                ElementalReactionCodex.BlendColor(combined),
                0.8f);

            ProceduralSpellPresentation.EmitField(
                SpellPresentationEventType.FieldMerged,
                gameObject,
                _signature,
                _radius,
                Mathf.Max(0.4f, duration),
                1.25f);
        }

        private void Update()
        {
            if (Time.time >= _expiresAt)
            {
                Destroy(gameObject);
                return;
            }

            float pulse =
                1f +
                Mathf.Sin(Time.time * 4.5f) * 0.045f;

            transform.localScale =
                new Vector3(
                    _baseScale.x * pulse,
                    _baseScale.y,
                    _baseScale.z * pulse);

            if (Time.time < _nextPulse)
                return;

            _nextPulse = Time.time + _pulseInterval;
            Pulse();
        }

        private void Pulse()
        {
            ProceduralSpellPresentation.EmitField(
                SpellPresentationEventType.FieldPulsed,
                gameObject,
                _signature,
                _radius,
                _pulseInterval,
                0.8f);

            List<EnemyController> enemies =
                ElementalReactionRuntime.EnemiesWithin(
                    transform.position,
                    _radius,
                    null);

            ReactionElement primary =
                ElementalReactionCodex.PrimaryElement(_signature);

            Color color =
                ElementalReactionCodex.BlendColor(_signature);

            foreach (EnemyController enemy in enemies)
            {
                float distance =
                    Vector3.Distance(
                        transform.position,
                        enemy.transform.position);

                float falloff =
                    Mathf.Clamp01(
                        1f - distance /
                        Mathf.Max(0.1f, _radius));

                ElementalReactionRuntime.DealReactionDamage(
                    enemy,
                    _damagePerPulse *
                    Mathf.Lerp(0.45f, 1f, falloff),
                    primary,
                    color,
                    false);

                int elementCount =
                    Mathf.Max(
                        1,
                        ElementalReactionCodex.CountBits(_signature));

                foreach (
                    ReactionElement element
                    in ElementalReactionCodex.Enumerate(_signature))
                {
                    ElementalReactionRuntime.ApplyBuildup(
                        enemy,
                        element,
                        _buildupPerPulse /
                        Mathf.Sqrt(elementCount),
                        3.5f,
                        true);
                }

                if (ElementalReactionCodex.Contains(
                    _signature,
                    ReactionElement.Void))
                {
                    enemy.PullToward(
                        transform.position,
                        4.5f + elementCount);
                }

                if (ElementalReactionCodex.Contains(
                    _signature,
                    ReactionElement.Physical))
                {
                    enemy.ApplyImpact(
                        enemy.transform.position -
                        transform.position,
                        8f + elementCount * 2f);
                }
            }
        }

        private void OnDestroy()
        {
            ProceduralSpellPresentation.EmitField(
                SpellPresentationEventType.FieldExpired,
                gameObject,
                _signature,
                _radius,
                0f,
                0f);

            Active.Remove(this);
        }

        private static void CleanupList()
        {
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                if (Active[i] == null)
                    Active.RemoveAt(i);
            }
        }
    }
}
