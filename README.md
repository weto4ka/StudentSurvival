# Student Survival

A pixel-art university survival game made by university friends during a game jam. Collect supplies, survive your exams, and earn your degree before Sleep or Intelligence runs out.

Documentation updated **2026-11-22** for implementation **1.6.6**. Unity Editor: **6000.3.27f1 (Unity 6.3 LTS)**. The modernization is implemented, with gameplay, controller and build verification still pending; this is not a verified release announcement.

## Creators

The original university game-jam team, using the names supplied by the team:

- [Максим Панасенко](https://github.com/maximys963) — `maximys963`
- [Іван Тюпменко](https://github.com/interess) — `interess`
- [Кирил Кулаков](https://github.com/qluckoff) — `qluckoff` (formerly `Morgan23102`)
- [Єлизавета Мельник](https://github.com/weto4ka) — `weto4ka` (original Git author name: `Zaza`)
- [Maxim Rebitskiy](https://github.com/maksa988) — `maksa988`, original repository owner

Individual development/art roles and the jam name/date have not yet been confirmed. The in-game Credits page uses the same roster and GitHub URLs from [Credits.json](Assets/Resources/Credits.json). Select any of the five names to open their GitHub profile; the names support mouse, keyboard and controller navigation. Historical Git author names are retained there separately; they are not extra teammates. GitHub links above were checked against linked repository commits and team clarification. The team confirmed Максим Панасенко’s profile as maximys963; his historical commits display an unlinked Maxim/Maxym author.

Original repository: [maksa988/StudentSurvival](https://github.com/maksa988/StudentSurvival). This project is the [weto4ka/StudentSurvival fork](https://github.com/weto4ka/StudentSurvival), with the 2026 modernization work recorded below. Max's public name is taken from his [LinkedIn profile](https://ua.linkedin.com/in/maksa988), which links the same GitHub account.

## Project history

The original repository initialization commit (`2be5c17`) is dated **2017-12-01**. This establishes recorded development history; it does not establish the game jam's exact name or dates. The **2026-11** modernization updates Unity, input, localization, UI, movement/combat feedback, saves, energy-drink effects and the optional flirting encounter. See [Updates.md](Updates.md) for the dated changes.

## How to play

Pass two exams, including a distinct final examiner. Defeating the final examiner ends the run with victory; there is no next-level scene yet. Follow the early objective guidance to the classroom. Doors and the university entrance use **Interact**; approach until the destination prompt appears, press the displayed control once, and stay nearby while the transfer completes.

- **Supplies:** collect items with Interact. Food restores 1 Sleep; books restore 1 Intelligence. Pickups automatically select their inventory slot.
- **Lab reports:** collected labs are ammunition that costs no Sleep to throw. Select their slot and use the selected-item action. Creating a fresh report with the fire action costs 1 Sleep per throw.
- **Combat:** watch the examiner's warning. Jump over low throws and crouch under high throws. The first exam rewards 3 Sleep and 1 Intelligence before the final encounter.
- **Energy drinks:** borrow 3 Sleep and boost movement for 20 gameplay seconds. Any unused borrowed Sleep disappears afterward, followed by a 10-second slowdown. Drinks do not stack during an active boost. Outside an active boost, zero Sleep gives you an 8-second rescue window to use food or a drink before losing; zero Intelligence ends the run immediately.
- **Optional flirting:** look for the dark-haired student and pink pixel heart just right of spawn, before the starter pickups. The nearby hint shows your Interact key and the cost: **one collected lab**. Interacting turns it into a love letter once per run. Walking past costs nothing.

World pickups are finite for a run: labs, energy drinks, food and books do not respawn on a timer. Continue keeps collected pickups removed. Find an uncollected lab pickup, or use the fresh-report fire action for one Sleep per throw; this creates a projectile rather than refilling your inventory.

The menu has separate How to play and Controls pages. Settings include language, master/interface volume, text size, title artwork and supported window-mode options. Pause and settings use translucent overlays; music crossfades between areas and resumes its playback position when returning.

## Default controls

Bindings can be changed in **Controls**, with separate keyboard/mouse and controller tabs. Xbox-style labels are shown below; the game displays the active binding.

| Action | Keyboard / mouse | Controller |
| --- | --- | --- |
| Move | A / D or Left / Right arrows | Left stick / D-pad |
| Jump | Space | South / A |
| Crouch | Hold S / Down arrow | Hold down on stick / D-pad |
| Interact: collect, doors, flirt | E | West / X |
| Throw a fresh lab: costs 1 Sleep | Left Ctrl / left mouse button | Right trigger |
| Use selected inventory item | F | North / Y |
| Previous / next inventory slot | [ / ] | Left / right shoulder |
| Use a numbered inventory slot | 1–9 | Select a slot, then use selected item |
| Toggle inventory | Tab | Select / View |
| Pause | Esc | Start / Menu |

Hold fire or the selected-lab action to repeat throws every 0.18 seconds. Other selected consumables use once per press. Click a toolbar slot to use it; hover to inspect its effect. The inventory fades when idle and can be kept visible with its toggle.

On the result screen, R retries the checkpoint after a loss or starts a fresh run after victory; Esc returns to the menu. The on-screen buttons also support navigation.

## Saves and restarting

Autosave preserves inventory, collected pickups, player stats/position, exam progress, energy-effect timers and the completed flirt choice. Returning to the menu preserves the active run; **Continue** appears only when there is a resumable run.

Checkpoints are established at the initial spawn, safe door arrivals and the first-exam reward. **Death rolls progress back to the latest checkpoint**, including its inventory; Continue after death uses that checkpoint. **New run** replaces the saved run after confirmation. Victory completes the run and removes Continue. Language, settings and binding preferences remain separate from run progress.

The desktop run file is `student-survival-run.json` under Unity's `Application.persistentDataPath`, with a backup beside it. See [save rules and implementation](Docu/Modernization/Save_System.md) for recovery and verification details. Menu/closed-application time does not consume saved effect timers.

## Languages

English, Ukrainian, Russian, French, Italian, German and Spanish are available through the Settings dropdown using Unity Localization. Text changes immediately; translation review by fluent speakers remains pending. Original lettering baked into artwork or video is not replaced by text localization.

See [localization authoring and setup](Docu/Localization/Localization.md). The source catalog is [Translations.json](Assets/Localization/Translations.json).

## Open and build the project

1. Add this repository folder as a project in Unity Hub and open it with **6000.3.27f1**. The required packages are recorded in [manifest.json](Packages/manifest.json) and [packages-lock.json](Packages/packages-lock.json); allow Unity to restore/import them and finish compiling.
2. Open [Assets/menu.unity](Assets/menu.unity) and enter Play Mode. The editor startup helper uses the menu by default; the **Tools > Student Survival** menu can select the open scene for targeted testing.
3. Keep **Active Input Handling** set to **Input System Package (New)**. The project already stores this setting. Default actions live in [Gameplay.inputactions](Assets/Resources/Controls/Gameplay.inputactions).
4. Localization setup runs in edit mode. If needed, stop Play Mode and use **Tools > Student Survival > Set up localization**. It adds missing entries without replacing translator-edited values.
5. For a player build, retain scene order: `Assets/menu.unity` first, then `Assets/Main.unity`. Include the target's Localization/Addressables content and verify the resulting build before sharing it. For Windows, run the executable from its generated build folder with its companion files intact.

**Platform status:** desktop is the development baseline. Web/itch.io browser play has been assessed but has not been ported, built or verified. Browser save persistence, video sources and fullscreen/navigation behavior need platform work. See [Web/itch.io readiness](Docu/Modernization/Web_Itch_Readiness_2026-11-21.md).

## Credits and asset attribution

- **Original team:** listed above and in the in-game Credits page.
- **Legacy in-game console:** its source header credits **Orng.Car**; see [guiConsole.cs](Assets/Scripts/UI/guiConsole.cs).
- **Pixel font:** **Press Start 2P Project Authors**, under the [SIL Open Font License 1.1](Assets/Resources/Fonts/PressStart2P-OFL.txt).
- **Packages:** Unity Technologies and the third-party contributors named in [ThirdPartyNotices.txt](Assets/Resources/ThirdPartyNotices.txt), including SmartFormat's authors.
- **Added interface sounds:** reproducible synthesized cues from [GenerateUiSounds.py](Tools/GenerateUiSounds.py). The original soundtrack remains in use.
- **Title artwork and animation updates:** title-art generation/provenance is recorded in [the 1.4.0 notes](Docu/Modernization/Energy_Distractions_1.4.0.md); crouch/jump reuse the original pixel character, as documented in [Jump_Animation.md](Docu/Modernization/Jump_Animation.md).

A project-wide license and complete creator/source attribution for the original art, music and videos have not yet been documented. See the [credits and attribution audit](Docu/Modernization/Credits_Attribution.md) for the known sources and remaining team information.

## Project notes and verification

- [Start here: architecture and agreed workflow](Docu/Start.md)
- [Version history](Updates.md)
- [Exact gameplay/controller/build checks](Docu/Modernization/User_Test_Checklist.md)
- [Combat](Docu/Modernization/Combat.md), [flirting](Docu/Modernization/Distractions.md), [saves](Docu/Modernization/Save_System.md)
- [Entrance transfer repair](Docu/Modernization/Entrance_1.6.1.md), [outdoor walking seam](Docu/Modernization/Entrance_1.6.5.md), [ceilings and music](Docu/Modernization/Ceiling_Music_1.6.2.md), [hall-floor seam](Docu/Modernization/Hall_Floor_1.6.6.md)

Source compilation, configuration checks and offline pose/icon inspection have been performed for the documented changes. A full Unity Play Mode run, controller pass and player-build verification remain pending; completed source checks do not establish those runtime results.
