# First teacher visibility and classroom music bounds

Date: 2026-11-22. Implementation version: 1.8.2.

## Report and source evidence

The user supplied [a screenshot](Attachments/1.8.2/teacher-hidden-music-region.png) showing Exam1/3 at5/5 while the professor was mostly hidden beside the new classroom doorway, and reported a wrongly configured sound collider. This does not show a defeated teacher. Source inspection found the original professor's SpriteRenderer at sorting order1, behind the new doorway scenery at31. Only later examiners had been promoted to order50. The teacher's dynamic body also allowed horizontal pushing; that can place it behind/against doorway scenery. Exact movement in the screenshot was not reproduced in Play Mode.

The original boss-music BoxCollider2D (990768317, Outside Music (2)) covered world X−11.53 to−6.53 and Y5.277 to7.986: a small upper-left patch. Crouching or walking near the teacher could leave it. This is separate from the correctly widened later-room music regions.

## Changes

- All three examiners now use foreground order50, with their warning marks at51. The original professor's scene renderer is also authored at50.
- Examiner Rigidbody2D constraints freeze X and rotation while preserving vertical gravity/floor settling. This is present in the original scene body and both later teacher prefabs, and enforced by the shared campaign setup. Teacher health, appearance, exam completion and save rules are unchanged.
- The original office doorway artwork is narrowed to1.7 world units at its existing X−0.8, retaining its3.05 height and original interaction/arrival. Its left edge X−1.65 clears the original teacher collider's right edge X−1.9 by0.25 units. Other room doors are unchanged.
- The original music trigger now spans X−12.2541 to0.6759 and Y3.04 to8.46, matching the playable classroom floor/roof/walls. Local box offset is(1.2409,−0.84), size(12.93,5.42); trigger mode is retained. The shared mixer, crossfade and exit grace remain unchanged.

## Verification and exact user checks

Characters, Inventory, UI and Editor source compilation passes. Offline transformed collider bounds match the classroom, original actor layer/constraint flags are correct, and office doorway/teacher clearance is positive. No computer control or Play Mode/audio test was performed. The historical1.8.0 reports are not a new runtime pass.

Stop Play Mode, allow import/compilation, and restart through Continue. Preserve desired unsaved scene edits and review instance overrides if old artwork positions remain. At the first exam, expect the professor visible in front of doorway art, stationary horizontally when approached/pushed, and warnings above the actor.5/5 should remain until accepted hits reduce it. Defeating the teacher still removes it intentionally; completed-run Continue keeps passed teachers inactive.

Walk across the original classroom, crouch near the teacher and jump beneath the roof: boss music should retain the same request rather than disappear because of the former tiny collider. Leave through an actual available exit: expect the existing grace/crossfade behavior. In Scene view, select Outside Music (2) and inspect the full-room trigger bounds. Also check later teachers remain visible/anchored and still settle on their floors.
