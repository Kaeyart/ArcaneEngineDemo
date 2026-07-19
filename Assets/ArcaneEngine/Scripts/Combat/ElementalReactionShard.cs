using UnityEngine;

namespace ArcaneEngine
{
    public sealed class ElementalReactionShard : MonoBehaviour
    {
        private EnemyController _source;
        private ReactionElement _signature;
        private ReactionElement _element;
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
            int shardCount = Mathf.Clamp(count, 3, 36);
            float life = Mathf.Clamp(duration, 0.5f, 4f);
            float speed = Mathf.Max(3f, radius / life);
            Color color = ElementalReactionCodex.BlendColor(signature);

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
                    new Vector3(0.1f, 0.1f, 0.38f);
                gameObject.transform.rotation =
                    Quaternion.LookRotation(direction, Vector3.up);

                Collider collider = gameObject.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);

                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = RuntimeVisuals.Material(color, 0.78f);

                ElementalReactionShard shard =
                    gameObject.AddComponent<ElementalReactionShard>();

                shard._source = source;
                shard._signature = signature;
                shard._element = element;
                shard._velocity = direction * speed;
                shard._damage = damage;
                shard._expiresAt = Time.time + life;
                shard._hitRadius = 0.42f;
            }
        }

        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;
            transform.Rotate(0f, 0f, 420f * Time.deltaTime, Space.Self);

            EnemyController target = FindTarget();

            if (target != null)
            {
                ElementalReactionRuntime.DealReactionDamage(
                    target,
                    _damage,
                    _element,
                    ElementalReactionCodex.BlendColor(_signature),
                    false);

                ElementalReactionMechanicExecutor.ApplyPayload(
                    target,
                    _signature,
                    0.65f,
                    4f,
                    true);

                GameFeelSystem.Burst(
                    transform.position,
                    ElementalReactionCodex.BlendColor(_signature),
                    0.38f);

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
