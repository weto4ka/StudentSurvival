# User testing and feedback

Date: 2026-11-21. Implementation version: 1.1.0.

Status: awaiting user testing. The agent has not entered Play Mode, run tests, or built/launched a player for this implementation.

## First: import and menu

1. Stop Play Mode and let Unity finish importing and compiling. In Console, report any red errors before proceeding. The setup should log `Student Survival localization set up: seven locales and Game tables. Existing translations preserved.` once when the seed/setup changes.
2. Open `Assets/menu.unity`, then press Play. Expected: a text-based Student Survival menu with Start a run, How to play, Settings and Quit. No missing-script warnings should originate from the legacy menu components.
3. Click Settings. Select Українська, Русский, Français, Italiano, Deutsch, Español and English in turn. Expected: labels change immediately, native language buttons remain legible, no `[key]` placeholders, missing glyph boxes or `{0}` text appear. Report any unnatural translation or clipped label with its language and screen.
4. Move Master volume to 0 and then about 80%. Move Interface size to minimum and maximum. Toggle Fullscreen in a standalone player. Expected: volume/text size respond; controls remain inside the screen. Fullscreen behavior inside the editor's Game view can differ from a player.
5. Back out of Settings, open How to play, return, then start a run. Expected: Main loads once even after repeated clicks. Language remains selected. Exit and re-enter Play Mode: language, volume and text-size preferences should remain.

## Second: movement and interactions

1. In Main, start values should be Sleep 10 and Intelligence 5. Move with A/D and arrows. Stop against walls and jump with Space. Expected: collision blocks movement, the camera follows smoothly, and there is no visible jitter.
2. Try a jump just after walking off a ledge and just before landing. Expected: a short forgiving window, no unlimited midair jumps. Move beside pickups/door triggers and confirm they do not make an airborne player count as grounded.
3. Approach an unlocked door. Expected: a localized E prompt. Press and hold E. Stay nearby for the door's configured delay (normally 2 seconds). Expected: one transfer, without extending the delay while E remains held.
4. Begin a door interaction, then walk away before it completes. Expected: transfer cancels. Approach a locked door and press E: it reports locked without moving the player. Bullets passing through a door should not produce a player interaction prompt elsewhere.
5. Stand near a pickup and tap E. Expected: exactly one nearby target responds, pickup disappears and its icon/name/count appears in the toolbar. Pick up the same item type again: its count should increase in the same slot.
6. Hover a toolbar slot or focus it through UI navigation. Expected: an effect hint. Press that slot's number or click it. Book: +1 Intelligence; shawarma: +1 Sleep; energy drink: +1 Sleep and +1 base movement speed; collected lab: one projectile and one fewer lab, with no Sleep cost.
7. Click a slot or Settings. Expected: the click itself does not also fire a paid lab. The separate world fire input (left mouse or Ctrl) should spawn one lab and cost exactly 1 Sleep.

## Third: pause and complete run

1. Press Esc during gameplay. Expected: pause panel appears, player/teacher/projectiles/door countdowns stop, and settings/help work. Resume: gameplay continues. Restart or Main menu from pause must restore normal simulation speed and audio after loading.
2. Collect a supply and reach the original teacher. The guided objective should advance from supplies to the first exam. The teacher meter should show exam 1/2 with 5 required hits in the shipped Main scene.
3. Land one lab hit. Expected: the remaining count drops by exactly one. Enemy grades reduce Intelligence by one, with a brief grace period against multiple immediate hits.
4. Defeat the first teacher. Expected: no victory yet; +3 Sleep and +1 Intelligence, a recovery message, then a final examiner in the same classroom after 2 seconds. The final exam requires 7 hits and uses orange grade projectiles at a slightly faster rate.
5. Defeat the final examiner. Expected: localized victory panel, stopped simulation, Restart run and Main menu buttons. R restarts; Esc returns to menu. A restart must reset the inventory, objective, stats, first examiner and session result.
6. On a separate run, fire ten paid labs without restoring Sleep. Expected: Sleep reaches 0 and a localized Sleep-loss result appears. No further shots should be possible after the result. R should restart normally.
7. On a separate run, take five grade hits without using a book (allow the grace period between hits). Expected: Intelligence reaches 0 and the Intelligence-loss result appears. Test the result buttons and keyboard controls again.

## Layout and build pass

Repeat menu/HUD/settings checks at 1920×1080, 1280×720 and a 4:3 Game view, including French/German and Cyrillic languages at maximum Interface size. A separate Windows development build is needed to validate packaged localization/Addressables, actual fullscreen, quit and player logs; do not overwrite the archived `build` folder. Compare initial and final Console errors and warnings. The known MP4 color-primary/timestamp warnings predate this implementation and need separate video work if playback looks wrong.

## Feedback to send back

Please report the first failed step, selected language, resolution, what you expected, what happened, and the full first red Console error/stack trace if present. For subjective feedback, say whether movement feels too fast/slippery, text is readable, prompts are clear, and the second exam/recovery reward feels fair. The default two-exam tuning is a proposal awaiting your playtest, not a verified balance result.
