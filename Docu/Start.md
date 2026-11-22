# Student Survival: start here

Date: 2026-11-22. Implementation version: 1.7.1.

Flirt discoverability1.6.3: [optional student and exact checks](Modernization/Distractions.md). An idle pixel heart marks the available encounter; the selected NPC has an anchored two-line localized hint showing the effective Interact binding and cost. Completed saves hide the marker/prompt.

Ceiling/music repair: [1.6.2 diagnosis and checks](Modernization/Ceiling_Music_1.6.2.md). Main now has a solid upper-classroom roof and full lower-hall roof coverage. Audio/MusicRegionMixer centralizes per-clip music voices across scenes; AudioController supplies region requests with exit grace and UIAudio supplies the menu request. Music crossfades and retains playback positions; pause still follows AudioListener.pause.

Entrance repair: [1.6.1 source diagnosis and exact user checks](Modernization/Entrance_1.6.1.md). The hall/outside passage uses Interact with a half-second delay; its two arrival markers now clear surrounding geometry and show destination-specific localized prompts.

README review2026-11-22: README.md now provides the creator roster and player/developer overview. Credits.json remains the in-game roster source; see the attribution audit for identity/source gaps.

## Completion and difficulty update — 2026-11-22, 1.7.1

[Completion_Difficulty_1.7.1.md](Modernization/Completion_Difficulty_1.7.1.md) supersedes all earlier victory-removes-Continue rules. Semester completion is a persistent optional snapshot flag. The celebration defaults to Keep exploring, with a separate confirmed New run and Main menu; Continue restores the same completed campus with teachers inactive. Completed free-roam cannot lose to exhaustion/damage. GameSession.Restart after victory now resumes exploration and result shortcuts retire after resuming. Combat keeps preparation-based hit counts but adds readable hit recovery, faster openings and stronger first-exam volleys. Source checks pass; user runtime/save-relaunch tests remain pending.

## Three-stage campaign — 2026-11-22, 1.7.0

The latest user request supersedes the earlier two-exam flow: build three connected stages with different teachers, hidden cheat notes/guard and a study/flirt preparation trade-off. [Campaign_1.7.0.md](Modernization/Campaign_1.7.0.md) is the authoritative feature/architecture description. The original campus remains stage1; runtime rooms atx25 andx45 host stages2/3. Each exam starts explicitly through Interact, and passing unlocks its forward portal. Final victory completes the first semester, not a degree.

World/CampaignState owns persistent notes/revision/preparation/relationship state at save order25. ExamProgression at30 owns stage/exam activity and teacher health; Character10 and Inventory20 restore first. FlirtDistraction is no longer a separate save participant. The additive campaignVersion1 payload supports three stages while retaining older saves through a preparation-state migration. New run is the recommended complete test; do not erase existing saves automatically.

The camera uses registered room frames and snaps after portal travel. Existing pixel backgrounds, distinct original teacher sprites, single floor colliders and solid roofs are reused. UI routes continue through all stages; guard suspicion and social costs use effective bindings and seven-language Localization. There are220 seed keys. Further semesters/20 authored stages and broader achievements are outside this three-stage delivery.

Gameplay testing remains assigned to the user. Source compilation and offline checks are reported separately; no Play Mode/build pass has been performed. Earlier dated notes below describe historical iterations and must not override1.7.0 behavior.

## Scope and agreed workflow

The user requested interaction and movement fixes, a complete-run verification plan, player feedback/settings/HUD improvements, and additional content. They selected Unity Localization, English plus Ukrainian/Russian/French/Italian/German/Spanish, and a guided objective followed by a second teacher encounter. Gameplay testing and subjective feedback belong to the user for this iteration. Do not start Play Mode or run a player/test suite without their later instruction. Source review and reading the editor's automatic import/compilation log are permitted development checks, not a gameplay pass.

The repository already had a substantial uncommitted Unity upgrade, script relocation, Input System migration, and inventory/session improvements before this work. Preserve those changes. Existing script .meta GUIDs have been retained.

## Architecture after the 2026-11-21 changes

