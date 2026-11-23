# Editable campus rooms, student exchanges and finite food

Date: 2026-11-22. Implementation version: 1.8.0.

1.8.1 visual follow-up — 2026-11-22: the practical-room WC entrance now uses its earlier open-recess artwork and integrated sign, replacing only that wooden door graphic. The separate WC and Interact travel remain. See [entrance preference](WC_Entrance_1.8.1.md).

## Requests and final scope

The user requested editable Unity rooms with unique pixel artwork and purpose-specific objects, replacing runtime blocks and copied backgrounds. Follow-up feedback required wider rooms, doors clear of furniture, a separate toilet room, exchanges with the two existing upstairs corridor students, and a truthful food interaction after eating a previously collected shawarma. The user clarified that the food spot was the same one. On 2026-11-22 the user stopped computer control, then authorized finishing documentation and file/source checks only; gameplay verification remains assigned to the user.

The original first classroom remains intact. Two authored classroom prefabs replace the runtime room builder. A third authored interior is a separate WC connected to the practical classroom. The WC is an auxiliary space, not a fourth exam or level. There are still three exam stages.

## Open and edit in Unity

Open `Assets/Main.unity` outside Play Mode. The scene contains these prefab instances:

| Prefab in Assets/Prefabs/Campaign | Purpose | Scene placement | Playable width |
| --- | --- | --- | --- |
| PracticalClassroom.prefab | Second teacher; revision computer/workbook, chair, equipment locker, forward/back doors, WC entrance | Root X25 | 18 units |
| OralExamRoom.prefab | Final teacher; reference books, green examiner/preparation desk, archive cabinet, candidate chair, optional friendly student | Root X45 | 18 units |
| CampusToilet.prefab | Separate tiled WC; washbasin, wall mirror, toilet and hidden-notes marker | Root X70 | 10 units |
| CampaignEntrance.prefab | Door from the original classroom to the practical room | Root X−0.8 | Door only |

Double-click a prefab to edit it in Prefab Mode. Backgrounds and props are separate SpriteRenderer objects. The artwork textures and sprite slices are under `Assets/Art/Campaign`. Floors, roofs and side walls are ordinary editable BoxCollider2D objects. Teacher and friendly-student sprites are visible outside Play Mode; their gameplay components are authored on the same prefab actors. Changes are not erased by a room-generation script.

Move a study prop through its studyPoint parent so its interaction follows. Move a door through its doorway parent so the trigger, sprite and status light stay together. Keep arrival markers clear of doorway triggers; the supplied doorway-to-arrival separation is 1.4 units. Move the hidden-notes/toilet group through the WC notesPoint marker. Guard start and patrol endpoints are editable in the practical room. Supply markers determine the spawned pickup positions while stable pickup IDs remain unchanged.

Edit the room's camera localBounds together with its floor, roof and walls if changing its dimensions. The supplied rooms use floor topY3.04 and ceiling undersideY8.46. Keep roots unrotated and at unit scale for the existing horizontal patrol/movement assumptions. Door destinationStage is a room ID: 2 practical, 3 oral, 4 WC; unlockedAt remains an exam-stage requirement. WC return uses the practical room's toiletArrival, rather than its front entrance. The original-classroom return destination remains an Inspector-editable Vector3.

Unity previously showed unsaved Main scene edits. Those edits were not saved, reverted or overwritten through computer control. Before reloading Main, preserve any desired unsaved layout work, for example in a separate scene copy. Prefab-instance overrides can retain older positions or scales even after the prefab asset changes; review overrides if a door still appears in its previous location.

## Runtime responsibilities and compatibility

Characters/CampaignRooms is now a registry of serialized scene references. It no longer constructs backgrounds, furniture, floors, roofs, walls or doorways. ExamProgression finds the original professor outside authored CampaignRooms, then binds the two authored Prepod actors instead of cloning the first professor. The existing appearance assets select the blonde teacher in orange and the green-jacket final examiner.

CampaignPortal uses authored trigger colliders and arrival references. CameraBoundsRegion reads serialized local bounds on enable. Objectives, room supplies, revision, toilet notes, patrol endpoints and social placement use authored markers. Music requests retain the shared mixer and crossfades; exam-region widths follow the widened room bounds. The WC does not get a cloned boss-music region.

Runtime systems still create moving projectiles, transient warning/heart effects, the guard and hidden-note indicator, finite held-prefab pickups at authored markers, admission targets and music-region instances. This migration does not claim that every game system has become a prefab. PreparationDesk no longer builds rectangle furniture, and the social target reuses its authored sprite and trigger. The unused CampaignRoomArt ScriptableObject and original RoomArt asset remain for historical compatibility; the new room renderer does not load them. One-time bootstrap working files are excluded under .utmp; no room reauthoring/import generator ships in Tools.

Classroom centers and floor heights are retained, so prior saved positions remain within the expanded floors. The existing save envelope/campaign version remains compatible. New drinkTradeCompleted and tipTradeCompleted booleans default to false for old saves and are captured/restored with CampaignState at order 25. Continue preserves inventory and trade receipts; New run clears both receipts. Checkpoint retry restores the coherent inventory/receipt snapshot rather than keeping a paid reward while refunding its cost.

## Corridor students

The two existing pixel students on the older building's upper-left corridor now have StudentTrade components and authored trigger colliders. Their scene names are `Student - lab for energy drink` and `Student - lab for exam tip`. The second was moved 2.1 units right to separate targeting. Approximate world X positions are −62.16 and −59.49, with the original sprite-root Y3.426; they are not the third-room flirting NPC.

