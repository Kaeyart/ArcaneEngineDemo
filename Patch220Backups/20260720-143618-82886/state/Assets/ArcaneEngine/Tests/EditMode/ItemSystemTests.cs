using NUnit.Framework;

namespace ArcaneEngine.Tests
{
    public sealed class ItemSystemTests
    {
        [SetUp]
        public void Prepare()
        {
            ProfileManager.Load(0);
            DemoCatalog.Ensure();
            V11Itemization.Ensure();
        }

        [Test]
        public void AddItem_ReturnsItemWithCorrectRarity()
        {
            EquipmentInventory inventory = new EquipmentInventory();
            ItemInstance item = inventory.Add("item_helm_common_01", 1, banked: true);
            Assert.That(item, Is.Not.Null);
            Assert.That(item.Definition, Is.Not.Null);
            Assert.That(item.rarity, Is.EqualTo(ItemRarity.Common));
        }

        [Test]
        public void AddItem_HigherRarity_ReflectsOnItem()
        {
            EquipmentInventory inventory = new EquipmentInventory();
            ItemInstance rare = inventory.Add("item_chest_rare_01", 1, banked: true);
            Assert.That(rare.rarity, Is.EqualTo(ItemRarity.Rare));
        }

        [Test]
        public void Inventory_AddItem_IncreasesBackpackCount()
        {
            EquipmentInventory inventory = new EquipmentInventory();
            int before = inventory.backpack.Count;
            inventory.Add("item_helm_common_01", 1, banked: true);
            Assert.That(inventory.backpack.Count, Is.EqualTo(before + 1));
        }

        [Test]
        public void Inventory_BackpackRemoval_DecreasesCount()
        {
            EquipmentInventory inventory = new EquipmentInventory();
            ItemInstance item = inventory.Add("item_glove_magic_01", 1, banked: true);
            int before = inventory.backpack.Count;
            inventory.backpack.Remove(item);
            Assert.That(inventory.backpack.Count, Is.EqualTo(before - 1));
        }

        [Test]
        public void ItemInstance_HasDefinitionProperty()
        {
            EquipmentInventory inventory = new EquipmentInventory();
            ItemInstance item = inventory.Add("item_helm_common_01", 1, banked: true);
            Assert.That(item.Definition, Is.Not.Null);
            Assert.That(item.Definition.id, Is.EqualTo("item_helm_common_01"));
        }

        [Test]
        public void ItemRarity_EnumValues_AreInOrder()
        {
            Assert.That((int)ItemRarity.Common, Is.LessThan((int)ItemRarity.Magic));
            Assert.That((int)ItemRarity.Magic, Is.LessThan((int)ItemRarity.Rare));
            Assert.That((int)ItemRarity.Rare, Is.LessThan((int)ItemRarity.Unique));
        }

        [Test]
        public void AddItem_WithLevel_ProducesCorrectLevel()
        {
            EquipmentInventory inventory = new EquipmentInventory();
            ItemInstance item = inventory.Add("item_helm_common_01", 15, banked: true);
            Assert.That(item.itemLevel, Is.EqualTo(15));
        }

        [Test]
        public void EquipmentInventory_Backpack_StartsEmpty()
        {
            EquipmentInventory inventory = new EquipmentInventory();
            Assert.That(inventory.backpack, Is.Empty);
        }
    }
}