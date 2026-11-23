# File registry

Date: 2026-11-22. Implementation version: 1.8.5.

Coverage: reviewed project files, authored/generated source assets and user feedback for modernization/localization. Excludes Library/, Temp/, .utmp/, logs, bin/, obj/ and generated/archived player builds. IDs are shared across registries and generated locally because exployt ID tools are unavailable.

## Documentation

ba34eeec01 Docu/Modernization/Intro_Passage_1.8.3.md: Intro encoding correction and slim passage markers

1b5827e620 Docu/Modernization/Teacher_Music_1.8.2.md: First-professor occlusion/pushing and original classroom music trigger diagnosis, repairs and user checks

80d57e1aff Docu/Modernization/WC_Entrance_1.8.1.md: Restored earlier open WC entrance artwork with separate-room travel retained and user visual checks

6b3ad406d7 Docu/Modernization/Editor_Assets_1.8.0.json: Preserved final automatic Unity four-prefab asset-reference report; not a gameplay test

e4d2ad034e Assets/Art/Campaign/README.md: Generated pixel artwork provenance, sprite slicing and maintained prefab editing workflow
8956457ce7 Docu/Modernization/Authored_Campus_1.8.0.md: Editable room architecture, separate WC, student exchanges, finite food, compatibility and exact user checks
09e290f0ff Docu/Modernization/Checks_1.8.0.json: Final offline structural, geometry, localization, font and source-compilation results; runtime checks pending
81c8a4b409 Docu/Modernization/Known_Issues_1.8.0.md: Confirmed reports, unresolved exit failure, verification limits and retained implementation lessons

e723216043 Docu/Modernization/Room_Identity_1.7.3.md: Distinct classroom interiors, pixel doorways, teacher appearance selection and user checks

57a07114f2 Docu/Modernization/Map_Boundaries_1.7.2.md: Missing west end walls, last-grounded fall recovery, save compatibility and user checks

5b4d6bb571 Docu/Modernization/Completion_Difficulty_1.7.1.md: Persistent post-victory exploration, save compatibility, harder combat pacing and user checks

e35cd95744 Docu/Modernization/Campaign_1.7.0.md: Three-stage campaign rules, architecture, save compatibility, verification and expansion boundary

b8fb1d7e64 Docu/Modernization/Hall_Floor_1.6.6.md: Hall-tree directional snag diagnosis, unified collision floor and user checks

cbdc8d11d1 Docu/Modernization/Entrance_1.6.5.md: Outdoor column follow-up, rotated ramp lip correction, failed earlier assumption and run completion/user checks

f03b98f64d Docu/Modernization/Ceiling_Music_1.6.2.md: Missing roof and music-region feedback, source diagnosis, repairs and exact runtime checks

4c2f4e9aed Docu/Modernization/Entrance_1.6.1.md: Entrance feedback, exact collider-clearance diagnosis, destination prompts and user checks

45ffd3c122 Docu/Localization/Files_Localization.md: Modernization/localization source
1ba7df1752 Docu/Localization/Localization.md: Seven-language Unity Localization authoring, setup, fallback and build workflow
69e71dd5c3 Docu/Modernization/Combat.md: Telegraphs, final examiner, projectile ownership and combat verification steps
cb16ad525a Docu/Modernization/Credits_Attribution.md: Sourced contributor/asset/package audit and unresolved original media credits
a6a0262ddf Docu/Modernization/Distractions.md: Optional student interaction implementation, art attribution and exact user checks
4dba12733b Docu/Modernization/Energy_Distractions_1.4.0.md: User feedback, energy/distraction rules, banner generation prompt and 1.4.0 handoff
6ab5c037eb Docu/Modernization/Feedback_1.6.0.md: Five preserved screenshots, readable UI/physical-key labels, measured poses and lab/control clarification
285c4a68d3 Docu/Modernization/Feedback_2026-11-21.md: User feedback, implementation mapping, attachments and revision-specific learnings
4dec71f7ca Docu/Modernization/Files_Modernization.md: Modernization workstream file registry
6d34360f91 Docu/Modernization/Font_Audit.md: Font source/license/hash/import and actual cmap coverage audit
7155ef583a Docu/Modernization/Jump_Animation.md: Shared articulated jump ascent/descent/landing implementation and user checks
a0f13cc250 Docu/Modernization/Menu_Feedback_1.5.0.md: User menu complaints, navigation/layout/focus repairs, approval correction and validation limits
80acbd1e9e Docu/Modernization/Menu_Repair_Audio.md: Obsolete camera reference diagnosis and measured louder interface cues
69aad42c73 Docu/Modernization/Next_Steps_2026-11-21.md: User feedback and evidence-based proposals for autosave, student title art and prioritized product gaps
762812c908 Docu/Modernization/Release_1.3.0.md: Approved scope, integrated handoff, generation prompt, validation evidence and learnings
e14e3cb984 Docu/Modernization/Review_2026-11-21.md: Dated project map, prior work, code findings, runtime limits, TODOs and learnings
f1fc976f2c Docu/Modernization/Save_System.md: Persistence schema, player rules, failure handling and user checks
5a339bdf4a Docu/Modernization/User_Test_Checklist.md: Exact user-owned menu, language, interaction, movement and complete-run checks
ae5fc9d621 Docu/Modernization/Web_Itch_Readiness_2026-11-21.md: Official-source Web/itch compatibility assessment, unresolved publication rights and proposed browser checks
15d7c7001b Docu/Start.md: Cold-start architecture, screenshot-driven 1.6.0 fixes and user runtime fallback
1d46bbe443 Docu/_Files.md: Global registry initialized for the reviewed project files
e3b4fff93b README.md: Project overview, supplied creator roster, gameplay/controls, saves, languages, Unity setup and attribution/verification status
3bf7ec00ca Updates.md: Dated semantic-version change history and validation status

