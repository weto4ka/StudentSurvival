# Combat and final examiner — 2026-11-21

## Behavior and architecture

`Prepod` owns one combat state machine for both encounters. The first teacher warns for one second before a single low throw. The final examiner in orange/red alternates low and high two-projectile volleys: 0.9 seconds warning, 0.18 seconds between projectiles, then at least 1.4 seconds recovery. Low means jump; high means hold crouch. The teacher flashes red for 0.16 seconds when hit, including the first teacher whose normal sprite tint is white. Character damage protection and player hit flashes remain in Character.

The warning is a thin world-space line through the exact throw lane. `AttackWarningKey` supplies `combat.jump` or `combat.crouch` for the localized HUD. Color is supplementary to these action labels. Direction locks toward the student's position when the warning begins and remains horizontal throughout the volley. Attacks stop when the player leaves a 14-unit horizontal / 4-unit vertical neighborhood. Timers and projectile movement stop whenever GameSession blocks gameplay. Leaving the neighborhood cancels the pending volley and resets a 1.2-second approach grace period.

Lanes use the teacher collider's world-space bottom plus 0.8 or 2.2 units. The scene's student collider is approximately 2.81 units tall standing and 1.55 units crouching. Grade projectiles have radius 0.22, matching smaller visible artwork, so the high lane has clearance over crouching. Both lane heights and dodge timing require the user's actual gameplay verification.

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

1. Approach the first teacher. Expect a thin low warning line and a localized jump warning, followed about one second later by one grade. Press Jump (Space by default) near the end of the warning. Expect a successful jump to avoid damage. Stay standing for another low throw: Intelligence should decrease once, followed by visible temporary protection.
2. During a warning, press Escape to pause, wait, then Resume. Expect no grade movement or attack timer progress during pause; the warning should resume from its previous timing.
3. Face the teacher. Select a collected lab with the inventory's previous/next controls, then press F (controller north face button) to use it; alternatively press its inventory number. Ordinary Throw uses Left Ctrl / left mouse button (controller right trigger) and costs Sleep. Expect one health decrement and a brief red teacher flash. Open the menu and Continue: expect the same remaining teacher health and a fresh warning sequence.
4. Defeat teacher one. Expect one +3 Sleep / +1 Intelligence reward, a short transition, then the visibly different examiner in orange/red with seven required hits. Check that her feet meet the floor and she remains upright. Expect no leftover first-teacher grades.
5. In the final, jump over the low two-shot volley. On the following cyan high warning, hold Down/S (or controller down) until both grades pass. Expect no damage while crouched on the same floor. Release crouch and intentionally remain standing during the next high volley: expect a hit. The pattern alternates low/high with a recovery gap for counterattacks.
6. Return to the menu during the transition or final encounter and Continue. Expect stage two, saved health and inventory/stats, without another first-exam reward. A saved transition starts the final immediately. Retry from the checkpoint after a loss should restore the first-pass reward and final at full health.
7. Defeat the final examiner. Expect the victory result once. Restart starts a fresh run; completed-run Continue availability follows the save/menu rules.
8. Walk away from the teacher into a different room, then return. Expect no offscreen volleys while away and an approach grace period before a fresh warning. Repeat combat using the configured controller actions and in a standalone build.

## Review status

Source flow inspected for destroyed teachers, duplicate impact callbacks, cloned telegraph material ownership, stage restore/reward replay, paused timers, and projectile cleanup. Runtime, controller, build rendering and combat balance are intentionally awaiting user verification. The world warning uses the Unity `Sprites/Default` shader and should be checked in the built player as well as the Editor.
