# File registry

Date: 2026-11-21. Implementation version: 1.3.0.

Coverage: reviewed project files, authored/generated source assets and user feedback for modernization/localization. Excludes Library/, Temp/, .utmp/, logs, bin/, obj/ and generated/archived player builds. IDs are shared across registries and generated locally because exployt ID tools are unavailable.

## Documentation

45ffd3c122 Docu/Localization/Files_Localization.md: Modernization/localization source
1ba7df1752 Docu/Localization/Localization.md: Seven-language Unity Localization authoring, setup, fallback and build workflow
762812c908 Docu/Modernization/Release_1.3.0.md: Approved scope, integrated handoff, generation prompt, validation evidence and learnings
15d7c7001b Docu/Start.md: Cold-start architecture, save/combat/UI integration and user-testing workflow for 1.3.0
3bf7ec00ca Updates.md: Dated semantic-version change history and validation status

## Scripts

1971f24ac2 Assets/Editor/LocalizationSetup.cs: Idempotent locale/table setup and Addressables player-build configuration
33bbf2be48 Assets/Scripts/Localization/GameLocalization.cs: Asynchronous Unity String Table access, retained fallback and locale preferences
cb7d7ee7e4 Assets/Scripts/UI/GameUI.RunFlow.cs: Continue/new game/retry UI, briefing, credits, save feedback and combat/route cues
116f52cf53 Assets/Scripts/UI/GameUI.cs: Translucent contextual HUD, modal navigation, remapped controls and shared UI helpers

## Configuration

4dd888d1cf Assets/AddressableAssetsData/AddressableAssetSettings.asset: Addressables source configuration for localized content
b5c052d659 Assets/AddressableAssetsData/AssetGroupTemplates/Packed Assets.asset: Addressables source configuration for localized content
3ac9d8ff70 Assets/AddressableAssetsData/AssetGroups/Default Local Group.asset: Addressables source configuration for localized content
3edbb0405b Assets/AddressableAssetsData/AssetGroups/Localization-Assets-Shared.asset: Addressables source configuration for localized content
f9518d3d0c Assets/AddressableAssetsData/AssetGroups/Localization-Locales.asset: Addressables source configuration for localized content
5070298f48 Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-de.asset: Addressables source configuration for localized content
ebd065ae3c Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-en.asset: Addressables source configuration for localized content
2c5441aa47 Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-es.asset: Addressables source configuration for localized content
1e2f2704e6 Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-fr.asset: Addressables source configuration for localized content
96f51632c5 Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-it.asset: Addressables source configuration for localized content
606fb64b8e Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-ru.asset: Addressables source configuration for localized content
ed09ac4d03 Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-uk.asset: Addressables source configuration for localized content
f8267e03b2 Assets/AddressableAssetsData/AssetGroups/Schemas/Default Local Group_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
45d38e70b5 Assets/AddressableAssetsData/AssetGroups/Schemas/Default Local Group_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
39603df112 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Assets-Shared_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
303e23d377 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Assets-Shared_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
4d7507f379 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Assets-Shared_LocalizationGroupSchema.asset: Addressables source configuration for localized content
5accf584f2 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Locales_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
0b8c1123f5 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Locales_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
539da11397 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Locales_LocalizationGroupSchema.asset: Addressables source configuration for localized content
a568f47832 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-de_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
68aedf3ca7 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-de_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
02fff98270 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-de_LocalizationGroupSchema.asset: Addressables source configuration for localized content
be46ea13dc Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-en_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
73a67d375e Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-en_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
e933ca8dba Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-en_LocalizationGroupSchema.asset: Addressables source configuration for localized content
ab06d84ac9 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-es_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
402598dac8 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-es_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
5f4a312671 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-es_LocalizationGroupSchema.asset: Addressables source configuration for localized content
6255b978e3 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-fr_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
e4a2dd4c3b Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-fr_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
0b9f078a84 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-fr_LocalizationGroupSchema.asset: Addressables source configuration for localized content
85bd51c49b Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-it_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
e304e1023b Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-it_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
f80a31508c Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-it_LocalizationGroupSchema.asset: Addressables source configuration for localized content
2f613ef21e Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-ru_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
409bc62dcd Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-ru_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
430d2278e2 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-ru_LocalizationGroupSchema.asset: Addressables source configuration for localized content
363677dbd0 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-uk_BundledAssetGroupSchema.asset: Addressables source configuration for localized content
0416cd3b7b Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-uk_ContentUpdateGroupSchema.asset: Addressables source configuration for localized content
47d1d40fe5 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-uk_LocalizationGroupSchema.asset: Addressables source configuration for localized content
c26d269cfe Assets/AddressableAssetsData/DataBuilders/BuildScriptFastMode.asset: Addressables source configuration for localized content
1f66b275f4 Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset: Addressables source configuration for localized content
6d27f91af6 Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedPlayMode.asset: Addressables source configuration for localized content
60820035ff Assets/AddressableAssetsData/DefaultObject.asset: Addressables source configuration for localized content
40466c485c Assets/Editor/StudentSurvival.Editor.asmdef: Assembly definition and dependencies
2a475ab229 Assets/Localization/Locales/de.asset: Registered Unity locale asset
92354354c6 Assets/Localization/Locales/en.asset: Registered Unity locale asset
0db1fc7b42 Assets/Localization/Locales/es.asset: Registered Unity locale asset
cfde29faa3 Assets/Localization/Locales/fr.asset: Registered Unity locale asset
583441aa8a Assets/Localization/Locales/it.asset: Registered Unity locale asset
1f42ee434e Assets/Localization/Locales/ru.asset: Registered Unity locale asset
c5eb53524d Assets/Localization/Locales/uk.asset: Registered Unity locale asset
a061523ea5 Assets/Localization/LocalizationSettings.asset: Modernization/localization project source
faff2bb0e1 Assets/Localization/Tables/Game Shared Data.asset: Unity Game string table, shared keys or collection asset
597457597b Assets/Localization/Tables/Game.asset: Unity Game string table, shared keys or collection asset
3be7f0d3fe Assets/Localization/Tables/Game_de.asset: Unity Game string table, shared keys or collection asset
dce940d384 Assets/Localization/Tables/Game_en.asset: Unity Game string table, shared keys or collection asset
8fc28712b5 Assets/Localization/Tables/Game_es.asset: Unity Game string table, shared keys or collection asset
f3ec8ca3c5 Assets/Localization/Tables/Game_fr.asset: Unity Game string table, shared keys or collection asset
998c9da39f Assets/Localization/Tables/Game_it.asset: Unity Game string table, shared keys or collection asset
6c32a48c4b Assets/Localization/Tables/Game_ru.asset: Unity Game string table, shared keys or collection asset
18117b539a Assets/Localization/Tables/Game_uk.asset: Unity Game string table, shared keys or collection asset
009446dfeb Assets/Localization/Translations.json: 116 translation seed entries across English, Ukrainian, Russian, French, Italian, German and Spanish
a739a6871d Assets/Resources/Credits.json: Evidence-based contributor/package/audio credits and pending attribution records
8107f24753 Assets/Scripts/Localization/StudentSurvival.Localization.asmdef: Assembly definition and dependencies
d778fa91ef Packages/manifest.json: Reviewed project configuration
4ef83532d5 Packages/packages-lock.json: Reviewed project configuration