## Scripts

19d2700df0 Assets/Editor/CampaignRoomValidation.cs: Read-only Unity prefab validation and explicitly requested preview rendering menus
077e9d4655 Assets/Scripts/Inventory/StudentTrade.cs: Authored student offers, atomic lab-for-drink exchange, paid repeatable advice and persistent receipts

3378d4d23e Assets/Scripts/World/CampaignState.cs: Shared campaign preparation, quests, completion and student trade receipt save state

cb6c69c308 Assets/Scripts/UI/GameUI.Campaign.cs: Continuous stage objectives, contextual action formatting and campaign HUD routing

e14e75f205 Assets/Scripts/Inventory/PreparationDesk.cs: Study interaction reusing authored desk artwork and trigger collider

0873b6d2a2 Assets/Scripts/Inventory/CheatingPixelArt.cs: Original point-filtered pixel guard/toilet/note props with resource cleanup

7ea65fbe3a Assets/Scripts/Inventory/CheatingInteraction.cs: Notes/study/use targets sharing E interactions and persistent campaign state

4088f8fa4c Assets/Scripts/Inventory/CheatingEncounter.cs: Guard patrol, separate-WC hidden notes, authored revision markers and recoverable confiscation

c0d6638380 Assets/Scripts/Inventory/CampaignSupplies.cs: Finite room supplies with stable pickup IDs initialized before save restoration

74376f6142 Assets/Scripts/Characters/CampaignRooms.cs: Serialized authored-room registry, gameplay anchors and shared music regions; no room geometry construction

ab50eb795f Assets/Scripts/Characters/CampaignRoomArt.cs: Serialized references to original pixel room artwork

453f807b94 Assets/Scripts/Characters/CampaignPortal.cs: Authored door triggers, stage gates, room arrival markers and checkpoint relocation

d353d24e3f Assets/Scripts/Characters/CampaignExamAdmission.cs: Explicit preparation-gated exam start through shared interaction targeting

60ce3f150a Assets/Scripts/Cameras/CameraBoundsRegion.cs: Serialized local camera bounds registered for authored room frames

12bffcee59 Assets/Scripts/Audio/MusicRegionMixer.cs: Shared persistent music voices, region priority, crossfades and retained clip cursors

