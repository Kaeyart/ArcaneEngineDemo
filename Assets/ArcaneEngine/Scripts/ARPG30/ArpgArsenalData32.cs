using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public enum ArpgStashTabKind32 { General, Currency, Maps, Cores, Runes, Relics, CraftingProjects, Recovery }
    public enum ArpgFilterAction32 { Show, Hide }

    [Serializable] public sealed class ArpgCraftRecord32 { public string operation; public string detail; public long unix; }
    [Serializable] public sealed class ArpgItemState32
    {
        public string itemInstanceId;
        public string ownerCharacterId;
        public string containerId;
        public int x, y, width=1, height=1;
        public bool favorite;
        public string uniqueId;
        public bool exceptional;
        public string elevatedAffixId;
        public List<string> sealedAffixIds = new List<string>();
        public string corruptedImplicitId;
        public bool prefixWard, suffixWard;
        public string omen;
        public List<ArpgCraftRecord32> craftHistory = new List<ArpgCraftRecord32>();
    }
    [Serializable] public sealed class ArpgStashTab32
    {
        public string id, displayName;
        public ArpgStashTabKind32 kind;
        public int order, columns, rows;
        public string search;
    }
    [Serializable] public sealed class ArpgCurrencyStack32 { public ArpgCurrency32 currency; public int amount; }
    [Serializable] public sealed class ArpgLootFilterRule32
    {
        public string id, displayName;
        public bool enabled=true;
        public ArpgFilterAction32 action=ArpgFilterAction32.Show;
        public ArpgItemRarity30 minimumRarity=ArpgItemRarity30.Normal;
        public int minimumItemLevel;
        public string itemClass, requiredTag;
        public bool uniquesOnly, exceptionalOnly, corruptedOnly;
        public int labelSize=14;
        public bool beam, sound;
    }
    [Serializable] public sealed class ArpgLootFilterPreset32
    {
        public string id, displayName;
        public bool active;
        public List<ArpgLootFilterRule32> rules = new List<ArpgLootFilterRule32>();
    }
    [Serializable] public sealed class ArpgVendorOffer32
    {
        public string offerId;
        public ArpgItem30 item;
        public int goldCost;
        public bool reserved;
    }
    [Serializable] public sealed class ArpgVendorState32
    {
        public int refreshRevision, mapsCompletedAtRefresh;
        public List<ArpgVendorOffer32> offers = new List<ArpgVendorOffer32>();
    }
    [Serializable] public sealed class ArpgCharacterArsenal32
    {
        public string characterId;
        public bool migrated;
        public string selectedStashTabId="stash.general.1";
        public string selectedItemId;
    }
    [Serializable] public sealed class ArpgArsenalSave32
    {
        public int schema=32000, revision, gold;
        public string lastMigrationUtc;
        public List<ArpgCharacterArsenal32> characters = new List<ArpgCharacterArsenal32>();
        public List<ArpgItemState32> itemStates = new List<ArpgItemState32>();
        public List<ArpgItem30> sharedItems = new List<ArpgItem30>();
        public List<ArpgMapItem30> sharedMaps = new List<ArpgMapItem30>();
        public List<string> sharedCoreIds = new List<string>();
        public List<string> sharedRuneIds = new List<string>();
        public List<int> sharedLinkConditionIds = new List<int>();
        public List<ArpgStashTab32> stashTabs = new List<ArpgStashTab32>();
        public List<ArpgCurrencyStack32> currencies = new List<ArpgCurrencyStack32>();
        public List<ArpgLootFilterPreset32> filters = new List<ArpgLootFilterPreset32>();
        public ArpgVendorState32 vendor = new ArpgVendorState32();
    }

    public static class ArpgArsenalStore32
    {
        public const int Schema=32000;
        private const string FileName="arcane_arsenal_320_account.json";
        private const string BackupName="arcane_arsenal_320_account.backup.json";
        private static ArpgArsenalSave32 _current;
        public static string SavePath { get { return Path.Combine(Application.persistentDataPath,FileName); } }
        public static string BackupPath { get { return Path.Combine(Application.persistentDataPath,BackupName); } }
        public static ArpgArsenalSave32 Current { get { if(_current==null)_current=LoadInternal(); Repair(_current); return _current; } }

        public static bool Save()
        {
            Repair(Current);CaptureCollections(ArpgFoundation30.Profile); Current.revision++;
            try
            {
                string dir=Path.GetDirectoryName(SavePath); if(!string.IsNullOrEmpty(dir))Directory.CreateDirectory(dir);
                string tmp=SavePath+".tmp"; File.WriteAllText(tmp,JsonUtility.ToJson(Current,true));
                if(File.Exists(SavePath))File.Copy(SavePath,BackupPath,true);
                if(File.Exists(SavePath))File.Delete(SavePath);
                File.Move(tmp,SavePath); return true;
            }
            catch(Exception e){Debug.LogError("Arcane Arsenal save failed: "+e.Message);return false;}
        }

        public static ArpgCharacterArsenal32 EnsureProfile(ArpgProfile30 profile)
        {
            if(profile==null)return null;
            Repair(Current);
            ArpgCharacterArsenal32 c=Current.characters.FirstOrDefault(x=>x!=null&&x.characterId==profile.characterId);
            if(c==null){c=new ArpgCharacterArsenal32{characterId=profile.characterId};Current.characters.Add(c);}
            AttachCollections(profile,!c.migrated);
            if(!c.migrated)
            {
                MigrateProfile(profile,c);c.migrated=true;Current.lastMigrationUtc=DateTime.UtcNow.ToString("O");
                profile.dataVersion=Schema;profile.inventoryCapacity=200;CaptureCollections(profile);ArpgProfileStore30.Save(profile);Save();
            }
            else EnsureItemStates(profile);
            return c;
        }

        private static void AttachCollections(ArpgProfile30 profile,bool importLegacy)
        {
            if(profile==null)return;
            if(profile.ownedCoreIds==null)profile.ownedCoreIds=new List<string>();if(profile.ownedRuneIds==null)profile.ownedRuneIds=new List<string>();if(profile.ownedLinkConditionIds==null)profile.ownedLinkConditionIds=new List<int>();if(profile.maps==null)profile.maps=new List<ArpgMapItem30>();
            if(importLegacy)
            {
                foreach(string id in profile.ownedCoreIds)if(!string.IsNullOrEmpty(id)&&!Current.sharedCoreIds.Contains(id))Current.sharedCoreIds.Add(id);
                foreach(string id in profile.ownedRuneIds)if(!string.IsNullOrEmpty(id)&&!Current.sharedRuneIds.Contains(id))Current.sharedRuneIds.Add(id);
                foreach(int id in profile.ownedLinkConditionIds)if(!Current.sharedLinkConditionIds.Contains(id))Current.sharedLinkConditionIds.Add(id);
                foreach(ArpgMapItem30 map in profile.maps.Where(value=>value!=null))if(!Current.sharedMaps.Any(value=>value!=null&&value.instanceId==map.instanceId))Current.sharedMaps.Add(map);
            }
            profile.ownedCoreIds=Current.sharedCoreIds;profile.ownedRuneIds=Current.sharedRuneIds;profile.ownedLinkConditionIds=Current.sharedLinkConditionIds;profile.maps=Current.sharedMaps;
        }

        private static void CaptureCollections(ArpgProfile30 profile)
        {
            if(profile==null)return;
            if(profile.ownedCoreIds!=null)Current.sharedCoreIds=profile.ownedCoreIds.Where(value=>!string.IsNullOrEmpty(value)).Distinct().ToList();
            if(profile.ownedRuneIds!=null)Current.sharedRuneIds=profile.ownedRuneIds.Where(value=>!string.IsNullOrEmpty(value)).Distinct().ToList();
            if(profile.ownedLinkConditionIds!=null)Current.sharedLinkConditionIds=profile.ownedLinkConditionIds.Distinct().ToList();
            if(profile.maps!=null)Current.sharedMaps=profile.maps.Where(value=>value!=null&&!string.IsNullOrEmpty(value.instanceId)).GroupBy(value=>value.instanceId).Select(value=>value.First()).ToList();
            profile.ownedCoreIds=Current.sharedCoreIds;profile.ownedRuneIds=Current.sharedRuneIds;profile.ownedLinkConditionIds=Current.sharedLinkConditionIds;profile.maps=Current.sharedMaps;
        }

        public static ArpgItem30 FindItem(ArpgProfile30 profile,string id)
        {
            if(string.IsNullOrEmpty(id))return null;
            ArpgItem30 item=profile==null?null:profile.GetItem(id);
            return item??Current.sharedItems.FirstOrDefault(x=>x!=null&&x.instanceId==id);
        }
        public static bool PrepareForCharacter(ArpgProfile30 profile,ArpgItem30 item,out string message)
        {
            message=string.Empty;if(profile==null||item==null){message="Invalid item.";return false;}
            ArpgItem30 shared=Current.sharedItems.FirstOrDefault(x=>x!=null&&x.instanceId==item.instanceId);
            if(shared!=null){Current.sharedItems.Remove(shared);if(!profile.items.Any(x=>x!=null&&x.instanceId==item.instanceId))profile.items.Add(shared);}
            return true;
        }
        public static ArpgItemState32 ItemState(string id){return string.IsNullOrEmpty(id)?null:Current.itemStates.FirstOrDefault(x=>x!=null&&x.itemInstanceId==id);}
        public static ArpgItemState32 EnsureItemState(ArpgProfile30 profile,ArpgItem30 item)
        {
            if(profile==null||item==null)return null;
            ArpgItemState32 s=ItemState(item.instanceId);
            if(s==null)
            {
                ArpgFootprintDefinition32 f=ArpgArsenalContent32.Footprint(item);
                s=new ArpgItemState32{itemInstanceId=item.instanceId,ownerCharacterId=profile.characterId,width=f.width,height=f.height};
                Current.itemStates.Add(s);
            }
            if(s.width<=0||s.height<=0){ArpgFootprintDefinition32 f=ArpgArsenalContent32.Footprint(item);s.width=f.width;s.height=f.height;}
            return s;
        }

        public static void RegisterGeneratedItem(ArpgItem30 item,string uniqueId,bool exceptional)
        {
            if(item==null)return;
            ArpgItemState32 s=ItemState(item.instanceId);
            if(s==null)
            {
                ArpgFootprintDefinition32 f=ArpgArsenalContent32.Footprint(item);
                s=new ArpgItemState32{itemInstanceId=item.instanceId,width=f.width,height=f.height};Current.itemStates.Add(s);
            }
            s.uniqueId=uniqueId??string.Empty;s.exceptional=exceptional;
            ArpgAffixRoll30 first=(item.prefixes??new List<ArpgAffixRoll30>()).FirstOrDefault();
            if(exceptional&&first!=null){s.elevatedAffixId=first.affixId;if(!s.sealedAffixIds.Contains(first.affixId))s.sealedAffixIds.Add(first.affixId);}
            Save();
        }

        public static string InventoryContainer(string charId){return "inventory:"+charId;}
        public static string EquippedContainer(string charId,ArpgItemSlot30 slot){return "equipped:"+charId+":"+slot;}
        public static IEnumerable<ArpgItemState32> InContainer(string id){return Current.itemStates.Where(x=>x!=null&&x.containerId==id);}

        public static bool Move(ArpgProfile30 profile,ArpgItem30 item,string container,int x,int y,out string message)
        {
            message=string.Empty;if(profile==null||item==null){message="No item selected.";return false;}
            ArpgItemState32 s=EnsureItemState(profile,item);int cols,rows;GridSize(container,out cols,out rows);
            if(cols<=0||rows<=0){message="That container cannot accept equipment.";return false;}
            if(!Fits(s,container,x,y,cols,rows)){message="The item does not fit there.";return false;}
            bool toShared=container.StartsWith("stash.",StringComparison.Ordinal);
            if(toShared)
            {
                profile.equipped.RemoveAll(value=>value!=null&&value.itemInstanceId==item.instanceId);
                profile.items.RemoveAll(value=>value!=null&&value.instanceId==item.instanceId);
                if(!Current.sharedItems.Any(value=>value!=null&&value.instanceId==item.instanceId))Current.sharedItems.Add(item);
            }
            else if(container.StartsWith("inventory:",StringComparison.Ordinal))
            {
                Current.sharedItems.RemoveAll(value=>value!=null&&value.instanceId==item.instanceId);
                if(!profile.items.Any(value=>value!=null&&value.instanceId==item.instanceId))profile.items.Add(item);
            }
            s.containerId=container;s.x=x;s.y=y;s.ownerCharacterId=container.StartsWith("inventory:")?profile.characterId:string.Empty;
            SyncLegacyStash(profile);Save();ArpgProfileStore30.Save(profile);message="Moved "+item.displayName+".";return true;
        }

        public static bool AutoPlace(ArpgProfile30 profile,ArpgItem30 item,string container,out string message)
        {
            message=string.Empty;ArpgItemState32 s=EnsureItemState(profile,item);int cols,rows;GridSize(container,out cols,out rows);
            for(int y=0;y<=rows-s.height;y++)for(int x=0;x<=cols-s.width;x++)if(Fits(s,container,x,y,cols,rows))return Move(profile,item,container,x,y,out message);
            message="No free space remains in "+ContainerName(container)+".";return false;
        }

        public static bool RemoveItem(ArpgProfile30 profile,ArpgItem30 item)
        {
            if(profile==null||item==null)return false;
            profile.items.Remove(item);Current.sharedItems.RemoveAll(value=>value!=null&&value.instanceId==item.instanceId);profile.stashItemIds.Remove(item.instanceId);profile.equipped.RemoveAll(x=>x!=null&&x.itemInstanceId==item.instanceId);
            Current.itemStates.RemoveAll(x=>x!=null&&x.itemInstanceId==item.instanceId);Save();ArpgProfileStore30.Save(profile);return true;
        }

        public static int Currency(ArpgCurrency32 c){ArpgCurrencyStack32 s=Current.currencies.FirstOrDefault(x=>x!=null&&x.currency==c);return s==null?0:s.amount;}
        public static void AddCurrency(ArpgCurrency32 c,int amount)
        {
            if(amount==0)return;ArpgCurrencyStack32 s=Current.currencies.FirstOrDefault(x=>x!=null&&x.currency==c);
            if(s==null){s=new ArpgCurrencyStack32{currency=c};Current.currencies.Add(s);}
            s.amount=Mathf.Clamp(s.amount+amount,0,999999);Save();
        }
        public static bool SpendCurrency(ArpgCurrency32 c,int amount){amount=Mathf.Max(0,amount);if(Currency(c)<amount)return false;AddCurrency(c,-amount);return true;}
        public static ArpgStashTab32 StashTab(string id){return Current.stashTabs.FirstOrDefault(x=>x!=null&&x.id==id);}
        public static ArpgLootFilterPreset32 ActiveFilter(){return Current.filters.FirstOrDefault(x=>x!=null&&x.active)??Current.filters.FirstOrDefault();}

        public static bool Fits(ArpgItemState32 moving,string container,int x,int y,int cols,int rows)
        {
            if(moving==null||x<0||y<0||x+moving.width>cols||y+moving.height>rows)return false;
            foreach(ArpgItemState32 o in InContainer(container))
            {
                if(o==null||o.itemInstanceId==moving.itemInstanceId)continue;
                if(x<o.x+o.width&&x+moving.width>o.x&&y<o.y+o.height&&y+moving.height>o.y)return false;
            }
            return true;
        }

        public static void GridSize(string id,out int cols,out int rows)
        {
            cols=0;rows=0;if(id!=null&&id.StartsWith("inventory:")){cols=12;rows=5;return;}
            ArpgStashTab32 t=StashTab(id);
            if(t!=null&&(t.kind==ArpgStashTabKind32.General||t.kind==ArpgStashTabKind32.Relics||t.kind==ArpgStashTabKind32.CraftingProjects||t.kind==ArpgStashTabKind32.Recovery)){cols=t.columns;rows=t.rows;}
        }
        public static string ContainerName(string id){if(id!=null&&id.StartsWith("inventory:"))return "the backpack";ArpgStashTab32 t=StashTab(id);return t==null?"the container":t.displayName;}
        public static void SyncLegacyStash(ArpgProfile30 profile)
        {
            if(profile==null)return;if(profile.stashItemIds==null)profile.stashItemIds=new List<string>();profile.stashItemIds.Clear();
            foreach(ArpgItemState32 s in Current.itemStates)if(s!=null&&s.containerId!=null&&s.containerId.StartsWith("stash."))profile.stashItemIds.Add(s.itemInstanceId);
        }

        private static void MigrateProfile(ArpgProfile30 profile,ArpgCharacterArsenal32 c)
        {
            EnsureItemStates(profile);string inv=InventoryContainer(profile.characterId);
            foreach(ArpgItem30 item in profile.items.Where(x=>x!=null).ToList())
            {
                ArpgItemState32 s=EnsureItemState(profile,item);
                if(profile.IsEquipped(item.instanceId)){s.containerId=EquippedContainer(profile.characterId,item.slot);s.ownerCharacterId=profile.characterId;continue;}
                string target=profile.IsInStash(item.instanceId)?"stash.general.1":inv;string result;
                if(!AutoPlace(profile,item,target,out result))AutoPlace(profile,item,"stash.recovery",out result);
            }
            foreach(ArpgCurrencyStack30 stack in profile.currencies.Where(x=>x!=null))AddCurrency(ArpgArsenalContent32.FromLegacy(stack.currency),stack.amount);
            SyncLegacyStash(profile);
        }
        private static void EnsureItemStates(ArpgProfile30 profile){if(profile.items!=null)foreach(ArpgItem30 i in profile.items)if(i!=null)EnsureItemState(profile,i);}

        private static ArpgArsenalSave32 LoadInternal(){ArpgArsenalSave32 s=TryLoad(SavePath)??TryLoad(BackupPath)??new ArpgArsenalSave32();Repair(s);return s;}
        private static ArpgArsenalSave32 TryLoad(string path)
        {
            try{if(!File.Exists(path))return null;string j=File.ReadAllText(path);return string.IsNullOrWhiteSpace(j)?null:JsonUtility.FromJson<ArpgArsenalSave32>(j);}
            catch(Exception e){Debug.LogWarning("Arcane Arsenal load failed: "+e.Message);return null;}
        }
        private static void Repair(ArpgArsenalSave32 s)
        {
            if(s==null)return;s.schema=Schema;s.gold=Mathf.Max(0,s.gold);
            if(s.characters==null)s.characters=new List<ArpgCharacterArsenal32>();if(s.itemStates==null)s.itemStates=new List<ArpgItemState32>();if(s.sharedItems==null)s.sharedItems=new List<ArpgItem30>();if(s.sharedMaps==null)s.sharedMaps=new List<ArpgMapItem30>();if(s.sharedCoreIds==null)s.sharedCoreIds=new List<string>();if(s.sharedRuneIds==null)s.sharedRuneIds=new List<string>();if(s.sharedLinkConditionIds==null)s.sharedLinkConditionIds=new List<int>();
            if(s.stashTabs==null)s.stashTabs=new List<ArpgStashTab32>();if(s.currencies==null)s.currencies=new List<ArpgCurrencyStack32>();
            if(s.filters==null)s.filters=new List<ArpgLootFilterPreset32>();if(s.vendor==null)s.vendor=new ArpgVendorState32();if(s.vendor.offers==null)s.vendor.offers=new List<ArpgVendorOffer32>();
            s.itemStates.RemoveAll(x=>x==null||string.IsNullOrEmpty(x.itemInstanceId));s.itemStates=s.itemStates.GroupBy(x=>x.itemInstanceId).Select(x=>x.First()).ToList();s.sharedMaps.RemoveAll(x=>x==null||string.IsNullOrEmpty(x.instanceId));s.sharedMaps=s.sharedMaps.GroupBy(x=>x.instanceId).Select(x=>x.First()).ToList();s.sharedCoreIds=s.sharedCoreIds.Where(x=>!string.IsNullOrEmpty(x)).Distinct().ToList();s.sharedRuneIds=s.sharedRuneIds.Where(x=>!string.IsNullOrEmpty(x)).Distinct().ToList();s.sharedLinkConditionIds=s.sharedLinkConditionIds.Distinct().ToList();s.sharedItems.RemoveAll(x=>x==null||string.IsNullOrEmpty(x.instanceId));s.sharedItems=s.sharedItems.GroupBy(x=>x.instanceId).Select(x=>x.First()).ToList();
            foreach(ArpgItemState32 x in s.itemStates){if(x.sealedAffixIds==null)x.sealedAffixIds=new List<string>();if(x.craftHistory==null)x.craftHistory=new List<ArpgCraftRecord32>();x.width=Mathf.Clamp(x.width,1,4);x.height=Mathf.Clamp(x.height,1,4);}
            EnsureTabs(s);EnsureFilters(s);
            foreach(ArpgCurrency32 c in Enum.GetValues(typeof(ArpgCurrency32)).Cast<ArpgCurrency32>())if(!s.currencies.Any(x=>x!=null&&x.currency==c))s.currencies.Add(new ArpgCurrencyStack32{currency=c});
        }
        private static void EnsureTabs(ArpgArsenalSave32 s)
        {
            AddTab(s,"stash.general.1","General I",ArpgStashTabKind32.General,0,12,12);AddTab(s,"stash.general.2","General II",ArpgStashTabKind32.General,1,12,12);
            AddTab(s,"stash.currency","Currency",ArpgStashTabKind32.Currency,2,0,0);AddTab(s,"stash.maps","Maps",ArpgStashTabKind32.Maps,3,0,0);
            AddTab(s,"stash.cores","Spell Cores",ArpgStashTabKind32.Cores,4,0,0);AddTab(s,"stash.runes","Support Runes",ArpgStashTabKind32.Runes,5,0,0);
            AddTab(s,"stash.relics","Relics",ArpgStashTabKind32.Relics,6,12,12);AddTab(s,"stash.projects","Crafting Projects",ArpgStashTabKind32.CraftingProjects,7,12,12);
            AddTab(s,"stash.recovery","Recovery",ArpgStashTabKind32.Recovery,8,12,20);
        }
        private static void AddTab(ArpgArsenalSave32 s,string id,string name,ArpgStashTabKind32 kind,int order,int cols,int rows)
        {if(!s.stashTabs.Any(x=>x!=null&&x.id==id))s.stashTabs.Add(new ArpgStashTab32{id=id,displayName=name,kind=kind,order=order,columns=cols,rows=rows});}
        private static void EnsureFilters(ArpgArsenalSave32 s)
        {
            if(s.filters.Count>0)return;ArpgLootFilterPreset32 p=new ArpgLootFilterPreset32{id="filter.default",displayName="Default",active=true};
            p.rules.Add(new ArpgLootFilterRule32{id="rule.unique",displayName="Show Unique and Exceptional",minimumRarity=ArpgItemRarity30.Unique,labelSize=18,beam=true,sound=true});
            p.rules.Add(new ArpgLootFilterRule32{id="rule.rare",displayName="Show Rare Equipment",minimumRarity=ArpgItemRarity30.Rare,labelSize=16,beam=true});
            p.rules.Add(new ArpgLootFilterRule32{id="rule.all",displayName="Show Early Equipment",minimumRarity=ArpgItemRarity30.Normal,labelSize=14});
            s.filters.Add(p);
        }
    }
}
