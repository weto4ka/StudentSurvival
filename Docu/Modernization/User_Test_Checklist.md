# User testing and feedback

Date: 2026-11-22. Implementation version: 1.6.0. Status: awaiting user testing.

The agents did not enter Play Mode, execute a test suite or build/launch a player. All eleven source assemblies compiled against the installed Unity references. That does not establish runtime correctness. Use your remapped binding wherever the steps list a default button.

## First check for 1.6.0 — 2026-11-22

1. Stop Play Mode and allow import/compilation. Start again and confirm1.6.0. Inspect Help/Controls/HUD in the same small Game view as the screenshots, then at1280×720/1920×1080. Expect larger physical body text, generous line spacing, square-cell back arrows, no overlapping objective/route/cue rows and no German OS key names in an English interface.
2. New game: move right from the starting position toward the marked ground lab reports. Press E/X to collect. Expect the pickup's slot selected automatically and a hint naming exactly what F/Y uses. Brackets/LB/RB change selection. F/Y uses food if food is selected; it does not mean unlimited attack.
3. Hold F/Y with several collected reports selected. Expect immediate first use then one report every0.18s; Sleep stays unchanged. Release stops it. Holding F on food/drinks/books must consume at most one. At empty/depleted lab selection, expect clear feedback and no paid fallback. Hold Ctrl/LMB/RT to throw fresh reports at0.18s for1 Sleep each. Click/hold a UI control then move into the world: no paid throw until mouse release and a new press.
4. Crouch both directions: expect an upright15°-leaning squat with knees forward, flat shoes, connected body parts and smooth standing recovery. The collider is75% of standing. Release under a ceiling: stay crouched until headroom allows standing. Jump, apex and land: legs must stay attached, retain layering and blend without the former detached appearance. Offline previews are supporting evidence only; actual frame/collision appearance must be checked here.
5. Approach the teacher. There must be no long yellow line. Compact marks/tint prepare a throw; readable JUMP/CROUCH plus the effective binding tells the dodge. High grades should pass over crouch and hit standing; low grades require jumping. Route help should hide while an attack/protection/resource warning occupies its region.

## 1. Import, startup and presentation

1. Stop Play Mode. Focus Unity and let import/compilation finish; use Assets > Refresh if needed. Console should have no red errors. Localization setup should report seven locales/Game tables and add absent entries (164 total seed keys). Existing translator table edits remain intact. Send the first complete red error/stack trace if compilation fails.
2. Press Play while Main is open. Expect the main menu first: minimal/mysterious student title illustration, Continue, New game, How to play, Credits, Quit and a corner gear. Continue is completely hidden without an unfinished save; other actions reflow and controller focus starts on New game. With a valid saved run, Continue is visible and restores it. Version reads 1.6.0. There should be one interactive menu.
3. Open Settings with the upper-right gear. Expect aligned labels, a native-name language dropdown, Master/UI sounds/text-size controls and Window mode. Back is an upper-left arrow. Pause/settings stay translucent.
4. Select English, Ukrainian, Russian, French, Italian, German and Spanish. Check the menu, briefing, settings, credits, warnings and results for raw keys, missing glyphs, clipped text or visible format placeholders. Try maximum text size and 1920×1080, 1280×720 and 4:3. Esc/controller B closes an expanded dropdown before leaving settings.
5. Hover/click buttons and pause/resume in-game. Expect quiet appropriate cues even when game audio is paused. Set UI sounds to 0 then about 60%, Master to 0 then about 80%. Existing music remains; report balance/loop issues. Window mode is intentionally disabled in the Editor; verify it later in a player.
6. Open Credits. Expect pixel-font section headings, one contributor per row, wrapped secondary detail rows and the pending attribution note in a scrollable panel. Text must not overlap or shrink into unreadable blocks. There is one upper-left back arrow. Supply preferred names/roles and original art/music/video creator/source/license information separately.

## Menu repair checks — 1.5.0