dd715dc486 Assets/Editor/GameStartup.cs: Menu-first Editor Play default with explicit menu/open-scene tools
1971f24ac2 Assets/Editor/LocalizationSetup.cs: Idempotent locale/table setup and Addressables player-build configuration
d0660744f2 Assets/Scripts/Audio/AudioController.cs: Reviewed AudioController implementation
534d708792 Assets/Scripts/Cameras/CameraController.cs: Reviewed CameraController implementation
049dc9b8da Assets/Scripts/Characters/BulletLab.cs: Reviewed BulletLab implementation
142a8cc85e Assets/Scripts/Characters/BulletOcenka.cs: Reviewed BulletOcenka implementation
b26231380c Assets/Scripts/Characters/Character.Energy.cs: Timed caffeine borrowing, overstimulation/crash, zero-Sleep rescue and save hooks
183f2efcad Assets/Scripts/Characters/Character.cs: Physics movement/crouch, hit protection, safe grounded stance save and restore
ab4b60a0e8 Assets/Scripts/Characters/CombatProjectile.cs: Pause-aware shared projectile motion/lifetime and once-only damage
84e5f71379 Assets/Scripts/Characters/ExamProgression.cs: Three-stage exam progression bound to authored teachers and room-aware objective routing
c36101052d Assets/Scripts/Characters/ExaminerAppearance.cs: Resource-configured distinct final-exam art fitting to the original collider
74fec759d9 Assets/Scripts/Characters/Monster.cs: Reviewed Monster implementation
62bc4cf8ca Assets/Scripts/Characters/Prepod.cs: Compact wind-up marks and warning phases, natural-crouch-safe high throws and shared two-stage combat
e90817392d Assets/Scripts/Characters/Unit.cs: Reviewed Unit implementation
275fc7e7f2 Assets/Scripts/Characters/Visuals/CrouchVisual.cs: Measured overlapping hierarchical pixel-body joints, ground solver and crouch/jump transitions
843f334f29 Assets/Scripts/Controls/GameInput.cs: Shared held/pressed actions and canonical named keys with printed keyboard-letter preservation
8495432e50 Assets/Scripts/Inventory/FlirtDistraction.cs: Explicit one-lab friendly-student distraction and saved once-per-run choice
4f3de9285a Assets/Scripts/Inventory/InventoryController.cs: Stacked item use/save/restore and atomic collected-lab exchange capacity/payment handling
ddf030be88 Assets/Scripts/Inventory/InventoryItem.cs: Reviewed InventoryItem implementation
cdc9997794 Assets/Scripts/Inventory/Items/ItemBook.cs: Reviewed ItemBook implementation
cb7438fd4d Assets/Scripts/Inventory/Items/ItemEnergyDring.cs: Reviewed ItemEnergyDring implementation
1307773470 Assets/Scripts/Inventory/Items/ItemLab.cs: Reviewed ItemLab implementation
21ce7d2c8b Assets/Scripts/Inventory/Items/ItemShaverma.cs: Reviewed ItemShaverma implementation
4cdb6fe7c0 Assets/Scripts/Inventory/LootBoxItem.cs: Finite world pickup availability, immediate target retirement and held-item collider cleanup
33bbf2be48 Assets/Scripts/Localization/GameLocalization.cs: Asynchronous official String Tables, locale/fallback lifecycle and pixel-font minus normalization
4d0923d42b Assets/Scripts/Timers/AbstractTimeProvider.cs: Reviewed AbstractTimeProvider implementation
8f56dbc59e Assets/Scripts/Timers/Providers/FadeLoaderTimer.cs: Reviewed FadeLoaderTimer implementation
2f3c3fbd1b Assets/Scripts/Timers/Timer.cs: Reviewed Timer implementation
67bcdd558c Assets/Scripts/UI/ButtonManager.cs: Reviewed ButtonManager implementation
e664116dce Assets/Scripts/UI/DropdownListMarker.cs: Counts the dropdown template/list active lifecycle without private reflection
d5cf3ee185 Assets/Scripts/UI/DropdownOpenState.cs: Public lifecycle-based state for the standard UGUI dropdown
078e9bf04e Assets/Scripts/UI/GameAudioSettings.cs: Referenced original menu music configuration type
9c3f457ae4 Assets/Scripts/UI/GamePreferences.cs: Persistent audio/text/window preferences and minimal/detailed title artwork selection
cb7d7ee7e4 Assets/Scripts/UI/GameUI.RunFlow.cs: Conditional Continue, flowing guide/credits, single back/start actions and run feedback
116f52cf53 Assets/Scripts/UI/GameUI.cs: Pixel-arrow and readable adaptive contextual UI with separated HUD regions and explicit selected-item hints
5969ef1ff7 Assets/Scripts/UI/GameUIBootstrap.cs: Installs the shared runtime interface in menu and Main scenes
addb07c9ad Assets/Scripts/UI/ItemSlotHint.cs: Inventory effect feedback for mouse hover and UI selection
3d5f2780e4 Assets/Scripts/UI/LoadingScreenManager.cs: Reviewed LoadingScreenManager implementation
773af43638 Assets/Scripts/UI/ObjectiveRoute.cs: Cached directed door graph for the first-minute classroom route
94e5226b9e Assets/Scripts/UI/ScrollSelectionIntoView.cs: Keeps controller/keyboard-focused binding rows visible
72625d50ab Assets/Scripts/UI/UIAudio.cs: Louder pause-safe cues, singleton recovery and missing-resource diagnostics
d62883745b Assets/Scripts/UI/UIControlSound.cs: Pointer/controller sound and independent bright focus border with dropdown clone recovery
ec7cd0a193 Assets/Scripts/UI/UIInputModuleBootstrap.cs: Reviewed UIInputModuleBootstrap implementation
72ceaf5e11 Assets/Scripts/UI/WindowsError.cs: Reviewed WindowsError implementation
03279d2a6a Assets/Scripts/UI/guiConsole.cs: Reviewed guiConsole implementation
b64b50a6fd Assets/Scripts/Video/PlayVideo.cs: Reviewed PlayVideo implementation
91dc87b2f7 Assets/Scripts/Video/playtexture.cs: Reviewed playtexture implementation
f2bfcaaf52 Assets/Scripts/World/DoorOpenTrigger.cs: Reviewed DoorOpenTrigger implementation
17f601f067 Assets/Scripts/World/GameSession.cs: Reviewed GameSession implementation
dbf78b9f4c Assets/Scripts/World/GameSessionControls.cs: Reviewed GameSessionControls implementation
e21b1e966e Assets/Scripts/World/GameplayFeedback.cs: Localized transient feedback state and notifications
4da9980a9a Assets/Scripts/World/PlayerInteractionTarget.cs: Shared player-only collider overlap registration for doors and pickups
a31aade312 Assets/Scripts/World/PlayerInteractor.cs: Selects and activates one nearby interaction target per E press
cd549d9c02 Assets/Scripts/World/RunSaveDriver.cs: Scene restoration and periodic/focus/pause/quit persistence driver
7ab04af2d9 Assets/Scripts/World/RunSaveService.cs: Atomic local autosave, backup, checkpoint and restore coordination
ce3f7dd624 Assets/Scripts/World/RunSnapshot.cs: Versioned run DTOs and ordered save-participant contract
00a7f4f165 Assets/Scripts/World/Teleport.cs: Reviewed Teleport implementation
0e51ebcee0 Assets/Scripts/World/TeleportController.cs: Reviewed TeleportController implementation
629c0dd138 Assets/Scripts/World/Test.cs: Reviewed Test implementation
e57fc84697 Tools/GenerateUiSounds.py: Reproducible original hover/click/pause/resume PCM WAV synthesis