- Scenes remain `Assets/menu.unity` (build index 0) and `Assets/Main.unity` (1). `World/GameSession.cs` owns pause, terminal results, scene transitions, pickup progress and state notifications. Time scale and audio pause are restored on scene changes. First terminal result wins.
- `Resources/Controls/Gameplay.inputactions` is the editable, stable-ID input asset. `Controls/GameInput.cs` loads one runtime clone, applies saved overrides, exposes effective binding labels, and owns cancellable/conflict-checked interactive rebinding for keyboard/mouse and controllers. `World/PlayerInteractor.cs` selects one nearby `PlayerInteractionTarget` on an E press. Doors and pickups share this proximity path; overlap counts prevent one collider exit from cancelling another collider's overlap. Non-player colliders cannot register interactions.
- `World/Teleport.cs` starts one delayed transfer, cancels if the player leaves, and transports the player's Rigidbody2D. `TeleportController.cs` caches door animators. Availability queries no longer play audio as a side effect.
- `Characters/Character.cs` captures input in Update and applies horizontal Rigidbody2D velocity/jump impulses in FixedUpdate. Ground casts reuse a fixed result buffer, filter triggers/actors/items/projectiles, and check the surface normal. Ground layers, jump buffer, coyote time and damage grace are Inspector settings. Hold S/Down (controller stick/D-pad down) to crouch: the BoxCollider2D shrinks with its feet anchored, movement slows, and headroom is checked before standing. The `Visuals/CrouchVisual` helper articulates cropped sections of the original idle frame into bent knees, a leaning torso, breathing and a shuffle cycle, without whole-sprite squashing. Hit flashes and a localized protection cue show the damage grace interval. Grounded save positions retain the matching crouch/standing stance; door relocation updates that position explicitly. Main explicitly starts with 10 Sleep and 5 Intelligence. Speed retains the existing base-speed-plus-Sleep rule.
- `Inventory/InventoryController.cs` owns nine slots and stacking. `InventoryItem.TryUse` is the single implementation of consumable and lab effects. Keyboard and on-screen controls call the same UseSlot method. Held objects remain inactive children of the inventory. Their existing sprites are reused in the screen UI.
- `Characters/Prepod.cs` implements warning/recovery phases, a visible lane and a hit flash. The first teacher uses single low throws; the final alternates low/high two-shot bursts, with binding-aware jump/crouch HUD warnings. `CombatProjectile` centralizes pause-aware motion/lifetime and once-only impacts. `ExamProgression` owns the two encounters and one-time +3 Sleep/+1 Intelligence reward/checkpoint. `ExaminerAppearance` references the existing orange/red character sprite via a Resources ScriptableObject, fits it upright to the collider, and gives the final examiner distinct art and behavior. See [combat implementation](Modernization/Combat.md).
- `UI/GameUIBootstrap.cs` creates one `GameUI` per supported scene. The UI uses a screen-space Canvas, safe-area anchoring, Expand scaling, existing item sprites and one shared solid sprite. The old baked-lettering buttons are disabled; a generated student-themed title illustration is placed over the original university backdrop, which remains available as a fallback. Game art and videos are preserved apart from the metadata-only BT.709 repair to usb_soft.mp4. The menu's legacy UGUI script references were migrated to installed UGUI GUIDs and two missing hover-handler components removed from serialized data.
- `UI/GameUI.cs` presents a compact translucent stat readout, fading objectives, a proximity-based exam meter, a small occupied-slot inventory that fades or is toggled by its action, and contextual binding-aware hints. Pause/settings are translucent overlays; the corner gear opens settings, language is a native-name dropdown, and upper-left back arrows preserve navigation history. The How to play page explains the goal, Sleep cost, free collected labs and save rules; its controls button opens real editable bindings in keyboard/mouse and controller tabs. A fresh run opens this briefing once. `GameUI.RunFlow` adds Continue/New game, confirmed destructive reset/checkpoint retry, scrolling credits and visible save failures/status. `ObjectiveRoute` caches the existing door graph and provides a first-90-seconds route cue toward the active classroom, using actual directed destinations. It subscribes to gameplay/language events; per-frame work is limited to input, proximity/expiry/viewport/availability changes. `UIInputModuleBootstrap.EnsureInputModule` is the shared UI input setup.
- `UI/GamePreferences.cs` persists master/UI volume, text size and window-mode preferences. The Editor disables window-mode changes with an Editor-only explanation; standalone players request explicit Windowed/Borderless Fullscreen modes. `Editor/GameStartup.cs` makes Play start at menu by default, with Tools options for menu/open-scene starts. Localization persists the language separately. Settings use PlayerPrefs, not remote storage. Clicking on UI does not spend Sleep by firing a mouse shot.
- `Localization/GameLocalization.cs` is a small asynchronous facade over Unity Localization's `Game` String Tables. It saves locale choice, detects supported system languages, and retains an English fallback handle across locale cache releases. It does not maintain an alternative translation system. See [localization workflow](Localization/Localization.md).
- `Editor/LocalizationSetup.cs` creates/registers missing locale/table assets in edit mode, imports absent seed entries, preserves edits to existing entries, and enables Addressables-with-player builds. The setup marker includes the seed content hash. A Tools command and prebuild callback are available. No builds were run by the agent.
- Camera bounds now update when aspect/size changes and handle a viewport larger than the map. Facing offset follows sprite orientation, smoothing is frame-rate independent, and missing players are safe. Audio area triggers filter the player and tolerate a missing AudioSource. The original fake Windows error now uses localized feedback rather than hiding the HUD with a baked image.

