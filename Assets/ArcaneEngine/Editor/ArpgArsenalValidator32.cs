#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    public static class ArpgArsenalValidator32
    {
        [MenuItem("Arcane Engine/3.2/Validate Arsenal & Economy")]
        public static void Validate()
        {
            ArpgContent30.Ensure();
            ArpgArsenalContent32.Ensure();
            int errors = 0;
            errors += Check(ArpgArsenalContent32.ItemBases.Count >= 100, "100+ new equipment bases");
            errors += Check(ArpgArsenalContent32.ItemAffixes.Count >= 250, "250+ affix definitions");
            errors += Check(ArpgArsenalContent32.UniqueItems.Count >= 24, "24 Unique definitions");
            errors += Check(ArpgArsenalContent32.CurrencyDefinitions.Count >= 14, "14+ crafting currencies");
            errors += Check(ArpgArsenalContent32.MapAffixes.Count >= 36, "36 map modifiers");
            errors += Check(ArpgArsenalContent32.CorruptedImplicitDefinitions.Count >= 12, "12 corrupted implicits");
            errors += Check(ArpgContent30.ItemBases.Select(x => x.id).Distinct().Count() == ArpgContent30.ItemBases.Count, "unique base IDs");
            errors += Check(ArpgContent30.Affixes.Select(x => x.id).Distinct().Count() == ArpgContent30.Affixes.Count, "unique affix IDs");
            errors += Check(ArpgArsenalContent32.UniqueItems.All(x => ArpgContent30.ItemBase(x.baseId) != null), "Unique bases resolve");
            errors += Check(Enum.GetValues(typeof(ArpgCurrency32)).Length >= 14, "currency enum coverage");
            if (errors == 0) Debug.Log("[Arcane Engine 3.2] Arsenal & Economy validation PASS.");
            else Debug.LogError("[Arcane Engine 3.2] Validation failed with " + errors + " error(s).");
        }
        private static int Check(bool condition, string label)
        {
            if (condition) { Debug.Log("[3.2 PASS] " + label); return 0; }
            Debug.LogError("[3.2 FAIL] " + label); return 1;
        }
    }
}
#endif
