using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class GameWorld : MonoBehaviour
    {
        public static GameWorld Instance { get; private set; }

        public readonly List<EnemyController> Enemies = new List<EnemyController>();
        public readonly Dictionary<string, int> ModifierInventory = new Dictionary<string, int>();
        public readonly Dictionary<string, int> OwnedModifierCounts = new Dictionary<string, int>();
        public readonly List<CoreSaveData> RunCoreSatchel = new List<CoreSaveData>();
        public readonly List<string> CombatLog = new List<string>();

        public EquipmentInventory Equipment { get; private set; }
        public PlayerController Player { get; private set; }
        public PlayerStats Stats { get; private set; }
        public bool RunActive { get; set; }
        public bool TrainingMode { get; private set; }
        public RoomTemplate CurrentRoom { get; private set; }
        public SpellLinkSystem SpellLinks { get; private set; }
        public bool PendingSpellUpgrade { get; private set; }
        public bool HasPendingSpellSystemReward { get { return PendingSpellUpgrade || (SpellLinks != null && SpellLinks.HasPendingReward); } }
        public bool CanEditSpells
        {
            get
            {
                if (!RunActive || TrainingMode) return true;
                RunDirector run = _runDirectorResolved ? _cachedRunDirector : GetComponent<RunDirector>();
                if (run == null) return Enemies.Count == 0;
                if (run.EncounterActive || run.ReinforcementsPending || Enemies.Count > 0) return false;
                return true;
            }
        }

        public string SpellEditLockReason
        {
            get
            {
                if (CanEditSpells) return string.Empty;
                RunDirector run = _runDirectorResolved ? _cachedRunDirector : GetComponent<RunDirector>();
                if (run != null && run.ReinforcementsPending) return "Reinforcements are still incoming.";
                if (Enemies.Count > 0 || (run != null && run.EncounterActive)) return "Clear the room before changing spells.";
                return "Wait for the current room state to become safe.";
            }
        }

        private readonly SpellBoard[] _boards = new SpellBoard[3];
        private readonly CompiledSpell[] _spellCache = new CompiledSpell[3];
        private readonly SpellBehaviorGraph[] _spellGraphCache = new SpellBehaviorGraph[3];
        private readonly CoreSaveData[] _activeCores = new CoreSaveData[3];
        private readonly bool[] _activeCoreBankable = new bool[3];
        private readonly HashSet<string> _bankableCoreInstances = new HashSet<string>();
        private Transform _environment;
        private Transform _equipmentVisuals;
        private RunDirector _cachedRunDirector;
        private bool _runDirectorResolved;

        public bool ModalOpen
        {
            get
            {
                return (V21ProductUI.Instance != null && V21ProductUI.Instance.IsOpen) ||
                    (DemoUI.Instance != null && DemoUI.Instance.ModalOpen) || V1TitleScreen.IsOpen || V1TitleScreen.IsMapOpen;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            ProfileManager.Load(0);
            DemoCatalog.Ensure();
            RuntimeValidation.ValidateRuntimeEssentials();
            ProceduralVisualValidation.Validate();
            VisualCorrectiveContractValidation.Validate();
            SpellLinks = new SpellLinkSystem(this);
            bool migrateItems = ProfileManager.Current.armory.Any(item => item != null && item.dataVersion < V11Itemization.DataVersion);
            Equipment = new EquipmentInventory();
            Equipment.Changed += OnEquipmentChanged;
            BuildSanctuaryEnvironment();
            Equipment.LoadFromProfile(false);
            if (migrateItems) Equipment.SaveSanctuaryEquipment();
        }

        private void Start()
        {
            _cachedRunDirector = GetComponent<RunDirector>();
            _runDirectorResolved = true;
            CreatePlayerAndCamera();
            RecalculateStats(false);
            RunActive = false;
            Log("Welcome to Home Base. Prepare your spells, equipment, and next run.");
            if (HomeBaseController.Instance != null) HomeBaseController.Instance.Rebuild();
        }

        public void ResetRunSystems(DifficultySettings difficulty)
        {
            Equipment.LoadFromProfile(difficulty != null && difficulty.noStartingEquipment);
            ModifierInventory.Clear();
            OwnedModifierCounts.Clear();
            RunCoreSatchel.Clear();
            SpellLinks.ResetRun();
            PendingSpellUpgrade = false;
            _bankableCoreInstances.Clear();
            for (int i = 0; i < 3; i++)
            {
                _boards[i] = null;
                _activeCores[i] = null;
                _activeCoreBankable[i] = false;
            }

            ProfileData profile = ProfileManager.Current;
            foreach (int condition in profile.unlockedLinkConditionIds)
                if (Enum.IsDefined(typeof(SpellLinkCondition), condition)) SpellLinks.GrantLegacyCondition((SpellLinkCondition)condition);
            int allowedSlots = profile.StartingSpellSlots;
            int relicsEquipped = 0;
            Dictionary<string, int> usedStandardCores = new Dictionary<string, int>();
            foreach (PreparedSpellSave prepared in profile.preparedSpells.OrderBy(p => p.slotIndex))
            {
                if (prepared.slotIndex >= allowedSlots || prepared.slotIndex < 0 || prepared.slotIndex > 2) continue;
                string coreId = prepared.contentId;
                string relicId = null;
                if (prepared.isRelic)
                {
                    RelicDefinition relic = MegaCatalog.GetRelic(prepared.contentId);
                    if (relic == null || relicsEquipped > 0 || profile.relicArchive.All(r => r.relicId != prepared.contentId)) continue;
                    coreId = relic.sourceCoreId;
                    relicId = relic.id;
                    relicsEquipped++;
                }
                else
                {
                    int used;
                    usedStandardCores.TryGetValue(coreId, out used);
                    if (profile.spellArchive.Count(c => c.coreId == coreId) <= used) continue;
                    usedStandardCores[coreId] = used + 1;
                }
                if (DemoCatalog.GetCore(coreId) == null) continue;
                CoreSaveData runtimeCore = new CoreSaveData(coreId);
                _activeCores[prepared.slotIndex] = runtimeCore;
                _boards[prepared.slotIndex] = new SpellBoard((SpellSlot)prepared.slotIndex, coreId, relicId);
            }

            if (_boards[0] == null)
            {
                CoreSaveData fallback = new CoreSaveData("fireball");
                _activeCores[0] = fallback;
                _boards[0] = new SpellBoard(SpellSlot.Slot1, "fireball");
            }

            if (difficulty == null || !difficulty.noStartingModifiers)
            {
                int spent = 0;
                foreach (PreparedModifierSave prepared in profile.preparedModifiers)
                {
                    SpellModifierDefinition definition = DemoCatalog.GetModifier(prepared.modifierId);
                    if (definition == null || !profile.unlockedModifierIds.Contains(prepared.modifierId)) continue;
                    for (int copy = 0; copy < prepared.count; copy++)
                    {
                        if (spent + definition.preparationCost > profile.PreparationBudget) break;
                        AddOwnedModifier(prepared.modifierId, 1, false);
                        spent += definition.preparationCost;
                    }
                }
            }
            MarkSpellsDirty();
            RecalculateModifierAvailability();
            RefreshEquipmentVisuals();
        }

        public void CaptureRunState(RunSnapshotData snapshot)
        {
            if (snapshot == null) return;
            snapshot.healthRatio = Player == null || Stats == null ? 1f : Player.Health / Mathf.Max(1f, Stats.maxHealth);
            snapshot.manaRatio = Player == null || Stats == null ? 1f : Player.Mana / Mathf.Max(1f, Stats.maxMana);
            snapshot.spells.Clear();
            for (int i = 0; i < 3; i++)
            {
                if (_boards[i] == null || _activeCores[i] == null) continue;
                RunSpellSnapshot saved = new RunSpellSnapshot
                {
                    slotIndex = i,
                    activeSpell = new CoreSaveData { instanceId = _activeCores[i].instanceId, coreId = _activeCores[i].coreId },
                    canBeKept = _activeCoreBankable[i],
                    baseSpellId = _boards[i].coreId,
                    legendarySpellId = _boards[i].relicId,
                    boardRadiusBonus = _boards[i].temporaryRadiusBonus,
                    spellLevel = _boards[i].spellLevel
                };
                foreach (PlacedModifier piece in _boards[i].placed)
                    saved.modifiers.Add(new PlacedModifierSave { modifierId = piece.modifierId, q = piece.anchor.q, r = piece.anchor.r, rotation = piece.rotation });
                snapshot.spells.Add(saved);
            }

            snapshot.ownedModifiers = OwnedModifierCounts.Select(pair => new PreparedModifierSave { modifierId = pair.Key, count = pair.Value }).ToList();
            snapshot.storedSpells = RunCoreSatchel.Select(core => new CoreSaveData { instanceId = core.instanceId, coreId = core.coreId }).ToList();
            snapshot.keepableSpellInstanceIds = _bankableCoreInstances.ToList();
            snapshot.backpack.Clear();
            snapshot.unsecuredItems = Equipment.runBag.Select(item => item.ToSaveData(false)).ToList();
            snapshot.equippedItems = Equipment.equipped.Select(pair => new RunEquippedItemSnapshot { slot = pair.Key, item = pair.Value.ToSaveData(false) }).ToList();
            snapshot.linkSlots = SpellLinks.Slots;
            snapshot.ownedLinkConditions = SpellLinks.OwnedConditions.Select(value => (int)value).ToList();
            snapshot.spellLinks = SpellLinks.Links.Select(value => value.Clone()).ToList();
        }

        public bool RestoreRunState(RunSnapshotData snapshot)
        {
            if (snapshot == null) return false;
            ModifierInventory.Clear();
            OwnedModifierCounts.Clear();
            RunCoreSatchel.Clear();
            _bankableCoreInstances.Clear();
            PendingSpellUpgrade = false;
            List<int> restoredLinkConditions = snapshot.ownedLinkConditions == null ? new List<int>() : snapshot.ownedLinkConditions.ToList();
            for (int i = 0; i < 3; i++)
            {
                _boards[i] = null;
                _activeCores[i] = null;
                _activeCoreBankable[i] = false;
            }

            foreach (PreparedModifierSave saved in snapshot.ownedModifiers)
            {
                if (saved == null) continue;
                SpellModifierDefinition definition = DemoCatalog.GetModifier(saved.modifierId);
                if (definition == null) continue;
                if (!definition.availableAsSupport)
                {
                    SpellLinkCondition migrated;
                    if (TryMapLegacyLink(saved.modifierId, out migrated) && !restoredLinkConditions.Contains((int)migrated)) restoredLinkConditions.Add((int)migrated);
                    continue;
                }
                OwnedModifierCounts[saved.modifierId] = Mathf.Max(0, saved.count);
            }
            foreach (CoreSaveData saved in snapshot.storedSpells)
                if (saved != null && DemoCatalog.GetCore(saved.coreId) != null) RunCoreSatchel.Add(new CoreSaveData { instanceId = saved.instanceId, coreId = saved.coreId });
            foreach (string instanceId in snapshot.keepableSpellInstanceIds) if (!string.IsNullOrEmpty(instanceId)) _bankableCoreInstances.Add(instanceId);

            foreach (RunSpellSnapshot saved in snapshot.spells)
            {
                if (saved == null || saved.slotIndex < 0 || saved.slotIndex > 2 || saved.activeSpell == null || DemoCatalog.GetCore(saved.baseSpellId) == null) continue;
                _activeCores[saved.slotIndex] = new CoreSaveData { instanceId = saved.activeSpell.instanceId, coreId = saved.activeSpell.coreId };
                _activeCoreBankable[saved.slotIndex] = saved.canBeKept;
                SpellBoard board = new SpellBoard((SpellSlot)saved.slotIndex, saved.baseSpellId, saved.legendarySpellId);
                board.temporaryRadiusBonus = saved.boardRadiusBonus;
                board.spellLevel = Mathf.Clamp(saved.spellLevel <= 0 ? 1 : saved.spellLevel, 1, 5);
                board.LoadLayout(new SavedSpellLayout { name = "Restored Run", coreId = saved.baseSpellId, relicId = saved.legendarySpellId, pieces = saved.modifiers ?? new List<PlacedModifierSave>() });
                _boards[saved.slotIndex] = board;
            }

            SpellLinks.Restore(snapshot.linkSlots, restoredLinkConditions, snapshot.spellLinks);

            Equipment.RestoreRunState(snapshot.unsecuredItems, snapshot.equippedItems);
            RecalculateModifierAvailability();
            RefreshEquipmentVisuals();
            return _boards.Any(board => board != null);
        }

        public void RecalculateStats(bool preserveResourceRatio)
        {
            float healthRatio = Player == null || Stats == null ? 1f : Player.Health / Mathf.Max(1f, Stats.maxHealth);
            float manaRatio = Player == null || Stats == null ? 1f : Player.Mana / Mathf.Max(1f, Stats.maxMana);
            ProfileData profile = ProfileManager.Current;
            Stats = Equipment.BuildStats(profile.healthRank, profile.manaRank, profile.powerRank);
            RunDirector run = GetComponent<RunDirector>();
            if (run != null && run.Difficulty.reducedHealing) Stats.healingMultiplier *= 0.55f;
            if (run != null) run.ApplyRunBonuses(Stats);
            if (Player != null) Player.ApplyStats(Stats, preserveResourceRatio ? healthRatio : 1f, preserveResourceRatio ? manaRatio : 1f);
            MarkSpellsDirty();
        }

        public CompiledSpell GetSpell(SpellSlot slot)
        {
            int index = SlotIndex(slot);
            if (index < 0 || _boards[index] == null) return null;
            if (_spellCache[index] == null)
            {
                _spellCache[index] = SpellCompiler.Compile(_boards[index], Stats, Equipment);
                RunDirector run = GetComponent<RunDirector>();
                if (_spellCache[index] != null && run != null && run.Difficulty.unstableWorld) _spellCache[index].instability += 18f;
                if (_spellCache[index] != null && SpellLinks != null)
                {
                    _spellCache[index].incomingSpellLinks = SpellLinks.Links.Count(link => link.destinationSlot == index);
                    _spellCache[index].outgoingSpellLinks = SpellLinks.Links.Count(link => link.sourceSlot == index);
                    _spellCache[index].outgoingLinkConditions = SpellLinks.Links.Where(link => link.sourceSlot == index).Select(link => link.condition).ToArray();
                    SpellVisualCompiler.Rebuild(_spellCache[index]);
                }
                _spellGraphCache[index] = V12SpellGraphCompiler.Compile(_boards[index], _spellCache[index]);
            }
            return _spellCache[index];
        }

        public SpellBehaviorGraph GetSpellGraph(SpellSlot slot)
        {
            int index = SlotIndex(slot);
            if (index < 0 || _boards[index] == null) return null;
            if (_spellCache[index] == null || _spellGraphCache[index] == null) GetSpell(slot);
            return _spellGraphCache[index];
        }

        public SpellBoard GetBoard(SpellSlot slot)
        {
            int index = SlotIndex(slot);
            return index < 0 ? null : _boards[index];
        }

        public bool HasSpell(SpellSlot slot) { return GetBoard(slot) != null; }

        public void MarkSpellsDirty()
        {
            for (int i = 0; i < _spellCache.Length; i++)
            {
                _spellCache[i] = null;
                _spellGraphCache[i] = null;
            }
        }

        public void AddModifier(string id, int count = 1)
        {
            AddOwnedModifier(id, count, true);
            RecalculateModifierAvailability();
        }

        public bool BeginSpellUpgradeReward(out string message)
        {
            if (!_boards.Any(board => board != null && board.spellLevel < 5))
            {
                message = "Every equipped spell is already Level 5.";
                return false;
            }
            PendingSpellUpgrade = true;
            message = "Choose one equipped spell to level up.";
            return true;
        }

        public bool ApplySpellUpgrade(SpellSlot slot, out string message)
        {
            if (!CanEditSpells) { message = SpellEditLockReason; return false; }
            if (!PendingSpellUpgrade) { message = "No Spell Upgrade is waiting to be assigned."; return false; }
            SpellBoard board = GetBoard(slot);
            if (board == null) { message = "That spell slot is empty."; return false; }
            if (!board.TryLevelUp(out message)) return false;
            PendingSpellUpgrade = false;
            MarkSpellsDirty();
            Log(DemoCatalog.GetCore(board.coreId).displayName + " reached Level " + board.spellLevel + ".");
            CompletePendingSpellSystemReward();
            return true;
        }

        public bool BeginSpellLinkReward(int seed, out string message)
        {
            if (Enumerable.Range(0, 3).Count(index => HasSpell((SpellSlot)index)) < 2)
            {
                message = "A Spell Link requires two equipped spells.";
                return false;
            }
            SpellLinks.GrantChoices(seed);
            message = "Choose a Spell Link condition, then connect two spells.";
            return true;
        }

        public bool SelectSpellLinkCondition(SpellLinkCondition condition, out string message)
        {
            if (!CanEditSpells) { message = SpellEditLockReason; return false; }
            return SpellLinks.SelectPendingCondition(condition, out message);
        }

        public bool CreateSpellLink(int sourceSlot, int destinationSlot, SpellLinkCondition condition, out string message)
        {
            if (!CanEditSpells) { message = SpellEditLockReason; return false; }
            bool hadPendingReward = SpellLinks.HasPendingReward;
            if (!SpellLinks.TryAdd(sourceSlot, destinationSlot, condition, out message)) return false;
            if (hadPendingReward && !SpellLinks.HasPendingReward) CompletePendingSpellSystemReward();
            MarkSpellsDirty();
            return true;
        }

        public void KeepPendingLinkConditionForLater()
        {
            if (SpellLinks.PendingCondition.HasValue)
            {
                SpellLinks.ClearPendingCondition();
                Log("The Link condition remains available for this run.");
                CompletePendingSpellSystemReward();
            }
        }

        public bool TryQuickInstallModifier(string id, out string message)
        {
            if (!CanEditSpells) { message = SpellEditLockReason + " The Support Rune was collected instead."; return false; }
            SpellModifierDefinition definition = DemoCatalog.GetModifier(id);
            if (definition == null || !definition.availableAsSupport) { message = "Unknown Support Rune."; return false; }
            int preferred = Player == null ? 0 : (int)Player.LastCastSlot;
            int[] slotOrder = { preferred, (preferred + 1) % 3, (preferred + 2) % 3 };
            foreach (int slot in slotOrder)
            {
                SpellBoard board = _boards[slot]; if (board == null) continue;
                for (int radius = 1; radius <= board.Radius; radius++)
                for (int q = -radius; q <= radius; q++)
                for (int r = -radius; r <= radius; r++)
                {
                    HexCoord coordinate = new HexCoord(q, r); if (coordinate.DistanceFromOrigin() > radius || coordinate.DistanceFromOrigin() == 0) continue;
                    for (int rotation = 0; rotation < 6; rotation++)
                    {
                        string reason;
                        if (!board.TryPlace(id, coordinate, rotation, out reason)) continue;
                        PlacedModifier placed = board.placed.Last();
                        if (board.IsPlacementConnected(placed))
                        {
                            RecalculateModifierAvailability();
                            message = definition.displayName + " installed on Spell Slot " + (slot + 1) + ".";
                            Log(message); return true;
                        }
                        string removed; board.RemoveAt(coordinate, out removed);
                    }
                }
            }
            message = definition.displayName + " was collected, but no connected board position is currently open.";
            return false;
        }

        public void RecalculateModifierAvailability()
        {
            ModifierInventory.Clear();
            foreach (KeyValuePair<string, int> pair in OwnedModifierCounts) ModifierInventory[pair.Key] = pair.Value;
            foreach (SpellBoard board in _boards)
            {
                if (board == null) continue;
                foreach (PlacedModifier piece in board.placed)
                {
                    if (!ModifierInventory.ContainsKey(piece.modifierId)) ModifierInventory[piece.modifierId] = 0;
                    ModifierInventory[piece.modifierId] = Mathf.Max(0, ModifierInventory[piece.modifierId] - 1);
                }
            }
            MarkSpellsDirty();
        }

        public int RepairModifierOwnership()
        {
            Dictionary<string, int> installed = new Dictionary<string, int>();
            int removed = 0;
            for (int slot = 0; slot < _boards.Length; slot++)
            {
                SpellBoard board = _boards[slot];
                if (board == null) continue;
                foreach (PlacedModifier piece in board.placed.OrderBy(value => value.placementOrder).ToArray())
                {
                    int used; installed.TryGetValue(piece.modifierId, out used);
                    int owned; OwnedModifierCounts.TryGetValue(piece.modifierId, out owned);
                    if (used >= Mathf.Max(0, owned))
                    {
                        string ignored;
                        if (board.RemoveAt(piece.anchor, out ignored)) removed++;
                        continue;
                    }
                    installed[piece.modifierId] = used + 1;
                }
            }
            RecalculateModifierAvailability();
            if (removed > 0) Log(removed + " invalid modifier placement(s) were returned by the ownership audit.");
            return removed;
        }

        public bool ConsumeModifier(string id)
        {
            int count;
            return ModifierInventory.TryGetValue(id, out count) && count > 0;
        }

        public void ReturnModifier(string id) { RecalculateModifierAvailability(); }

        public bool InstallCore(SpellSlot slot, CoreSaveData core, out string message)
        {
            if (!CanEditSpells) { message = SpellEditLockReason; return false; }
            int index = SlotIndex(slot);
            if (index < 0 || core == null || DemoCatalog.GetCore(core.coreId) == null)
            {
                message = "That Spell Core cannot be equipped.";
                return false;
            }
            if (!RunCoreSatchel.Contains(core))
            {
                message = "That Spell Copy is not in Stored Spells.";
                return false;
            }

            if (_activeCores[index] != null)
            {
                RunCoreSatchel.Add(_activeCores[index]);
                if (_activeCoreBankable[index]) _bankableCoreInstances.Add(_activeCores[index].instanceId);
            }
            if (_boards[index] != null) _boards[index].Clear(false);
            RunCoreSatchel.Remove(core);
            _activeCores[index] = core;
            _activeCoreBankable[index] = _bankableCoreInstances.Contains(core.instanceId);
            _boards[index] = new SpellBoard((SpellSlot)index, core.coreId);
            RecalculateModifierAvailability();
            message = "Equipped " + DemoCatalog.GetCore(core.coreId).displayName + " in Spell Slot " + (index + 1) + ". Removed Support Runes were returned safely.";
            return true;
        }

        public void AddCoreCopy(string coreId)
        {
            if (DemoCatalog.GetCore(coreId) == null) return;
            CoreSaveData copy = new CoreSaveData(coreId);
            RunCoreSatchel.Add(copy);
            _bankableCoreInstances.Add(copy.instanceId);
            ProfileManager.Discover("core:" + coreId);
            Log("Found Spell Copy: " + DemoCatalog.GetCore(coreId).displayName + ".");
        }

        public void BankRunLoot()
        {
            foreach (CoreSaveData core in RunCoreSatchel)
                if (_bankableCoreInstances.Contains(core.instanceId) && ProfileManager.Current.spellArchive.All(c => c.instanceId != core.instanceId))
                    ProfileManager.Current.spellArchive.Add(core);
            for (int i = 0; i < 3; i++)
                if (_activeCores[i] != null && _activeCoreBankable[i] && ProfileManager.Current.spellArchive.All(c => c.instanceId != _activeCores[i].instanceId))
                    ProfileManager.Current.spellArchive.Add(_activeCores[i]);
            Equipment.BankAllRunItems();
            RunDirector run = _runDirectorResolved ? _cachedRunDirector : GetComponent<RunDirector>();
            if (run != null) ProfileManager.SecureForgeMaterials(run.ForgeDust, run.BindingRunes, run.CorruptionCores);
            Equipment.SaveSanctuaryEquipment(false);
            ProfileManager.Save();
        }

        public void Log(string message)
        {
            CombatLog.Insert(0, message);
            if (CombatLog.Count > 10) CombatLog.RemoveAt(CombatLog.Count - 1);
        }

        public EnemyController NearestEnemy(Vector3 position, float maxDistance = 999f, HashSet<EntityId> excluded = null)
        {
            EnemyController best = null;
            float bestDistance = maxDistance * maxDistance;
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                EnemyController enemy = Enemies[i];
                if (enemy == null || enemy.IsDead) continue;
                if (excluded != null && excluded.Contains(enemy.GetEntityId())) continue;
                float distance = (enemy.transform.position - position).sqrMagnitude;
                if (distance < bestDistance) { bestDistance = distance; best = enemy; }
            }
            return best;
        }

        public void SpawnLoot(Vector3 position, bool guaranteedUnique = false)
        {
            RunDirector director = _runDirectorResolved ? _cachedRunDirector : GetComponent<RunDirector>();
            if (director == null) return;
            if (!guaranteedUnique && LootPickup.ActiveCount >= 24) return;
            if (guaranteedUnique)
            {
                ItemDefinition[] uniques = DemoCatalog.AllItems.Where(value => value.rarity == ItemRarity.Unique).ToArray();
                if (uniques.Length > 0)
                    LootPickup.CreateItem(position, uniques[Mathf.Abs(director.CurrentSeed + director.RoomIndex * 31 + director.Kills) % uniques.Length].id, director.RoomIndex + 1);
                return;
            }
            float roll = UnityEngine.Random.value;
            if (roll < 0.1f) LootPickup.CreateDrachmas(position, UnityEngine.Random.Range(3, 8));
            else if (roll < 0.14f)
            {
                List<SpellModifierDefinition> pool = DemoCatalog.AllModifiers.Where(m => ProfileManager.Current.unlockedModifierIds.Contains(m.id)).ToList();
                if (pool.Count > 0) LootPickup.CreateModifier(position, pool[UnityEngine.Random.Range(0, pool.Count)].id);
            }
            else if (roll < 0.165f)
            {
                ItemDefinition selected = SelectLootBase(V1Determinism.Combine(director.CurrentSeed, director.RoomIndex, "enemy_drop", director.Kills), false);
                if (selected != null) LootPickup.CreateItem(position, selected.id, director.RoomIndex + 1);
            }
            else if (roll < 0.185f) LootPickup.CreateEssence(position, 1);
        }

        public ItemDefinition SelectLootBase(int seed, bool allowUnique)
        {
            List<ItemDefinition> pool = DemoCatalog.AllItems.Where(value =>
                value.rarity == ItemRarity.Unique ? allowUnique : !string.IsNullOrEmpty(value.baseFamily) || !string.IsNullOrEmpty(value.setId)).ToList();
            if (pool.Count == 0)
                pool = DemoCatalog.AllItems.Where(value => value.rarity != ItemRarity.Unique).ToList();
            if (pool.Count == 0) return null;
            HashSet<string> recent = new HashSet<string>(Equipment.runBag.Concat(Equipment.equipped.Values).Where(value => value != null)
                .Reverse().Take(6).Select(value => value.definitionId));
            HashSet<string> buildTags = new HashSet<string> { "caster" };
            for (int i = 0; i < 3; i++)
            {
                CompiledSpell spell = GetSpell((SpellSlot)i);
                if (spell != null && spell.triggers.Count > 0) buildTags.Add("trigger");
            }
            if (SpellLinks != null && SpellLinks.Links.Count > 0) buildTags.Add("trigger");
            HashSet<EquipmentSlot> emptySlots = new HashSet<EquipmentSlot>(Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>()
                .Where(slot => !Equipment.equipped.ContainsKey(slot)));
            System.Random random = new System.Random(seed);
            List<int> weights = new List<int>();
            int total = 0;
            foreach (ItemDefinition candidate in pool)
            {
                int weight = candidate.rarity == ItemRarity.Unique ? 2 : 20;
                if (emptySlots.Contains(candidate.slot)) weight += 8;
                if (candidate.itemTags != null && candidate.itemTags.Any(buildTags.Contains)) weight += 7;
                if (recent.Contains(candidate.id)) weight = Mathf.Max(1, weight / 5);
                weights.Add(weight); total += weight;
            }
            int roll = random.Next(Mathf.Max(1, total));
            for (int i = 0; i < pool.Count; i++) { roll -= weights[i]; if (roll < 0) return pool[i]; }
            return pool[pool.Count - 1];
        }

        public void ClearTransientObjects()
        {
            // ARCANE_PATCH_224_GAMEWORLD_CLEANUP
            SpellEffectLifecycle224.ClearAll(SpellEffectCleanupReason224.PlayerDeathOrRunEnd);

            if (DemoV05Director.Instance != null) DemoV05Director.Instance.Objective.Clear();
            foreach (EnemyController enemy in Enemies.ToArray()) if (enemy != null) Destroy(enemy.gameObject);
            Enemies.Clear();
            foreach (LootPickup loot in FindObjectsByType<LootPickup>()) Destroy(loot.gameObject);
            foreach (SpellProjectile projectile in FindObjectsByType<SpellProjectile>()) Destroy(projectile.gameObject);
            foreach (NovaEffect nova in FindObjectsByType<NovaEffect>()) Destroy(nova.gameObject);
            foreach (DelayedSpellEffect delayed in FindObjectsByType<DelayedSpellEffect>()) Destroy(delayed.gameObject);
            foreach (PersistentSpellZone zone in FindObjectsByType<PersistentSpellZone>()) Destroy(zone.gameObject);
            foreach (SpellFamiliar familiar in FindObjectsByType<SpellFamiliar>()) Destroy(familiar.gameObject);
            foreach (EnemyBolt bolt in FindObjectsByType<EnemyBolt>()) Destroy(bolt.gameObject);
            foreach (BreakableProp prop in FindObjectsByType<BreakableProp>()) Destroy(prop.gameObject);
            foreach (DungeonBladeTrap trap in FindObjectsByType<DungeonBladeTrap>()) Destroy(trap.gameObject);
            foreach (PhysicalRoomProp prop in FindObjectsByType<PhysicalRoomProp>()) Destroy(prop.gameObject);
            foreach (VeiledEnemy veil in FindObjectsByType<VeiledEnemy>()) Destroy(veil);
            RoomDoorStateVisuals.ClearLocked();
            ProceduralVisualRuntime.ClearActive();
            WorldInteractionController.ClearRoomInteractions();
            VisualRuntimeRegistry.ClearRoom();
        }

        public void BuildRoom(RoomTemplate room, int depth, int seed)
        {
            CurrentRoom = room;
            if (_environment != null) Destroy(_environment.gameObject);
            _environment = new GameObject("Room · " + room.displayName).transform;
            DontDestroyOnLoad(_environment.gameObject);
            UnityEngine.Random.State oldState = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);
            List<Vector3> reservedPlacements = V21RoomLayoutRuntime.Build(_environment, room, depth, seed);
            for (int i = 0; i < 8; i++)
            {
                float angle = i / 8f * Mathf.PI * 2f;
                GameObject rune = RuntimeVisuals.Primitive("Room Rune", PrimitiveType.Cylinder,
                    new Vector3(Mathf.Cos(angle) * 14f, 0.08f, Mathf.Sin(angle) * 14f), new Vector3(0.45f, 0.04f, 0.45f), room.accentColor, _environment);
                RuntimeVisuals.RemoveCollider(rune);
            }
            if (room.hasHazards)
            {
                for (int i = 0; i < 2 + depth / 5; i++)
                {
                    Vector3 position = DungeonLayoutValidator.FindPlacement(-11f, 11f, 1.8f, reservedPlacements);
                    position.y = 0.02f;
                    reservedPlacements.Add(position);
                    RoomHazard.Create(position, 1.5f + depth * 0.04f, room.accentColor, _environment);
                }
            }
            UnityEngine.Random.state = oldState;
            ProceduralDungeonVisuals.Build(_environment, room, depth, seed);
            SetupLighting(room.accentColor);
        }

        public void BuildSanctuaryEnvironment()
        {
            RoomTemplate sanctuary = new RoomTemplate
            {
                id = "sanctuary", displayName = "Home Base", type = DungeonRoomType.SafeWorkshop,
                floorColor = new Color(0.045f, 0.06f, 0.085f), accentColor = new Color(0.2f, 0.85f, 0.75f), obstaclePattern = 0
            };
            BuildRoom(sanctuary, 0, 1907);
            if (HomeBaseController.Instance != null && Player != null) HomeBaseController.Instance.Rebuild();
        }

        public void EnterTraining()
        {
            TrainingMode = true;
            RunActive = true;
            ClearTransientObjects();
            ResetRunSystems(new DifficultySettings());
            RecalculateStats(false);
            Player.ResetForRun();
            Player.transform.position = new Vector3(0f, 1f, 0f);
            BuildRoom(new RoomTemplate { id = "training", displayName = "Training Chamber", type = DungeonRoomType.Combat,
                floorColor = new Color(0.045f, 0.055f, 0.08f), accentColor = new Color(0.25f, 0.75f, 1f) }, 0, 88);
            for (int i = 0; i < 5; i++) EnemyController.Spawn(new Vector3(-8f + i * 4f, 0.75f, 5f), EnemyArchetype.TrainingDummy, 1, new DifficultySettings(), false, false);
            if (V21TrainingAnalytics.Instance != null) V21TrainingAnalytics.Instance.SetTargetProfile(V21TrainingTargetProfile.Normal);
            Log("Training Room active. Escape returns to Home Base.");
        }

        public void LeaveTraining()
        {
            if (!TrainingMode) return;
            TrainingMode = false;
            RunActive = false;
            ClearTransientObjects();
            BuildSanctuaryEnvironment();
            Equipment.LoadFromProfile(false);
            RecalculateStats(false);
            Player.transform.position = new Vector3(0f, 1f, 0f);
        }

        public void EnterBossPractice()
        {
            EnterTraining();
            foreach (EnemyController enemy in Enemies.ToArray()) if (enemy != null) Destroy(enemy.gameObject);
            Enemies.Clear();
            EnemyController.Spawn(new Vector3(0f, 0.75f, 6f), EnemyArchetype.OssuaryWarden, 6, new DifficultySettings(), false, true);
            Player.transform.position = new Vector3(0f, 1f, -10f);
            Player.GrantSpawnProtection(2f);
            Log("Dungeon Warden practice active. No rewards or progression are granted.");
        }

        private void AddOwnedModifier(string id, int count, bool log)
        {
            if (DemoCatalog.GetModifier(id) == null) return;
            if (!OwnedModifierCounts.ContainsKey(id)) OwnedModifierCounts[id] = 0;
            OwnedModifierCounts[id] += Mathf.Max(0, count);
            ProfileManager.Discover("modifier:" + id);
            if (log) Log("Found Support Rune: " + DemoCatalog.GetModifier(id).displayName + ". It belongs to this run.");
        }

        private void OnEquipmentChanged()
        {
            RecalculateStats(true);
            RefreshEquipmentVisuals();
        }

        private void CreatePlayerAndCamera()
        {
            foreach (Camera existing in FindObjectsByType<Camera>())
                if (existing != null && existing.gameObject != null)
                {
                    existing.enabled = false;
                    if (existing.gameObject.CompareTag("MainCamera")) existing.gameObject.tag = "Untagged";
                    Destroy(existing.gameObject);
                }
            GameObject playerObject = RuntimeVisuals.Primitive("Player", PrimitiveType.Capsule, new Vector3(0f, 1f, 0f),
                new Vector3(0.8f, 1f, 0.8f), new Color(0.2f, 0.85f, 1f));
            playerObject.AddComponent<HardwareMouseAim>();
            Player = playerObject.AddComponent<PlayerController>();
            GameObject facing = RuntimeVisuals.Primitive("Casting Focus", PrimitiveType.Sphere, new Vector3(0f, 1.3f, 0.65f),
                Vector3.one * 0.24f, Color.white, playerObject.transform);
            facing.transform.localPosition = new Vector3(0f, 0.35f, 0.7f);
            RuntimeVisuals.RemoveCollider(facing);

            foreach (AudioListener existing in Resources.FindObjectsOfTypeAll<AudioListener>())
                if (existing != null && existing.enabled) existing.enabled = false;
            GameObject cameraObject = new GameObject("Isometric Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.025f, 0.03f, 0.055f);
            camera.fieldOfView = 48f; camera.nearClipPlane = 0.1f; camera.farClipPlane = 160f;
            cameraObject.AddComponent<AudioListener>();
            IsometricCamera rig = cameraObject.AddComponent<IsometricCamera>();
            rig.target = playerObject.transform;
            cameraObject.tag = "MainCamera";
            if (_environment != null) ProceduralLightingDirector.Apply(_environment, CurrentRoom == null ? new Color(0.2f, 0.85f, 0.75f) : CurrentRoom.accentColor);
            RefreshEquipmentVisuals();
        }

        private void RefreshEquipmentVisuals()
        {
            if (Player == null) return;
            if (_equipmentVisuals != null) Destroy(_equipmentVisuals.gameObject);
            _equipmentVisuals = new GameObject("Equipment Silhouette").transform;
            _equipmentVisuals.SetParent(Player.transform, false);
            int index = 0;
            foreach (KeyValuePair<EquipmentSlot, ItemInstance> pair in Equipment.equipped)
            {
                ItemDefinition item = pair.Value.Definition;
                if (item == null) continue;
                float angle = index++ * 137.5f * Mathf.Deg2Rad;
                Vector3 local = new Vector3(Mathf.Cos(angle) * 0.48f, 0.15f + (index % 3) * 0.32f, Mathf.Sin(angle) * 0.48f);
                GameObject visual = RuntimeVisuals.Primitive(item.displayName + " Visual", PrimitiveType.Cube, Player.transform.position,
                    Vector3.one * (item.rarity == ItemRarity.Unique ? 0.22f : 0.14f), item.color, _equipmentVisuals);
                visual.transform.localPosition = local;
                visual.transform.localRotation = Quaternion.Euler(20f, index * 31f, 15f);
                RuntimeVisuals.RemoveCollider(visual);
            }
        }

        private void SetupLighting(Color accent)
        {
            ProceduralLightingDirector.Apply(_environment, accent);
        }

        private static int SlotIndex(SpellSlot slot)
        {
            int index = (int)slot;
            return index >= 0 && index < 3 ? index : -1;
        }

        private void CompletePendingSpellSystemReward()
        {
            RunDirector run = _runDirectorResolved ? _cachedRunDirector : GetComponent<RunDirector>();
            if (run != null)
            {
                run.SaveRunCheckpoint();
                run.ResumeAfterSpellSystemReward();
            }
        }

        private static bool TryMapLegacyLink(string modifierId, out SpellLinkCondition condition)
        {
            if (modifierId == "trigger_slot2") { condition = SpellLinkCondition.OnHit; return true; }
            if (modifierId == "trigger_slot1") { condition = SpellLinkCondition.OnKill; return true; }
            if (modifierId == "trigger_slot3") { condition = SpellLinkCondition.OnCast; return true; }
            if (modifierId == "trigger_expire") { condition = SpellLinkCondition.OnExpire; return true; }
            condition = SpellLinkCondition.OnHit;
            return false;
        }
    }

    public sealed class DungeonObstacle : MonoBehaviour
    {
        private static readonly List<DungeonObstacle> Active = new List<DungeonObstacle>();
        public float radius = 1f;
        private void OnEnable() { if (!Active.Contains(this)) Active.Add(this); }
        private void OnDisable() { Active.Remove(this); }

        private void OnDrawGizmos()
        {
            if (!Debug.isDebugBuild) return;
            Gizmos.color = new Color(0.15f, 0.9f, 1f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public static Vector3 Resolve(Vector3 position, float actorRadius = 0.55f)
        {
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                DungeonObstacle obstacle = Active[i];
                if (obstacle == null) { Active.RemoveAt(i); continue; }
                Vector3 delta = position - obstacle.transform.position; delta.y = 0f;
                float minimum = obstacle.radius + actorRadius;
                if (delta.sqrMagnitude >= minimum * minimum) continue;
                Vector3 direction = delta.sqrMagnitude < 0.001f ? Vector3.forward : delta.normalized;
                position = obstacle.transform.position + direction * minimum;
                position.y = 1f;
            }
            return position;
        }
    }

    public sealed class RoomHazard : MonoBehaviour
    {
        private float _radius;
        private float _tick;

        public static RoomHazard Create(Vector3 position, float radius, Color color, Transform parent)
        {
            GameObject go = new GameObject("Room Hazard");
            go.transform.position = position;
            go.transform.SetParent(parent, true);
            LineRenderer ring = RuntimeVisuals.Ring("Hazard Ring", position, color, radius, 0.18f, go.transform);
            ring.transform.localPosition = Vector3.zero;
            RoomHazard hazard = go.AddComponent<RoomHazard>();
            hazard._radius = radius;
            return hazard;
        }

        private void Update()
        {
            if (GameWorld.Instance == null || !GameWorld.Instance.RunActive || GameWorld.Instance.ModalOpen) return;
            _tick -= Time.deltaTime;
            PlayerController player = GameWorld.Instance.Player;
            if (_tick <= 0f && player != null && (player.transform.position - transform.position).sqrMagnitude < _radius * _radius)
            {
                _tick = 0.75f;
                player.TakeDamage(7f, "Dungeon hazard");
            }
        }
    }

    [DefaultExecutionOrder(500)]
    public sealed class IsometricCamera : MonoBehaviour
    {
        public Transform target;
        private float _yaw;
        private float _distance = 14f;
        private float _height = 17f;
        private float _shake;
        private float _shakeUntil;
        private float _nextShakeRequest;
        private int _shakePriority;
        private readonly RaycastHit[] _obstructionHits = new RaycastHit[24];
        private readonly List<Renderer> _hiddenObstructions = new List<Renderer>();

        public Vector3 PlanarForward
        {
            get
            {
                Vector3 value = Quaternion.Euler(0f, _yaw, 0f) * Vector3.forward;
                value.y = 0f;
                return value.normalized;
            }
        }

        public Vector3 PlanarRight { get { return Quaternion.Euler(0f, _yaw, 0f) * Vector3.right; } }

        public void Shake(float amount)
        {
            Shake(amount, target == null ? transform.position : target.position, 1, 0.16f);
        }

        public void Shake(float amount, Vector3 source, int priority, float duration)
        {
            if (ProfileManager.Current == null || ProfileManager.Current.accessibility.reducedMotion || ProfileManager.Current.accessibility.screenShake <= 0f) return;
            if (Time.unscaledTime < _nextShakeRequest && priority < _shakePriority) return;
            float distance = target == null ? 0f : Vector3.Distance(target.position, source);
            float attenuation = Mathf.Clamp01(1f - distance / 24f);
            float resolved = Mathf.Max(0f, amount) * Mathf.Lerp(0.2f, 1f, attenuation) * ProfileManager.Current.accessibility.screenShake;
            if (Time.unscaledTime < _shakeUntil && priority < _shakePriority && resolved <= _shake) return;
            _shake = Mathf.Max(_shake, resolved);
            _shakePriority = Mathf.Max(priority, Time.unscaledTime < _shakeUntil ? _shakePriority : 0);
            _shakeUntil = Mathf.Max(_shakeUntil, Time.unscaledTime + Mathf.Clamp(duration, 0.05f, 0.8f));
            _nextShakeRequest = Time.unscaledTime + 0.025f;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            if (GameWorld.Instance != null && !GameWorld.Instance.ModalOpen)
            {
                float wheel = ArcaneInput.MouseScrollY;
                if (Mathf.Abs(wheel) > 0.01f)
                {
                    _distance = Mathf.Clamp(_distance - wheel * 1.15f, 9f, 20f);
                    _height = Mathf.Lerp(11.5f, 23f, Mathf.InverseLerp(9f, 20f, _distance));
                }
                AccessibilitySettings accessibility = ProfileManager.Current.accessibility;
                if (accessibility.cameraMode != 2 && accessibility.cameraRotationEnabled && ArcaneInput.GetMouseButton(2))
                    _yaw += ArcaneInput.MouseDeltaX * 3.2f * accessibility.cameraRotationSensitivity;
                if (ArcaneInput.GetKeyDown(KeyCode.R)) _yaw = 0f;
            }
            Vector3 jitter = _shake > 0.001f && !ProfileManager.Current.accessibility.reducedMotion ? UnityEngine.Random.insideUnitSphere * _shake : Vector3.zero;
            float decay = Time.unscaledTime < _shakeUntil ? 1.5f : 5f;
            _shake = Mathf.MoveTowards(_shake, 0f, Time.unscaledDeltaTime * decay);
            if (_shake <= 0.001f) _shakePriority = 0;
            Vector3 offset = Quaternion.Euler(0f, _yaw, 0f) * new Vector3(0f, _height, -_distance);
            Vector3 desired = target.position + offset + jitter;
            float follow = ProfileManager.Current.accessibility.cameraMode == 0 ? 1f : 1f - Mathf.Exp(-8f * Time.unscaledDeltaTime);
            transform.position = Vector3.Lerp(transform.position, desired, follow);
            transform.rotation = Quaternion.LookRotation((target.position + Vector3.up * 0.5f) - transform.position, Vector3.up);
            UpdateObstructions();
        }

        private void UpdateObstructions()
        {
            RestoreObstructions();
            if (ProfileManager.Current == null || !ProfileManager.Current.accessibility.cameraObstructionFade || target == null) return;
            Vector3 origin = target.position + Vector3.up * 0.8f;
            Vector3 delta = transform.position - origin;
            float distance = delta.magnitude;
            if (distance <= 0.1f) return;
            int count = Physics.RaycastNonAlloc(origin, delta / distance, _obstructionHits, distance, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
            {
                Transform hit = _obstructionHits[i].transform;
                if (hit == null || hit == target || hit.IsChildOf(target) || hit.GetComponentInParent<EnemyController>() != null) continue;
                Renderer renderer = hit.GetComponent<Renderer>();
                if (renderer == null || _hiddenObstructions.Contains(renderer)) continue;
                renderer.enabled = false;
                _hiddenObstructions.Add(renderer);
            }
        }

        private void OnDisable() { RestoreObstructions(); }
        private void OnDestroy() { RestoreObstructions(); }

        private void RestoreObstructions()
        {
            for (int i = 0; i < _hiddenObstructions.Count; i++)
                if (_hiddenObstructions[i] != null) _hiddenObstructions[i].enabled = true;
            _hiddenObstructions.Clear();
        }
    }
}
