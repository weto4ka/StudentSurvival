# User testing and feedback

Date: 2026-11-21. Implementation version: 1.2.0.

Status: awaiting user testing. The agent has not started Play Mode, run gameplay tests or built/launched a player for this revision. Source consistency and media-metadata/frame checks are separate from a runtime gameplay pass.

## First pass: menu, settings and presentation

1. Stop Play Mode and let Unity compile/import. If red Console errors appear, send the first full error/stack trace before proceeding. Localization setup should add the new keys to all seven Game tables (86 seed keys) and log its setup completion.
2. Press Play while Main is open. Expected: the main menu opens first. It contains Start a run, How to play and Quit, with a small gear at the upper-right. The original university backdrop is retained; the previous opaque modernization panel and baked-lettering buttons should not form a second interactive menu. For intentional direct scene testing, use Tools > Student Survival > Start Play Mode at open scene; switch back with Start Play Mode at menu.
3. Open the gear. Expected: aligned labels/controls, translucent background, a language dropdown, Master volume, UI sounds, Interface size and Window mode. The artwork/video translation note must be absent. Back is an arrow at the upper-left.
4. Use the dropdown to select Ukrainian, Russian, French, Italian, German, Spanish and English. Try both mouse and controller navigation. Expected: readable native language names, visible selected/focused option, no missing glyphs, raw keys or untranslated placeholders. Esc/B should close an open dropdown before leaving Settings.
5. Hover and click controls. Expected: short quiet hover/click sounds. Set UI sounds to 0 and back to about 60%; set Master volume to 0 and back to about 80%. Expected: the respective sounds change, including UI cues while gameplay is paused. Menu music should use the existing track. No new game-music track was substituted.
6. Window mode is disabled in the Editor with an Editor-only explanation. In a built Windows player, switch Windowed -> Borderless Fullscreen -> Windowed. Expected: the actual application window changes mode, returning to its previous window size. This is not an Editor Game-view maximize toggle.
7. Use the back arrow and reopen Settings. Check saved language/volume/text-size after leaving and re-entering Play Mode. Test at 1920×1080, 1280×720 and a 4:3 view, including French/German/Cyrillic at maximum text size.

## Second pass: editable keyboard and controller controls

1. Open How to play from the main menu or Settings. Expected: a short objective explanation and Keyboard / mouse and Controller tabs. Each action shows its actual binding, with a scrollable list. The control names are read from the Input Actions asset, not a hardcoded help paragraph.
2. In Keyboard / mouse, select the Jump binding (Space), press J, and close the page. Start a run and verify that J jumps. Return to the controls screen and confirm J is displayed. Re-enter Play Mode and confirm it remains saved.
3. Try assigning a binding already used by a different action. Expected: a conflict message and the prior binding is retained. Pause and Restart can share a button because they are active in different gameplay contexts.
4. Start a rebind and press Esc, or wait ten seconds. Expected: cancellation without changing the binding or unexpectedly backing out twice. Keyboard/mouse capture uses Esc/timeout so clicking a cancel control cannot accidentally become a mouse binding. Controller capture also permits its on-screen mouse Cancel button.
5. Select Restore defaults. Expected: default labels and behavior return. Scroll through the entire list with keyboard/controller navigation; focused binding buttons should scroll into view.
6. With a controller, verify south/A jump, west/X interact, right trigger throw, north/Y use selected item, shoulders previous/next occupied slot, Select/View inventory and Start pause. Stick/D-pad down crouches. Rebind an action in the Controller tab and verify its label and behavior. Standard menu UI uses stick/D-pad, south/A submit and east/B back.

## Third pass: contextual HUD, crouch and doors

1. Start a run. Expected: Sleep 10 and Intelligence 5 in a small translucent stat display. The objective/start hint appears briefly and fades. No full-width top/bottom opaque bands should obscure the room.
2. Hold S or Down. Expected: shorter collider and existing sprite visual, reduced movement speed, feet remain at ground level. Release to stand. If crouched under a solid low ceiling, releasing must not expand the collider into it; move clear to stand. The initial visual scales existing art rather than using a dedicated crouch animation.
3. Walk/run with A/D/arrows and jump. Check wall collisions, camera follow and a brief forgiving jump near ledges/landing. Pickups/triggers must not provide unlimited airborne jumps.
4. Approach an unlocked door, press and hold its displayed Interact binding, and remain inside its trigger for its configured delay. Expected: exactly one transfer; holding the key does not reset the timer. Walk away during another transfer: it cancels.
5. Approach the two locked decorative doors and interact. Expected: a locked message and no transfer. They should not log destination errors merely because they are locked and have no target. An actually unlocked/misconfigured door should still be diagnosed.
6. Collect an item. Expected: one target responds per Interact press; only occupied slots appear in a compact bar. The bar fades after about four seconds. Tab/Select toggles it open/closed; keyboard slot shortcuts and controller item selection still work while it is hidden.
7. Hover/select an item: its effect hint should appear. Use book (+1 Intelligence), shawarma (+1 Sleep), energy drink (+1 Sleep/+1 base speed) and collected lab (one projectile, no Sleep cost). Same items stack. Clicking an item/gear must not also fire a paid lab.
8. Check an unlocked door/pickup after remapping Interact. Expected: its contextual prompt displays the new binding. Changing between keyboard and controller should refresh the hint labels.

## Fourth pass: pause and complete run

1. Pause during movement/combat. Expected: translucent Paused overlay with the world visible, stopped simulation/audio and an audible pause cue. Resume returns simulation/audio with a resume cue. Use Settings/Controls from pause and ensure Back returns to the page/game that opened it.
2. Reach and fight the first teacher. The exam meter should appear in the teacher's vicinity, not as a permanent distant header. Each lab hit reduces the count exactly once; each grade costs 1 Intelligence with a brief damage grace window.
3. Pass the first exam (5 hits in the shipped scene). Expected: +3 Sleep/+1 Intelligence, a recovery message, and a second examiner after two seconds in the same classroom. The final examiner requires 7 hits and fires orange grades slightly faster.
4. Pass the final exam. Expected: localized victory overlay and stopped simulation. Test Restart and Main menu buttons and result-state bindings. Restart resets stats, inventory, objective and both encounters.
5. On a separate run, fire ten paid labs without restoring Sleep. Expected: Sleep-loss result at 0; no further shooting after the result. Restart must work.
6. On another run, take five grades without using books, allowing the grace interval between hits. Expected: Intelligence-loss result at 0. Test result navigation again.
7. Check existing game music and area effects. Reimported usb_soft.mp4 should no longer report unknown color primaries from absent metadata; report any remaining warning with the clip name. Its original trailing packet/timestamp characteristics were not re-encoded away.

## Feedback to return

Start with the first pass, not the entire checklist at once. Report the first failing step, language/resolution, expected/actual result and first red error/stack trace. For presentation, tell me whether the transparency, hint duration, compact inventory and sound levels feel right. Movement/crouch feel and two-exam difficulty remain proposals awaiting your playtest. Build/quit/window mode and packaged localization need a separate user-built Windows player; do not overwrite the archived build folder.