1. Stop Play Mode, refresh/import and start again. The menu camera must no longer report removed GUI Layer or missing component index2 warnings. Preserve/send any different first error rather than assuming it has the same cause.
2. With no resumable save, Continue must be absent, not greyed out. With a valid unfinished save, it appears. Start a real run, return to menu and confirm Continue remains available for that run; this fix must not delete progress.
3. Main and Pause have distinct How to play and Controls entries. How to play contains only the sectioned game guide; Controls contains Keyboard/mouse and Controller tabs plus editable bindings. Its heading must read Controls in the selected language. Settings links directly to Controls.
4. Ordinary help, credits, settings and controls have one upper-left back arrow and no duplicate bottom Back. New-game/retry confirmations have one cancellation arrow plus one commit action. A fresh-run guide has one Let's go action and no duplicate arrow. Check Esc/controller B returns once to the actual prior page.
5. Hover several buttons, then navigate them with keyboard/controller. Expect an obvious brighter green fill and an independent bright three-pixel border around the current control. Test dropdown options and scrollbar/slider handles as well. Borders should not steal pointer events or leave the previously selected button highlighted.
6. Use Master about80% and UI sounds about60%; ensure Unity's Game-view mute toggle is off. Hover should play a short audible blip and clicking a distinct click, not spoken narration. Pause/resume cues should remain audible while game audio is paused. A zero UI slider intentionally mutes these cues.
7. Review Help/Credits in all seven languages at1280×720 and1920×1080, including maximum text size. Expect genuine pixel-font glyphs, readable wrapped paragraphs and section spacing. Scroll with wheel and controller/keyboard focus on the vertical scrollbar; verify B/Esc exits. After rebinding or changing input device, Help's binding labels should update.

## 2. New game, first minute and controls

1. Choose New game. With an unfinished save, Back must cancel replacement and leave Continue intact; choosing New game in the confirmation deliberately replaces it. A fresh run opens a sectioned scrollable guide with one Let's go action. Ordinary How to play uses one top-left back arrow and has no nested How to play/Controls button.
2. Expect Sleep 10/Intelligence 5, compact translucent stats, short-lived objective/help and an arrow toward the classroom through actual open doors. After about 90 seconds the route cue disappears. Collect supplies with E/X; use the displayed binding at doors. Report a specific room/door if the arrow is confusing or wrong.
3. Pause > Controls (a separate entry from How to play). In Keyboard/mouse, change Jump from Space to J. Back to game and verify J; reopen controls and verify its label, then re-enter Play Mode and check persistence. Restore defaults when finished. Remapping Interact/Fire must update contextual prompts/warnings.
4. Try a duplicate binding. Expect rejection and the original assignment retained. Esc or ten seconds cancels capture; no double-back navigation. Controller capture also offers mouse Cancel. Focused binding rows should scroll into view. Pause/Restart may share a binding because their contexts differ.
5. Controller defaults: south/A jump, west/X interact, RT paid throw, north/Y use selected item, LB/RB previous/next occupied item, Select/View inventory, Start pause; stick/D-pad down crouches. Standard menu navigation uses stick/D-pad, A submit and B back. Test one controller rebind and restore defaults.

## 3. Movement, crouch, inventory and doors

1. Hold S/Down or controller down. Expect bent knees/leaning torso with normal head/body proportions and a small shuffle while moving. Feet should stay grounded. Check left/right facing, jump/standing transitions, damage flash and paused shuffle. Under a low solid ceiling, releasing crouch must keep the smaller collider/pose until you move clear.
2. Walk/run, jump near ledges and land. Check walls/camera and buffered jumps. Pickups/triggers must not allow extra airborne jumps. Movement speed decreases when Sleep is spent and when crouching.
3. Approach an open door; press and hold Interact while staying inside its trigger for two seconds. Expect one transfer, not a reset countdown. Walk away during another attempt: it cancels. Locked decorative doors show a message and no destination error.
4. Collect several identical items. Expect stacking and one target per press. Occupied inventory slots appear briefly and fade after about four seconds; Tab/Select pins them. Brackets/LB/RB select an occupied item; F/Y uses it. Number shortcuts or clicking a slot use that slot directly. Hover/selection shows the effect.
5. Book adds 1 Intelligence, shawarma 1 Sleep, energy drink 3 borrowed Sleep plus timed stimulation and crash (see section 7). Throw a fresh lab with Ctrl/LMB/RT: Sleep drops by exactly 1. Use a collected lab via its slot/F/Y: one lab is spent and Sleep stays unchanged. Clicking gear/slots must not also fire a paid lab.

