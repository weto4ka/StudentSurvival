# Local run persistence


## Completed campus persistence — 1.7.1, 2026-11-22

Victory now captures a semesterCompleted active snapshot and completion checkpoint; it does not remove Continue. GameSession.KeepExploring resumes the same scene, and menu/Continue restores completed exploration with teachers inactive. Completed snapshots allow zero teacher health and zero player stats, while unfinished-run validation retains prior requirements. Character restores zero Intelligence only for a completed snapshot, before CampaignState restores at25. Completed free-roam ignores terminal loss/exhaustion; boost/crash effects remain. Explicit New run still requires replacement confirmation.

An optional missing semesterCompleted field defaultsfalse for old saves; the schema1 envelope is retained. Legacy victory envelopes that already discarded their active/checkpoint snapshots remain empty, without fabricated restoration. [1.7.1 flow and exact checks](Completion_Difficulty_1.7.1.md) supersede all earlier victory-removes-Continue instructions below.


## Campaign update — 1.7.0, 2026-11-22

[Campaign_1.7.0.md](Campaign_1.7.0.md) supersedes the two-exam rules below. Additive campaignVersion1 supports stages1..3, explicit exam activity, carried/used notes, honest revision, guard-warning count, two free periods, readiness and relationship/help flags. CampaignState restores at25 before ExamProgression30; the old FlirtDistraction order40 participant is removed. The existing envelope remains schema1. Legacy version0 saves retain inventory/stats/pickups; the former second exam becomes stage2 preparation, with any previous flirt carried forward as one spent free period. No automatic save deletion occurs.

Stage1/2 pass rewards are now +5/+6 Sleep respectively plus1 Intelligence, with coherent stage checkpoints. Safe room portals and positive-Sleep guard recovery checkpoint normally. Campaign supplies have fixed IDs and never reactivate after a stage event. Confiscation resets the current exam and notes opportunity without a whole-run reset. Stage3 alone marks victory/completion. Free periods/readiness/relationship sum must remain consistent; impossible combinations are rejected. Read the1.7.0 checklist before the historical tests below.

Date: 2026-11-21. Gameplay verification remains assigned to the user; no Play Mode, player build or test suite was started for this implementation.

## Player rules

- New run explicitly replaces the saved run. Returning to menu and quitting preserve the active run. Continue restores inventory quantities and selected slot, collected world pickups, Sleep, Intelligence, base speed, facing, a stable player position, and examiner stage/remaining health.
- The initial spawn, completed door arrivals with positive Sleep, and the reward after passing the first exam establish checkpoints. Death rolls the active save back to the latest checkpoint; retry/Continue starts there. Items used or collected after that checkpoint roll back together with player/exam state. This prevents duplicating items by dying.
- Victory completes the run and removes Continue. A new run resets progress; settings, language and input overrides remain separate preferences.
- Energy-drink stimulation, borrowed Sleep, crash and exhaustion-grace timers persist as remaining gameplay seconds. Continuing does not restart an effect's full duration, and time in menus or while the application is closed does not consume it. The one-time flirt state also persists.
- Airborne saves use the player's last grounded position and matching crouch/standing collider stance, preventing an airborne apex or low-ceiling save from restoring an unsafe standing position. Velocity, projectiles, attack timers and temporary damage protection are transient. The restored teacher starts a fresh warning cycle. A save between exams restores exam two with full health and the already-awarded player recovery, without awarding it twice.

## Data and ownership

`World/RunSnapshot.cs` contains version-independent run DTOs and `IRunSaveParticipant`. The file envelope in `RunSaveService` has schema version 1, an active snapshot, a safe checkpoint and a completed marker. Changes to authored scene hierarchy/pickup placement require reviewing save compatibility and either migrating IDs or increasing the schema version.

The 2026-11-21 energy/exhaustion extension is additive within schema 1: optional fields `stimulationRemaining`, `crashRemaining`, `exhaustionRemaining`, `borrowedSleep` and `flirted` deserialize to zero/false when absent. Existing positive-Sleep saves therefore remain valid without a migration or reset. Validation requires finite nonnegative timers capped at 20/10/8 seconds, respectively; borrowed Sleep must be between zero and total Sleep. A zero-Sleep snapshot is accepted only while stimulation or exhaustion grace remains; it cannot restore a terminal exhausted run as active gameplay. Character captures/restores effect semantics; the persistence service validates and stores the shared DTO.

