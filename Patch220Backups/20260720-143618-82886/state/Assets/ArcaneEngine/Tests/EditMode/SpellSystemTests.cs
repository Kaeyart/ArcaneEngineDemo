using NUnit.Framework;

namespace ArcaneEngine.Tests
{
    public sealed class SpellSystemTests
    {
        [SetUp]
        public void Prepare()
        {
            ProfileManager.Load(0);
            DemoCatalog.Ensure();
        }

        [Test]
        public void Compile_WithValidCore_ProducesSpell()
        {
            SpellBoard board = new SpellBoard(SpellSlot.Slot1, "fireball");
            EquipmentInventory equipment = new EquipmentInventory();
            PlayerStats stats = new PlayerStats
            {
                spellPower = 1f,
                manaCostMultiplier = 1f,
                cooldownMultiplier = 1f
            };
            CompiledSpell spell = SpellCompiler.Compile(board, stats, equipment);
            Assert.That(spell, Is.Not.Null);
            Assert.That(spell.cooldown, Is.GreaterThan(0f));
            Assert.That(spell.manaCost, Is.GreaterThanOrEqualTo(0f));
            Assert.That(spell.damage, Is.GreaterThanOrEqualTo(0f));
        }

        [Test]
        public void Compile_SpellPowerMultiplier_IncreasesDamage()
        {
            SpellBoard board = new SpellBoard(SpellSlot.Slot1, "fireball");
            EquipmentInventory equipment = new EquipmentInventory();
            PlayerStats lowPower = new PlayerStats
            {
                spellPower = 0.5f,
                manaCostMultiplier = 1f,
                cooldownMultiplier = 1f
            };
            PlayerStats highPower = new PlayerStats
            {
                spellPower = 2f,
                manaCostMultiplier = 1f,
                cooldownMultiplier = 1f
            };
            CompiledSpell low = SpellCompiler.Compile(board, lowPower, equipment);
            CompiledSpell high = SpellCompiler.Compile(board, highPower, equipment);
            Assert.That(high.damage, Is.GreaterThan(low.damage));
        }

        [Test]
        public void Compile_ManaCostMultiplier_IncreasesCost()
        {
            SpellBoard board = new SpellBoard(SpellSlot.Slot1, "fireball");
            EquipmentInventory equipment = new EquipmentInventory();
            PlayerStats cheap = new PlayerStats
            {
                spellPower = 1f,
                manaCostMultiplier = 0.5f,
                cooldownMultiplier = 1f
            };
            PlayerStats expensive = new PlayerStats
            {
                spellPower = 1f,
                manaCostMultiplier = 2f,
                cooldownMultiplier = 1f
            };
            CompiledSpell low = SpellCompiler.Compile(board, cheap, equipment);
            CompiledSpell high = SpellCompiler.Compile(board, expensive, equipment);
            Assert.That(high.manaCost, Is.GreaterThanOrEqualTo(low.manaCost));
        }

        [Test]
        public void Compile_CooldownMultiplier_IncreasesCooldown()
        {
            SpellBoard board = new SpellBoard(SpellSlot.Slot1, "fireball");
            EquipmentInventory equipment = new EquipmentInventory();
            PlayerStats fast = new PlayerStats
            {
                spellPower = 1f,
                manaCostMultiplier = 1f,
                cooldownMultiplier = 0.5f
            };
            PlayerStats slow = new PlayerStats
            {
                spellPower = 1f,
                manaCostMultiplier = 1f,
                cooldownMultiplier = 2f
            };
            CompiledSpell quick = SpellCompiler.Compile(board, fast, equipment);
            CompiledSpell sluggish = SpellCompiler.Compile(board, slow, equipment);
            Assert.That(sluggish.cooldown, Is.GreaterThanOrEqualTo(quick.cooldown));
        }

        [Test]
        public void HexBoard_Capacity_GrowsWithLevel()
        {
            SpellBoard board = new SpellBoard(SpellSlot.Slot1, "fireball");
            int baseCapacity = board.Capacity;
            board.spellLevel = 10;
            Assert.That(board.Capacity, Is.GreaterThan(baseCapacity));
        }

        [Test]
        public void SpellLinkRules_EmptyGraph_HasNoCycles()
        {
            SpellLinkSave[] empty = System.Array.Empty<SpellLinkSave>();
            Assert.That(SpellLinkRules.HasCycle(empty), Is.False);
        }

        [Test]
        public void SpellLinkRules_LinearChain_HasNoCycles()
        {
            SpellLinkSave[] linear =
            {
                new SpellLinkSave { sourceSlot = 0, destinationSlot = 1 },
                new SpellLinkSave { sourceSlot = 1, destinationSlot = 2 }
            };
            Assert.That(SpellLinkRules.HasCycle(linear), Is.False);
        }

        [Test]
        public void SpellCastBudget_FiniteEnergy_EventuallyExhausts()
        {
            SpellCastBudget budget = new SpellCastBudget(5f);
            TriggerSpec trigger = new TriggerSpec
            {
                sourceId = "test",
                energyCost = 2f,
                maxActivationsPerEvent = BalanceTuning.MaximumTriggerActivations,
                internalCooldown = 0f
            };
            int count = 0;
            while (budget.TrySpend(trigger) && count < 1000) count++;
            Assert.That(count, Is.EqualTo(2)); // 5 / 2 = 2 spend, 1 remaining
        }

        [Test]
        public void SpellCastBudget_UnlimitedEnergy_AllowsAll()
        {
            SpellCastBudget budget = new SpellCastBudget(float.MaxValue);
            TriggerSpec trigger = new TriggerSpec
            {
                sourceId = "test",
                energyCost = 0.01f,
                maxActivationsPerEvent = BalanceTuning.MaximumTriggerActivations,
                internalCooldown = 0f
            };
            int count = 0;
            while (budget.TrySpend(trigger) && count < 1000) count++;
            Assert.That(count, Is.EqualTo(BalanceTuning.MaximumTriggerActivations));
        }
    }
}