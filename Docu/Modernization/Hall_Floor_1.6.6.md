# Hall floor seam near the tree

Date: 2026-11-22. Implementation version: 1.6.6.

## Feedback and diagnosis

The user reports needing to jump when moving LEFT to RIGHT near the hall's potted tree, while RIGHT to LEFT works without jumping, and asks whether this is a bug. Attachment: [hall tree](Attachments/1.6.6/hall-tree.png).

The two authored hall-floor colliders meet at worldX0.6759056. Left Ground instance786223521 has localY-3.68 and world topY-4.0304175. Right Ground(1) instance1086468160 has localY-3.65 and world topY-4.0004175. The right collider forms a0.03-unit upward step for rightward box movement; reverse travel drops down it. The hall artwork at this location has no separate tree collision component: it is a SpriteRenderer on the hall background, not a solid tree obstacle.

## Repair

Use one continuous hall floor collision box. Instance786223521 retains its existing height, moves its local centerX from-3.9 to2.565 and overrides collider width from12.93 to25.86. It now spans exactly the combined old floor bounds, worldX[-12.2540944,13.6059056], at topY-4.0304175. Only the redundant collider component on instance1086468160 is disabled with m_Enabled0. Neither scene object is removed or renamed, and the artwork/pickups/door graph remain unchanged. Right-side saved positions settle0.03 units lower on the next physics step; no save reset or migration is introduced.

This removes the internal physics edge entirely, rather than adding a direction-dependent movement exception or requiring a jump beside decoration. The independent outdoor ramp repair from1.6.5 is retained.

## Checks and remaining verification

Static scene inspection confirms the merged box matches the union of both former horizontal spans, has one uniform top plane, and the second collider is disabled. Scene IDs remain unique. This change modifies scene collision data, not movement scripts; no source compilation or runtime collision pass is claimed. Unity MCP remains unavailable, so the user must check actual Play Mode movement.

Stop Play Mode and allow1.6.6 to import, then reload the scene. Walk A/Left and D/Right past the tree in both directions without jumping or crouching; both should pass without catching. Repeat on the controller, after Continue, and while returning through the hall entrance. Confirm the original floor artwork and pickups stay in place. Check that ordinary jumps still meet the ceiling and the previously repaired outside ramp remains traversable.

## Learning

An apparently decorative obstruction can actually be a mismatch between adjoining floor collision planes. Compare both height and shared-edge location; asymmetric passability can come from a small upward step rather than a one-way/tree collider. Prefer one continuous collider for a flat authored walking surface when its pieces need no independent behavior.
