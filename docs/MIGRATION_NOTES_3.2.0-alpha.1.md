# Migration Notes — 3.2.0-alpha.1

Existing 3.1 characters migrate on first gameplay load.

- Equipped items remain equipped.
- Carried equipment is placed into the 12×5 backpack.
- Existing stash equipment is placed into General I.
- Overflow is placed into Recovery.
- Existing currencies are mirrored into account storage.
- Item instance IDs, rarity, quality, affixes, maps, Cores, Runes, and progression remain intact.
- The 3.2 account file uses atomic writes and a backup.
