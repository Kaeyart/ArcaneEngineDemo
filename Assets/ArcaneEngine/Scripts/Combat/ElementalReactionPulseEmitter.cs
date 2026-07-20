using UnityEngine;

namespace ArcaneEngine
{
    public enum ReactionPulseMode
    {
        Normal,
        Pull,
        Push,
        Thermal
    }

    public sealed class ElementalReactionPulseEmitter : MonoBehaviour
    {
        private EnemyController _source;
        private ReactionElement _signature;
        private ReactionElement _element;
        private ReactionPulseMode _mode;
        private ReactionContext22 _context;
        private float _damage;
        private float _startRadius;
        private float _endRadius;
        private int _pulseCount;
        private float _interval;
        private float _nextPulse;
        private int _pulseIndex;

        public static ElementalReactionPulseEmitter Spawn(
            Vector3 position,
            EnemyController source,
            ReactionElement signature,
            ReactionElement element,
            float damage,
            float startRadius,
            float endRadius,
            int pulseCount,
            float interval,
            float delay,
            ReactionPulseMode mode)
        {
            return Spawn(
                position, source, signature, element, damage, startRadius,
                endRadius, pulseCount, interval, delay, mode,
                ReactionContext22.Legacy(true));
        }

        public static ElementalReactionPulseEmitter Spawn(
            Vector3 position,
            EnemyController source,
            ReactionElement signature,
            ReactionElement element,
            float damage,
            float startRadius,
            float endRadius,
            int pulseCount,
            float interval,
            float delay,
            ReactionPulseMode mode,
            ReactionContext22 context)
        {
            GameObject gameObject = new GameObject(
                "Reaction Pulses " +
                ElementalReactionCodex.SignatureText(signature));

            gameObject.transform.position = position;

            ElementalReactionPulseEmitter emitter =
                gameObject.AddComponent<ElementalReactionPulseEmitter>();

            emitter._source = source;
            emitter._signature = signature;
            emitter._element = element;
            emitter._mode = mode;
            emitter._context = context.IsValid
                ? context
                : ReactionContext22.Legacy(true);
            emitter._damage = Mathf.Max(0f, damage);
            emitter._startRadius = Mathf.Max(0.25f, startRadius);
            emitter._endRadius = Mathf.Max(emitter._startRadius, endRadius);
            emitter._pulseCount = Mathf.Clamp(pulseCount, 1, 5);
            emitter._interval = Mathf.Clamp(interval, 0.18f, 1.2f);
            emitter._nextPulse = Time.time + Mathf.Max(0f, delay);
            return emitter;
        }

        private void Update()
        {
            if (Time.time < _nextPulse)
                return;

            Pulse();
            _pulseIndex++;

            if (_pulseIndex >= _pulseCount)
            {
                Destroy(gameObject);
                return;
            }

            _nextPulse = Time.time + _interval;
        }

        private void Pulse()
        {
            float t =
                _pulseCount <= 1
                    ? 1f
                    : _pulseIndex / (float)(_pulseCount - 1);

            float radius = Mathf.Lerp(_startRadius, _endRadius, t);
            ReactionElement element = _element;
            ReactionElement payload = _signature;

            if (_mode == ReactionPulseMode.Thermal)
            {
                bool fire = (_pulseIndex & 1) == 0;
                element = fire ? ReactionElement.Fire : ReactionElement.Cold;
                payload = element;
            }

            ReactionContext22 pulseContext = _pulseIndex == 0
                ? _context.AsSource(ReactionSourceKind22.Echo)
                : _context.Derive(ReactionSourceKind22.Echo);

            Color color = ElementalReactionCodex.BlendColor(payload);
            ReactionPresentation22.TryBurst(
                transform.position + Vector3.up * 0.12f,
                color,
                Mathf.Clamp(0.35f + radius * 0.08f, 0.38f, 1.05f),
                pulseContext,
                false);

            ElementalReactionMechanicExecutor.DamageArea(
                transform.position,
                _source,
                radius,
                _damage * Mathf.Lerp(1f, 0.7f, t),
                element,
                payload,
                0.32f,
                false,
                pulseContext,
                6);

            if (_mode == ReactionPulseMode.Pull)
            {
                ElementalReactionMechanicExecutor.PullArea(
                    transform.position,
                    _source,
                    radius,
                    5.5f + radius,
                    pulseContext,
                    4);
            }
            else if (_mode == ReactionPulseMode.Push)
            {
                ElementalReactionMechanicExecutor.PushArea(
                    transform.position,
                    _source,
                    radius,
                    16f + radius * 2f,
                    pulseContext,
                    4);
            }
            else if (_mode == ReactionPulseMode.Thermal &&
                     element == ReactionElement.Cold)
            {
                ElementalReactionMechanicExecutor.FreezeArea(
                    transform.position,
                    _source,
                    radius,
                    0.35f + t * 0.4f,
                    pulseContext,
                    4);
            }
        }
    }
}
