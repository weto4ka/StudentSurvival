# University entrance repair

Date: 2026-11-22. Implementation version: 1.6.1.

## Feedback and cause

The user could not go outside and reported a column blocking movement. Attachment: [entrance](Attachments/1.6.1/entrance.png).

Source inspection shows an interaction-based transfer across the entrance boundary, not a walk-through opening. Both entrance triggers are active and have destinations. Hall wall instance277717438 and outside wall1025456872 separate the areas. The exterior raised platform has a clear left edge onto a lower floor; removing structural colliders would change the level rather than repair the transfer.

Two authored arrival errors were found: return exit764442314 placed the standing player partly inside the hall wall, and exterior exit1227625565 put the standing head about0.015 units into the overhead collider. Teleport destinations feed the checkpoint position, so arrival correctness also matters to saves.

## Changes

- Return exit local X -13.02 becomes -12.39: standing player has0.184 units of horizontal clearance from the hall wall.
- Exterior exit local Y -3.59 becomes -3.63: feet have0.0165 units above the raised platform and head has0.0254 units below the overhead beam. The player settles onto the floor normally.
- Teleport has an optional interactionPromptKey; existing doors retain the generic prompt. Only the two university entrance instances override it. UI supplies the effective Interact binding, with Go outside / Enter university translations across all seven languages. Seed total166 keys.
- Existing half-second entrance delay, collision boundaries, save data and other destinations are retained.

## Verification and remaining check

World, UI and Editor source assemblies compile with the installed Unity Roslyn compiler and staged Unity response files. Seven-language keys and placeholder sets pass validation. The authored standing collider bounds clear the named floor, ceiling and wall. These checks do not run physics or prove gameplay. Unity MCP tools are unavailable in this session; no Play Mode test was performed.

Stop Play Mode and allow Unity to import1.6.1. From the hall, approach the left entrance until the localized Go outside prompt appears. Press E (or rebound Interact) once and remain nearby for0.5 seconds. Outside, walk left away from the column and onto the lower path. Return to the entrance, press Interact at Enter university, and verify the student arrives clear of the hall wall and can walk right. Test Continue after each direction. Existing old saves made inside the wall are not rewritten; a successful transfer updates the active checkpoint.

## Learning

Validate arrival markers against the entire standing collider, including its offset and parent transforms. A destination root that appears above a floor can still overlap the ceiling or an adjacent wall. An architectural passage requiring an interaction needs a destination-specific prompt.
