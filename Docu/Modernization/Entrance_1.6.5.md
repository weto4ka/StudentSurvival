# Outdoor landing seam and run completion

Date: 2026-11-22. Implementation version: 1.6.5.

## Feedback

The user reported that the column area was still impassable and asked what happens when finishing the level. They clarified the blocked action is walking LEFT into the outdoor area, not pressing Interact or entering the university. Preserved screenshot: [outside column](Attachments/1.6.5/outside-column.png).

## Corrected diagnosis

The1.6.1 investigation checked arrival markers but missed the rotation of outdoor Ground(6), instance1302934067. Its BoxCollider2D is rotated about21.25 degrees. Treating that collider as an unrotated rectangle incorrectly placed its upper surface well below the player.

Resolving the prefab offset/size, quaternion rotation, parent translation and actual standing player collider shows the ramp's old upper-right corner at world(-15.534880020,-3.501252277). The landing Ground(3), instance1587026373, has left/top corner(-15.588715450,-3.520417500). The ramp therefore protruded0.019165223 units ABOVE the flat landing, with its corner extending0.053835430 units into the landing's span. A box moving left can catch this small upward lip. The low beam leaves only0.0418883 units of standing head clearance, so using a jump is not an appropriate walking-path fix.

## Repair

Only Ground(6)'s local position changes: (-19.489998,-5.46) becomes(-19.543833430,-5.479165223). Its top-right corner now joins the landing's left/top corner. Size, angle, ground/wall behavior, artwork, door destinations and player movement are retained. Walking left onto the descending ramp and walking right back to the landing use the same joined seam.

A first considered correction lowered the ramp by0.04 units; source review showed that would introduce a small return step, so it was replaced before completion with the two-axis corner alignment above. The scene contains only the final aligned coordinates.

## Verification and learning

The offline geometry inspection resolves all nearby authored scene BoxCollider2D records and the wall/ground prefab instances, including parent transforms, scale and quaternion rotation. The final ramp endpoint matches the landing corner within1e-6 units on both axes. No ramp corner is above the landing plane; standing head clearance remains0.04189 units and grows on descent. Scene IDs remain unique, and the edit adds/removes no objects or trigger references. This is a scene-only physics repair, not a movement-script change. No Unity Play Mode collision test was performed because Unity MCP tools are unavailable.

Do not treat an axis-aligned estimate of a rotated collider as proof that a route is clear. A near-flat collider seam can block a box even when the apparent obstacle is decorative artwork. Arrival clearance alone does not establish that the full walking route is traversable. This supersedes the1.6.1 claim that the exterior's left route was clear.

## Run completion, traced from code

ExamProgression uses two encounters in the same Main gameplay scene. Passing the first exam grants3 Sleep and1 Intelligence, saves a checkpoint, then activates the final examiner after2 gameplay seconds. Defeating the final examiner calls GameSession.Win. The run is marked completed, gameplay pauses and the victory/result page offers a new run or return to the main menu. Continue is removed for a completed run. There is no next-level scene or post-victory exploration flow in the implementation. The outdoor walkway is ordinary exploration space, not the victory exit or a locked next-level gate.

## Exact user checks

1. Stop Play Mode and allow Unity to import1.6.5; restart Play Mode so the scene's updated collider position is loaded. Do not rely on the already-running scene instance.
2. Use the university exit to reach the column landing, or Continue an existing grounded save there. Hold LEFT/A without jumping, crouching or pressing E. Expect movement off the landing, down the short slope and onto the outdoor path, with no stop behind the column.
3. Turn around and hold RIGHT/D up the ramp back to the landing. Expect no return lip. At the university boundary use the Enter university prompt and Interact to transfer inside.
4. Repeat with the left stick/D-pad. Save/Continue at the landing and repeat the leftward walk. Existing save data is not erased or migrated.
5. Pass the first exam: expect the reward and final examiner after2 gameplay seconds. Defeat the final examiner: expect victory, a new-run/menu choice, and no Continue for the completed run. Returning outside is not required to complete it.