## 4. Autosave, menu and death rules

1. Collect at least two item types, use one, and note counts/stats/position. Pause > Main menu > Continue. Expect the same counts, used item state, stats and location (last grounded position if you left in the air). Collected world items must remain absent; no duplication.
2. Repeat after closing/relaunching the player, and after leaving/re-entering Editor Play Mode. Language, audio and bindings also remain saved. The local run file is under Application.persistentDataPath as student-survival-run.json with a .bak sibling.
3. Transfer through a door to establish a checkpoint. Then collect/use something and lose Sleep or Intelligence. Force a loss by spending all Sleep or taking enough hits. On the result choose Retry checkpoint. Expect the door-arrival checkpoint state, undoing later pickups/uses/damage together. Main menu > Continue after death must restore that same checkpoint.
4. Save while crouched under a low ceiling, return to menu and Continue. Expect the crouched collider/pose until headroom is clear. Save during a jump: restore at a grounded position, not an airborne apex. Try menu exit immediately after a door transfer.
5. In pause, Retry checkpoint must warn that later progress rolls back; Back cancels. New game in the menu warns before replacing an unfinished run. Settings/input preferences survive either operation.
6. If a save error occurs, expect a visible readable message. A failed write must not silently leave gameplay for the menu; restore failure keeps partial state blocked but permits returning to menu and retrying. Report the full warning and what action triggered it; do not delete your save while diagnosing.

## 5. Combat and complete run

1. Near the first teacher, expect a nearby exam meter, a thin low lane and a localized jump warning showing your real Jump binding before each grade. Lab hits reduce health exactly once and briefly tint the teacher red. Five hits pass the first exam in Main.
2. Let a grade hit. Expect one Intelligence lost, red/flickering student and a brief protection label. Closely spaced grades during the grace interval should not repeatedly drain Intelligence. Pause during a warning/projectile: time, grade motion/lifetime and animation must freeze, then resume.
3. First exam success gives +3 Sleep/+1 Intelligence once and creates a checkpoint. The final examiner appears after two seconds, with a different upright orange/red character sprite, feet aligned with the floor, seven hits required, alternating low/high warnings and two-shot bursts. Jump low throws; hold crouch for high throws. Report warning clarity, dodge fairness and resource balance.
4. During exam two, return to menu > Continue. Expect stage two and remaining examiner health; the first-exam reward must not repeat. Die or choose Retry checkpoint: expect the post-first-exam checkpoint, full final-exam health and the already-awarded recovery once.
5. Defeat the final examiner. Expect victory and stopped simulation. Main menu must remove Continue; New game starts fresh. Result restart starts a new game after victory and retries a checkpoint after defeat.
6. On separate runs, spend Sleep to zero, then take Intelligence to zero with time between hits. Expect the correct localized loss reason and no gameplay actions after results. Verify mouse, keyboard result shortcut and controller result navigation.

## 6. Windows player checks

Build to a fresh folder; preserve archived builds. Verify menu-first startup, all seven languages/Addressables content, real Windowed > Borderless Fullscreen > Windowed changes, audio levels, quit/relaunch Continue and controller navigation. The Editor Game-view maximize function is separate from player fullscreen. Check usb_soft.mp4 for any remaining media warning; its missing color metadata was repaired without re-encoding in 1.2.0.

## 7. Distractions, energy rescue, jumping and banner choice

