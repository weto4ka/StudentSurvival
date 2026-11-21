# Credits and attribution audit — 2026-11-21

## Evidence and limits

`Assets/Resources/Credits.json` supplies the in-game credits. Names are taken from the repository, not inferred from filenames or account handles. Git records changes and uploads; it does not establish ownership of contributed assets. No project-level license file or legacy asset attribution manifest was found among tracked files (`git ls-files '*license*' '*LICENSE*' '*credit*' '*Credit*' '*readme*' '*README*'`). `README.md` has an Authors heading but does not list names, roles, or asset sources.

Repository remotes recorded on 2026-11-21:

- Origin: https://github.com/weto4ka/StudentSurvival
- Upstream: https://github.com/maksa988/StudentSurvival.git

These are source-history references, not license grants.

## Team names

`git shortlog -sn --all` records Morgan23102 (58 commits), Zaza (47), Maxim (26), maksa988 (20), Ivan (14), Yelyzaveta Melnyk (4), Kiril Kulakov (1), and Maxym (1). Credits preserve these exact names. Maxim and Maxym have not been merged into one identity. Preferred display names, roles, and whether multiple handles belong to the same person still require team confirmation.

## Legacy art, video, and audio: unresolved source attribution

Paths requiring a team-provided source/creator/license record:

| Category | Paths | Evidence already available | Still missing |
| --- | --- | --- | --- |
| Student and other artwork | `Assets/Graphic`, `Assets/Sprites` | Git commit `6a19c42` (2017-12-01, Maxim, “main character”), `e34adf3` and `dde728d` (2017-12-02, Maxim, frames and PNG/PSD), plus other contributors' changes | Actual creators and permissions; identify original team art versus imported assets |
| Soundtrack and effects | `Assets/Music` | `0ccdb5a` (2017-12-01, Ivan, “add music”), `e70740b` and `ac4c74d` (2017-12-02, Ivan, soundpacks), later audio commits | Track/effect source URLs, composers, licenses, required attribution text |
| Video | `Assets/video` | Tracked game files, including `usb_soft.mp4` | Creator, source, and permission/license |
| In-game console | `Assets/Scripts/UI/guiConsole.cs` | Header says “Created by Orng.Car” and describes the in-game console | License/permission for this legacy code; author credit alone does not resolve it |

Do not attribute music authorship to the person who committed it. Track names, project inclusion, and public repository access do not supply an asset license. The UI explicitly identifies legacy attribution as pending instead of inventing credits.

## Documented 2026-11-21 additions

- `Tools/GenerateUiSounds.py` documents and reproduces the short synthesized hover, click, pause, and resume cues under `Assets/Resources/Audio/UI`. This credit applies to those cues only, not to the legacy soundtrack.
- `Assets/Scripts/Characters/Visuals/CrouchVisual.cs` articulates cropped sections of the existing idle PSD artwork. It does not introduce externally sourced character art. The underlying character-art attribution remains subject to the legacy audit above.

## Package notices

Installed package files were read directly on 2026-11-21. `Packages/manifest.json` records Localization 1.5.14, Input System 1.20.0, and uGUI 2.0.0; Addressables is recorded in `Packages/packages-lock.json` as a dependency.

The installed `LICENSE.md` files for Localization, Input System, uGUI, and Addressables identify Unity Technologies and the Unity Companion License. Localization's `Third Party Notices.md` includes SmartFormat (MIT; copyright 2011–2019 axuno gGmbH, Scott Rippey, Bernhard Millauer and other contributors), Google.Apis.Sheets.v4, CsvHelper, and further package notices. Package notices can include editor-only tooling; inclusion in that document does not establish that every component executes in a player build.

Exact installed notice texts are preserved in `Assets/Resources/ThirdPartyNotices.txt`, including Localization's complete third-party notice file. The game can load this text with `Resources.Load<TextAsset>("ThirdPartyNotices")`. Preserve and update notices when dependencies change. This document is a factual source audit, not a project-wide license declaration.

## User verification after implementation

No gameplay/controller/build verification was performed by this agent, as requested. During the user's next test, hold Down/S (or the rebound crouch control): the student should bend both knees, lean forward, retain normal head/body proportions, and animate a low shuffle when moving. Release in open space: the original standing animation should return. Release beneath a low ceiling: the crouched pose should remain until standing is possible. Pausing must freeze the shuffle. Confirm both facing directions and damage flashes remain visible.

For credits, verify the menu opens the credits view and the names fit in each of the seven languages. A team member should supply missing creator/source/permission details before this audit can be marked complete.
