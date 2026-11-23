# Localization workflow

Date: 2026-11-22. Implementation version: 1.8.0.

Boundary update — 2026-11-22, 1.7.2: added world.fallRecovered in all seven locales. The seed now contains221 entries.

1.8.0 update — 2026-11-22: added 14 food/trade/WC keys for 235 seed entries. Prompts use effective Interact bindings; completed tip exchanges expose free repeat advice. Seven-language values, placeholder sets and all235 imported official table entries are verified; native-speaker and displayed-layout review remain pending. Earlier counts below are dated history.

## Decision and supported languages

The user selected Unity Localization rather than I2 and requested English (`en`), Ukrainian (`uk`, not `ua`), Russian (`ru`), French (`fr`), Italian (`it`), German (`de`) and Spanish (`es`). Unity Localization 1.5.14 is pinned in Packages/manifest.json. The resolved Addressables version in packages-lock.json is 2.10.3 (Localization declares a minimum dependency of 1.25.0). No I2 asset, online translation service or credentials are needed.

## Editing translations

The editable runtime source is the `Game` String Table Collection under `Assets/Localization/Tables`. Open Window > Asset Management > Localization Tables and choose Game. Edit the seven language columns there. The game loads those official tables asynchronously and updates displayed strings when the locale changes. `Assets/Localization/Translations.json` contains 235 seed keys across seven languages and is an import seed, not a second runtime database.

`Tools > Student Survival > Set up localization` creates any missing locales/tables and imports missing keys. It preserves translations for keys already in the tables. Therefore editing a seed value alone does not overwrite a translated table value: edit the table for existing keys. Add new entries to the seed for reproducible setup, then invoke the setup command or allow the edit-mode content-hash check to run. Setup never runs while Play Mode is active.

Formatting uses numbered .NET placeholders, such as `{0}`, consistently across all seven translations. These entries are ordinary String Table values, not Smart Strings. Keep the same placeholder set in each language. The source review found no missing keys, empty translations, or mismatched placeholder sets on 2026-11-21.

The language dropdown uses native-language names so it remains understandable after a language switch. Choice is stored in `StudentSurvival.Language` in PlayerPrefs. A supported system language is selected on first use, with English as fallback. The English fallback is explicitly retained because Unity's database releases cached tables when a locale changes. A later locale request cancels the prior UI loading coroutine so stale loads cannot overwrite a more recent choice.

## What is covered

Menus, controls/help, pause/settings, stats, objectives, teacher progress, door/pickup prompts, inventory names/effects, gameplay feedback, exam rewards, save/confirmation/error messages, the first-run briefing, route/combat/protection hints, credits headings and win/loss messages use table keys. Contributor names and package names remain proper names. Original scene script diagnostics and the unused developer console are developer-facing and remain English.

The initial translations are draft content and need human review for natural wording and consistency. Press Start 2P is bundled as the pixel UI font with its complete OFL notice and actual cmap coverage checks. Unsupported U+2212 minus is displayed as ASCII hyphen-minus centrally; verify Cyrillic and accented characters and line wrapping in the actual player. No language has been certified by a native-language reviewer in this iteration.

Artwork, signage, logos and video frames retain their original lettering. The previous menu buttons had baked lettering, so the runtime menu uses localized text controls instead. For in-world art/video localization, add localized asset tables or subtitles with approved replacement assets; do not claim the entire media library has been translated.

## 1.6.0 additions

Fifteen added keys (164 total) explain selected-item use, lab location/hold-repeat, empty ammo, shorter HUD stats and actionable teacher dodges. Adaptive pixel font sizing and line spacing replace cramped small-view paragraphs. Keyboard display names come from canonical binding layouts independently of OS language. Native translations and actual runtime legibility still need user review.

## 1.5.0 additions

Seventeen added keys separate How to play from Controls, provide flowing guide sections and credit the font. All 148 rows have complete seven-language cells and matching placeholders. Credits and help wrap in flowing layouts; font cmap coverage includes all displayed seed characters, names and arrows after minus normalization. Native wording/readability approval remains pending.

## 1.4.0 additions

Fifteen added keys cover timed caffeine/zero-Sleep rescue, optional flirting, versioned resource/briefing/loss wording and title-art choices. Existing translator edits are preserved: old effect/briefing keys remain available but the runtime selects the accurate versioned keys. All 131 rows retain seven nonempty cells and matching numbered placeholders.

## 1.3.0 review

The 30 added entries cover persistence, onboarding, combat and credits. All 116 × seven cells are nonempty, keys are unique and numbered placeholder multisets match English. Literal UI/feedback references and Credits.json key references resolve. Source review corrected hold-to-crouch phrasing in German/Spanish and clarified the zero-Sleep-or-Intelligence loss rule in French/Italian/German/Spanish. These are source/editorial checks, not native-speaker approval or a font/layout pass.

The open Editor may still have the 86-key tables until its next refresh/setup. New entries import from the reviewed seed; existing table values are intentionally preserved. If a translator subsequently changes a table, do not replace it with the seed automatically.

## Build handling

Locales, String Tables, the collection/shared keys, LocalizationSettings, and `Assets/AddressableAssetsData` are project source assets and belong in version control with their .meta files. Generated Addressables build output, Library, Temp and logs are excluded from the registries. Setup enables Build Addressables With Player Build so the user-built executable includes localization content. The agent did not build or launch a player.

## References checked on 2026-11-21

- [Unity 6.3 released Localization package](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.localization.html): 1.5.14 is listed for Unity 6000.3.
- [Localization editor API](https://docs.unity3d.com/Packages/com.unity.localization@1.5/api/UnityEditor.Localization.LocalizationEditorSettings.html): creating collections, registering locales and marking tables for preload.
- [LocalizationSettings API](https://docs.unity3d.com/Packages/com.unity.localization@1.5/api/UnityEngine.Localization.Settings.LocalizationSettings.html): asynchronous initialization and selected-locale events.
- Installed package source under `Library/PackageCache/com.unity.localization@5ae578344e19`: verified editor API signatures and LocalizedDatabase.OnLocaleChanged releasing tables. Cache folder hashes are machine-specific; do not hardcode them in scripts.


Entrance update1.6.1 (2026-11-22):166 seed keys across seven languages. door.leaveUniversity and door.enterUniversity use the effective Interact binding as {0}; scene overrides select these keys through the shared Teleport prompt implementation.


Flirt discoverability1.6.3 (2026-11-22):167 seven-language seed keys. flirt.nearbyHint has two lines, the rebound Interact control as {0}, and an explicit cost of one collected lab. A fresh key lets setup add it without overwriting translator-edited table entries.


Clickable credits1.6.4 (2026-11-22):168 seed keys. credits.profileHint explains that selecting a name opens the creator’s GitHub profile. The five authored names and githubUrl values live in Resources/Credits.json and remain the same across languages.


Campaign1.7.0 (2026-11-22):215 source entries across en/uk/ru/fr/it/de/es. Added teacher names, explicit admission/portals, guard/notes, free-period/readiness/social hints, three-stage How to play and semester completion text. Existing table entries are retained; new keys are added by the content-hash/version setup in edit mode. Font cmap and format placeholders were checked against the shipped font. Native-speaker/runtime layout review remains pending.


Completion/pacing1.7.1 (2026-11-22):220 keys. Added Keep exploring, completed-campus explanation/objective, teacher-recovery cue and revised combat instructions. Fresh keys avoid overwriting existing translator-edited entries. Seven-language placeholders and pixel-font coverage pass; runtime layout review remains pending.
