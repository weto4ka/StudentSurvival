# Student Survival

A pixel-art university survival game made by university friends during a game jam. Collect supplies, make preparation choices, and survive three connected exams to finish your first semester before Sleep or Intelligence runs out.

Documentation updated **2026-11-22** for implementation **1.8.5**. Unity Editor: **6000.3.27f1 (Unity 6.3 LTS)**. The modernization is implemented, with gameplay, controller and build verification still pending; this is not a verified release announcement.

WC entrance: [1.8.1 visual adjustment](Docu/Modernization/WC_Entrance_1.8.1.md) restores the earlier open WC recess while retaining the separate toilet room.

Editable rooms: [1.8.0 campus guide](Docu/Modernization/Authored_Campus_1.8.0.md) covers the wider practical/oral classroom prefabs, separate WC, unique pixel artwork/props, student exchanges and finite food. Open Assets/Main.unity outside Play Mode or double-click the prefabs in Assets/Prefabs/Campaign. [Exact user checks](Docu/Modernization/Authored_Campus_1.8.0.md#exact-user-checks) and [known issues](Docu/Modernization/Known_Issues_1.8.0.md) distinguish source verification from pending Unity gameplay tests.

Map safety: [1.7.2 boundary repair](Docu/Modernization/Map_Boundaries_1.7.2.md) closes the older building’s west ends and upper east end and returns deep falls to the last grounded position while retaining run progress.

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

The campaign has **three connected exam stages**. Collect supplies, prepare, then press your Interact control near a teacher to start that exam. Passing unlocks the door to the next room; fights do not start automatically.

| Stage | Objective and choices | Examiner |
| --- | --- | --- |
| 1 | Explore the original campus, collect supplies and take the first practical | Professor Byte: 5 hits, two low throws per volley |
| 2 | Revise honestly at the desk, or find hidden **шпоры** in the separate WC room and evade the patrol guard | Dr Redline: 6 hits; using carried notes during the active exam reduces the total to 4 |
| 3 | Spend two free periods on study or flirting, then take the last exam | Professor Deadline: 5/6/8 hits at 2/1/0 readiness, with longer alternating volleys |

- **Supplies:** collect items with E/Interact. Food restores 1 Sleep; books restore 1 Intelligence. Pickups automatically select their inventory slot. Each new preparation room has a finite bundle of 3 lab reports and 2 food items; stage 2 also has one energy drink.
- **Lab reports:** selected collected labs throw with F/Use item at no Sleep cost. Ctrl/left mouse/right trigger creates a fresh report projectile for 1 Sleep; it does not refill inventory. Pickups do not respawn on a timer or after Continue.
- **Exam warnings:** jump over low throws and crouch under high throws. Teachers briefly recover between accepted hits: watch the flicker and time throws rather than wasting reports during protection. Passing stages 1 and 2 grants 5 and 6 Sleep respectively, plus 1 Intelligence each, and creates a checkpoint.
- **Guard risk:** carrying cheat notes in the guard's sight builds suspicion. Crouch, get out of sight or hide in the separate WC room. At full suspicion the notes are confiscated, the exam is cancelled and you return to preparation. Notes can be found again, or you can use the free honest-study alternative. During an active stage-2 exam, E near the teacher uses carried notes.
- **Study versus flirting:** in stage 3, studying spends one free period and adds one readiness. Flirting spends one period and one collected report. Study twice for the easiest exam; or flirt and study once, then return to the friend for 2 free reports before starting. Flirting changes preparation rather than automatically failing the exam. Follow the pink heart and the displayed cost hint.
- **Energy drinks:** borrow 3 Sleep and boost movement for 20 gameplay seconds. Any unused borrowed Sleep disappears afterward, followed by a 10-second slowdown. Drinks do not stack during an active boost. Outside an active boost, zero Sleep gives you an 8-second rescue window to use food or a drink before losing; zero Intelligence ends the run immediately.

The two students in the older upper-left corridor now offer exchanges: one collected lab buys an energy drink once per run, or an exam tip that can then be repeated free. Offers and successful receipts are saved. Shawarma spots are finite: eating does not refill the same spot; collect another available food object instead.

Beat the third examiner to receive the **Semester passed** celebration, then **Keep exploring** the same saved campus. New run is a separate confirmed choice; Continue remains available after completion. Further semesters and a 20-stage campaign are future expansion, not implemented levels. [Authored rooms, exchanges and exact checks](Docu/Modernization/Authored_Campus_1.8.0.md).

The menu has separate How to play and Controls pages. Settings include language, master/interface volume, text size, title artwork and supported window-mode options. Pause/settings are translucent overlays; room music uses the shared crossfade system.

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

On the result screen, R retries the checkpoint after a loss. After victory, R/Esc and the back arrow resume exploration; New run is a separate confirmed button. Esc on a loss returns to the menu.

## Saves and restarting

Autosave preserves inventory, collected pickups, player stats/position, active exam health, carried/used notes, honest revision, free periods/readiness, the relationship/help claim and energy-effect timers. Returning to the menu preserves the active run; **Continue** appears only when there is a resumable run.

Checkpoints are established at the initial spawn, safe door arrivals, passed-exam rewards and recoverable positive-Sleep guard confiscations. **Death rolls progress back to the latest checkpoint**, including its inventory; Continue after death uses that checkpoint. **New run** replaces the saved run after confirmation. Victory records a completed campus and preserves Continue; passed teachers stay absent. Completed free-roam cannot lose from exhaustion or damage. Language, settings and binding preferences remain separate from run progress.

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
- [Post-exam exploration and combat pacing](Docu/Modernization/Completion_Difficulty_1.7.1.md)
- [Exact gameplay/controller/build checks](Docu/Modernization/User_Test_Checklist.md)
- [Combat](Docu/Modernization/Combat.md), [flirting](Docu/Modernization/Distractions.md), [saves](Docu/Modernization/Save_System.md)
- [Entrance transfer repair](Docu/Modernization/Entrance_1.6.1.md), [outdoor walking seam](Docu/Modernization/Entrance_1.6.5.md), [ceilings and music](Docu/Modernization/Ceiling_Music_1.6.2.md), [hall-floor seam](Docu/Modernization/Hall_Floor_1.6.6.md)

Source compilation, configuration checks and offline pose/icon inspection have been performed for the documented changes. A full Unity Play Mode run, controller pass and player-build verification remain pending; completed source checks do not establish those runtime results.
