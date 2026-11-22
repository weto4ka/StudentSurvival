# Optional student distraction — 2026-11-21


## Stage3 social preparation — 1.7.0, 2026-11-22

The user requested flirting to affect exam preparation. This supersedes the old starter-area interaction described in historical sections below. The friendly student now lives in room3 at(42,3.04), with a study desk at(46.5,3.04). There are two free periods. Study spends one and adds one readiness; flirt spends one plus one collected lab and records a relationship. Study twice gives the easiest5-hit exam. Flirt+study gives1 readiness and a6-hit exam, plus a one-time optional two-report gift when returning to the friend. Starting with0 readiness is an8-hit challenge, not an automatic failure.

The pixel heart and nearby prompt show the current action/cost. With no report or no free period, no flirt cost is charged. Friend help commits only after InventoryController.Add succeeds and cannot be claimed twice; its sprite is assigned even though the held prefab stays inactive. CampaignState is the only persistent source for all choice/help flags; FlirtDistraction no longer implements IRunSaveParticipant. Generated finite room supplies ensure reports are available before the social choice, without timed respawns.

[Campaign_1.7.0.md](Campaign_1.7.0.md) and the1.7.0 user checklist are authoritative for placement, rules and verification. The older starter-area walkthrough below records previous revisions and should not be followed for1.7.0.

The user requested a humorous university distraction: spending a collected lab on a crush instead of exam preparation. `FlirtDistraction` implements one explicit, optional interaction per run. Merely looking at or walking past the student has no cost. Interacting spends exactly one collected lab if available, shows `flirt.success` and a brief rising heart, and prevents another charge that run. No lab shows `flirt.noLab` without changing inventory or stats. The prompt `flirt.prompt` must show the configured Interact binding and the one-lab cost. The examiner is never a flirt target.

The student is attached only in `Assets/Main.unity`, at authored x=-4.6 near the start x=-7.45, between the start and starter pickup cluster around x=-1.5. A one-time downward floor query aligns visible feet to the nearest solid floor. The trigger is 1.1 units wide, keeping it separate from the pickup cluster. The student has no solid collider, damage receiver, movement interruption, or combat effect. PlayerInteractionTarget/PlayerInteractor owns targeting and rebinding exactly as it does for pickups and doors.

`Resources/Combat/FriendlyStudentAppearance.asset` reuses ExaminerAppearance's uniform fitting and visible-foot alignment. It references the existing `Assets/Graphic/other_characters/black_hair_woman.png` Sprite directly (GUID `0cd823765f26bb14bbe27b4d85dfe5cd`), with no duplicated image. Source alpha inspection confirms transparent background and visible pixels x=240..360, y=98..481 inclusive (bottom-origin rectangle x=240,y=218,width=121,height=384). This sprite uses 170 pixels per unit, which the shared fitting code reads from the Sprite. Original character artist/license attribution remains to be confirmed with the university team.

The Inventory assembly owns this interaction to retain dependency direction: Characters never references Inventory. InventoryController.SpendCollectedLab centralizes item spending. The `RunSnapshot.flirted` flag is captured/restored at order 40 after inventory and exam state. Continuing a saved flirt keeps the student visible but disables further interaction. Checkpoint retry restores the flag and inventory together; repeating the choice is possible only if the chosen checkpoint predates it, so the same restored lab is never charged twice without a reset.

As of1.6.3 (2026-11-22), the heart uses one runtime-generated9x8 point-filtered SpriteRenderer with a dark outline, pink fill and highlight. The same sprite marks availability before interaction and floats/fades for1.2 scaled seconds after success. Texture/sprite allocations occur once in Awake and are destroyed with the NPC. There is no per-frame allocation or scene search. The trigger is disabled and target registrations cleared immediately after success, while the component remains active to animate the heart.

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


## Visible flirt marker and hint — 1.6.3, 2026-11-22

The user requested a pixel heart above the student and a clear E-to-flirt/one-lab-cost hint. The idle pixel heart now gently bobs above the student's head whenever this run's opportunity is available. It stays visible at a distance so the optional character can be noticed before reaching the interaction trigger. A completed saved interaction hides the marker; no repeat charge is introduced.

When PlayerInteractor actually selects this student, GameUI shows one translucent, pink-edged panel anchored above her: Interact binding + Flirt on line one, Costs1 collected lab on line two. The panel uses the existing pixel font and seven-language Localization seed key flirt.nearbyHint (167 total). It follows the existing target selection rather than inventing another proximity/input path. It updates text on target/language/binding changes, projects only the selected character per frame, clamps inside the safe area away from the main HUD, follows interface text scaling, and never intercepts pointer input. The generic bottom prompt is suppressed for this encounter to avoid duplicate instructions. Walking away, pausing or completing the encounter hides the nearby panel.

Verification: Inventory/UI/Editor source assemblies compile with the installed Unity6000.3.27f1 Roslyn response files. All167 keys have seven nonempty translations and matching format placeholders. An offline rendering of the exact9x8 mask was visually inspected; it is a recognizable outlined pixel heart. No Play Mode or audio test was performed because Unity MCP is unavailable.

User checks: stop Play Mode and import1.6.3. On a run where flirting has not already been completed, walk right from spawn to the dark-haired student before the pickups. Expect a pink pixel heart before reaching her, and the two-line hint when close enough. With no lab, press E: expect the existing no-lab message and both marker/opportunity retained. Collect a lab, return and press E once: expect exactly one lab spent, the heart floating/fading away, and no further prompt. Return to menu/Continue: the marker and paid interaction should remain completed. Rebind Interact, change language and use a controller: the shown key/button should track the active binding and the one-lab cost should stay explicit.
