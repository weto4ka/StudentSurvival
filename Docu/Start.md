# Student Survival: start here

Date: 2026-11-21. Implementation version: 1.1.0.

## Scope and agreed workflow

The user requested interaction and movement fixes, a complete-run verification plan, player feedback/settings/HUD improvements, and additional content. They selected Unity Localization, English plus Ukrainian/Russian/French/Italian/German/Spanish, and a guided objective followed by a second teacher encounter. Gameplay testing and subjective feedback belong to the user for this iteration. Do not start Play Mode or run a player/test suite without their later instruction. Source review and reading the editor's automatic import/compilation log are permitted development checks, not a gameplay pass.

The repository already had a substantial uncommitted Unity upgrade, script relocation, Input System migration, and inventory/session improvements before this work. Preserve those changes. Existing script .meta GUIDs have been retained.

## Architecture after the 2026-11-21 changes

- Scenes remain `Assets/menu.unity` (build index 0) and `Assets/Main.unity` (1). `World/GameSession.cs` owns pause, terminal results, scene transitions, pickup progress and state notifications. Time scale and audio pause are restored on scene changes. First terminal result wins.
- `Controls/GameInput.cs` is the single input action map. `World/PlayerInteractor.cs` selects one nearby `PlayerInteractionTarget` on an E press. Doors and pickups share this proximity path; overlap counts prevent one collider exit from cancelling another collider's overlap. Non-player colliders cannot register interactions.
- `World/Teleport.cs` starts one delayed transfer, cancels if the player leaves, and transports the player's Rigidbody2D. `TeleportController.cs` caches door animators. Availability queries no longer play audio as a side effect.
- `Characters/Character.cs` captures input in Update and applies horizontal Rigidbody2D velocity/jump impulses in FixedUpdate. Ground casts reuse a fixed result buffer, filter triggers/actors/items/projectiles, and check the surface normal. Ground layers, jump buffer, coyote time and damage grace are Inspector settings. Main explicitly starts with 10 Sleep and 5 Intelligence. Speed retains the existing base-speed-plus-Sleep rule.
- `Inventory/InventoryController.cs` owns nine slots and stacking. `InventoryItem.TryUse` is the single implementation of consumable and lab effects. Keyboard and on-screen controls call the same UseSlot method. Held objects remain inactive children of the inventory. Their existing sprites are reused in the screen UI.
- `Characters/Prepod.cs` owns one teacher's shooting/damage. Its Inspector hit count is respected; startup randomness was removed. `ExamProgression.cs` owns the sequence in Main: original teacher (5 hits in Main), a two-second recovery interval, then a cloned final examiner (7 hits, 20% shorter shot interval, orange projectiles) in the same classroom. The first exam restores 3 Sleep and 1 Intelligence. Victory occurs after the final examiner. It uses the same combat implementation for both encounters.
- `UI/GameUIBootstrap.cs` creates one `GameUI` per supported scene. The UI uses a screen-space Canvas, safe-area anchoring, Expand scaling, existing item sprites and one shared solid sprite. The old menu Canvas is hidden at runtime; game art and videos are preserved. The menu's legacy UGUI script references were migrated to installed UGUI GUIDs and two missing hover-handler components removed from serialized data.
- `UI/GameUI.cs` presents menu/help/settings, a stat readout, guided objective, exam counter/progress, interaction prompts, inventory/effect hints, temporary feedback, pause and result panels. It subscribes to gameplay/language events; per-frame work is limited to input, proximity/expiry/viewport/availability changes. `UIInputModuleBootstrap.EnsureInputModule` is the shared UI input setup.
- `UI/GamePreferences.cs` persists volume, text size and fullscreen preferences. Localization persists the language separately. Settings use PlayerPrefs, not remote storage. Clicking on UI does not spend Sleep by firing a mouse shot.
- `Localization/GameLocalization.cs` is a small asynchronous facade over Unity Localization's `Game` String Tables. It saves locale choice, detects supported system languages, and retains an English fallback handle across locale cache releases. It does not maintain an alternative translation system. See [localization workflow](Localization/Localization.md).
- `Editor/LocalizationSetup.cs` creates/registers missing locale/table assets in edit mode, imports absent seed entries, preserves edits to existing entries, and enables Addressables-with-player builds. The setup marker includes the seed content hash. A Tools command and prebuild callback are available. No builds were run by the agent.
- Camera bounds now update when aspect/size changes and handle a viewport larger than the map. Facing offset follows sprite orientation, smoothing is frame-rate independent, and missing players are safe. Audio area triggers filter the player and tolerate a missing AudioSource. The original fake Windows error now uses localized feedback rather than hiding the HUD with a baked image.

## Validation and next action

Read [the user test checklist](Modernization/User_Test_Checklist.md). Compilation/import success does not establish runtime correctness. The user should begin with menu/language behavior and report Console errors, then exercise interactions/movement, then the complete two-exam run. Translation wording and font/layout coverage need user review in the selected languages.

The original artwork and video lettering are not translated. These require replacement localized media or subtitles as a separate content task. Full controller inventory bindings and input rebinding are not implemented; the existing controller movement/action buttons remain and UI/keyboard controls are documented.

The timer/provider prototype, unused legacy video helper scripts, developer console and alternate loading-scene prototype remain legacy systems. They are not a parallel implementation of the new runtime HUD or session path. Do not activate them without reviewing the prior audit findings.

## Files and history

Global registry: [Docu/_Files.md](_Files.md). Workstream registries: [Modernization](Modernization/Files_Modernization.md) and [Localization](Localization/Files_Localization.md). The initial [review](Modernization/Review_2026-11-21.md) describes the pre-implementation state; use this file for the implemented architecture. Versioned changes are in [Updates.md](../Updates.md).