1. At the start, pass the friendly woman student near x=-4.6 without interacting. Expect no lost labs/stats. With no collected lab, Interact should show a no-lab message without spending Sleep. Collect a lab from the starter supplies, return and press Interact: exactly one lab disappears, a brief heart/love-letter joke appears, no exam projectile is fired. The prompt must disclose the cost. Repeated interaction cannot spend more; menu > Continue preserves that completed choice. Checkpoint retry uses the choice state at that checkpoint.
2. Select an energy drink before exhausting Sleep. Spend Sleep down to zero. Expect no immediate defeat: inventory stays visible and a red eight-second rescue countdown appears. Use the drink with F/Y or its slot before time runs out. Expect +3 temporary Sleep, a 20-second stimulation label and noticeably faster/twitchier horizontal movement. Intelligence zero still ends the attempt immediately.
3. Try using another drink while stimulated. Expect a readable refusal, unchanged item quantity and no extension. Pause for several real seconds: the countdown and movement must freeze. Menu > Continue should preserve the remaining effect/debt state.
4. Spend one borrowed Sleep during the boost, then wait for its end without food. Only unused borrowed Sleep should be removed; a player who started from zero returns to zero. Expect a 10-second crash with slower movement/sluggish horizontal response and an eight-second rescue window. Food or a fresh drink can rescue you. A drink during the crash starts another boost, then another crash; active boosts never stack.
5. Repeat, but eat shawarma during stimulation. After expiry, that permanent +1 Sleep must remain. Books restore Intelligence, not Sleep. Normal food never causes caffeine side effects. Wait out the crash with positive Sleep and verify normal response returns; base speed has not permanently increased.
6. Allow the zero-Sleep rescue countdown to expire without supplies. Expect the revised loss reason and checkpoint retry. A door used during zero-Sleep grace must preserve the active save but retain the earlier positive-Sleep checkpoint. Continue while boosted, in a crash, and during rescue: compare displayed timers/quantity, checking no free renewed 20-second effect.
7. Jump from standing and while moving in both directions. Expect knees tucked during ascent, gradually extended bent knees on descent and a brief landing bend. Head/torso should keep normal proportions. Pause midair and check that pose/recovery freezes; crouch on landing should flow into the existing low pose without a duplicate sprite or straight-leg flash.
8. Settings > Title artwork defaults to Minimal / mysterious. Switch to Detailed illustration and back: menu backdrop changes immediately and preserves aspect ratio. Leave/re-enter Play Mode or relaunch and check the saved choice. Review sparse campus silhouettes, palette and readability behind the title/buttons at your resolution; report whether it now matches the game's art direction.

## Feedback to return

Start with sections 1 and 4, then movement/combat. Send the first failing numbered step, keyboard/controller, language/resolution, expected versus actual behavior and first complete Console error/warning. Confirm whether save/retry rules, onboarding, transparency, crouch pose and final-exam timing feel right. Native wording and original asset/music attribution remain team review tasks.


## Entrance regression — 1.6.1, 2026-11-22

Approach the hall left entrance: Go outside should appear with your Interact binding. Press it once, wait0.5 seconds nearby, then walk left outdoors past the column. Return using Enter university and walk right without sticking in the wall. Continue should restore the corrected arrival after either transfer. Stop Play Mode and reimport before checking.


## Ceiling and music regression — 1.6.2, 2026-11-22

See [exact ceiling/music checks](Ceiling_Music_1.6.2.md#user-runtime-checks). Verify repeated classroom/hall jumps stay below the roofs, boss music does not restart with jumping, region changes fade, pause retains musical position, and menu/run transitions do not create duplicate voices.


## Flirt discovery — 1.6.3, 2026-11-22

See [student marker and nearby prompt checks](Distractions.md#visible-flirt-marker-and-hint--163-2026-11-22). Look for the pixel heart before the starter pickups, check the nearby binding/cost hint, then test no-lab and successful use, Continue, language and controller/rebinding. Existing completed saves should have no available marker or paid prompt.


## Clickable creator credits — 1.6.4, 2026-11-22

Main menu > Credits: hover/click all five names and verify exact profiles maximys963, interess, qluckoff, weto4ka and maksa988. Return to the game between clicks. Navigate and Submit using keyboard/controller; off-screen names should scroll into view. Cancel returns normally. Check enlarged text and all languages for wrapped, unclipped clickable names. These external launches have not been runtime-tested by the agent.
