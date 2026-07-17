using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ArcaneEngine.Tests
{
    public sealed class ArcaneEngineEditModeTests
    {
        [SetUp]
        public void PrepareCatalogs()
        {
            ProfileManager.Load(0);
            DemoCatalog.Ensure();
            MegaCatalog.Ensure();
        }

        [Test]
        public void EveryBaseSpellCompiles()
        {
            EquipmentInventory equipment = new EquipmentInventory();
            PlayerStats stats = new PlayerStats { spellPower = 1f, manaCostMultiplier = 1f, cooldownMultiplier = 1f };
            foreach (SpellCoreDefinition core in DemoCatalog.AllCores)
            {
                CompiledSpell spell = SpellCompiler.Compile(new SpellBoard(SpellSlot.Slot1, core.id), stats, equipment);
                Assert.That(spell, Is.Not.Null, core.id);
                Assert.That(spell.cooldown, Is.GreaterThan(0f), core.id);
            }
        }

        [Test]
        public void EveryStandardCoreHasThreeDistinctLegendarySignatures()
        {
            foreach (SpellCoreDefinition core in DemoCatalog.AllCores.Where(value => value.id != "void_maw"))
            {
                RelicDefinition[] paths = MegaCatalog.RelicsForCore(core.id).ToArray();
                Assert.That(paths.Length, Is.EqualTo(3), core.id);
                Assert.That(paths.Select(value => value.signature).Distinct().Count(), Is.EqualTo(3), core.id);
            }
        }

        [Test]
        public void HexRotationAndCapacityAreStable()
        {
            HexCoord original = new HexCoord(2, -1);
            HexCoord rotated = original;
            for (int i = 0; i < 6; i++) rotated = HexCoord.Rotate(rotated, 1);
            Assert.That(rotated, Is.EqualTo(original));
            SpellBoard board = new SpellBoard(SpellSlot.Slot1, "fireball");
            Assert.That(board.Capacity, Is.EqualTo(6));
            board.spellLevel = 5;
            Assert.That(board.Capacity, Is.EqualTo(15));
        }

        [Test]
        public void CyclicLinksAreDetectedAndBudgetIsFinite()
        {
            SpellLinkSave[] graph =
            {
                new SpellLinkSave { sourceSlot = 0, destinationSlot = 1 },
                new SpellLinkSave { sourceSlot = 1, destinationSlot = 2 },
                new SpellLinkSave { sourceSlot = 2, destinationSlot = 0 }
            };
            Assert.That(SpellLinkRules.HasCycle(graph), Is.True);
            SpellCastBudget budget = new SpellCastBudget(10000f);
            TriggerSpec trigger = new TriggerSpec { sourceId = "cycle_test", energyCost = 1f,
                maxActivationsPerEvent = BalanceTuning.MaximumTriggerActivations, internalCooldown = 0f };
            int accepted = 0;
            while (budget.TrySpend(trigger) && accepted < 1000) accepted++;
            Assert.That(accepted, Is.LessThanOrEqualTo(BalanceTuning.MaximumTriggerActivations));
        }

        [Test]
        public void PersistentAuthoringCatalogIsValidAfterGeneration()
        {
            Assert.That(V21AuthoredContentOverlay.ValidatePersistentAssets(), Is.Empty);
        }
    }
}
