# Combat and final examiner — 2026-11-21


## Pacing update — 1.7.1, 2026-11-22

[Completion_Difficulty_1.7.1.md](Completion_Difficulty_1.7.1.md) records exact tuning: opening0.6s, windups0.8/0.7/0.65s, recoveries1/1.1/1.2s, accepted-hit recovery0.45/0.5/0.55s and two low throws in stage1. Stage2 retains mixed-height two-shot volleys and stage3 compact triples; each volley uses its warned lane. Preparation hit counts stay5,6→4 and5/6/8. Flicker and a localized recovery cue explain why rapid throws do not all deal damage. No invisible windup immunity is added. Removed Prepod’s standalone victory fallback: only the stage3 campaign owner can complete the semester, and exploration remains available afterward.


## Three-stage campaign — 1.7.0, 2026-11-22

[Campaign_1.7.0.md](Campaign_1.7.0.md) supersedes the automatic two-exam progression below. Teachers now require explicit admission and are idle/damage-immune before it. Stage1 uses5 hits/low singles, stage2 uses6 hits/mixed two-shot bursts (carried notes can reduce its total to4 while preserving damage), and stage3 uses5/6/8 hits at2/1/0 readiness with longer alternating volleys. Guard confiscation cancels/reset the second exam and clears its grades; only stage3 calls victory. New enclosed rooms use original pixel artwork, finite supplies, shared music and camera bounds. Gameplay remains user-tested after source checks.

## Behavior and architecture

`Prepod` owns one combat state machine for both encounters. The first teacher warns for one second before a single low throw. The final examiner in orange/red alternates low and high two-projectile volleys: 0.9 seconds warning, 0.18 seconds between projectiles, then at least 1.4 seconds recovery. Low means jump; high means hold crouch. The teacher flashes red for 0.16 seconds when hit, including the first teacher whose normal sprite tint is white. Character damage protection and player hit flashes remain in Character.

The warning is three small white attention marks directly above the teacher, with a subtle tint pulse during wind-up. `AttackWarningKey` supplies `combat.jump` or `combat.crouch` for the localized HUD, which explains the required dodge. No warning ray/laser crosses the classroom. Direction locks toward the student's position when the warning begins and remains horizontal throughout the volley. Attacks stop when the player leaves a 14-unit horizontal / 4-unit vertical neighborhood. Timers and projectile movement stop whenever GameSession blocks gameplay. Leaving the neighborhood cancels the pending volley and resets a 1.2-second approach grace period.

Lanes use the teacher collider's world-space bottom plus 0.8 or 2.55 units. The scene's student collider is approximately 2.81 units tall standing and 2.11 units crouching (75% height), matching the natural crouch pose. Grade projectiles have radius 0.22, matching smaller visible artwork, so the high lane's lower edge is 2.33 units above the floor: it clears crouching while intersecting standing. Both lane heights and dodge timing require the user's actual gameplay verification.

`CombatProjectile` centralizes motion, lifetime, sprite access and a once-only collision guard. BulletLab targets teachers and retains speed 10 / lifetime 1.4 seconds. BulletOcenka targets Character and uses speed 8 / lifetime 2.1 seconds when configured by Prepod. Each projectile is consumed even if Character's temporary damage protection ignores that hit. Defeating a teacher clears its remaining grades before rewarding or ending the run. No per-frame scene searches or allocations are added; projectile cleanup searches only once per teacher defeat.

The final examiner uses the existing `Assets/Graphic/other_characters/woomen_in_red(remastered).png` artwork through `Resources/Combat/ExaminerAppearance.asset`, a ScriptableObject reference to the original Sprite (GUID `4bfd32eb16d930249b65d511500a1ad1`). No bitmap is duplicated. The final has a different silhouette and its own alternating dodge pattern, while retaining the same collider and combat controller.

`ExaminerAppearance.Apply` disables animation that could overwrite the appearance, hides the cloned first-teacher renderer and creates one upright child renderer. The original child is rotated approximately 89.82 degrees and cannot safely be reused directly. The original collider is 2.9492474 units tall, center y=1.3990569, bottom y=-0.0755668. At 100 pixels/unit with centered pivot, the replacement art's visible bottom is -1.32 and height is 3.84 units (bottom-origin visible pixels x=239..361, y=218..602). Uniform scale is approximately 0.76803, with child offset approximately (0.38402,0.93824), placing visible feet on the collider bottom. These calculations use collider size/offset, which remain valid while the final clone is inactive, rather than inactive collider bounds. The artwork's original author/license attribution remains unresolved and must be confirmed with the university team before distribution.

## APIs and save boundaries

