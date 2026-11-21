# Articulated jump animation — 2026-11-21

`Assets/Scripts/Characters/Visuals/CrouchVisual.cs` centralizes crouch, jump, and landing visuals using the same six cached slices of the original 76 × 150 student idle PSD. No additional character artwork or independent animation controller was introduced.

The four-argument API is `SetState(bool isCrouching, float horizontalMovement, bool isAirborne, float yVelocity)`. Character supplies its grounded state and Rigidbody2D vertical velocity. The two-argument overload remains available for grounded callers. Airborne poses take priority over held crouch.

Ascending poses lift both knees forward with lower legs folded backward. Around the apex, velocity blends into a descending pose with the knees still bent and feet extending toward the floor. Contact starts a 0.24-second knee bend and recovery before the original standing/run animation becomes visible. Holding crouch on landing transitions directly into the existing crouch pose. All body parts retain their original proportions; joint rotation and position create the movement.

Scaled delta time controls pose smoothing, landing recovery, and crouch movement, so pause freezes animation. The helper follows the original renderer's facing direction, tint, sorting layer, material, visibility, and damage flashes. Six sprites are created once and released on destruction; frames do not create textures or sprites.

No gameplay, controller, or build tests were run for this change, as requested. User verification: jump while stationary and moving in both directions; knees should tuck during ascent, extend during descent, and bend briefly on landing. Pause in midair and confirm the pose stays frozen. Hold crouch while landing and confirm a low stance, then release in open space to stand. Confirm damage flashes remain visible in the air.

## Pixel artwork verification — 2026-11-21

The user explicitly requested verification that both crouch and jump use the original pixel student, and requested the actual menu banner preview. Main character Sprite renderer and Idle clip reference Assets/Graphic/main character/Main Character/Idle/1.psd (GUID454e59569a7bb274f963120166bfbe0c); Run and Jump clips use the same original PSD artwork family. Assets/Sprites/Character/Run/frame-1.png is the separate cartoon inspected in chat, not the active scene artwork.

Main now also serializes Character.articulationSprite to that exact original pixel idle PSD. CrouchVisual holds this explicit reference for every crop, independently of Animator frame changes, shared by crouch/jump/landing. No cartoon image is referenced by the active controller or articulated pose. The actual default banner is Assets/Resources/Art/StudentTitleMinimal.png. Source compilation/reference checks were performed without Play Mode.
