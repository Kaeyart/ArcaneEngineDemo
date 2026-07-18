# Arcane Engine Demo 2.1 — Unity Acceptance Checklist

Record Unity version, commit, OS, GPU, controller, resolution and date with every run. Do not mark a row passed from source inspection alone.

## Automated commands

Run `Tools/run-tests-linux.sh /path/to/Unity` from the project root. It writes Edit Mode and Play Mode XML/logs to `V21_TEST_RESULTS`.

Run the editor menu `Arcane Engine > 2.1 > Validate Promise Completion Source`. Preserve `V21_VALIDATION_REPORT.txt`.

## Manual critical path

- [ ] Clean import produces no compiler errors and no relevant warnings.
- [ ] Main scene contains exactly one `MainCamera`-tagged camera during play.
- [ ] Title → New Expedition → starting spell selection works with mouse/keyboard.
- [ ] The same flow works with a physical controller.
- [ ] Mouse aim covers every direction independently of movement; projectiles follow the combat-plane cursor point.
- [ ] Twin-stick aim covers every direction and honors response/dead-zone/assist settings.
- [ ] Clear room → collect reward → inspect three door icons → press Interact → next room.
- [ ] Workshop opens only outside active combat; drag, rotate, click-place, undo, redo, remove and save all work.
- [ ] Invalid Rune drops show a specific reason and never consume the Rune.
- [ ] Every visible Spell Link event fires as described; an abusive cycle stops within its displayed budget.
- [ ] Equipment cannot change during a run; Run Bag salvage works; extracted items reach the Stash.
- [ ] Armorer affix reroll preserves locks/rarity and charges once; save/continue cannot repeat a sold offer.
- [ ] Health bars remain attached at 30, 60, 120 and uncapped frame rates, including spawn/despawn and camera rotation.
- [ ] All four boss identities and all eight miniboss families execute their distinct mechanics.
- [ ] Training generates no rewards, permanent stats, achievements or drops.
- [ ] Profile export/import, duplicate, backup restore and typed delete each preserve the last valid copy.
- [ ] Continue succeeds through eight title-to-room-entry cycles.
- [ ] Every accessibility option visibly or behaviorally changes its named consumer.
- [ ] No persistent hum or stuck looping source remains after room, pause, focus or scene transitions.
- [ ] Ordinary combat, maximal spell stress, crowded Elite, boss, Workshop and Inventory profiler captures meet the chosen hardware budget.

## Evidence to retain

- Clean import Editor log
- Edit Mode and Play Mode XML plus logs
- `V21_VALIDATION_REPORT.txt`
- Save corruption/migration matrix
- Keyboard/mouse/controller matrix
- Accessibility consumer matrix
- Shop exploit matrix
- 50-room traversal report
- Legendary and enemy coverage reports
- Visual comparison captures
- Profiler captures and hardware specification
- Known issues and signed release decision