## Run saving and release scope — 1.3.0

`World/RunSaveService` owns one local versioned active run plus a checkpoint, atomically written with a backup under Application.persistentDataPath. `RunSaveDriver` restores participants after scene Start/bootstrap, blocks gameplay while restoring, and saves on coherent mutations, periodically and on focus/pause/quit. Character (order 10), Inventory (20), and ExamProgression (30) implement `IRunSaveParticipant`; World does not depend on their assemblies. Stable authored pickup IDs prevent duplicate world items after Continue. Saves include inventory/selection, collected items, stats/speed, grounded position/stance, and teacher stage/health. See [save rules and schema](Modernization/Save_System.md).

Menu exit preserves the active run. Death rolls Continue and Retry back to the most recent checkpoint (initial spawn, door arrival, first-exam reward); subsequent pickups/use/damage roll back together. Victory completes the run and removes Continue. New game explicitly replaces both saved snapshots after confirmation. Preferences remain separate. Restore failure blocks partial autosaves but permits menu recovery; write failure blocks a normal menu exit so it cannot silently discard progress.

The user explicitly authorized three parallel agents for saves, combat and crouch/credits, with root integration of UI, onboarding/localization and title art. Full scope, implementation evidence, pending human review and development learnings: [1.3.0 handoff](Modernization/Release_1.3.0.md). No agent entered Play Mode or launched a player/test suite. Eleven project assemblies compiled successfully with Unity's installed Roslyn and existing Unity response references into ignored staging outputs; this is source validation, not a gameplay or packaged-build pass.

Credits use repository contributor names and preserved installed package notices. Original asset/music/video creators/licenses and preferred contributor display names need team confirmation; never infer authorship from the person who committed a file. The seven-language seed has 220 keys; format/coverage review and obvious wording corrections are complete, while native-language and font/layout approval remains with the user.

## Menu repair — 1.5.0

The user reported obsolete camera warnings and concrete menu presentation/navigation problems. [Menu feedback and implementation](Modernization/Menu_Feedback_1.5.0.md) records the issues and limits. Main/pause now have separate How to play and Controls entries with separate translation keys. Continue hides/reflows when no save exists. Help and credits share a vertical flowing scroll layout with readable body sizes; ordinary pages have a single upper-left back arrow, and only fresh-run help has a Let's go action. Credits group names under headings rather than repeating a redundant contributor label.

`UIControlSound` centralizes pointer/controller focus and sound. UI loads the bundled Press Start 2P pixel font with a complete OFL notice; glyph coverage was checked for all 148 rows/names/arrows, including Ukrainian letters, and U+2212 is normalized to ASCII minus by the localization facade. The font has an in-game credit and a [coverage/provenance audit](Modernization/Font_Audit.md). Brighter button tint uses white base images, while separate child border graphics keep a bright three-pixel edge independent of button tint. Dropdown clone Awake recovers the template border reference. Input changes refresh Help and cached hints. `UIAudio` has louder nonclipping cues, explicit gain/priority, singleton recovery and missing-resource diagnostics. The obsolete native GUILayer was removed from menu Main Camera; its Camera/Listener/FlareLayer remain. These are source fixes; actual menu/audio verification must be distinguished from compilation checks.

## Energy, distractions and art direction — 1.4.0

