# Keep exploring after exams and combat pacing

Date: 2026-11-22. Implementation version: 1.7.1.

## User feedback

The user reported that the victory screen pushes New game instead of letting the student explore, and that the exams are a little too easy. Screenshot: [victory/new-game screen](Attachments/1.7.1/victory-new-game.png). It displays the older Degree earned/New game wording; the image alone does not establish which source revision or already-running UI instance was loaded. Stop/reload Play Mode after import to load updated runtime-created UI and campaign actors.

## Completion behavior

Winning the final exam records a persistent semesterCompleted flag and briefly shows the celebration. The first/default button is now Keep exploring; it resumes the same scene, inventory, collected pickups and finished exams. A separate New run button uses the existing replacement confirmation. Main menu is another explicit button. The victory back arrow, R and Esc resume exploration; result shortcuts destroy their handler after resuming, so R cannot reset the exploring run accidentally. Cancel from a New run confirmation does not replace the run.

CampaignState.Completed prevents further exams/preparation changes. Passed teachers are inactive, and Current is null. Stage3 remains the unlocked stage, so all campaign back/forward portals stay available between rooms. The HUD says the semester passed and invites campus exploration instead of pointing back at the final examiner or showing Next exam. Boost/crash effects can still finish, but completed free-roam cannot lose from exhaustion or damage; the zero-Sleep terminal warning is suppressed. This is a post-completion reward, not an unfinished-exam difficulty change.

## Persistent completed campus

RunSnapshot adds optional semesterCompleted, defaultfalse for older saves. World CampaignState captures/restores it before ExamProgression; completion restores with all teachers inactive and no active exam. A completed snapshot may have teacherRemaining0, Sleep0 or Intelligence0. Normal unfinished snapshots retain their previous validation rules. Character.Restore reads the snapshot flag directly because it restores before CampaignState.

RunSaveService.MarkCompleted captures the active run and a completion checkpoint instead of replacing the file with an empty completed envelope. SaveNow continues capturing while the celebration is open. HasContinue and HasCheckpoint use actual snapshot presence; completed is an informational envelope flag rather than a prohibition on Continue. Menu/Continue restores the completed campus directly into exploration, without respawning teachers or replaying the celebration.

Legacy completed envelopes written by the old logic may contain no active/checkpoint snapshot. These remain empty; this update does not invent lost inventory or progress and does not silently overwrite/restore a backup. Ordinary1.7.0 unfinished snapshots remain compatible. New run still deliberately replaces the run only after the visible confirmation.

## Combat pacing

Hit counts/preparation advantages remain5 for Byte,6→4 with notes for Redline, and5/6/8 by readiness for Deadline. Fast player input remains available; accepted enemy damage now has a brief readable recovery interval so rapid spam cannot finish an exam before its first warning.

| Setting | Stage1 | Stage2 | Stage3 |
| --- | --- | --- | --- |
| First warning delay |0.6s|0.6s|0.6s|
| Windup |0.8s|0.7s|0.65s|
| Shots per volley |2 low|2, alternating low/high volleys|3, alternating low/high volleys|
| Burst spacing |0.24s|0.22s|0.14s|
| Recovery between volleys |1s|1.1s|1.2s|
| Recovery between accepted hits |0.45s|0.5s|0.55s|

The whole volley uses its warned lane; there is no unwarned high/low swap inside a burst. The final triple remains compact enough for the existing jump envelope. Teachers flicker throughout damage protection, and the HUD explains timing the next throw. Reports hitting during recovery are spent but do not deal another hit; use deliberate cadence to conserve finite supplies. No invulnerable windup phase is added.

Prepod's standalone no-subscriber GameSession.Win fallback was removed. It now only notifies its campaign listener, with a once-per-defeat console warning if ownership is missing. Only ExamProgression after stage3 calls victory, so a loose first teacher cannot independently end the semester.

## Verification

World, Cameras, Characters, Inventory, UI and Editor source assemblies compile with installed Unity6000.3.27f1 Roslyn. All220 source keys have seven translations and matching placeholders, the shipped pixel font covers the translated text, and the only GameSession.Win call is the stage3 campaign owner. Reviewed completion capture order, zero-stat validation, teacher restoration, shortcut lifetime, UI confirmation/back flow and recovery warnings. No Play Mode/save-relaunch/click test or player build was run; the user owns gameplay and subjective difficulty verification.

## Exact user checks

1. Stop Play Mode, allow1.7.1 to finish importing/compiling and reload through the menu. Continue an unfinished save if one exists; a new run is optional. An already-empty legacy victory save cannot supply missing progress.
2. First exam: E begins it; expect the first warning after0.6s and two low throws after its0.8s windup. Rapid throws should still appear, but only spaced hits reduce the meter. The teacher should flicker and the HUD should explain recovery. Deliberate throws after flicker ends should count normally. Passing exams1/2 must only unlock their next rooms.
3. Pass exam3: expect Semester passed, Keep exploring selected first, separate New run and Main menu. Keep exploring/back arrow/R/Esc should restore movement in that same campus without resetting inventory or pickups.
4. Walk through all campaign return/forward doors, collect remaining supplies, pause/resume, and return to menu. Continue should remain visible and restore the completed state, position and inventory with all three teachers absent.
5. Close/relaunch or reload the scene and Continue. Do not expect another examiner or another forced restart popup. Verify the completion HUD and music resume normally.
6. After completion, spend Sleep to zero if desired and wait beyond8 seconds; the completed run should remain explorable, without an exhaustion-loss screen. Unfinished runs must retain their normal loss/grace rules.
7. Choose New run, then cancel: progress should remain intact. Choose and confirm it only when deliberately testing a reset; expect a fresh campaign. On a separate loss, Retry should still use its checkpoint and Main menu should remain available.
8. Compare all three teachers, preparation routes, controller bindings and seven-language labels. Report whether the faster openings and recovery timing feel fair; source timing checks do not establish that subjective balance.
