# Three connected exam stages


Latest behavior: [1.7.1 completion/exploration and pacing](Completion_Difficulty_1.7.1.md) supersedes this revision’s terminal-victory and single-shot first-teacher rules. Completed runs now remain explorable/continuable.

Date: 2026-11-22. Implementation version: 1.7.0. Unity6000.3.27f1.

## Agreed scope and delivery boundary

The user requested three connected levels with different teachers, hidden шпоры, a guard and flirting affecting preparation. Work was split across progression/rooms, cheating/guard, social preparation/supplies and root state/UI/localization/integration. Gameplay testing belongs to the user after implementation. No Play Mode session or player build has been run for this feature.

This is a three-stage first-semester campaign inside Main.unity, with runtime-created connected rooms. It is not20 finished levels. Final victory says Semester passed, rather than awarding a degree after three short encounters. Menu and gameplay remain the two build scenes. The existing hall, outdoor ramp, crouch/jump, save rules, music crossfades and clickable credits remain in place.

## Player progression

| Stage | Preparation and location | Teacher/exam | Outcome |
| --- | --- | --- | --- |
|1: first practical | Original campus/classroom; collect the existing supplies and explicitly interact with the teacher to start | Professor Byte:5 hits, low single throws | +5 Sleep/+1 Intelligence, checkpoint, next-room door unlocks |
|2: strict exam | Separate classroom/toilet alcove. Revise honestly at the desk, or take hidden cheat notes and avoid the guard | Dr Redline:6 hits with high/low two-shot bursts. During the active exam, use E near the teacher to consume carried notes and reduce the total to4 hits, preserving damage already dealt | +6 Sleep/+1 Intelligence, checkpoint, third room unlocks |
|3: preparation choices | Separate preparation room. Two free periods: study adds one readiness; flirting spends one period plus one collected lab | Professor Deadline: alternating longer volleys;2 readiness needs5 hits,1 needs6,0 needs8 | Semester-passed victory; new-run/menu options; completed run has no Continue |

Teachers are idle and ignore damage before their exam starts. The stage3 exam meter previews the reduced required hits as readiness changes, before admission. Interact explicitly begins each exam after its preparation gate. Passing does not auto-start another fight or teleport the student. Forward room doors unlock after the preceding pass; backtracking is allowed between exams and blocked by campaign portals during an active exam.

### Guard and hidden notes

The toilet alcove is at stage2's left side, centeredx22. It contains the note pickup and is a protected hiding area. The free revision desk is atx25.5. The pixel-art guard patrols x23.4..27 with turn pauses. Held notes build suspicion when visible to the guard;2.5 continuous seconds triggers confiscation. Moving out of sight lowers suspicion, and crouching reduces detection range from4 to1.05 units. A rising bar/attention mark and localized countdown explain the risk.

Confiscation is nonlethal: held notes are removed, an active exam is cancelled, the examiner's health/projectiles reset, and the student is returned to preparation at(25,3.04). Honest revision remains earned. The note opportunity returns, so the player can retry or study instead. Positive-Sleep confiscation creates a coherent checkpoint; zero-Sleep grace only requests an active save and retains the previous safe checkpoint.

The notes are a persistent campaign quest flag, not an inventory stack or endlessly farmable report pickup. The explicit notes-use interaction is available only during stage2's active exam while notes are carried; it does not compete with the preparation-time exam-start interaction.

### Social preparation

The friendly student is now at stage3x42, with the study desk atx46.5. This supersedes the old starter-area flirt placement. The pixel heart and nearby hint show when flirting/help is available. Costs appear before the interaction.

- Study twice: zero periods left,2 readiness,5-hit exam.
- Flirt and study once, in either order: one lab spent,1 readiness,6-hit exam. Return to the friend before starting to claim2 free lab reports once.
- Start after only one choice: a harder exam remains possible. Flirting does not automatically fail the run.

Friend help uses InventoryController.Add and a stable source ID. The help flag is committed only after the gift is accepted; a full inventory leaves it unclaimed. There is no social reward loop or automatic repeated spending. Time in menus does not spend free periods; they are explicit choice points, not a real-time countdown.

### Finite supplies

All five campaign supplies are created once before save restoration with stable IDs:

| ID | Contents | Position |
| --- | --- | --- |
|campaign.stage2.labs|3 reports|(20.8,3.55)|
|campaign.stage2.food|2 food items|(26.7,3.55)|
|campaign.stage2.energy|1 energy drink|(24,3.55)|
|campaign.stage3.labs|3 reports|(40.8,3.55)|
|campaign.stage3.food|2 food items|(44.4,3.55)|

These use the existing item effects, stacking, prompts and save-ID registry. They do not respawn after stage changes or Continue. Gift reports use campaign.stage3.friend-help and are claimed once. There is enough baseline Sleep plus exam rewards for ordinary fresh throws even without relying on these report bundles; supplies and preparation provide alternatives.

## Architecture and ownership