## Configuration

9492a2b920 Assets/Art.meta: Unity stable importer/asset GUID metadata
47cc29395d Assets/Art/Campaign.meta: Unity stable importer/asset GUID metadata
e8078d9f04 Assets/Art/Campaign/BathroomProps.png.meta: Unity stable importer/asset GUID metadata
a460b104cd Assets/Art/Campaign/CampusSignage.png.meta: Unity stable importer/asset GUID metadata
dae30e22ad Assets/Art/Campaign/CampusToilet.png.meta: Unity stable importer/asset GUID metadata
1325b8e28c Assets/Art/Campaign/OralExamRoom.png.meta: Unity stable importer/asset GUID metadata
91f8a571c4 Assets/Art/Campaign/PracticalRoom.png.meta: Unity stable importer/asset GUID metadata
3b820c863c Assets/Art/Campaign/README.md.meta: Unity stable importer/asset GUID metadata
b5a42c8d0e Assets/Art/Campaign/SpriteLayout.json: University-prop sprite slicing reference
a5380980de Assets/Art/Campaign/SpriteLayout.json.meta: Unity stable importer/asset GUID metadata
e073b9c9ca Assets/Art/Campaign/UniversityProps.png.meta: Unity stable importer/asset GUID metadata
11a254559d Assets/Editor/CampaignRoomValidation.cs.meta: Unity stable importer/asset GUID metadata
feaeaa471d Assets/Prefabs.meta: Unity stable importer/asset GUID metadata
35b08d9e20 Assets/Prefabs/Campaign.meta: Unity stable importer/asset GUID metadata
ddd3317e53 Assets/Prefabs/Campaign/CampaignEntrance.prefab.meta: Unity stable importer/asset GUID metadata
65d95c2f21 Assets/Prefabs/Campaign/CampusToilet.prefab.meta: Unity stable importer/asset GUID metadata
7a60d4bef0 Assets/Prefabs/Campaign/OralExamRoom.prefab.meta: Unity stable importer/asset GUID metadata
68c2f28704 Assets/Prefabs/Campaign/PracticalClassroom.prefab.meta: Unity stable importer/asset GUID metadata
3f9e8ed223 Assets/Scripts/Inventory/StudentTrade.cs.meta: Unity stable importer/asset GUID metadata

61c14b1bf1 Assets/Resources/Combat/FinalExaminerAppearance.asset: Distinct green-jacket original pixel teacher appearance for stage three

d7bc06fb17 Assets/Resources/Campaign/RoomArt.asset: Original classroom/corridor sprite references for campaign room backgrounds

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
009446dfeb Assets/Localization/Translations.json: 164 seed entries in seven languages covering clear ammo use, pickup location and dodge actions
70c9daedd6 Assets/Resources/Audio/GameAudioSettings.asset: References existing Mid4(Menu).wav without duplicating music
ff451dcbc9 Assets/Resources/Combat/ExaminerAppearance.asset: Original orange/red sprite reference and measured visible bounds for final examiner
5d764e6de6 Assets/Resources/Combat/FriendlyStudentAppearance.asset: Reference and measured bounds for existing black-haired friendly-student sprite
944a451468 Assets/Resources/Controls/Gameplay.inputactions: Editable default gameplay actions, keyboard/mouse and controller bindings with stable IDs
a739a6871d Assets/Resources/Credits.json: Evidence-based contributor/package/audio credits and pending attribution records
d882882f31 Assets/Scripts/Audio/StudentSurvival.Audio.asmdef: Reviewed StudentSurvival.Audio assembly definition
0530efe158 Assets/Scripts/Cameras/StudentSurvival.Cameras.asmdef: Reviewed StudentSurvival.Cameras assembly definition
d2433d358f Assets/Scripts/Characters/StudentSurvival.Characters.asmdef: Reviewed StudentSurvival.Characters assembly definition
89a66c49fd Assets/Scripts/Controls/StudentSurvival.Controls.asmdef: Reviewed StudentSurvival.Controls assembly definition
0715c15824 Assets/Scripts/Inventory/StudentSurvival.Inventory.asmdef: Reviewed StudentSurvival.Inventory assembly definition
8107f24753 Assets/Scripts/Localization/StudentSurvival.Localization.asmdef: Assembly definition and dependencies
7a15c364e4 Assets/Scripts/Timers/StudentSurvival.Timers.asmdef: Reviewed StudentSurvival.Timers assembly definition
f653d81af3 Assets/Scripts/UI/StudentSurvival.UI.asmdef: Reviewed StudentSurvival.UI assembly definition
8bae3b9a79 Assets/Scripts/Video/StudentSurvival.Video.asmdef: Reviewed StudentSurvival.Video assembly definition
e2844dc594 Assets/Scripts/World/StudentSurvival.World.asmdef: Reviewed StudentSurvival.World assembly definition
d778fa91ef Packages/manifest.json: Reviewed project configuration
4ef83532d5 Packages/packages-lock.json: Reviewed project configuration
60b79b3f6a ProjectSettings/EditorBuildSettings.asset: Reviewed project configuration
647e82eb8a ProjectSettings/ProjectSettings.asset: Reviewed project configuration
b5f19e363b ProjectSettings/TagManager.asset: Reviewed project configuration

