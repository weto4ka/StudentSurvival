# Local run persistence

Date: 2026-11-21. Gameplay verification remains assigned to the user; no Play Mode, player build or test suite was started for this implementation.

## Player rules

- New run explicitly replaces the saved run. Returning to menu and quitting preserve the active run. Continue restores inventory quantities and selected slot, collected world pickups, Sleep, Intelligence, base speed, facing, a stable player position, and examiner stage/remaining health.
- The initial spawn, completed door arrivals, and the reward after passing the first exam establish checkpoints. Death rolls the active save back to the latest checkpoint; retry/Continue starts there. Items used or collected after that checkpoint roll back together with player/exam state. This prevents duplicating items by dying.
- Victory completes the run and removes Continue. A new run resets progress; settings, language and input overrides remain separate preferences.
- Airborne saves use the player's last grounded position and matching crouch/standing collider stance, preventing an airborne apex or low-ceiling save from restoring an unsafe standing position. Velocity, projectiles, attack timers and temporary damage protection are transient. The restored teacher starts a fresh warning cycle. A save between exams restores exam two with full health and the already-awarded player recovery, without awarding it twice.

## Data and ownership

`World/RunSnapshot.cs` contains version-independent run DTOs and `IRunSaveParticipant`. The file envelope in `RunSaveService` has schema version 1, an active snapshot, a safe checkpoint and a completed marker. Changes to authored scene hierarchy/pickup placement require reviewing save compatibility and either migrating IDs or increasing the schema version.

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

Loading validates the format marker/schema, known scene, finite position/speed, positive player/exam values, nine valid inventory slots, enum ranges and pickup IDs. Empty inventory slots are explicit zero-count records, avoiding reliance on Unity inline-class null serialization. An unreadable/invalid primary tries the backup. Unsupported save versions are not silently deserialized as a compatible run. Restore exceptions keep gameplay and subsequent autosaves blocked, preserving the original file; the menu escape must bypass saving this partially restored scene.

Only source-level integration review and whitespace checking were performed. User verification must cover collect/use/stack -> menu -> Continue, closing/relaunching, a doorway checkpoint, death rollback, first exam reward -> retry, final victory -> menu, explicit New run, and restoring with each supported language/controller layout. Save write failures require checking the visible feedback and that navigation does not discard the run.