See [the 1.4.0 feedback/implementation record](Modernization/Energy_Distractions_1.4.0.md). `Character.Energy` centralizes an eight-second zero-Sleep rescue window, three borrowed Sleep/20-second overstimulation, and a ten-second slow/sluggish crash. Drinks no longer permanently increase speed. Existing participant capture/restore includes all timers/debt; saves with zero Sleep are valid only during a boost or rescue window. HUD labels/countdowns explain the state and keep inventory visible at zero. `InventoryItem.TryUse` delegates drink acceptance to Character and retains refused items.

`Inventory/FlirtDistraction` bootstraps one friendly student near the start using the existing black-haired artwork via an appearance resource. It reuses PlayerInteractionTarget, costs one collected lab only on explicit interaction, displays a short heart, and saves the once-per-run choice at participant order 40. World retains no Inventory/Character dependency. `Character.articulationSprite` in Main explicitly references the original pixel Idle/1.psd (not the separate cartoon PNG family); `CrouchVisual` crops that fixed artwork and also drives articulated ascent/descent/landing from Character's ground/velocity state. `GamePreferences.MinimalTitleArt` persists the new default minimal/mysterious banner versus the retained detailed illustration; settings update the menu image immediately. The first-run briefing uses versioned wording for the revised resource rules.

## 1.2.0 user feedback and audio

The user found the previous HUD intrusive, settings opaque/misaligned, control help non-editable, and startup/fullscreen behavior confusing. All feedback, screenshots, implementation decisions and known limits are recorded in [the feedback ledger](Modernization/Feedback_2026-11-21.md).

`UI/UIAudio.cs` plays four original generated cues from `Resources/Audio/UI` for hover/click/pause/resume, with UI playback allowed while the global listener is paused. `UI/GameAudioSettings.cs` references the existing Mid4(Menu).wav for menu music; Main retains its original music/area effects. `Tools/GenerateUiSounds.py` regenerates the short PCM effects without playing them or running Unity. The user explicitly chose to keep existing music.

The two no-destination warnings were caused by validating intentionally locked doors. Only an unlocked door now requires a destination. usb_soft.mp4 had absent color metadata; a lossless remux supplied BT.709 tags, with matching decoded frame payloads/audio durations. Unity playback verification is still delegated to the user.

## Validation and next action

Read [the user test checklist](Modernization/User_Test_Checklist.md). Compilation/import success does not establish runtime correctness. The user should begin with menu/language behavior and report Console errors, then exercise interactions/movement, then the complete two-exam run. Translation wording and font/layout coverage need user review in the selected languages.

The original artwork and video lettering are not translated. These require replacement localized media or subtitles as a separate content task. Controller inventory navigation, use, display and gameplay rebinding are implemented. UI navigation retains standard submit/cancel controls independently of gameplay remaps. See the control table in Docu/Modernization/Feedback_2026-11-21.md.

The timer/provider prototype, unused legacy video helper scripts, developer console and alternate loading-scene prototype remain legacy systems. They are not a parallel implementation of the new runtime HUD or session path. Do not activate them without reviewing the prior audit findings.

## Files and history

Global registry: [Docu/_Files.md](_Files.md). Workstream registries: [Modernization](Modernization/Files_Modernization.md) and [Localization](Localization/Files_Localization.md). The initial [review](Modernization/Review_2026-11-21.md) describes the pre-implementation state; use this file for the implemented architecture. Versioned changes are in [Updates.md](../Updates.md).

## 1.5.1 roster and test handoff — 2026-11-21

Credits display the user-supplied four names plus a localized fifth-person placeholder; original Git handles are retained only as separate provenance. CreditEntry.nameKey optionally localizes the unresolved row while supplied proper names remain unchanged across languages. The seed has149 keys. The user authorized MCP Play Mode verification and explicitly chose to test themselves if MCP is unavailable. ALL_TOOLS exposes no Unity MCP commands in this chat, so do not claim that a Play Mode check occurred. The latest Editor log confirms an automatic successful reload/localization setup only; follow the user checklist for actual menu/gameplay outcomes.

## Browser publication assessment — 2026-11-21

The user asked about browser play on itch.io and game-jam attribution. [Web/itch assessment](Modernization/Web_Itch_Readiness_2026-11-21.md) records official requirements and identified save/video/window-UI gaps. No Web port/build/upload has been performed or authorized by that assessment question. Original media permissions and jam/university/team agreements still need confirmation; attribution/free hosting is not distribution permission.

