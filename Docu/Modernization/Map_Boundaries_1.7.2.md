# Map boundaries and fall recovery

Date: 2026-11-22. Implementation version: 1.7.2.

## Report and diagnosis

The player reported falling out of the map by walking too far left on the first floor. Main.unity has internal walls, but the older far-left building lacked outer west walls on both reachable floors. Camera bounds do not stop physics movement. This diagnosis comes from scene/prefab geometry; the reported fall has not been reproduced in Play Mode.

## Changes

Two static, solid instances of the existing Wall prefab close those ends. The ground-floor wall spans world X -92.4844 to -92.0844 and Y -5.1904 to 2.8096. The upper-floor wall spans X -90.5449 to -90.1449 and Y 1.5396 to 9.5396. Their bottoms overlap the corresponding floor ends; their height prevents jumping over them. Existing connected outdoor routes remain unchanged.

Character.FixedUpdate recovers a fall more than 12 units below the last grounded position. It clears velocity and buffered movement, restores the saved stance, returns to that position and requests a save. It does not roll inventory, pickups, preparation or exam state back to a checkpoint. The ordinary 6.73-unit descent between these floors remains possible. A localized message explains recovery.

Character.Restore and fall recovery also resolve positions adjacent to the two added west walls. The allocation-free overlap query uses the existing collision filter and buffer, and moves old near-edge saves inside the wall using the actual stance collider dimensions. This is a targeted compatibility repair, not a general repair of arbitrarily corrupted save coordinates.

## Verification and exact user checks

Character and Editor source assemblies compile. Scene object IDs are unique; the two active, non-trigger wall prefab instances are parented under the older building walls. Offline bounds checks validate floor-end overlap. All 221 localization seed entries contain seven languages. Source and geometry checks do not establish a Play Mode pass.

1. Stop Play Mode, allow Unity to import/compile 1.7.2, then enter Play Mode again. Use Continue if available; no New run is required.
2. On each floor of the far-left building, hold Move left at the outer end. Repeat while crouching, jumping and boosted by an energy drink. Expect a solid end wall with no fall outside the level.
3. If an existing save was made immediately beside that edge, Continue should place the player on the playable side. Inventory and passed exams should remain intact.
4. Check the normal route between floors and the outdoor passage. Ordinary floor changes must not trigger recovery.
5. Optional Editor-only recovery check: while playing, move the player more than 12 units below its last grounded position. Expect return to that position, a brief recovery message, unchanged inventory/progress and no scene restart.

Runtime collision, controller and build checks remain pending with the user. No Unity MCP connection is available in this session.

## Right-side audit — 2026-11-22

The user asked whether right boundaries were checked on both floors and whether the later teachers have their own rooms. The main yellow hall has solid right walls on its lower floor (world X13.295–13.521) and upper classroom (X0.505–0.731). The older far-left building has a ground-floor east wall at X-37.325–-37.099; the adjacent floor continues into the outdoor route, so its farthest floor edge must not be sealed separately. Its upper floor lacked an east end wall. A third added wall now spans X-37.6733–-37.2733 and Y1.5396–9.5396. Saved near-edge positions resolve inward at east as well as west ends.

CampaignRooms.Build creates separate enclosed rooms centered at X25 and X45 for teachers two and three. Each has a floor, roof and two solid side walls; their inner side limits are X18/32 and X38/52. They are runtime rooms connected by campaign portals, not additional authored scene files. Each teacher has its own encounter space, using existing university artwork.

User checks: repeat walk/jump/crouch/boost boundary checks at the upper far-left building’s right end, both main hall right ends, and both sides of each later classroom. Confirm the lower outdoor route remains traversable. Geometry/source verification is separate from pending runtime checks.