## Other

6e5fe218e1 Docu/Modernization/Attachments/1.8.2/teacher-hidden-music-region.png: User screenshot showing undefeated teacher hidden beside doorway and reported sound-region issue

73cea5d789 Assets/Art/Campaign/BathroomProps.png: Generated campus pixel artwork source
412f38b5c9 Assets/Art/Campaign/CampusSignage.png: Generated campus pixel artwork source
a52b092876 Assets/Art/Campaign/CampusToilet.png: Generated campus pixel artwork source
460c3f8d9d Assets/Art/Campaign/OralExamRoom.png: Generated campus pixel artwork source
511272864a Assets/Art/Campaign/PracticalRoom.png: Generated campus pixel artwork source
d9edd49090 Assets/Art/Campaign/UniversityProps.png: Generated campus pixel artwork source
47a87fb5bd Assets/Prefabs/Campaign/CampaignEntrance.prefab: Editable authored campus prefab
7b8fce0928 Assets/Prefabs/Campaign/CampusToilet.prefab: Editable authored campus prefab
5d48d2e59b Assets/Prefabs/Campaign/OralExamRoom.prefab: Editable authored campus prefab
9bc2ad1664 Assets/Prefabs/Campaign/PracticalClassroom.prefab: Editable authored campus prefab

68072d71b8 Docu/Modernization/Attachments/1.7.3/repeated-room-door-blocks.png: User screenshot showing repeated classroom artwork and placeholder portal blocks

0251bed28b Docu/Modernization/Attachments/1.7.1/victory-new-game.png: User screenshot of forced new-game victory screen and easier exam feedback

348f2df5c0 Assets/Scripts/World/CampaignState.cs.meta: Stable Unity GUID/import metadata

b122bb53be Assets/Scripts/UI/GameUI.Campaign.cs.meta: Stable Unity GUID/import metadata

1f86748766 Assets/Scripts/Inventory/PreparationDesk.cs.meta: Stable Unity GUID/import metadata

72554d2050 Assets/Scripts/Inventory/CheatingPixelArt.cs.meta: Stable Unity GUID/import metadata

0b209765b7 Assets/Scripts/Inventory/CheatingInteraction.cs.meta: Stable Unity GUID/import metadata

3a97a7caf8 Assets/Scripts/Inventory/CheatingEncounter.cs.meta: Stable Unity GUID/import metadata

c5eabc8ca5 Assets/Scripts/Inventory/CampaignSupplies.cs.meta: Stable Unity GUID/import metadata

5f4354816b Assets/Scripts/Characters/CampaignRooms.cs.meta: Stable Unity GUID/import metadata

8078d7c670 Assets/Scripts/Characters/CampaignRoomArt.cs.meta: Stable Unity GUID/import metadata

b7489a8827 Assets/Scripts/Characters/CampaignPortal.cs.meta: Stable Unity GUID/import metadata

6b0c2251f7 Assets/Scripts/Characters/CampaignExamAdmission.cs.meta: Stable Unity GUID/import metadata

2f9c66acbe Assets/Scripts/Cameras/CameraBoundsRegion.cs.meta: Stable Unity GUID/import metadata

02c1b749d1 Assets/Resources/Combat/FinalExaminerAppearance.asset.meta: Stable Unity GUID/import metadata

c03657af81 Assets/Resources/Campaign/RoomArt.asset.meta: Stable Unity GUID/import metadata

a88391400d Assets/Resources/Campaign.meta: Stable Unity GUID/import metadata

cdcaae8f28 Docu/Modernization/Attachments/1.6.6/hall-tree.png: User screenshot of directional walking snag beside the hall tree

9ad2e16f95 Docu/Modernization/Attachments/1.6.5/outside-column.png: User screenshot of leftward outdoor-column blockage after entrance transfer repair

bca3313ca7 Assets/Scripts/Audio/MusicRegionMixer.cs.meta: Stable Unity GUID for shared music mixer

e48d2a125f Docu/Modernization/Attachments/1.6.1/entrance.png: User screenshot of university entrance/column obstruction