## 1.6.0 readability, poses and control feedback — 2026-11-22

[The screenshot feedback record](Modernization/Feedback_1.6.0.md) preserves all five attachments and implementation evidence. Canvas reference is1280×720; help/credits body sizes adapt to physical screen scale in8px steps, with1.5 line spacing and no body shadow/best-fit shrink. Pixel-cell arrows replace thin back strokes. HUD stats have two short rows and guide/status/cue regions are separated; the duplicate objective line was removed. Controls key names use canonical Input System layout metadata, preventing German OS names in English UI.

CrouchVisual now has measured, hierarchical upper-body/leg/shoe joints, a15° torso lean, meaningful crop overlap and a SortingGroup. Shin overhang was corrected after actual offline previews. Character crouchHeight is.75; high examiner grades are2.55 above teacher feet to clear it. The long yellow lane was removed; compact wind-up marks and short Jump/Crouch HUD prompts explain the dodge. See Jump_Animation.md and Combat.md for measured evidence and exact user checks.

Inventory automatically selects a collected item. F/Y press uses immediately; holding repeats only lab ammunition at.18s intervals. Food/drinks/books remain single-use per press. Empty/depleted slots explain what happened and never silently spend Sleep. Fresh Ctrl/LMB/RT throws also repeat at.18s while held and cost1 Sleep each. Mouse holds originating on UI require release before firing. First-minute route guidance targets available lab pickups and marks them with a pixel arrow; transforms are cached and camera projection updates every frame. Help explains where the authored first pickups are and exactly how selection/F differ from paid throws.

The seed now has164 entries in seven languages. No Unity MCP tools are exposed; no Play Mode result is claimed. Offline pose previews and source compilation are separate from actual game/controller/collision/UI testing. The user remains the runtime fallback they requested.


Creator roster correction2026-11-22: the user identified the fifth teammate as Max. README and Resources/Credits.json now list him by that supplied name; surname/role and Git-handle mapping remain unspecified. The roster has five entries, without the former unnamed placeholder.


Upstream identity clarification2026-11-22 supersedes the first-name-only entry: fifth teammate is Maxim Rebitskiy (Max, maksa988), owner of upstream StudentSurvival, identified by the user and matching public LinkedIn/GitHub profiles. README names the original repository and weto4ka fork explicitly; in-game Credits.json uses the full public name. See Credits_Attribution.md for evidence and retrieval limits.


GitHub credit links2026-11-22: README links Єлизавета/Zaza→weto4ka (user clarification), Кирил/Morgan23102→qluckoff and Ivan→interess (visible linked commit authors), plus Max→maksa988. Максим Панасенко is still pending because his Maxim/Maxym commit authors are unlinked. The user has been asked for his profile. See Credits_Attribution.md for source URLs and failed lookup methods; do not guess a fifth profile.


GitHub roster completion2026-11-22: the user supplied maximys963 for Максим Панасенко. All five README creators now have linked GitHub profiles; this supersedes the pending-profile note above.


Clickable credits1.6.4 (2026-11-22): Credits.json is the single in-game source for all five githubUrl fields. GameUI.RunFlow builds shared buttons for valid GitHub profiles and plain text otherwise, with measured wrapping and the existing ScrollSelectionIntoView helper. UIControlSound focus-border decoration ignores layout. See Credits_Attribution.md for implementation, official API reference, exact targets and user runtime checks.


Outdoor passage follow-up1.6.5 (2026-11-22): user confirms walking LEFT is blocked, not Interact. [Entrance_1.6.5.md](Modernization/Entrance_1.6.5.md) supersedes the1.6.1 route-clear conclusion: Ground(6)’s rotated upper corner formed a0.019-unit lip above the landing. Main aligns the ramp corner with the landing in both axes. Play Mode walking remains unverified. Final examiner victory ends the run; there is no next-level scene.


Hall-floor follow-up1.6.6 (2026-11-22): [Hall_Floor_1.6.6.md](Modernization/Hall_Floor_1.6.6.md) records the tree-area directional snag. Two hall-floor planes differed by0.03 units; Main now uses one continuous25.86-unit floor collider at the original left-floor height and disables the second collider. Source geometry checks pass; bidirectional Play Mode walking remains pending.