## Other

8101eb494f Assets/AddressableAssetsData.meta: Unity import GUID and asset metadata
f9a7134d0b Assets/AddressableAssetsData/AddressableAssetSettings.asset.meta: Unity import GUID and asset metadata
867468869e Assets/AddressableAssetsData/AssetGroupTemplates.meta: Unity import GUID and asset metadata
384c368119 Assets/AddressableAssetsData/AssetGroupTemplates/Packed Assets.asset.meta: Unity import GUID and asset metadata
c6a50af14a Assets/AddressableAssetsData/AssetGroups.meta: Unity import GUID and asset metadata
0d36c8ebbd Assets/AddressableAssetsData/AssetGroups/Default Local Group.asset.meta: Unity import GUID and asset metadata
5005c8d253 Assets/AddressableAssetsData/AssetGroups/Localization-Assets-Shared.asset.meta: Unity import GUID and asset metadata
2fb192d250 Assets/AddressableAssetsData/AssetGroups/Localization-Locales.asset.meta: Unity import GUID and asset metadata
5e3d8b792a Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-de.asset.meta: Unity import GUID and asset metadata
d5811f5e6b Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-en.asset.meta: Unity import GUID and asset metadata
4bdaa3c780 Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-es.asset.meta: Unity import GUID and asset metadata
9bc892fa9a Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-fr.asset.meta: Unity import GUID and asset metadata
f4ae6efb88 Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-it.asset.meta: Unity import GUID and asset metadata
7b94a4a1d9 Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-ru.asset.meta: Unity import GUID and asset metadata
d3d1c19e04 Assets/AddressableAssetsData/AssetGroups/Localization-String-Tables-uk.asset.meta: Unity import GUID and asset metadata
a79c2656fb Assets/AddressableAssetsData/AssetGroups/Schemas.meta: Unity import GUID and asset metadata
a25fcde4c6 Assets/AddressableAssetsData/AssetGroups/Schemas/Default Local Group_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
d64f98aae3 Assets/AddressableAssetsData/AssetGroups/Schemas/Default Local Group_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
b139d5cd6c Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Assets-Shared_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
6f988a8e39 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Assets-Shared_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
61c19602f4 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Assets-Shared_LocalizationGroupSchema.asset.meta: Unity import GUID and asset metadata
ba7c915b8b Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Locales_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
04cfed419e Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Locales_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
ddef049511 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-Locales_LocalizationGroupSchema.asset.meta: Unity import GUID and asset metadata
2cfc9dd581 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-de_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
16a23dc5ec Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-de_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
80b2d24268 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-de_LocalizationGroupSchema.asset.meta: Unity import GUID and asset metadata
25fa9d83f5 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-en_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
cdf84d8d30 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-en_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
60b8a85dd5 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-en_LocalizationGroupSchema.asset.meta: Unity import GUID and asset metadata
e1df9c368b Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-es_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
91be6a8fbd Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-es_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
981c8322c2 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-es_LocalizationGroupSchema.asset.meta: Unity import GUID and asset metadata
c3cfda7798 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-fr_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
2ff84045fe Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-fr_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
6d363e965c Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-fr_LocalizationGroupSchema.asset.meta: Unity import GUID and asset metadata
ef078e9c8a Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-it_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
299ea1eb0d Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-it_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
6eec37342b Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-it_LocalizationGroupSchema.asset.meta: Unity import GUID and asset metadata
7259c6e8dc Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-ru_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
8ad6834b62 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-ru_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
55c715d82c Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-ru_LocalizationGroupSchema.asset.meta: Unity import GUID and asset metadata
ab6a6b0e0e Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-uk_BundledAssetGroupSchema.asset.meta: Unity import GUID and asset metadata
ced595bc6e Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-uk_ContentUpdateGroupSchema.asset.meta: Unity import GUID and asset metadata
f831277be0 Assets/AddressableAssetsData/AssetGroups/Schemas/Localization-String-Tables-uk_LocalizationGroupSchema.asset.meta: Unity import GUID and asset metadata
44ce828ff4 Assets/AddressableAssetsData/DataBuilders.meta: Unity import GUID and asset metadata
75a4348309 Assets/AddressableAssetsData/DataBuilders/BuildScriptFastMode.asset.meta: Unity import GUID and asset metadata
17ccc23c6c Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset.meta: Unity import GUID and asset metadata
5bb675812c Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedPlayMode.asset.meta: Unity import GUID and asset metadata
3cc49069e7 Assets/AddressableAssetsData/DefaultObject.asset.meta: Unity import GUID and asset metadata
b783a9d991 Assets/Editor/LocalizationSetup.cs.meta: Unity import GUID and asset metadata
85619b538a Assets/Editor/StudentSurvival.Editor.asmdef.meta: Unity import GUID and asset metadata
1192baff0c Assets/Localization.meta: Unity import GUID and asset metadata
9f853333ef Assets/Localization/Locales.meta: Unity import GUID and asset metadata
35f48da033 Assets/Localization/Locales/de.asset.meta: Unity import GUID and asset metadata
708dcc58ae Assets/Localization/Locales/en.asset.meta: Unity import GUID and asset metadata
c3962b87c1 Assets/Localization/Locales/es.asset.meta: Unity import GUID and asset metadata
de89aecd51 Assets/Localization/Locales/fr.asset.meta: Unity import GUID and asset metadata
9dfd69664d Assets/Localization/Locales/it.asset.meta: Unity import GUID and asset metadata
84c78aaab1 Assets/Localization/Locales/ru.asset.meta: Unity import GUID and asset metadata
0b6dfc0575 Assets/Localization/Locales/uk.asset.meta: Unity import GUID and asset metadata
09123673b6 Assets/Localization/LocalizationSettings.asset.meta: Unity import GUID and asset metadata
0c1d023aca Assets/Localization/SetupVersion.txt: Modernization/localization project source
0fcc225251 Assets/Localization/SetupVersion.txt.meta: Unity import GUID and asset metadata
2b6bdc9984 Assets/Localization/Tables.meta: Unity import GUID and asset metadata
32bce5de3f Assets/Localization/Tables/Game Shared Data.asset.meta: Unity import GUID and asset metadata
a5e778ebf5 Assets/Localization/Tables/Game.asset.meta: Unity import GUID and asset metadata
8bac55396f Assets/Localization/Tables/Game_de.asset.meta: Unity import GUID and asset metadata
fe3bf98cda Assets/Localization/Tables/Game_en.asset.meta: Unity import GUID and asset metadata
f6456f282f Assets/Localization/Tables/Game_es.asset.meta: Unity import GUID and asset metadata
bfa944db74 Assets/Localization/Tables/Game_fr.asset.meta: Unity import GUID and asset metadata
0167edc898 Assets/Localization/Tables/Game_it.asset.meta: Unity import GUID and asset metadata
6a50cecbbc Assets/Localization/Tables/Game_ru.asset.meta: Unity import GUID and asset metadata
3183e1381e Assets/Localization/Tables/Game_uk.asset.meta: Unity import GUID and asset metadata
c2620eb3fb Assets/Localization/Translations.json.meta: Unity import GUID and asset metadata
fec6c447a7 Assets/Resources/Credits.json.meta: Unity import metadata for Assets/Resources/Credits.json
980d7bd5c5 Assets/Scripts/Localization.meta: Unity import GUID and asset metadata
5d4e9920f5 Assets/Scripts/Localization/GameLocalization.cs.meta: Unity import GUID and asset metadata
d4cfd1021f Assets/Scripts/Localization/StudentSurvival.Localization.asmdef.meta: Unity import GUID and asset metadata
fbcb5d88a0 Assets/Scripts/UI/GameUI.RunFlow.cs.meta: Unity import metadata for Assets/Scripts/UI/GameUI.RunFlow.cs
5447061bfa Assets/Scripts/UI/GameUI.cs.meta: Unity import GUID and asset metadata