- World/CampaignState is the single source for campaign stage/activity and all preparation flags. It is an IRunSaveParticipant at order25, after Character10/Inventory20 and before ExamProgression30. No Inventory→Characters→Inventory cycle is introduced.
- RunSnapshot adds campaignVersion1, examActive, stage2Studied, hasCheatNotes, cheatNotesUsed, guardWarnings, preparationRemaining, preparedPoints and relationshipHelpClaimed; existing flirted stores the relationship flag.
- ExamProgression owns three teacher instances, admission, progression rewards/checkpoints, active-teacher health and route targets. Stage3 alone calls GameSession.Win.
- Prepod retains the shared combat implementation. ConfigureCampaign varies artwork, warnings/volley pattern and preparation-based required hits. Idle teachers reject damage; stopping an exam clears their projectiles.
- Cameras/CameraBoundsRegion registers the two room frames; CameraController shares viewport clamp logic between those frames and the original map. Region changes/large portal relocations snap the camera, and normal movement stays smooth. Characters references Cameras without creating an assembly cycle.
- CampaignRooms creates two enclosed rooms using single flat floors and solid roofs/walls, references the original pixel backgrounds through Resources/Campaign/RoomArt.asset, and clones the existing boss-music region descriptor through the shared music mixer. FinalExaminerAppearance reuses the existing green-jacket pixel character.
- CampaignPortal and CampaignExamAdmission use PlayerInteractionTarget/PlayerInteractor and the rebound Action input. No second input system is added.
- CheatingEncounter owns patrol/suspicion and quest actors; CheatingInteraction funnels their operations into CampaignState. CheatingPixelArt creates small point-filtered original pixel props once and disposes its resources.
- FlirtDistraction and PreparationDesk read/write CampaignState. FlirtDistraction no longer captures a separate flirted flag at order40.
- CampaignSupplies creates inactive prefab clones, assigns stable IDs/counts/positions before Awake, then registers them once. It never reactivates collected pickups on stage events.
- GameUI.Campaign extends the existing route/HUD. Objectives continue through all three stages, ignore supplies in still-locked future rooms, guard suspicion takes warning priority, effective bindings appear in prompts, and How to play explains the preparation choices. The semester ending has its own localized title/body. Credits remain clickable.

## Rooms and route coordinates

Both new rooms use floorY3.04 and roof undersideY8.46, width14. Room2 is centeredx25, room3x45. Teachers stand atx28.5/48.5. All room geometry leaves enough standing and crouching headroom and a single floor surface without a central seam.

Portals: original classroom(-.8,3.04)→room2(19.7,3.08); room2back19.7→original(-1.3,3.08); room2forward30.3→room3(39.7,3.08); room3back39.7→room2(29.7,3.08). A small arrival-floor gap lets the player settle normally. Existing campus door routing brings the player to the original classroom first.

## Save compatibility and recovery

The envelope remains schema1 with additive campaignVersion1. Validation allows stages1..3 and rejects invalid preparation budgets, simultaneous carried/used notes, or a friend-help claim without the relationship and study. The invariant is remaining periods + readiness + (flirted?1:0) =2.

Older saves with campaignVersion0 are retained: inventory, stats, pickups and recorded stage remain; the old second encounter becomes stage2 preparation, with no unexpected active exam. A previously recorded flirt remains a relationship and consumes one of stage3's two periods. New campaign stage/quest/preparation data are captured on the next save. Original completed runs remain completed; test the full new campaign through New run after confirming replacement, not by silently erasing the user's save.

Exam health, notes, honest revision, preparation choices and help claims survive Continue. Guard suspicion/patrol and attack warning phases are transient and restart safely. Checkpoint retry restores its coherent quest/inventory/preparation state; later choices and pickups roll back together.

## Verification

World, Cameras, Characters, Inventory, UI and Editor source assemblies compile against Unity6000.3.27f1 with the added sources and assembly dependencies. Seven-language seed coverage is215 keys, with all new literal keys present and matching placeholder sets. The shipped pixel font's cmap covers the translated text after the existing minus normalization. Room/teacher asset GUIDs resolve. Scene/runtime lifecycle and save-state flows were reviewed for duplicate bootstrap, inactive-prefab setup, trigger ownership, event cleanup, failed gift acceptance and guard recovery. These are source/static checks, not a Unity runtime pass.

The automatic Editor log also contains successful assembly reloads after temporary missing-CampaignState errors from parallel source creation. Those earlier errors do not constitute a final runtime result; clear old Console entries after import and report only errors that recur.

The user verification procedure is in User_Test_Checklist.md under1.7.0. Runtime placement, camera framing, controller navigation, audio transitions and overall difficulty still need the user's test. No public build/upload is part of this change.

## Expansion after this pilot

Validate the three distinct loops before expanding toward20 stages. Extract further stage layouts/objectives/tuning into authored data once these mechanics are approved. Stage-specific actors and shared combat/preparation/save services are separated so later rooms can add objectives without another implementation of movement, inventory, interactions or saving. Additional semesters, relationship storylines and a broader achievement system are not implemented in this three-stage delivery.
