using System;
using System.Collections.Generic;
using System.Linq;

namespace ArcaneEngine
{
    /// <summary>
    /// Produces a stable signature from the saved inputs used by procedural
    /// room, item and spell presentation. It intentionally excludes transient
    /// object IDs, frame state and cached materials.
    /// </summary>
    public static class VisualContinuationValidation
    {
        private const string Schema = "arcane-visual-reconstruction-v2";

        public static string Compute(RunSnapshotData snapshot)
        {
            if (snapshot == null) return string.Empty;
            uint hash = 2166136261u;
            Add(ref hash, Schema);
            Add(ref hash, snapshot.runSeed);
            Add(ref hash, snapshot.roomIndex);
            Add(ref hash, snapshot.totalRooms);
            Add(ref hash, snapshot.roomId);

            IEnumerable<RunSpellSnapshot> spells = (snapshot.spells ?? new List<RunSpellSnapshot>())
                .Where(value => value != null).OrderBy(value => value.slotIndex);
            foreach (RunSpellSnapshot spell in spells)
            {
                Add(ref hash, spell.slotIndex); Add(ref hash, spell.baseSpellId); Add(ref hash, spell.legendarySpellId);
                Add(ref hash, spell.spellLevel); Add(ref hash, spell.boardRadiusBonus);
                IEnumerable<PlacedModifierSave> modifiers = (spell.modifiers ?? new List<PlacedModifierSave>()).Where(value => value != null)
                    .OrderBy(value => value.q).ThenBy(value => value.r).ThenBy(value => value.modifierId).ThenBy(value => value.rotation);
                foreach (PlacedModifierSave modifier in modifiers)
                {
                    Add(ref hash, modifier.modifierId); Add(ref hash, modifier.q); Add(ref hash, modifier.r); Add(ref hash, modifier.rotation);
                }
            }

            IEnumerable<SpellLinkSave> links = (snapshot.spellLinks ?? new List<SpellLinkSave>()).Where(value => value != null)
                .OrderBy(value => value.sourceSlot).ThenBy(value => value.destinationSlot).ThenBy(value => (int)value.condition);
            foreach (SpellLinkSave link in links)
            {
                Add(ref hash, link.sourceSlot); Add(ref hash, link.destinationSlot); Add(ref hash, (int)link.condition);
            }

            IEnumerable<RunEquippedItemSnapshot> equipment = (snapshot.equippedItems ?? new List<RunEquippedItemSnapshot>()).Where(value => value != null && value.item != null)
                .OrderBy(value => (int)value.slot);
            foreach (RunEquippedItemSnapshot equipped in equipment)
            {
                Add(ref hash, (int)equipped.slot);
                AddItem(ref hash, equipped.item);
            }
            return hash.ToString("X8");
        }

        public static string CaptureRuntime(GameWorld world, RunDirector run)
        {
            if (world == null || run == null || run.CurrentRoom == null) return string.Empty;
            return CaptureRuntime(world, run.CurrentSeed, run.RoomIndex, run.TotalRooms, run.CurrentRoom.id);
        }

        public static string CaptureRuntime(GameWorld world, int runSeed, int roomIndex, int totalRooms, string roomId)
        {
            if (world == null || string.IsNullOrEmpty(roomId)) return string.Empty;
            RunSnapshotData reconstructed = new RunSnapshotData
            {
                runSeed = runSeed,
                roomIndex = roomIndex,
                totalRooms = totalRooms,
                roomId = roomId
            };
            world.CaptureRunState(reconstructed);
            return Compute(reconstructed);
        }

        public static bool MatchesSavedInputs(RunSnapshotData snapshot, out string expected, out string actual)
        {
            expected = snapshot == null ? string.Empty : snapshot.visualReconstructionSignature;
            actual = Compute(snapshot);
            return string.IsNullOrEmpty(expected) || string.Equals(expected, actual, StringComparison.Ordinal);
        }

        private static void AddItem(ref uint hash, ItemSaveData item)
        {
            Add(ref hash, item.definitionId); Add(ref hash, item.itemLevel); Add(ref hash, item.upgradeLevel);
            Add(ref hash, (int)item.rarity); Add(ref hash, item.quality); Add(ref hash, item.generationSeed);
            Add(ref hash, item.corrupted ? 1 : 0); Add(ref hash, item.corruptionId); Add(ref hash, item.craftedAffixId);
            IEnumerable<AffixRoll> affixes = (item.affixes ?? new List<AffixRoll>()).Where(value => value != null)
                .OrderBy(value => value.id).ThenBy(value => value.tier).ThenBy(value => value.value);
            foreach (AffixRoll affix in affixes)
            {
                Add(ref hash, affix.id); Add(ref hash, affix.tier); Add(ref hash, (int)affix.kind);
                Add(ref hash, BitConverter.ToInt32(BitConverter.GetBytes(affix.value), 0));
            }
        }

        private static void Add(ref uint hash, int value)
        {
            unchecked
            {
                hash = (hash ^ (byte)value) * 16777619u;
                hash = (hash ^ (byte)(value >> 8)) * 16777619u;
                hash = (hash ^ (byte)(value >> 16)) * 16777619u;
                hash = (hash ^ (byte)(value >> 24)) * 16777619u;
            }
        }

        private static void Add(ref uint hash, string value)
        {
            string text = value ?? string.Empty;
            unchecked
            {
                for (int i = 0; i < text.Length; i++)
                {
                    char character = text[i];
                    hash = (hash ^ (byte)character) * 16777619u;
                    hash = (hash ^ (byte)(character >> 8)) * 16777619u;
                }
                hash = (hash ^ 255u) * 16777619u;
            }
        }
    }
}
