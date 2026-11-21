# Optional student distraction — 2026-11-21

The user requested a humorous university distraction: spending a collected lab on a crush instead of exam preparation. `FlirtDistraction` implements one explicit, optional interaction per run. Merely looking at or walking past the student has no cost. Interacting spends exactly one collected lab if available, shows `flirt.success` and a brief rising heart, and prevents another charge that run. No lab shows `flirt.noLab` without changing inventory or stats. The prompt `flirt.prompt` must show the configured Interact binding and the one-lab cost. The examiner is never a flirt target.

The student is attached only in `Assets/Main.unity`, at authored x=-4.6 near the start x=-7.45, between the start and starter pickup cluster around x=-1.5. A one-time downward floor query aligns visible feet to the nearest solid floor. The trigger is 1.1 units wide, keeping it separate from the pickup cluster. The student has no solid collider, damage receiver, movement interruption, or combat effect. PlayerInteractionTarget/PlayerInteractor owns targeting and rebinding exactly as it does for pickups and doors.

`Resources/Combat/FriendlyStudentAppearance.asset` reuses ExaminerAppearance's uniform fitting and visible-foot alignment. It references the existing `Assets/Graphic/other_characters/black_hair_woman.png` Sprite directly (GUID `0cd823765f26bb14bbe27b4d85dfe5cd`), with no duplicated image. Source alpha inspection confirms transparent background and visible pixels x=240..360, y=98..481 inclusive (bottom-origin rectangle x=240,y=218,width=121,height=384). This sprite uses 170 pixels per unit, which the shared fitting code reads from the Sprite. Original character artist/license attribution remains to be confirmed with the university team.

The Inventory assembly owns this interaction to retain dependency direction: Characters never references Inventory. InventoryController.SpendCollectedLab centralizes item spending. The `RunSnapshot.flirted` flag is captured/restored at order 40 after inventory and exam state. Continuing a saved flirt keeps the student visible but disables further interaction. Checkpoint retry restores the flag and inventory together; repeating the choice is possible only if the chosen checkpoint predates it, so the same restored lab is never charged twice without a reset.

The heart uses one reusable LineRenderer, no generated audio/image/font dependency, and no per-frame allocation or scene search. It floats/fades for 1.2 scaled seconds and pauses with gameplay. The trigger is disabled and target registrations cleared immediately after success, while the component remains active to animate the heart.

## Exact user checks (not run by agents)

1. Start a fresh run. Walk right from the start toward the black-haired student, before the starter pickups. Expect an Interact prompt that explicitly says it costs one lab. Do not press Interact: expect no inventory/stat change and no movement blocking.
2. With zero collected labs, press Interact (E by default or the displayed controller binding). Expect the no-lab message, no heart, and unchanged stats. The opportunity must remain available.
3. Walk past, collect a lab, then return. Press Interact once. Expect exactly one collected lab removed, the flirt message, and a short pink heart. Sleep and Intelligence stay unchanged. Walking through the student still works.
4. Press Interact repeatedly or hold it. Expect no additional lab loss; the target/prompt should disappear after success. Pause during the heart, wait, Resume: expect the effect to continue from its paused point.
5. Return to Main menu and Continue. Expect the same remaining inventory and no further flirt prompt. Retry a checkpoint from before the flirt: expect its inventory and opportunity restored consistently. Start a genuinely fresh run: expect the opportunity again.
6. Repeat with controller Interact and after rebinding Interact. At the actual final examiner, expect combat controls and warning prompts only, with no flirt charge.

Source inspected for reentrant interactions, failed spending, duplicate bootstrap, disabled-trigger stale targeting, pause timing, and restore order. Placement/rendering and the humor's wording await the user's gameplay/translation feedback. No Play Mode, gameplay tests, or builds were run by this agent.


## Discoverability feedback — 2026-11-22

The user asked where flirting is available, whether an NPC exists and whether an interaction hint is shown. Source review confirms Main scene runtime bootstrap creates one dark-haired friendly student atx=-4.6 near the starting positionx=-7.45, before the starter pickups. The proximity prompt is localized and uses the rebound Interact action. It explicitly announces the one-collected-lab cost. The saved flirted flag disables the prompt after success. This is a single optional humorous encounter, not a relationship/dialogue progression system. No new runtime visibility evidence was obtained; user feedback suggests discoverability needs verification at the authored location.
