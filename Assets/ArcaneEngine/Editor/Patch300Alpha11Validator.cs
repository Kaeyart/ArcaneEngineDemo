#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    public static class Patch300Alpha11Validator
    {
        [MenuItem("Arcane Engine/3.0/Validate Foundation Hardening 3.0.0-alpha.1.1")]
        public static void Validate()
        {
            List<string> failures = new List<string>();
            try
            {
                ArpgContent30.Ensure();
                string contentResult = ArpgContent30.Validate();
                if (!string.IsNullOrEmpty(contentResult)) failures.Add(contentResult);

                if (ArpgContent30.Classes.Count != 3) failures.Add("Expected exactly three base classes.");
                if (ArpgContent30.Ascendancies.Count != 9) failures.Add("Expected exactly nine Ascendancies.");
                if (ArpgContent30.Maps.Count != 80) failures.Add("Expected exactly eighty standard map definitions.");

                int hashA = ArpgDeterminism30.Combine("item", "currency", 7);
                int hashB = ArpgDeterminism30.Combine("item", "currency", 7);
                if (hashA != hashB || hashA < 0) failures.Add("Stable deterministic hashing failed.");
                if (ArpgDeterminism30.Index(int.MinValue, 7) < 0) failures.Add("Positive index handling failed for int.MinValue.");

                ArpgProfile30 repairProbe = new ArpgProfile30
                {
                    dataVersion = 30001,
                    currencies = new List<ArpgCurrencyStack30>
                    {
                        null,
                        new ArpgCurrencyStack30 { currency = ArpgCurrency30.NullOrb, amount = 2 },
                        new ArpgCurrencyStack30 { currency = ArpgCurrency30.NullOrb, amount = 3 }
                    }
                };
                ArpgProfileStore30.Repair(repairProbe);
                if (repairProbe.dataVersion != 30002) failures.Add("Profile migration did not advance to data version 30002.");
                if (repairProbe.Currency(ArpgCurrency30.NullOrb) != 5) failures.Add("Duplicate currency stacks were not merged.");

                ArpgItem30 fractureProbe = new ArpgItem30
                {
                    instanceId = "validator-fracture",
                    baseId = ArpgContent30.ItemBases.First().id,
                    displayName = "Validator Item",
                    slot = ArpgContent30.ItemBases.First().slot,
                    rarity = ArpgItemRarity30.Rare,
                    itemLevel = 80,
                    fractured = true,
                    fracturedAffixId = "validator-affix",
                    prefixes = new List<ArpgAffixRoll30>
                    {
                        new ArpgAffixRoll30 { affixId = "validator-affix", tier = 1, value = 1f }
                    }
                };
                ArpgProfile30 fractureProfile = new ArpgProfile30();
                fractureProfile.items.Add(fractureProbe);
                ArpgProfileStore30.Repair(fractureProfile);
                if (!fractureProbe.fractured || fractureProbe.fracturedAffixId != "validator-affix") failures.Add("Fractured-affix migration failed.");
            }
            catch (Exception exception)
            {
                failures.Add(exception.ToString());
            }

            if (failures.Count > 0)
            {
                foreach (string failure in failures) Debug.LogError("[3.0.0-alpha.1.1] " + failure);
                throw new InvalidOperationException("Arcane Engine 3.0.0-alpha.1.1 validation failed with " + failures.Count + " issue(s).");
            }
            Debug.Log("Arcane Engine 3.0.0-alpha.1.1 foundation hardening validation passed.");
        }
    }
}
#endif
