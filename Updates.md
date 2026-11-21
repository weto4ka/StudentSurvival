# Updates

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