ac9c21bb8c .gitignore: Reviewed project configuration
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
e9f36d3aea Assets/Editor.meta: Unity import GUID and asset metadata
472f8328c3 Assets/Editor/GameStartup.cs.meta: Unity GUID and importer metadata
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
514672c33c Assets/Main.unity: Reviewed serialized scene or prefab
1b7fda214a Assets/Music/Mid4(Menu).wav: Existing menu track reused by the audio settings asset
9f8603cfd2 Assets/Music/Mid4(Menu).wav.meta: Existing menu-track GUID referenced by audio settings
3ec6952896 Assets/Resources/Art.meta: Unity import metadata for Assets/Resources/Art
b0f240fcd1 Assets/Resources/Art/StudentTitle.png: Generated student-themed pixel-art title illustration without baked menu text
8525b02716 Assets/Resources/Art/StudentTitle.png.meta: Unity import metadata for Assets/Resources/Art/StudentTitle.png
929399914f Assets/Resources/Art/StudentTitleMinimal.png: Default sparse mysterious campus title image with retro pixel-art direction
ec4855e8c1 Assets/Resources/Art/StudentTitleMinimal.png.meta: Unity import metadata for Assets/Resources/Art/StudentTitleMinimal.png
d0548e35d6 Assets/Resources/Audio.meta: Unity GUID and importer metadata
da2cd5423f Assets/Resources/Audio/GameAudioSettings.asset.meta: Unity GUID and importer metadata
b3990ed2f0 Assets/Resources/Audio/UI.meta: Unity GUID and importer metadata
329b0e2378 Assets/Resources/Audio/UI/click.wav: Original generated short UI cue
4cd1b0d602 Assets/Resources/Audio/UI/click.wav.meta: Unity GUID and importer metadata
756b923daf Assets/Resources/Audio/UI/hover.wav: Original generated short UI cue
07b2cf2c00 Assets/Resources/Audio/UI/hover.wav.meta: Unity GUID and importer metadata
82fc66bc53 Assets/Resources/Audio/UI/pause.wav: Original generated short UI cue
081c6cc8bd Assets/Resources/Audio/UI/pause.wav.meta: Unity GUID and importer metadata
4276492adc Assets/Resources/Audio/UI/resume.wav: Original generated short UI cue
d4564c6bd0 Assets/Resources/Audio/UI/resume.wav.meta: Unity GUID and importer metadata
349c8e4c13 Assets/Resources/Combat.meta: Unity import metadata for Assets/Resources/Combat
cb6bbb3cc4 Assets/Resources/Combat/ExaminerAppearance.asset.meta: Unity import metadata for Assets/Resources/Combat/ExaminerAppearance.asset
d99655f56a Assets/Resources/Combat/FriendlyStudentAppearance.asset.meta: Unity import metadata for Assets/Resources/Combat/FriendlyStudentAppearance.asset
e1f88a453d Assets/Resources/Controls.meta: Unity GUID and importer metadata
24ac657222 Assets/Resources/Controls/Gameplay.inputactions.meta: Unity GUID and importer metadata
fec6c447a7 Assets/Resources/Credits.json.meta: Unity import metadata for Assets/Resources/Credits.json
176e1d739b Assets/Resources/Fonts.meta: Unity import metadata for Assets/Resources/Fonts
89674df2b0 Assets/Resources/Fonts/PressStart2P-OFL.txt: Complete copyright and SIL Open Font License notice accompanying pixel font
4b61c5a7e5 Assets/Resources/Fonts/PressStart2P-OFL.txt.meta: Unity import metadata for Assets/Resources/Fonts/PressStart2P-OFL.txt
c66f1a4652 Assets/Resources/Fonts/PressStart2P-Regular.ttf: Unmodified Press Start 2P pixel font with verified seven-language glyph coverage
f6331c737a Assets/Resources/Fonts/PressStart2P-Regular.ttf.meta: Unity import metadata for Assets/Resources/Fonts/PressStart2P-Regular.ttf
379d3a2876 Assets/Resources/ThirdPartyNotices.txt: Preserved exact installed package licenses and Localization third-party notices
7898283d2e Assets/Resources/ThirdPartyNotices.txt.meta: Unity import metadata for Assets/Resources/ThirdPartyNotices.txt
387279e814 Assets/Resources/Timers/Timer.prefab: Reviewed serialized scene or prefab
0462ff0371 Assets/Scripts.meta: Unity import GUID and asset metadata
5eee31db21 Assets/Scripts/Audio.meta: Unity import GUID and asset metadata
5ab8c4087a Assets/Scripts/Audio/AudioController.cs.meta: Unity import GUID and asset metadata
2f00faab1a Assets/Scripts/Audio/StudentSurvival.Audio.asmdef.meta: Unity import GUID and asset metadata
192abac436 Assets/Scripts/Cameras.meta: Unity import GUID and asset metadata
0f7296ec5a Assets/Scripts/Cameras/CameraController.cs.meta: Unity import GUID and asset metadata
f3c8b5ae01 Assets/Scripts/Cameras/StudentSurvival.Cameras.asmdef.meta: Unity import GUID and asset metadata
ed105e1ead Assets/Scripts/Characters.meta: Unity import GUID and asset metadata
f57c234b44 Assets/Scripts/Characters/BulletLab.cs.meta: Unity import GUID and asset metadata
308b9c8359 Assets/Scripts/Characters/BulletOcenka.cs.meta: Unity import GUID and asset metadata
d9d1ad011a Assets/Scripts/Characters/Character.Energy.cs.meta: Unity import metadata for Assets/Scripts/Characters/Character.Energy.cs
46ed2c3ace Assets/Scripts/Characters/Character.cs.meta: Unity import GUID and asset metadata
dde2c62ac1 Assets/Scripts/Characters/CombatProjectile.cs.meta: Unity import metadata for Assets/Scripts/Characters/CombatProjectile.cs
2b53805d7d Assets/Scripts/Characters/ExamProgression.cs.meta: Unity import GUID and asset metadata
57f0012d2e Assets/Scripts/Characters/ExaminerAppearance.cs.meta: Unity import metadata for Assets/Scripts/Characters/ExaminerAppearance.cs
5148c3089a Assets/Scripts/Characters/Monster.cs.meta: Unity import GUID and asset metadata
74cec51741 Assets/Scripts/Characters/Prepod.cs.meta: Unity import GUID and asset metadata
f7b621b5d3 Assets/Scripts/Characters/StudentSurvival.Characters.asmdef.meta: Unity import GUID and asset metadata
25efd2c64b Assets/Scripts/Characters/Unit.cs.meta: Unity import GUID and asset metadata
3ff34bdbff Assets/Scripts/Characters/Visuals.meta: Unity import metadata for Assets/Scripts/Characters/Visuals
d3140c895c Assets/Scripts/Characters/Visuals/CrouchVisual.cs.meta: Unity import metadata for Assets/Scripts/Characters/Visuals/CrouchVisual.cs
7a5db84a51 Assets/Scripts/Controls.meta: Unity import GUID and asset metadata
b91da3af0d Assets/Scripts/Controls/GameInput.cs.meta: Unity import GUID and asset metadata
81a2e84fe1 Assets/Scripts/Controls/StudentSurvival.Controls.asmdef.meta: Unity import GUID and asset metadata
f286d8771d Assets/Scripts/Inventory.meta: Unity import GUID and asset metadata
6851b5ab60 Assets/Scripts/Inventory/FlirtDistraction.cs.meta: Unity import metadata for Assets/Scripts/Inventory/FlirtDistraction.cs
9eb7247a4a Assets/Scripts/Inventory/InventoryController.cs.meta: Unity import GUID and asset metadata
7c6cdcf58e Assets/Scripts/Inventory/InventoryItem.cs.meta: Unity import GUID and asset metadata
4c05709225 Assets/Scripts/Inventory/Items.meta: Unity import GUID and asset metadata
61686db990 Assets/Scripts/Inventory/Items/ItemBook.cs.meta: Unity import GUID and asset metadata
ff08dfaf5d Assets/Scripts/Inventory/Items/ItemEnergyDring.cs.meta: Unity import GUID and asset metadata
a1f802d088 Assets/Scripts/Inventory/Items/ItemLab.cs.meta: Unity import GUID and asset metadata
71bd450279 Assets/Scripts/Inventory/Items/ItemShaverma.cs.meta: Unity import GUID and asset metadata
9cdc9cf970 Assets/Scripts/Inventory/LootBoxItem.cs.meta: Unity import GUID and asset metadata
aa9bf0b4e3 Assets/Scripts/Inventory/StudentSurvival.Inventory.asmdef.meta: Unity import GUID and asset metadata
980d7bd5c5 Assets/Scripts/Localization.meta: Unity import GUID and asset metadata
5d4e9920f5 Assets/Scripts/Localization/GameLocalization.cs.meta: Unity import GUID and asset metadata
d4cfd1021f Assets/Scripts/Localization/StudentSurvival.Localization.asmdef.meta: Unity import GUID and asset metadata
ff9867c06d Assets/Scripts/Timers.meta: Unity import GUID and asset metadata
9915f154f6 Assets/Scripts/Timers/AbstractTimeProvider.cs.meta: Unity import GUID and asset metadata
980355d28f Assets/Scripts/Timers/Providers.meta: Unity import GUID and asset metadata
9b0374d44f Assets/Scripts/Timers/Providers/FadeLoaderTimer.cs.meta: Unity import GUID and asset metadata
f3224592ce Assets/Scripts/Timers/StudentSurvival.Timers.asmdef.meta: Unity import GUID and asset metadata
e9537cb034 Assets/Scripts/Timers/Timer.cs.meta: Unity import GUID and asset metadata
e4da852247 Assets/Scripts/UI.meta: Unity import GUID and asset metadata
626a40993e Assets/Scripts/UI/ButtonManager.cs.meta: Unity import GUID and asset metadata
f54a6e3025 Assets/Scripts/UI/DropdownListMarker.cs.meta: Unity GUID and importer metadata
6ee1c8cce8 Assets/Scripts/UI/DropdownOpenState.cs.meta: Unity GUID and importer metadata
0e834c2575 Assets/Scripts/UI/GameAudioSettings.cs.meta: Unity GUID and importer metadata
b315416bc4 Assets/Scripts/UI/GamePreferences.cs.meta: Unity import GUID and asset metadata
fbcb5d88a0 Assets/Scripts/UI/GameUI.RunFlow.cs.meta: Unity import metadata for Assets/Scripts/UI/GameUI.RunFlow.cs
5447061bfa Assets/Scripts/UI/GameUI.cs.meta: Unity import GUID and asset metadata
f0cf04268b Assets/Scripts/UI/GameUIBootstrap.cs.meta: Unity import GUID and asset metadata
6ba74be548 Assets/Scripts/UI/ItemSlotHint.cs.meta: Unity import GUID and asset metadata
136dd4ff01 Assets/Scripts/UI/LoadingScreenManager.cs.meta: Unity import GUID and asset metadata
c611113c7d Assets/Scripts/UI/ObjectiveRoute.cs.meta: Unity import metadata for Assets/Scripts/UI/ObjectiveRoute.cs
97f2452421 Assets/Scripts/UI/ScrollSelectionIntoView.cs.meta: Unity GUID and importer metadata
1d9aca855a Assets/Scripts/UI/StudentSurvival.UI.asmdef.meta: Unity import GUID and asset metadata
9292ca88b3 Assets/Scripts/UI/UIAudio.cs.meta: Unity GUID and importer metadata
674e645dfd Assets/Scripts/UI/UIControlSound.cs.meta: Unity GUID and importer metadata
2dd2c10110 Assets/Scripts/UI/UIInputModuleBootstrap.cs.meta: Unity import GUID and asset metadata
6148d0587d Assets/Scripts/UI/WindowsError.cs.meta: Unity import GUID and asset metadata
02a5f342f3 Assets/Scripts/UI/guiConsole.cs.meta: Unity import GUID and asset metadata
d7dce30cad Assets/Scripts/Video.meta: Unity import GUID and asset metadata
8230681ba2 Assets/Scripts/Video/PlayVideo.cs.meta: Unity import GUID and asset metadata
4f614b113f Assets/Scripts/Video/StudentSurvival.Video.asmdef.meta: Unity import GUID and asset metadata
83b6a9740d Assets/Scripts/Video/playtexture.cs.meta: Unity import GUID and asset metadata
8830fc804e Assets/Scripts/World.meta: Unity import GUID and asset metadata
c9d644abfb Assets/Scripts/World/DoorOpenTrigger.cs.meta: Unity import GUID and asset metadata
1f69e6f660 Assets/Scripts/World/GameSession.cs.meta: Unity import GUID and asset metadata
5b34cda18f Assets/Scripts/World/GameSessionControls.cs.meta: Unity import GUID and asset metadata
02d3d74f6c Assets/Scripts/World/GameplayFeedback.cs.meta: Unity import GUID and asset metadata
52a9808815 Assets/Scripts/World/PlayerInteractionTarget.cs.meta: Unity import GUID and asset metadata
6c4c6a3db1 Assets/Scripts/World/PlayerInteractor.cs.meta: Unity import GUID and asset metadata
881666a711 Assets/Scripts/World/RunSaveDriver.cs.meta: Unity import metadata for Assets/Scripts/World/RunSaveDriver.cs
1e4ac0ae51 Assets/Scripts/World/RunSaveService.cs.meta: Unity import metadata for Assets/Scripts/World/RunSaveService.cs
2d6dcd2179 Assets/Scripts/World/RunSnapshot.cs.meta: Unity import metadata for Assets/Scripts/World/RunSnapshot.cs
1eeb22e66d Assets/Scripts/World/StudentSurvival.World.asmdef.meta: Unity import GUID and asset metadata
908b764f34 Assets/Scripts/World/Teleport.cs.meta: Unity import GUID and asset metadata
4a9b33c342 Assets/Scripts/World/TeleportController.cs.meta: Unity import GUID and asset metadata
4a6eb827dc Assets/Scripts/World/Test.cs.meta: Unity import GUID and asset metadata
3a40370917 Assets/menu.unity: Reviewed serialized scene or prefab
562d43c9c6 Assets/video/usb_soft.mp4: Intro encoded as H.264 Baseline with regular timestamps and BT.709
4d46d7de0f Assets/video/usb_soft.mp4.meta: Preserved Unity video import GUID/settings
329b7fcd1d Docu/Modernization/Attachments/1.6.0/controls.png: Preserved user screenshot for1.6.0 controls
4ec1b0b75e Docu/Modernization/Attachments/1.6.0/crouch.png: Preserved user screenshot for1.6.0 crouch
77ff783926 Docu/Modernization/Attachments/1.6.0/help.png: Preserved user screenshot for1.6.0 help
2aafe3fc00 Docu/Modernization/Attachments/1.6.0/hud.png: Preserved user screenshot for1.6.0 hud
0fa22ff8a8 Docu/Modernization/Attachments/1.6.0/jump.png: Preserved user screenshot for1.6.0 jump
12cf4e7c80 Docu/Modernization/Feedback_2026-11-21/LocalizationNote.png: User-supplied feedback screenshot
e88a8fd1b3 Docu/Modernization/Feedback_2026-11-21/Pause.png: User-supplied feedback screenshot
cb928b82cb Docu/Modernization/Feedback_2026-11-21/Settings.png: User-supplied feedback screenshot
fedf3dd581 ProjectSettings/ProjectVersion.txt: Reviewed project configuration
