# Localization workflow

Date: 2026-11-21. Implementation version: 1.1.0.

## Decision and supported languages

The user selected Unity Localization rather than I2 and requested English (`en`), Ukrainian (`uk`, not `ua`), Russian (`ru`), French (`fr`), Italian (`it`), German (`de`) and Spanish (`es`). Unity Localization 1.5.14 is pinned in Packages/manifest.json. The resolved Addressables version in packages-lock.json is 2.10.3 (Localization declares a minimum dependency of 1.25.0). No I2 asset, online translation service or credentials are needed.

## Editing translations

The editable runtime source is the `Game` String Table Collection under `Assets/Localization/Tables`. Open Window > Asset Management > Localization Tables and choose Game. Edit the seven language columns there. The game loads those official tables asynchronously and updates displayed strings when the locale changes. `Assets/Localization/Translations.json` contains 54 initial keys across seven languages and is an import seed, not a second runtime database.

`Tools > Student Survival > Set up localization` creates any missing locales/tables and imports missing keys. It preserves translations for keys already in the tables. Therefore editing a seed value alone does not overwrite a translated table value: edit the table for existing keys. Add new entries to the seed for reproducible setup, then invoke the setup command or allow the edit-mode content-hash check to run. Setup never runs while Play Mode is active.

Formatting uses numbered .NET placeholders, such as `{0}`, consistently across all seven translations. These entries are ordinary String Table values, not Smart Strings. Keep the same placeholder set in each language. The source review found no missing keys, empty translations, or mismatched placeholder sets on 2026-11-21.

The language selector uses native-language names so it remains understandable after a language switch. Choice is stored in `StudentSurvival.Language` in PlayerPrefs. A supported system language is selected on first use, with English as fallback. The English fallback is explicitly retained because Unity's database releases cached tables when a locale changes. A later locale request cancels the prior UI loading coroutine so stale loads cannot overwrite a more recent choice.

## What is covered

Menus, controls/help, pause/settings, stats, objectives, teacher progress, door/pickup prompts, inventory names/effects, gameplay feedback, exam rewards and win/loss messages use table keys. Original scene script diagnostics and the unused developer console are developer-facing and remain English.

The initial translations are draft content and need human review for natural wording and consistency. The dynamic LegacyRuntime font is reused; verify Cyrillic and accented characters and line wrapping in the actual player. No language has been certified by a native-language reviewer in this iteration.

Artwork, signage, logos and video frames retain their original lettering. The previous menu buttons had baked lettering, so the runtime menu uses localized text controls instead. For in-world art/video localization, add localized asset tables or subtitles with approved replacement assets; do not claim the entire media library has been translated.

## Build handling

Locales, String Tables, the collection/shared keys, LocalizationSettings, and `Assets/AddressableAssetsData` are project source assets and belong in version control with their .meta files. Generated Addressables build output, Library, Temp and logs are excluded from the registries. Setup enables Build Addressables With Player Build so the user-built executable includes localization content. The agent did not build or launch a player.

## References checked on 2026-11-21

- [Unity 6.3 released Localization package](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.localization.html): 1.5.14 is listed for Unity 6000.3.
- [Localization editor API](https://docs.unity3d.com/Packages/com.unity.localization@1.5/api/UnityEditor.Localization.LocalizationEditorSettings.html): creating collections, registering locales and marking tables for preload.
- [LocalizationSettings API](https://docs.unity3d.com/Packages/com.unity.localization@1.5/api/UnityEngine.Localization.Settings.LocalizationSettings.html): asynchronous initialization and selected-locale events.
- Installed package source under `Library/PackageCache/com.unity.localization@5ae578344e19`: verified editor API signatures and LocalizedDatabase.OnLocaleChanged releasing tables. Cache folder hashes are machine-specific; do not hardcode them in scripts.
