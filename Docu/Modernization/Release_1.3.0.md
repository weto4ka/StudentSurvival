# Implementation handoff — 1.3.0

Date: 2026-11-21. Unity: 6000.3.27f1. Status: implemented and source-checked; awaiting user gameplay/controller/build feedback.

## Approved work and owners

The user asked to complete the following in parallel with agents and reserved gameplay testing for themselves. Three agents handled persistence, combat, and crouch/credits; root handled integration, onboarding, seven-language UI, title art, documentation and final source review.

| Requested item | Implemented result | Human follow-up |
| --- | --- | --- |
| Autosave and restart/death rules | Active run + checkpoint, Continue, confirmed New game/retry, atomic disk save/backup, coherent world/inventory restoration | Menu/relaunch/door/death/reward/victory checks |
| Clear first minute | New-run briefing, remapped control labels, resource economy explanation, directed door graph/classroom arrow for 90 seconds | Route clarity and pacing |
| User verification | Exact revised checklist with expected results; no agent playtest | User runs the checklist and supplies first failing step |
| Readable combat | Lane warning, jump/crouch cue, enemy hit tint, player flicker and short protection label | Visibility, timing and difficulty |
| Distinct final examiner | Existing orange/red character artwork, upright fitted silhouette, alternating low/high two-shot bursts | Collider/art alignment and fairness |
| Crouch animation | Original idle artwork articulated at knees/torso, breathing and shuffle, no whole-body squash | Both facing directions, low ceilings, pause and hit tint |
| Translation/credits/attribution | 116 keys × seven languages, placeholder/coverage review and wording corrections, contributor credits and exact installed package notices | Native review; original asset/music/video creators, sources and permissions |

## Menu and save behavior

Continue restores the most recent autosaved state. Initial spawn, successful door arrival and the first-exam reward create checkpoints. Returning to the menu preserves progress. Death makes Continue/Retry use the checkpoint, rolling back later pickups, uses and damage together. Victory completes the run; New game replaces it. New game requires confirmation when an unfinished run exists. Pause offers a confirmed checkpoint retry rather than silently resetting all progress. Keyboard result restart follows the same retry rule, and starts a fresh run after victory.

Saving retains stable pickup identities, held quantities/selection, stats, energy-drink base-speed changes, grounded player position/stance and exam health/stage. Airborne motion and projectiles are transient. Crouched saves restore the smaller collider before gameplay resumes, and headroom prevents expanding into a ceiling. Door saves use the Rigidbody destination immediately. Restore errors keep partial state from overwriting disk; a visible menu recovery remains available. OS-forced termination can lose the last unflushed fraction of a second.

The project's company/product names remain unchanged because changing them can move Unity's persistentDataPath and hide existing preferences. Only bundleVersion advances to 1.3.0.

## Title illustration and generation record

Final project asset: `Assets/Resources/Art/StudentTitle.png`. Generated using the built-in OpenAI image-generation tool, then copied into the workspace; the original generated file was left intact. The menu loads it as an opaque texture with aspect-preserving fill, behind localized UI. The original campus background remains the fallback. The illustration has no baked title/buttons; small paper marks are incidental artwork, not translated instructions.

Final prompt:

> Use case: stylized-concept. Asset type: 16:9 landscape title-screen illustration for a humorous university student survival pixel-art platformer. Primary request: funny tired university students struggling through exam week, juggling laboratory reports, books and coffee in a campus corridor; an intimidating professor holds an enormous stack of papers. Style: polished charming pixel art, readable chunky pixels, teal campus shadows and warm amber lights, playful expressive poses. Composition: characters around the outer sides and lower edge, quiet dark open space through the center for separately rendered localized menu buttons and title. No text, letters, numbers, logos, watermarks or UI. Opaque background. Make it welcoming and funny, suitable for a small game jam game.

## Validation evidence and limits

Source-compiled eleven StudentSurvival assemblies with Unity's installed NetCoreRuntime/DotNetSdkRoslyn, using the Editor-generated response files and references. Output/reference DLLs and amended source lists went only to ignored `.utmp/source-compile`; no assemblies were executed, no player built, and no Library DLL replaced. Localization, Editor, Audio, Controls, Cameras, World, Characters, Inventory, Timers, UI and Video compiled successfully with no compiler diagnostics. This verifies cross-assembly API/syntax consistency, not runtime imports, animation appearance, controller feel or packaged Addressables content.

Reviewed full save/restore/death flow and fixed same-frame checkpoint timing, stale scene-init coroutines, inline-null serialization ambiguity, grounded crouch restore and interpolated door positions. Seven-language rows are nonempty, unique and placeholder-compatible. Metadata/resource references and whitespace are checked separately. The open Editor's existing compilation log predates the final source change; it is not evidence of a final Unity import or runtime pass.

Remaining evidence gaps are explicit: native speakers have not approved translations; the team has not confirmed legacy media provenance; gameplay/controller/build/window/audio balance awaits the user. See [user checks](User_Test_Checklist.md), [save details](Save_System.md), [combat](Combat.md) and [attribution audit](Credits_Attribution.md).

## Development learnings / first occurrences

- 2026-11-21: a multiline translation-writing script treated escaped paragraph newlines as row separators before any catalog write. Corrected by using raw input strings, validating eight fields per row, then decoding newline escapes after splitting. Do not split interpreted multiline cell content as records.
- 2026-11-21: Python's default Windows cp1252 decoding rejected a UTF-8 UI source file containing arrows. The earlier ASCII Character edit had completed, later UI edits had not. Resumed only the pending edits with explicit UTF-8 reads/writes. Always specify encoding for project text; no source file was truncated.
- 2026-11-21: the localization-registry merge selected regex group 1 (ID) instead of group 2 (path), causing a KeyError after the two broader registries were written. Corrected the path extraction, rebuilt only the pending registry, and verified every registered target exists. Use named capture groups for record merges.
- 2026-11-21: disabling Git autocrlf for a whitespace check made valid Windows CRLF endings appear as trailing whitespace. Re-ran with the repository conversion policy and explicit cr-at-eol handling; the check passed. Do not disable normalization merely to suppress conversion notices.
- exployt message/learning/ID MCP tools are unavailable in this session. User scope and these learnings are recorded here; registry IDs use shared ten-character UUID-derived hex IDs locally.
