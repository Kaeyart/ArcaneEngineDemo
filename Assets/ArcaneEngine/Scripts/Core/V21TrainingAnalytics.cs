using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum V21TrainingTargetProfile { Normal, Armored, Shielded, Resistant, Vulnerable, Mobile, BossSized, StatusImmune }

    [Serializable]
    public sealed class V21TrainingSnapshot
    {
        public float elapsed;
        public float totalDamage;
        public float directDamage;
        public float triggeredDamage;
        public int hits;
        public int criticalHits;
        public int statusesApplied;
        public int triggerCasts;
        public int kills;
        public float manaSpent;
        public float dps;
        public float criticalRate;
        public float damagePerMana;
        [NonSerialized] public Dictionary<string, float> damageBySpell = new Dictionary<string, float>();
    }

    public sealed class V21TrainingAnalytics : MonoBehaviour
    {
        public static V21TrainingAnalytics Instance { get; private set; }
        public V21TrainingSnapshot Snapshot { get; private set; } = new V21TrainingSnapshot();
        public V21TrainingTargetProfile TargetProfile { get; private set; }
        private float _windowStarted;
        private float _lastMana;

        private void Awake()
        {
            Instance = this;
            V12CombatEventBus.Published += OnCombatEvent;
            ResetWindow();
        }

        private void OnDestroy()
        {
            V12CombatEventBus.Published -= OnCombatEvent;
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (GameWorld.Instance == null || !GameWorld.Instance.TrainingMode) return;
            Snapshot.elapsed = Mathf.Max(0f, Time.time - _windowStarted);
            Snapshot.dps = Snapshot.totalDamage / Mathf.Max(0.1f, Snapshot.elapsed);
            Snapshot.criticalRate = Snapshot.hits <= 0 ? 0f : Snapshot.criticalHits / (float)Snapshot.hits;
            Snapshot.damagePerMana = Snapshot.totalDamage / Mathf.Max(1f, Snapshot.manaSpent);
            PlayerController player = GameWorld.Instance.Player;
            if (player != null)
            {
                float current = player.Mana;
                if (current < _lastMana) Snapshot.manaSpent += _lastMana - current;
                _lastMana = current;
            }
        }

        public void ResetWindow()
        {
            Snapshot = new V21TrainingSnapshot();
            _windowStarted = Time.time;
            _lastMana = GameWorld.Instance == null || GameWorld.Instance.Player == null ? 0f : GameWorld.Instance.Player.Mana;
        }

        public void SetTargetProfile(V21TrainingTargetProfile profile)
        {
            TargetProfile = profile;
            foreach (EnemyController enemy in FindObjectsByType<EnemyController>())
                if (enemy != null && enemy.Archetype == EnemyArchetype.TrainingDummy) enemy.ConfigureTrainingTarget(profile);
            ResetWindow();
        }

        public string Report()
        {
            return "WINDOW " + Snapshot.elapsed.ToString("0.0") + "s · DPS " + Snapshot.dps.ToString("0.0") +
                " · TOTAL " + Snapshot.totalDamage.ToString("0") + " · HITS " + Snapshot.hits +
                " · CRIT " + (Snapshot.criticalRate * 100f).ToString("0.0") + "% · STATUS " + Snapshot.statusesApplied +
                " · TRIGGERS " + Snapshot.triggerCasts + " · DAMAGE/MANA " + Snapshot.damagePerMana.ToString("0.0");
        }

        private void OnCombatEvent(V12CombatEvent record)
        {
            if (GameWorld.Instance == null || !GameWorld.Instance.TrainingMode) return;
            if (record.type == V12CombatEventType.Hit || record.type == V12CombatEventType.CriticalHit)
            {
                Snapshot.hits++;
                Snapshot.totalDamage += Mathf.Max(0f, record.amount);
                if (record.generation > 0) Snapshot.triggeredDamage += Mathf.Max(0f, record.amount);
                else Snapshot.directDamage += Mathf.Max(0f, record.amount);
                if (record.type == V12CombatEventType.CriticalHit) Snapshot.criticalHits++;
                string spell = string.IsNullOrEmpty(record.sourceId) ? "Unknown" : record.sourceId;
                if (!Snapshot.damageBySpell.ContainsKey(spell)) Snapshot.damageBySpell[spell] = 0f;
                Snapshot.damageBySpell[spell] += Mathf.Max(0f, record.amount);
            }
            else if (record.type == V12CombatEventType.StatusApplied) Snapshot.statusesApplied++;
            else if (record.type == V12CombatEventType.TriggeredCast) Snapshot.triggerCasts++;
            else if (record.type == V12CombatEventType.EnemyKilled) Snapshot.kills++;
        }
    }
}
