using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ElementalReactionOrbiters : MonoBehaviour
    {
        private readonly List<Transform> _nodes = new List<Transform>();
        private EnemyController _source;
        private ReactionElement _signature;
        private ReactionElement _element;
        private ReactionContext22 _context;
        private float _damage;
        private float _radius;
        private float _expiresAt;
        private float _nextAttack;
        private float _angleOffset;
        private int _attackIndex;

        public static ElementalReactionOrbiters Spawn(
            Vector3 position,
            EnemyController source,
            ReactionElement signature,
            ReactionElement element,
            float damage,
            float radius,
            float duration,
            int count)
        {
            return Spawn(
                position, source, signature, element, damage, radius, duration,
                count, ReactionContext22.Legacy(true));
        }

        public static ElementalReactionOrbiters Spawn(
            Vector3 position,
            EnemyController source,
            ReactionElement signature,
            ReactionElement element,
            float damage,
            float radius,
            float duration,
            int count,
            ReactionContext22 context)
        {
            GameObject root = new GameObject(
                "Reaction Orbiters " +
                ElementalReactionCodex.SignatureText(signature));

            root.transform.position = position + Vector3.up * 0.8f;

            ElementalReactionOrbiters orbiters =
                root.AddComponent<ElementalReactionOrbiters>();

            orbiters._source = source;
            orbiters._signature = signature;
            orbiters._element = element;
            orbiters._context = context.IsValid
                ? context
                : ReactionContext22.Legacy(true);
            orbiters._damage = Mathf.Max(0f, damage);
            orbiters._radius = Mathf.Clamp(radius * 0.45f, 1.2f, 4.5f);
            orbiters._expiresAt = Time.time + Mathf.Clamp(duration, 1f, 8f);
            orbiters._nextAttack = Time.time + 0.55f;
            orbiters.CreateNodes(Mathf.Clamp(count, 2, 6));
            return orbiters;
        }

        private void CreateNodes(int count)
        {
            Color color = ElementalReactionCodex.BlendColor(_signature);

            for (int i = 0; i < count; i++)
            {
                GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                node.name = "Reaction Conductor";
                node.transform.SetParent(transform, false);
                node.transform.localScale = Vector3.one * 0.18f;

                Collider collider = node.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);

                Renderer renderer = node.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = RuntimeVisuals.Material(color, 0.74f);

                _nodes.Add(node.transform);
            }
        }

        private void Update()
        {
            _angleOffset += Time.deltaTime * 1.55f;

            for (int i = 0; i < _nodes.Count; i++)
            {
                float angle =
                    _angleOffset + Mathf.PI * 2f * i / _nodes.Count;

                _nodes[i].localPosition = new Vector3(
                    Mathf.Cos(angle) * _radius,
                    Mathf.Sin(angle * 2f) * 0.22f,
                    Mathf.Sin(angle) * _radius);
            }

            if (Time.time >= _nextAttack)
            {
                _nextAttack = Time.time + 0.75f;
                Attack();
            }

            if (Time.time >= _expiresAt)
                Destroy(gameObject);
        }

        private void Attack()
        {
            if (_nodes.Count == 0)
                return;

            int nodeIndex = Mathf.Abs(_attackIndex) % _nodes.Count;
            _attackIndex++;
            Vector3 start = _nodes[nodeIndex].position;

            ReactionContext22 attackContext =
                _context.Derive(ReactionSourceKind22.Echo);

            ElementalReactionMechanicExecutor.Chain(
                start,
                _source,
                _radius + 4f,
                Mathf.Min(ReactionBalance22.MaximumChainTargets,
                    Mathf.Max(1, _nodes.Count / 2)),
                _damage * 0.32f,
                _element,
                _signature,
                attackContext);
        }
    }
}
