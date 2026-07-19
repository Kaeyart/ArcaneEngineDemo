using UnityEngine;

namespace ArcaneEngine
{
    public enum V21MinibossFamily
    {
        BoneColossus, GraveCantor,
        FurnaceKnight, MoltenOracle,
        DrownedCurator, PrismAbbot,
        PlagueBearer, BroodGuardian
    }

    /// <summary>Mechanics layered onto the two named elites in every miniboss room.</summary>
    public sealed class V21MinibossMechanics : MonoBehaviour
    {
        public V21MinibossFamily Family { get; private set; }
        public string Counterplay { get; private set; }
        private EnemyController _owner;
        private DifficultySettings _difficulty;
        private float _cooldown = 1.5f;

        public void Initialize(EnemyController owner, string biome, int encounterIndex, DifficultySettings difficulty)
        {
            _owner = owner;
            _difficulty = difficulty;
            int pair = string.IsNullOrEmpty(biome) ? 0 : biome.IndexOf("Ember", System.StringComparison.OrdinalIgnoreCase) >= 0 ? 1 :
                biome.IndexOf("Archive", System.StringComparison.OrdinalIgnoreCase) >= 0 ? 2 :
                biome.IndexOf("Venom", System.StringComparison.OrdinalIgnoreCase) >= 0 ? 3 : 0;
            Family = (V21MinibossFamily)(pair * 2 + Mathf.Abs(encounterIndex) % 2);
            Counterplay = IsSummoner ? "Destroy its support enemies before they control the arena." :
                IsReflector ? "Pause projectile fire during the reflection pulse and change angle." :
                "Read the marked safe gaps, then attack during recovery.";
            gameObject.name = Family + " Miniboss";
        }

        private bool IsSummoner { get { return Family == V21MinibossFamily.GraveCantor || Family == V21MinibossFamily.BroodGuardian; } }
        private bool IsReflector { get { return Family == V21MinibossFamily.PrismAbbot; } }

        private void Update()
        {
            if (_owner == null || _owner.IsDead || GameWorld.Instance == null || !GameWorld.Instance.RunActive || GameWorld.Instance.ModalOpen) return;
            _cooldown -= Time.deltaTime;
            if (_cooldown > 0f) return;
            _cooldown = IsSummoner ? 6.5f : IsReflector ? 4f : 4.8f;
            PlayerController player = GameWorld.Instance.Player;
            if (player == null) return;
            if (IsSummoner)
            {
                EnemyVisualEvents.TelegraphAt(_owner, Family + " Summon", transform.position, Vector3.zero, 3f, 0.7f);
                Invoke(nameof(SummonSupport), 0.7f);
            }
            else if (IsReflector)
            {
                EnemyVisualEvents.TelegraphAt(_owner, "Prism Reflection", transform.position, Vector3.zero, 3.2f, 0.55f);
                Invoke(nameof(ReflectProjectiles), 0.55f);
            }
            else if (Family == V21MinibossFamily.BoneColossus || Family == V21MinibossFamily.DrownedCurator)
            {
                Vector3 direction = player.transform.position - transform.position; direction.y = 0f;
                EnemyVisualEvents.TelegraphAt(_owner, Family + " Fan", transform.position, direction, 10f, 0.65f);
                StartCoroutine(FanVolley(direction.normalized, 0.65f));
            }
            else
            {
                Vector3 target = player.transform.position;
                EnemyVisualEvents.TelegraphAt(_owner, Family + " Arena Mark", target, Vector3.zero, 2.7f, 0.72f);
                StartCoroutine(ArenaHazard(target, 0.72f));
            }
        }

        private void SummonSupport()
        {
            if (_owner == null || _owner.IsDead || GameWorld.Instance.Enemies.Count >= 16) return;
            EnemyArchetype archetype = Family == V21MinibossFamily.GraveCantor ? EnemyArchetype.Leech : EnemyArchetype.Assassin;
            EnemyController.Spawn(transform.position + transform.right * 2.5f, archetype, 7, _difficulty, false, false);
        }

        private void ReflectProjectiles()
        {
            if (_owner == null || _owner.IsDead || GameWorld.Instance.Player == null) return;
            SpellProjectile.InterceptNear(transform.position, 4f, GameWorld.Instance.Player.transform.position, 15f);
        }

        private System.Collections.IEnumerator FanVolley(Vector3 direction, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_owner == null || _owner.IsDead) yield break;
            Color color = Family == V21MinibossFamily.BoneColossus ? new Color(0.85f, 0.72f, 0.55f) : new Color(0.15f, 0.75f, 1f);
            for (int i = 0; i < 5; i++)
                EnemyBolt.Create(transform.position + Vector3.up, Quaternion.Euler(0f, -28f + i * 14f, 0f) * direction, 14f, color);
        }

        private System.Collections.IEnumerator ArenaHazard(Vector3 target, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_owner == null || _owner.IsDead) yield break;
            Color color = Family == V21MinibossFamily.FurnaceKnight || Family == V21MinibossFamily.MoltenOracle
                ? new Color(1f, 0.25f, 0.04f) : new Color(0.3f, 1f, 0.12f);
            PersistentEnemyHazard.Create(target, 2.7f, 15f, color, 0f);
        }
    }
}
