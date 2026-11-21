# Articulated jump animation — 2026-11-21

`Assets/Scripts/Characters/Visuals/CrouchVisual.cs` centralizes crouch, jump, and landing visuals using seven cached slices of the original 76 × 150 student idle PSD. No additional character artwork or independent animation controller was introduced. The 2026-11-21 pose revision replaces the earlier independent limb pivots and 55-degree torso fold with measured, overlapping hierarchical joints.

The four-argument API is `SetState(bool isCrouching, float horizontalMovement, bool isAirborne, float yVelocity)`. Character supplies its grounded state and Rigidbody2D vertical velocity. The two-argument overload remains available for grounded callers. Airborne poses take priority over held crouch. `Configure(SpriteRenderer, Sprite originalPixelArtwork = null)` accepts an explicit original pixel-art source so Animator frame changes cannot substitute another style.

Ascending poses lift both knees forward with lower legs folded backward. Around the apex, velocity blends into a descending pose with the knees still bent and feet extending toward the floor. Contact starts a 0.24-second knee bend and recovery before the original standing/run animation becomes visible. Holding crouch on landing transitions directly into the existing crouch pose. Releasing crouch keeps the articulated rig visible for 0.18 seconds while it rises back to the original standing pose. All body parts retain their original proportions; joint rotation and position create the movement.

Scaled delta time controls pose smoothing, landing recovery, and crouch movement, so pause freezes animation. The helper follows the original renderer's facing direction, tint, sorting layer, material, visibility, and damage flashes. Seven sprites are created once and released on destruction; frames do not create textures or sprites. A SortingGroup preserves the character's scene order while rendering far leg, near leg, and intact upper body in that order.

## Measured geometry and offline rendering — 2026-11-21

Coordinates below use the PSD's bottom-left origin. The upper body is one intact rectangle `(12,44,44,106)` containing head, shirt, both complete arms, and pelvis; it pivots at waist `(38,64)`. Keeping it intact removes the head/neck seam and preserves the original hands ending at y48.

| Joint | Far leg | Near leg |
| --- | --- | --- |
| Hip | (32,52) | (48,52) |
| Knee | (30,28) | (48,28) |
| Ankle | (30,8) | (48,8) |

Hierarchy: upper body → thigh → shin → shoe. Local knee position is `sourceKnee-sourceHip`; local ankle position is `sourceAnkle-sourceKnee`. Child rotation is its requested absolute angle minus its parent's absolute angle. Consequently each joint position is shared by its connected sprites even while the torso leans. Crops overlap by 14 pixels at the hip, 6 at the knee, and 4 at the ankle. Each shoe counter-rotates independently so grounded soles stay horizontal.

For grounded poses, waist height is solved from the lower of the two transformed shoe soles rather than guessed. The idle crouch uses a 15-degree torso lean, both thighs forward, folded shins backward, and flat shoes. The computed waist height is 32.089 source pixels; the opaque head reaches approximately 102.91 versus 140 standing pixels (73.5%). This matches a crouch collider near 75% of standing height without squashing the body. In the air, the original 64-pixel waist reference is retained while knees tuck or extend.

Offline diagnostic PNGs in `.utmp/pose-diagnostics/` mechanically rasterize the existing sprite crops at the same measured transforms using nearest sampling. They are diagnostic scene renders, not replacement or retouched character artwork. Files: `standing.png`, `crouch.png`, `rising.png`, `falling.png`, and `landing.png`; the rendering script is `render.py` in that ignored directory. Inspection confirmed intact head/arms, forward-bending knees, and grounded crouch soles. A four-neighbor pixel connectivity check found exactly one opaque connected component in each of the five composed poses. Connectivity only detects detached islands; it does not establish anatomical or visual quality.

The first diagnostic revision exposed a knee silhouette error: shin crops extended eight pixels above their knee pivots. Rotating that excess in a deep bend produced a horizontal blue protrusion. The revised shin crops stop at y30, only two pixels above the y28 knee, retaining six pixels of knee overlap with the thigh's four-pixel lower extension. Rerendered crouch and rising poses show the protrusion removed. Source pose angles, shoe-ground solution, and 15-degree torso lean are unchanged. The audit also inspected 20%, 40%, 60%, and 80% angle blends for standing-to-crouch, ascent-to-descent, and landing-to-standing (12 intermediate diagnostic frames). These are representative static blend samples, not exhaustive Unity frame or gameplay validation.

No gameplay, controller, or build tests were run for this change, as requested. User verification: jump while stationary and moving in both directions; knees should tuck during ascent, extend during descent, and bend briefly on landing. Pause in midair and confirm the pose stays frozen. Hold crouch while landing and confirm a low stance, then release in open space to stand. Confirm damage flashes remain visible in the air.

## Pixel artwork verification — 2026-11-21

The user explicitly requested verification that both crouch and jump use the original pixel student, and requested the actual menu banner preview. Main character Sprite renderer and Idle clip reference Assets/Graphic/main character/Main Character/Idle/1.psd (GUID454e59569a7bb274f963120166bfbe0c); Run and Jump clips use the same original PSD artwork family. Assets/Sprites/Character/Run/frame-1.png is the separate cartoon inspected in chat, not the active scene artwork.

Main now also serializes Character.articulationSprite to that exact original pixel idle PSD. CrouchVisual holds this explicit reference for every crop, independently of Animator frame changes, shared by crouch/jump/landing. No cartoon image is referenced by the active controller or articulated pose. The actual default banner is Assets/Resources/Art/StudentTitleMinimal.png. Source compilation/reference checks were performed without Play Mode.