Participants expose `RestoreOrder`, `Capture(RunSnapshot)` and `Restore(RunSnapshot)`. World owns no Character/Inventory assembly reference. Character restores at order 10, InventoryController at 20 and ExamProgression at 30. `RunSaveDriver` waits one scene frame for Start/bootstrap construction before discovering participants once. Gameplay is blocked by `IsRestoring` during initialization. After restore, pickup progress and session/UI notifications refresh.

`LootBoxItem` caches an optional Inspector ID or a deterministic scene/hierarchy path before an item is reparented or destroyed. Successful collection records this ID only after the inventory accepts the item. Restore disables those authored world objects and reconstructs held items from the existing Resources prefabs. Stacks preserve actual quantities independently of how many world objects contributed. Prefabs are all resolved before replacing inventory contents. Restored held prefab clones never register as new world pickups.

## Service API

- `PrepareNewRun()`, `PrepareContinue()`, `PrepareRetryCheckpoint()` return success; GameSession performs the subsequent scene load. The service never calls GameSession.Load.
- `RequestSave()` marks a coherent mutation; capture occurs in LateUpdate, throttled to at most twice per second. A ten-second periodic capture covers movement. `CreateCheckpoint()` immediately captures coherent state into both active run and checkpoint, avoiding a death during the autosave throttle window rolling back an earned checkpoint. Doors pass their explicit destination to avoid transform interpolation and crouch-position lag.
- `SaveNow()` synchronously saves before leaving gameplay; `OnRunLost()` stores the checkpoint as the next Continue; `MarkCompleted()` writes a completed envelope. These return false on write failures so navigation can stay in the scene and surface an error.
- `HasContinue`, `HasCheckpoint`, `IsRestoring`, `LastErrorKey` and `Changed` drive menu availability and feedback. Error keys are `save.readFailed`, `save.writeFailed` and `save.restoreFailed`.
- Application focus loss, application pause and application quit request immediate saves. An operating-system forced termination cannot promise that the final unsaved fraction of a second was flushed.

## Storage and failure handling

The local path is `Application.persistentDataPath/student-survival-run.json`. Serialization goes to a `.tmp` sibling, flushes the file stream to disk, and atomically replaces the primary while retaining the preceding file as `.bak`. A failed write leaves the prior valid primary intact and reports an actionable error. The local file contains no credentials or remote synchronization.

Loading validates the format marker/schema, known scene, finite position/speed, valid player/effect values, positive examiner values, nine valid inventory slots, enum ranges and pickup IDs. Empty inventory slots are explicit zero-count records, avoiding reliance on Unity inline-class null serialization. An unreadable/invalid primary tries the backup. Unsupported save versions are not silently deserialized as a compatible run. Restore exceptions keep gameplay and subsequent autosaves blocked, preserving the original file; the menu escape must bypass saving this partially restored scene.

Only source-level integration review and whitespace checking were performed. User verification must cover collect/use/stack -> menu -> Continue, closing/relaunching, a doorway checkpoint, death rollback, first exam reward -> retry, final victory -> menu, explicit New run, and restoring with each supported language/controller layout. Save write failures require checking the visible feedback and that navigation does not discard the run.

For energy persistence, drink once, wait about five gameplay seconds, return to menu and Continue: roughly fifteen seconds of boost should remain. A second drink during active boost must remain in inventory. Continue during the crash must retain the remaining crash duration. Spend Sleep down to zero, return to menu during the eight-second grace and Continue: the remaining grace resumes, and using food/drink before expiry rescues the run. Retrying a checkpoint restores its recorded timers and borrowed Sleep together with its inventory quantities, rather than retaining newer effects or consuming the same item twice.

1.4.0 checkpoint safety: a door arrival during zero-Sleep grace still saves the active run and relocation, but does not replace the previous positive-Sleep checkpoint. This prevents retrying into a nearly expired exhaustion deadline. First-exam recovery grants positive Sleep before its checkpoint.
