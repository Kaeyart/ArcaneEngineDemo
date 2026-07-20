# Arcane Engine 3.0.0-alpha.1.1 Implementation Manifest

## Replaced runtime files

- `ArpgData30.cs`: schema 30002 migration and defensive repair.
- `ArpgItems30.cs`: deterministic crafting and real fractured-affix protection.
- `ArpgFoundation30.cs`: safe deterministic indexing for maps and rewards.
- `ArpgLegacyBridge30.cs`: safe deterministic discovery selection.

## Added runtime file

- `ArpgDeterminism30.cs`: stable FNV-1a hashing, seed combination, positive normalization, and collection indexing.

## Added Editor validation

- `Patch300Alpha11Validator.cs`

## Added marker

- `PATCH_3_0_0_ALPHA_1_1.txt`

## Persistent migration

Loading any 30001 profile advances it to 30002. Existing items marked only with the old `fractured` boolean are migrated by selecting their first valid affix as the protected fractured affix. If no affix exists, the invalid fracture state is removed.
