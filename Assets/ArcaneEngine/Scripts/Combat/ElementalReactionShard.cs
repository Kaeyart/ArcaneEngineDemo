using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ElementalReactionShard : MonoBehaviour
    {
        private EnemyController _source;
        private ReactionElement _signature;
        private ReactionElement _element;
        private ReactionContext22 _context;
        private Vector3 _velocity;
        private float _damage;
        private float _expiresAt;
        private float _hitRadius;

        public static void SpawnNova(
            Vector3 position,
            EnemyController source,
            ReactionElement signature,
            ReactionElement element,
            float damage,
            float radius,
            float duration,
            int count)
        {
            SpawnNova(
                position, source, signature, element, damage, radius, duration,
                count, ReactionContext22.Legacy(true));
        }

        public static void SpawnNova(
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
            int shardCount = Mathf.Clamp(count, 3, 14);
            float life = Mathf.Clamp(duration, 0.5f, 3f);
            float speed = Mathf.Max(3f, radius / life);
            Color color = ElementalReactionCodex.BlendColor(signature);
            ReactionContext22 shardContext =
                context.Derive(ReactionSourceKind22.Echo);

            for (int i = 0; i < shardCount; i++)
            {
                float angle = Mathf.PI * 2f * i / shardCount;
                Vector3 direction = new Vector3(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle * 2f) * 0.08f,
                    Mathf.Sin(angle)).normalized;

                GameObject gameObject =
                    GameObject.CreatePrimitive(PrimitiveType.Cube);

                gameObject.name = "Reaction Shard";
                gameObject.transform.position = position;
                gameObject.transform.localScale =
                    new Vector3(0.08f, 0.08f, 0.28f);
                gameObject.transform.rotation =
                    Quaternion.LookRotation(direction, Vector3.up);

                Collider collider = gameObject.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);

                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = RuntimeVisuals.Material(color, 0.68f);

                ElementalReactionShard shard =
                    gameObject.AddComponent<ElementalReactionShard>();

                shard._source = source;
                shard._signature = signature;
                shard._element = element;
                shard._context = shardContext;
                shard._velocity = direction * speed;
                shard._damage = Mathf.Max(0f, damage);
                shard._expiresAt = Time.time + life;
                shard._hitRadius = 0.38f;
            }
        }

        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;
            transform.Rotate(0f, 0f, 420f * Time.deltaTime, Space.Self);

            EnemyController target = FindTarget();

            if (target != null)
            {
                if (ReactionLineageRegistry22.TryMarkTarget(_context, target, 0.20f))
                {
                    ElementalReactionRuntime.DealReactionDamage(
                        target,
                        _damage,
                        _element,
                        ElementalReactionCodex.BlendColor(_signature),
                        false,
                        _context);

                    ElementalReactionMechanicExecutor.ApplyPayload(
                        target,
                        _signature,
                        0.45f,
                        4f,
                        _context);

                    ReactionPresentation22.TryBurst(
                        transform.position,
                        ElementalReactionCodex.BlendColor(_signature),
                        0.28f,
                        _context,
                        false);
                }

                Destroy(gameObject);
                return;
            }

            if (Time.time >= _expiresAt)
                Destroy(gameObject);
        }

        private EnemyController FindTarget()
        {
            foreach (
                EnemyController enemy
                in ElementalReactionRuntime.EnemiesWithin(
                    transform.position,
                    _hitRadius,
                    _source))
            {
                return enemy;
            }

            return null;
        }
    }
}