Approach and press the effective Interact binding (default E). The offer states the cost before payment. One collected lab report buys one energy drink; that offer is once per run. The other student charges one collected lab for advice about jumping low grades, crouching under high ones and waiting out teacher hit protection. Paid advice can be repeated free, including after Continue. Advice does not silently add preparation, change stats or weaken an exam. Walking past spends nothing.

Missing labs, missing offer configuration/resources and insufficient inventory capacity do not consume a lab or claim the drink. The inventory method reserves a destination before decrementing the report and emits a single inventory change. Receipt flags are committed synchronously after successful delivery. An empty slot, an existing energy stack or a slot freed by spending the last report can receive the drink. Holding/repeating Interact cannot buy the once-per-run offer repeatedly.

## Shawarma

The original pickup implementation destroyed collected food; no original vending/refill behavior was found. Each food object is a finite world pickup. Eating restores one Sleep per unit and does not replenish the same spot. Separate food pickups can still be collected and stacked, and the campaign bundles contain two units each.

LootBoxItem now exposes truthful availability and retires its interaction immediately when collected, before inventory callbacks/reparenting. Held/restored items disable their pickup colliders and cannot advertise a world-food interaction. Food pickup prompts and feedback explicitly describe one-time supply. The generic empty-inventory pickup tutorial stops after the first recorded pickup. This is source hardening and feedback correction; the user's exact stale hint has not been reproduced in Play Mode.

## Artwork and localization

Two new classroom wall paintings, a separate WC wall, eight university furniture/door sprites and two bathroom fixture sprites were generated using imagegen, with the user's original classroom screenshot as a style reference. Artwork uses muted campus colors, fluorescent lighting and angular pixel forms. Walls are not copies of the original classroom/corridor background. Props are individually sliced and placed; the WC sign uses a non-destructive sprite crop from a generated draft. Original human character artwork is reused. See the detailed [art provenance](../../Assets/Art/Campaign/README.md).

Fourteen added food/trade/WC keys bring the seed to 235 entries across en/uk/ru/fr/it/de/es. Numbered placeholders must match across locales. Unity's official String Tables remain the runtime source. The edit-mode setup imports missing keys after compilation; if a raw key appears, stop Play and run Tools > Student Survival > Set up localization. All235 keys are present in the imported official tables across the seven locales with matching placeholders. Translation/editorial and displayed-layout review remains pending with speakers of each language.

## Verification boundaries

Source compilation, native-YAML references, sprite bounds, static door/furniture/arrival geometry, localization placeholders and documentation links are checked separately from Unity gameplay. The machine-readable final static results are in [Checks_1.8.0.json](Checks_1.8.0.json).

The initial three-prefab Editor report predates the final layout. A later automatic edit-mode report, read from disk without computer control, validates the final four-prefab layout: 29 sprites, 22 colliders, two authored teachers, one authored social student and no missing references. The preserved [Editor asset report](Editor_Assets_1.8.0.json) is an import/reference check, not a visual or gameplay pass. Computer control ended on physical Escape and was not resumed after the user's no-control instruction. Final appearance, collision behavior, travel, gamepad, audio, save/relaunch, food availability and trades remain user tests. No build or WebGL test was performed.

The Editor menu Tools > Student Survival > Validate authored campaign rooms checks asset loading, missing scripts/sprites and markers without changing the open scene. It also runs after an idle edit-mode assembly reload. Tools > Student Survival > Render authored room previews is an optional explicit render command, not a gameplay test. The preview implementation follows [Unity's PreviewRenderUtility reference source](https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/PreviewRenderUtility.cs).

## Exact user checks

1. Stop Play Mode, allow compilation/import, preserve any desired unsaved Main edits, then reopen Main or inspect the prefab assets directly. Expect the two 18-unit classrooms and separate 10-unit WC to be visible outside Play Mode, with movable props and solid floor/roof/side colliders. Check doorway/furniture spacing and character size. Run the authored-room validation menu; expect no errors.
2. Start at the menu and use Continue for save compatibility. For the full sequence, use New run only if willing to replace the saved run. Pass the original exam, use its framed classroom door, and confirm arrival clear of the practical-room return door.
3. In the practical room, use E at the WC doorway. Expect the separate tiled WC, sink, mirror, toilet and hidden-notes interaction. Collect notes there; they should not be inside the classroom. Exit the WC and expect arrival beside that same WC doorway in the practical room, clear of its trigger. The guard should not catch a player inside the remote WC; carrying notes near the practical-room patrol can still lead to confiscation.
4. Test free revision, guard warnings/confiscation, note use during the second exam, the forward door after passing, and the final room's study/flirt choices. Confirm original bindings/costs and teacher warnings. Walk/jump at all widened room boundaries and roofs. Check music at each room's far ends.
5. Visit the two original upper-left corridor students. Walking past must spend nothing. With no collected lab, E must explain the requirement. With a report, the drink trade must remove exactly1 report and add exactly1 energy drink; repeated E must not charge again. The tip student must charge exactly1 report once, show advice, then repeat it free. Menu/Continue and a later app relaunch must preserve both receipts and counts. A New run resets them. Missing/full-capacity failure paths must not spend a report.
6. At a shawarma spot, collect with E, note the inventory count, eat one unit, then return to that same spot. Expect no food object and no food interaction/refill offer. Try a separate available food object: it should collect normally and stack with remaining food. Continue must not respawn previously collected food.
7. Select items with the effective previous/next bindings (defaults [ and ] or LB/RB), use one unit with F/Y, and check displayed quantities. Keyboard number shortcuts and clicking a slot use that slot directly. Verify all added prompts in each supported language; raw keys or clipped text are failures.
8. If the explicit 'failed because you left the room' message/teleport still occurs, record its exact wording, classroom and door. That older report remains unconfirmed: source searches did not find the stated failure rule, and this room migration is not a claimed fix for it.
