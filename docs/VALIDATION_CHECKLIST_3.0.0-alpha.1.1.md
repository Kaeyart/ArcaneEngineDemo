# Arcane Engine 3.0.0-alpha.1.1 Validation Checklist

## Compilation

- Open Unity 6000.5.2f1.
- Wait for script compilation.
- Confirm the Console contains no compiler errors.
- Run `Arcane Engine > 3.0 > Validate Foundation Hardening 3.0.0-alpha.1.1`.

## Migration

- Load an existing 3.0.0-alpha.1 profile.
- Confirm progression, maps, items, currencies, Cores, Runes, and Links remain present.
- Confirm the saved JSON reports `dataVersion` 30002 after the next save.
- Confirm duplicate currency stacks, if manually introduced in a test copy, merge correctly.

## Determinism

- Apply the same crafting operation to identical test copies of an item in separate editor sessions.
- Confirm the selected result is stable for the same item ID, currency, and state.
- Launch multiple free Tier 0 maps and confirm no negative collection-index exception occurs.

## Fracture

- Apply a Fracture Rune to a Rare item with multiple modifiers.
- Confirm one modifier is marked `[FRACTURED]`.
- Use Alteration, Chaos, Null, Reformation, and Divine operations where legal.
- Confirm the fractured modifier remains present and unchanged by removal/value-reroll operations.
- Confirm a second Fracture Rune is rejected and refunded.

## Regression

- Choose a class, open Tier 0, complete it, return to the refuge, save, quit, and reload.
- Confirm SpellForge discovery restoration still works.
- Confirm the legacy roguelite profile and run entry remain available.
