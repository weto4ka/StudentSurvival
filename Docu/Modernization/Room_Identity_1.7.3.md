# Distinct classrooms and recognizable doors

Superseded for room rendering/layout on2026-11-22: [1.8.0 authored campus](Authored_Campus_1.8.0.md) replaces runtime rooms, moves the toilet into a separate room and adds corridor trades/finite-food feedback. Earlier version descriptions below are historical.

Date: 2026-11-22. Implementation version: 1.7.3.

## User feedback and diagnosis

The user expected unique characters and rooms, and supplied a screenshot showing the reused fireplace/globe classroom and two colored rectangular door blocks. [Original feedback image](Attachments/1.7.3/repeated-room-door-blocks.png). The colored blocks are the campaign portal graphics, not a missing-material diagnosis. Stage2 previously reused the entire original classroom backdrop and stage3 the original corridor backdrop, with identical rectangular shells. That reused art did not deliver sufficiently distinct rooms.

## Implementation

CampaignRooms retains the original first classroom and the existing campaign travel/unlock coordinates. Stage2 now has a cool tiled dado, a large chalkboard with chalk exercises, a timetable, fluorescent fittings and a recessed toilet alcove at the hidden-notes position. Stage3 now has warm wooden paneling, a tall bookcase, an evening campus window and a framed diploma. The existing study desks, notes, guard, supplies and friendly student remain interactive; decorative furnishings do not introduce colliders or new mechanics.

The two later rooms no longer render the copied full-room classroom/corridor backdrops. CampaignRoomArt and its resource asset remain in the repository for compatibility/historical reference but are not loaded by CampaignRooms. All added decoration uses the existing shared pixel sprite helper: startup geometry only, no per-frame generation and no generated raster artwork or external art dependency. Roughly a hundred decorative renderers are created across the rooms; the shared white sprite/material allows normal Unity sprite batching, but player-build rendering performance has not been measured.

Campaign portals now use layered wooden leaf/inset, jambs, lintel, handle and a destination plaque with Roman numerals I/II/III. A small status light is green when traversable and red while locked or during an exam; the existing localized interaction hint remains the explanation. Indicator updates subscribe to CampaignState.Changed; disabling a portal unsubscribes and calls the base interaction cleanup. Door graphics do not change trigger dimensions or teleport destinations.

Teacher identities reuse three distinct original character sprites: the original professor for stage1, woomen_in_red(remastered).png for stage2, and max.png for stage3. Prepod.Configure now selects the appropriate appearance directly once per encounter stage. The final examiner no longer applies the stage2 sprite before applying its own on repeated configuration. No new teacher animation set has been authored; these later teachers use the existing static pixel character artwork and combat feedback.

## Verification and exact user checks

Character, Inventory, UI and Editor source assemblies compile successfully. Teacher resource references, documentation links and whitespace checks pass. Source compilation and static checks are separate from Play Mode verification. Unity MCP is unavailable in this session, so room rendering, contrast, visual proportions and traversal still need user confirmation.

1. Stop Play Mode and allow Unity to import/compile 1.7.3; start again and use Continue. Existing saves and campaign progress remain compatible.
2. In the first classroom expect the original fireplace/globe and professor, plus a framed wooden doorway in place of the blue block. Before the first pass, the doorway indicator should be red and Interact should show the existing locked message.
3. Pass the first exam and use Interact at that door. Expect a green status indicator and arrival in a tiled practical room with a chalkboard, toilet alcove and blonde teacher in orange. There should be no copied fireplace/globe room background. Verify guard/notes/study interactions still work.
4. Pass the second exam and use its forward door. Expect the wooden-paneled room, tall bookcase, evening window, green-jacket teacher and existing optional friendly student. Confirm their sprites differ from the first professor and second teacher.
5. Check destination plaques, backward doors, locked status during exams, teacher warnings and saved Continue. Decoration should neither block walking nor cover characters or interaction feedback. First-classroom furniture remains unchanged.

The earlier screenshot and the reported visual mismatch are retained as evidence. A screenshot from Unity after reloading is still needed before claiming a visual/runtime pass.

## Follow-up feedback — 2026-11-22

The user clarified that the mid-exam exit used E; which classroom and any feedback message remain unconfirmed. Further reports: the social NPC is hard to find/non-interactable, only one shawarma seems collectible, individual inventory-object use is unclear, and runtime-built rooms do not meet the expectation of purpose-built editable room artwork. Requested direction: preserve common lighting/style while giving rooms distinct purpose-specific objects/designs; move room authoring to editable scene/prefab assets rather than runtime-only construction.

Source findings: the social NPC is at X42 in stage3 only. Interaction is disabled outside stage3 preparation, after both free periods are spent, after a relationship without an available gift, or during exams. Existing map/background women are not automatically social NPCs. Inventory.Add stacks matching types with no one-shawarma limit; world pickups are finite, and campaign food bundles contain two units. Every successful food use consumes one unit and restores one Sleep; keyboard slot shortcuts and clicking a slot use one unit directly, while previous/next selection plus UseItem allows deliberate selection. No reported pickup failure or door-return cause has yet been reproduced in runtime.

Runtime geometry was a prototype implementation choice, not a Unity requirement. The visual repair in1.7.3 still uses runtime code geometry and must not be represented as authored new background artwork or a completed prefab-authoring migration. Remaining backlog: editable purpose-built room assets, clearer NPC unavailable-state hints, and pickup verification at the reported food location.