- `Prepod.IsTelegraphing`, `AttackWarningKey`, `TelegraphSecondsRemaining`; `AttackChanged` is an `Action` emitted at warning start/end. HUD consumers rebind on `ExamProgression.Changed`.
- `Prepod.HealthChanged`, `RemainingLabs`, `RequiredLabs`, `RestoreHealth(int)`; restoring resets the pattern and gives 1.2 seconds before the next warning.
- `Configure(int labs, float shotInterval, Color color, bool isFinalExam = false)` preserves existing callers.
- ExamProgression implements `IRunSaveParticipant`, restore order 30. `Capture` records stage, teacher health/requirement and the between-exams flag. `RestoreProgress(int stage, int health)` selects the saved teacher without giving rewards again.
- The first completion awards +3 Sleep / +1 Intelligence, sets stage two and BetweenExams, then requests a checkpoint. A transition snapshot stores the final examiner at full health. Restoring it begins the final encounter immediately with fresh warning grace; no in-flight projectiles, partial timers or transition delay are restored.
- Nonlethal teacher hits and final activation request autosaves. Victory uses GameSession's completion flow without creating another checkpoint.

## Exact user verification (not executed by the agents)

Use the default keyboard controls below, or the action names shown in Controls if rebound.

1. Approach the first teacher. Expect three compact white marks above the teacher, a subtle teacher tint pulse and a localized jump warning, followed about one second later by one grade. There should be no long yellow line across the game. Press Jump (Space by default) near the end of the warning. Expect a successful jump to avoid damage. Stay standing for another low throw: Intelligence should decrease once, followed by visible temporary protection.
2. During a warning, press Escape to pause, wait, then Resume. Expect no grade movement or attack timer progress during pause; the warning should resume from its previous timing.
3. Face the teacher. Select a collected lab with the inventory's previous/next controls, then press F (controller north face button) to use it; alternatively press its inventory number. Ordinary Throw uses Left Ctrl / left mouse button (controller right trigger) and costs Sleep. Expect one health decrement and a brief red teacher flash. Open the menu and Continue: expect the same remaining teacher health and a fresh warning sequence.
4. Defeat teacher one. Expect one +3 Sleep / +1 Intelligence reward, a short transition, then the visibly different examiner in orange/red with seven required hits. Check that her feet meet the floor and she remains upright. Expect no leftover first-teacher grades.
5. In the final, jump over the low two-shot volley. On the following HUD crouch warning, hold Down/S (or controller down) until both high grades pass. Expect no damage while crouched on the same floor. Release crouch and intentionally remain standing during the next high volley: expect a hit. The pattern alternates low/high with a recovery gap for counterattacks.
6. Return to the menu during the transition or final encounter and Continue. Expect stage two, saved health and inventory/stats, without another first-exam reward. A saved transition starts the final immediately. Retry from the checkpoint after a loss should restore the first-pass reward and final at full health.
7. Defeat the final examiner. Expect the victory result once. Restart starts a fresh run; completed-run Continue availability follows the save/menu rules.
8. Walk away from the teacher into a different room, then return. Expect no offscreen volleys while away and an approach grace period before a fresh warning. Repeat combat using the configured controller actions and in a standalone build.

## Review status

Source flow inspected for destroyed teachers, duplicate impact callbacks, cloned warning sprite ownership, stage restore/reward replay, paused timers, and projectile cleanup. Each teacher owns one runtime white sprite shared by its three warning marks; the final clone rebinds its marks to its own sprite before the first teacher is destroyed. Marks reuse the teacher material and sorting layer. Runtime, controller, build rendering and combat balance are intentionally awaiting user verification.

## Feedback follow-up — 2026-11-21

The user found the yellow classroom line confusing and asked what it meant. That world-space warning line was introduced by the modernization work and was not original game artwork. It has been removed entirely from Prepod; compact attention marks identify the attacking teacher while the localized HUD supplies the actionable instruction. Attack delays and the alternating final pattern are unchanged. The high lane was raised from 2.2 to 2.55 to retain clearance for the revised natural crouch pose/collider (75% of standing height); the low lane remains 0.8.

The user also reported that using F feels slow. Source tracing finds no imposed item-use cooldown: the UseItem action has no Hold/SlowTap interaction, GameInput uses WasPressedThisFrame, InventoryController immediately calls UseSlot, and the collected lab immediately calls Character.ThrowLab/SpawnLab. One press uses one selected item; holding F does not auto-repeat. A lab travels at 10 units/second, so a teacher 10 units away is hit roughly one second after the press. These facts do not establish the user's actual runtime cause; user verification should distinguish immediate item consumption/projectile appearance from delayed target impact, and confirm that a lab is the selected item. No item-use code or timing was changed in this follow-up.


## Supply replenishment clarification — 2026-11-22

The user asked how to get more labs and whether energy drinks respawn over time. Source review confirms finite authored pickups: LootBoxItem registers a persistent ID; InventoryController.Add records collection and removes/deactivates the world item; RunSaveService restores collected IDs and hides those pickups. There is no timed respawn/cooldown implementation for labs, food, books or drinks. Continue does not restock them. A new run resets world pickups; checkpoint retry restores only the pickup/inventory state from that checkpoint.

Players can collect remaining lab pickups with Interact and throw the selected collected labs with F/UseItem at no Sleep cost. Left Ctrl/left mouse/RT creates a fresh projectile directly, spending one Sleep per successful throw; it does not refill the inventory or require a collected lab. Energy drinks are collected once, selected and used as consumables; waiting after consumption does not recreate their world pickup. The question is recorded as a discoverability/resource-balance concern, not authorization to implement a respawn system.
