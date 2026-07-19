#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ArcaneEngine.Editor
{
    public static class V21AcceptanceValidation
    {
        [MenuItem("Arcane Engine/2.1/Validate Promise Completion Source")]
        public static void ValidateFromMenu()
        {
            int failures = Validate();
            if (failures > 0) throw new BuildFailedException("Arcane Engine 2.1 validation found " + failures + " failure(s). See Console and V21_VALIDATION_REPORT.txt.");
        }

        public static int Validate()
        {
            V21ContentBuilder.RebuildAll();
            DemoCatalog.Ensure();
            MegaCatalog.Ensure();
            List<string> messages = new List<string>();
            int failures = 0;
            foreach (string failure in V21AuthoredContentOverlay.ValidatePersistentAssets()) Fail(failure, messages, ref failures);

            foreach (SpellCoreDefinition core in DemoCatalog.AllCores.Where(value => value != null && value.id != "void_maw"))
            {
                RelicDefinition[] paths = MegaCatalog.RelicsForCore(core.id).ToArray();
                if (paths.Length != 3 || paths.Select(value => value.signature).Distinct().Count() != 3)
                    Fail(core.id + " does not have exactly three distinct Legendary signatures.", messages, ref failures);
            }

            V21RoomLayoutAsset[] rooms = Resources.LoadAll<V21RoomLayoutAsset>("V21Content/Rooms");
            string[] biomes = { "The Ossuary Catacombs", "Ember Foundry", "Sunken Archive", "Venom Cistern" };
            foreach (string biome in biomes)
            {
                if (rooms.Count(value => value != null && value.biome == biome && value.roomType == DungeonRoomType.Combat) < 6)
                    Fail(biome + " has fewer than six authored combat layouts.", messages, ref failures);
                if (rooms.Count(value => value != null && value.biome == biome &&
                    (value.roomType == DungeonRoomType.Elite || value.roomType == DungeonRoomType.Challenge)) < 2)
                    Fail(biome + " has fewer than two objective-focused layouts.", messages, ref failures);
            }
            HashSet<string> traversalLayouts = new HashSet<string>();
            RoomTemplate sampleRoom = MegaCatalog.AllRooms.FirstOrDefault(value => value.type == DungeonRoomType.Combat);
            for (int seed = 1; seed <= 50 && sampleRoom != null; seed++)
            {
                V21RoomLayoutAsset layout = V21RoomLayoutRuntime.Select(sampleRoom, seed * 7919);
                if (layout != null) traversalLayouts.Add(layout.stableId);
            }
            if (traversalLayouts.Count < 4) Fail("Seeded 50-room layout selection did not produce at least four distinct authored layouts.", messages, ref failures);

            string[] audioEvents = { "spell_cast", "spell_travel", "spell_impact", "status_apply", "status_consume", "spell_trigger", "summon",
                "defensive", "enemy_telegraph", "enemy_attack", "enemy_hurt", "enemy_death", "reward_reveal", "door", "shop_buy", "ui_confirm",
                "ui_cancel", "boss_phase", "music_explore", "music_combat", "music_elite", "music_boss", "music_reward", "music_home" };
            Dictionary<string, V21AudioEventAsset> audio = Resources.LoadAll<V21AudioEventAsset>("V21Content/Audio")
                .Where(value => value != null && !string.IsNullOrEmpty(value.stableId)).GroupBy(value => value.stableId)
                .ToDictionary(group => group.Key, group => group.First());
            foreach (string id in audioEvents)
                if (!audio.ContainsKey(id) || audio[id].clips == null || audio[id].clips.Length == 0 || audio[id].clips.Any(clip => clip == null))
                    Fail("Audio event " + id + " has no authored clip variant.", messages, ref failures);

            if (Enum.GetValues(typeof(V21MinibossFamily)).Length < 8) Fail("The miniboss roster has fewer than two families per biome.", messages, ref failures);
            if (Enum.GetValues(typeof(SpellLinkCondition)).Length < 14) Fail("Spell Link vocabulary is incomplete.", messages, ref failures);
            failures += RuntimeValidation.ValidateCombatAndContent();

            string heading = "Arcane Engine 2.1 source acceptance\nUTC " + DateTime.UtcNow.ToString("o") + "\nUnity " + Application.unityVersion +
                "\nResult " + (failures == 0 ? "PASS" : "FAIL") + "\nFailures " + failures + "\n\n";
            File.WriteAllText(Path.GetFullPath("V21_VALIDATION_REPORT.txt"), heading + (messages.Count == 0 ? "No source/content failures. Play Mode and hardware acceptance remain separate.\n" : string.Join("\n", messages)));
            if (failures == 0) Debug.Log("Arcane Engine 2.1 source/content validation passed. Unity Play Mode, controller hardware, audio listening, visual review and profiler acceptance remain required.");
            return failures;
        }

        private static void Fail(string message, List<string> messages, ref int failures)
        {
            failures++;
            messages.Add("FAIL · " + message);
            Debug.LogError("Arcane Engine 2.1 validation: " + message);
        }
    }
}
#endif
