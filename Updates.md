# Updates

## 1.5.1 — 2026-11-21

- Replaced public Git-handle team rows with the user's four named teammates and a localized fifth-teammate placeholder. Preserved Git handles separately without guessing identity mappings; recorded the apparent Максм/Максим typo assumption.
- Added optional localized credit-name support and one seven-language seed row (149 total).
- The user authorized a Play Mode check through Unity MCP, with user testing as fallback. Unity MCP tools are not exposed to this chat; no Play Mode check was run. The latest automatic Editor log showed a successful assembly reload/localization setup, which is not a gameplay result.

## 1.5.0 — 2026-11-21

- Removed menu camera's obsolete GUILayer causing both removed-component warnings; retained supported camera/audio components.
- Hid Continue when no save exists and reflowed menu actions. Separated How to play and Controls throughout the menu/pause/settings hierarchy with accurate localized titles.
- Rebuilt help and credits as readable flowing scroll layouts, removed duplicate Back actions, and kept a single meaningful back/start action per context.
- Fixed dark multiplied hover tint, added bright independent pixel borders for pointer/controller focus, and recovered focus borders in cloned dropdown rows. Input changes now refresh help and cached hint bindings.
- Bundled the licensed Press Start 2P pixel font with its complete OFL notice, Point filtering and seven-language/name/arrow glyph coverage checks. Central minus normalization avoids a missing glyph without changing font data or translator tables.
- Strengthened synthesized UI cues without clipping; added audio bootstrap recovery, priority and missing-resource diagnostics while respecting user volumes.
- Added 15 guide/menu and 2 font-credit translation keys (148 total), source review and exact menu-repair checks. Actual runtime/audio approval remains pending under the user's earlier testing instruction.

## 1.4.1 — 2026-11-21

- Verified Main and its Idle/Run/Jump controller clips use original pixel PSD artwork; explicitly serialized that pixel idle reference for shared crouch/jump/landing crops, independent of changing Animator frames. The cartoon PNG shown during inspection remains outside this path.
- Confirmed and previewed the actual minimal menu banner. Source checks only; gameplay remains user-owned.

## 1.4.0 — 2026-11-21

- Added an optional friendly-student flirt interaction: one collected lab becomes a love letter, with a brief heart and persisted once-per-run state. Walking past never consumes items.
- Reworked energy drinks into 3 borrowed Sleep, 20 seconds of overstimulation/speed and a 10-second slower/sluggish crash. Unused borrowed Sleep expires; food remains permanent. Active boosts do not stack or consume a refused drink. Zero Sleep now gives an eight-second supply-rescue window with a visible countdown and inventory; zero Intelligence remains immediate loss.
- Saved temporary effect durations and energy debt alongside progression; older positive-Sleep snapshots remain compatible. Updated seven-language onboarding/effect/error/choice strings to 131 seed keys.
- Added shared articulated jump ascent, descent and landing poses instead of straight legs.
- Added the default minimal/mysterious campus banner and a persisted Settings artwork choice retaining the previous detailed illustration.
- Source-compiled all eleven assemblies without executing gameplay or building a player. User checks and balance/style approval remain pending.

## 1.3.0 — 2026-11-21

- Added versioned local autosave, atomic replacement/backup, Continue and confirmed New game. Inventory/world pickup IDs, stats/speed, grounded position/stance and examiner stage/health restore together. Menu exit preserves progress; death retries the last checkpoint; victory closes the run. Write/restore failures surface without overwriting a partially restored run.
- Added a first-run briefing, real-binding resource explanations and a short-lived classroom route through the authored doors. Paid throws cost 1 Sleep; collected labs remain free to throw.
- Added lane telegraphs, jump/crouch warnings, teacher/player hit reactions and visible damage protection. The final examiner has different original character art and alternating low/high two-shot bursts. Shared projectile logic prevents repeated impacts and respects pause.
- Replaced the squashed crouch sprite with an articulated pose and breathing/shuffle animation using the original artwork.
- Added student-themed generated title art, in-game credits, preserved dependency notices and a sourced attribution audit. Legacy asset/music/video permissions remain unresolved pending team input.
- Expanded seven-language seed coverage from 86 to 116 entries; reviewed placeholder parity, completeness and obvious wording. Native review remains pending. Table setup preserves existing translator edits.
- Source-compiled all eleven project assemblies against installed Unity references without running them. No gameplay, controller or player-build tests were performed; exact user checks are in Docu/Modernization/User_Test_Checklist.md.

## 1.2.0 — 2026-11-21

- Revised the 1.1.0 UI in response to user screenshots: translucent compact/contextual HUD, fading occupied-slot inventory, corner settings gear, aligned settings, language dropdown, upper-left back arrows and transparent pause overlays. Removed the developer-facing localization/media note from Settings.
- Added hold-to-crouch with reduced collider/speed and headroom protection, using the existing sprite art.
- Moved defaults to a stable-ID editable Input Actions asset; added keyboard/mouse and controller rebinding, saved overrides, conflict rejection, cancellation, restore defaults and effective-binding hints. Controller shoulders select inventory items and north/Y uses them.
- Added four original generated UI sound cues, saved UI volume and existing menu music wiring; kept original in-game music.
- Made Editor Play start at menu by default and clarified window modes: disabled in the Editor preview, explicit Windowed/Borderless Fullscreen requests in a player.
- Corrected validation of the two intentionally locked doors with no destination. Added BT.709 metadata to usb_soft.mp4 without re-encoding its video/audio; matching decoded frame payloads were verified outside Unity.
- Extended the translation seed to 86 keys in seven languages. Gameplay/audio audition/UI runtime checks remain delegated to the user. See the feedback ledger and updated user checklist.

## 1.1.0 — 2026-11-21

- Implemented one player-only interaction path for doors and pickups; E presses no longer repeatedly reset door countdowns. Leaving a door cancels a pending transfer.
- Moved horizontal movement to Rigidbody2D physics; added filtered reusable ground casts, jump buffering, coyote time and damage grace. Main explicitly starts at 10 Sleep.
- Added a localized screen-space HUD/inventory, item effect hints, nearby prompts, guided objective, exam progress, pause/settings, terminal result panels and saved language/volume/text-size/fullscreen preferences.
- Added the user-selected final examiner in the existing classroom, using the same combat code. First exam grants +3 Sleep/+1 Intelligence; second examiner requires 7 lab hits.
- Integrated Unity Localization 1.5.14 with English, Ukrainian, Russian, French, Italian, German and Spanish, 54 seed keys, generated tables/locales, saved locale selection and asynchronous fallback handling. Source translations are draft content awaiting user language review; baked artwork/video lettering is unchanged.
- Migrated legacy menu UGUI script references, centralized scene loading, improved camera bounds/facing and safe audio triggers, and replaced the baked fake-error overlay with localized feedback.
- Added architecture/start documentation, localization workflow and a full user test checklist. Existing pre-task uncommitted work is preserved. Gameplay tests/builds are intentionally delegated to the user; this version is not a verified release.
