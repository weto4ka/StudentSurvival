# Saving, title art and remaining product gaps

Date: 2026-11-21. Context: follow-up feedback after implementation 1.2.0.

## User feedback captured

Update: implementation 1.3.0 is documented in [Release_1.3.0.md](Release_1.3.0.md); the original assessment below describes 1.2.0 and is retained as decision history. The user subsequently approved all seven tasks and parallel agents. Death now rolls back to checkpoints; native wording, attribution details and user gameplay/build verification remain pending.

The user reports losing collected items/progress after returning to the main menu, asks whether autosaving should exist, asks about a funny student-themed starting image, and requests a candid assessment of what remains missing, unclear or unfinished. This is an assessment/proposal; no gameplay save system or title illustration was implemented/generated in this follow-up. Gameplay testing remains user-owned.

## Historical findings at 1.2.0

- GameSession.BackToMenu loads scene index 0; starting a run loads Main afresh. Single-scene loading resets session result and pickup count. Inventory is scene-owned; ExamProgression always begins its first encounter when Main loads. There is no persistent run snapshot/restore path.
- PlayerPrefs currently stores language, master/UI volume, text size, fullscreen preference and binding overrides. Those are settings, not saved gameplay progress.
- The menu reuses Assets/buttons/NAU_shadow.png, an existing pixel-art university campus/airplane backdrop. It is not a student character/title illustration. The menu/settings/pause layouts are modernization work; they are not the original jam button layout.
- The added final examiner currently reuses the original teacher/classroom, with different hit count, projectile color and shot interval. It is an initial second encounter, not a distinct enemy archetype.
- Crouch currently rescales the existing sprite and changes collider/speed; no dedicated crouch animation was created.
- UI, controls, saves-to-be, language readability, audio balance and full-run balance are not certified by an agent runtime test. The user supplied UI feedback, and further user playtesting remains required.

## Proposed next priority: resumable run

Recommended UX: one autosaved active run, a Continue button when it exists, and an explicit New game / Restart operation for intentionally starting over. Returning to menu or closing the application should preserve that run. A brief saved indicator can confirm persistence.

The snapshot needs stable pickup identifiers and collected/remaining world-item state as well as inventory slot types/counts/selection. It also needs player stats, energy-drink speed changes, a safe player location/room, objective/progression state, exam stage/teacher health, and relevant persistent interactions. Saving only inventory would allow collected items to respawn and be collected twice.

Save after committed changes such as successful pickup/use, door arrival and exam progression, and flush before returning to menu/on application suspension. Capture a coherent state after each operation; do not serialize during a half-completed inventory/scene transition. Include a versioned local save format, atomic replacement and a backup so an interrupted write does not destroy the previous valid run. Invalid/missing saves should yield a clear New game path rather than a broken Continue button.

Decision still needed before implementing: whether death invalidates the active run or offers retry from a safe checkpoint. The user's request to preserve progress on menu exit does not by itself settle death/retry rules. Existing progress should never be silently discarded by a normal menu transition.

## Proposed title illustration

Use a humorous pixel-art main-menu illustration matching the existing game: three exhausted students sprinting toward an exam, one holding shawarma/coffee, another carrying an absurd stack of lab reports, with a looming teacher marking everything red. Reuse the recognizable campus/airplane setting in the background. Leave quiet space for menu buttons; render title/buttons as localized UI text instead of baking words into the image. A title-menu image would explain the premise without requiring a separate mandatory intro sequence.

## Prioritized TODOs

1. [ ] Implement versioned run autosave/restore and Continue/New game UX after settling death/retry semantics. Verify world pickup restoration, inventory and both exam stages together.
2. [ ] Complete the user feedback pass for 1.2.0: first compilation/menu/dropdown/sounds, then movement/crouch/bindings/inventory, then the full two-exam run and standalone build.
3. [ ] Improve first-minute onboarding: show a short premise, explain why throwing costs Sleep, demonstrate one pickup/use/door, and make the route to the first classroom understandable. Context hints should use effective remapped bindings.
4. [ ] Add the approved student-themed title art and review it with the user in the real menu layout.
5. [ ] Make combat more readable: teacher wind-up/attack warning, clear hit feedback, visible damage grace, and a distinct second-exam behavior instead of relying only on speed/health changes.
6. [ ] Replace crouch's provisional scaled-sprite visual with a dedicated pose/animation; review movement, resource economy, rewards and difficulty with user playtest evidence.
7. [ ] Review translations with native-language feedback, long-string/font/layout behavior, and the treatment of lettering in existing art/video.
8. [ ] Finish release basics: credits for the university team and asset/music attribution, version/build identity, clean standalone startup/quit/window modes and a packaged build check. Investigate the legacy timer/video/console prototypes and historic broken metadata recorded in the prior review before expanding content.

## Order recommended

Protect progress first. Then establish a clear first-minute experience and a reliable complete run. Add title art and the most useful combat/animation polish after that baseline; expand rooms/content using user feedback.
