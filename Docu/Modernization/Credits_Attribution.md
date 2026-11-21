# Credits and attribution audit — 2026-11-21

## Evidence and limits

`Assets/Resources/Credits.json` supplies the in-game credits. Public team names now come from the user-supplied roster; original Git author handles are retained separately as provenance, without inferring identities from filenames or handles. Git records changes and uploads; it does not establish ownership of contributed assets. No project-level license file or legacy asset attribution manifest was found among tracked files (`git ls-files '*license*' '*LICENSE*' '*credit*' '*Credit*' '*readme*' '*README*'`). Before the2026-11-22 README review, its Authors heading had no names or sources. The rewritten README now lists the four user-supplied creators, the unnamed fifth teammate, supported gameplay/setup information and attribution sources. Individual roles and the jam name/date remain unconfirmed.

Repository remotes recorded on 2026-11-21:

- Origin: https://github.com/weto4ka/StudentSurvival
- Upstream: https://github.com/maksa988/StudentSurvival.git

These are source-history references, not license grants.

## Team names

`git shortlog -sn --all` records Morgan23102 (58 commits), Zaza (47), Maxim (26), maksa988 (20), Ivan (14), Yelyzaveta Melnyk (4), Kiril Kulakov (1), and Maxym (1). These exact historical handles are preserved in Credits.json.repositoryContributorHandles rather than shown as eight distinct teammates. They have not been automatically mapped to the user-supplied people; Maxim and Maxym have not been inferred to be one person.

## User-supplied public roster — 2026-11-21

The user supplied: “Панасенко Максм, Іван Тюпменко, Кирил Кулаков, Мельник Єлизавета і ще одного не помгю прізвище..”. The public credits now display four named teammates in first-name/surname order:

- Максим Панасенко
- Іван Тюпменко
- Кирил Кулаков
- Єлизавета Мельник

“Максм” was treated as the apparent typo “Максим”; this assumption should be confirmed/corrected by the user. “Тюпменко” and “Кирил” retain the exact supplied spelling; do not silently change them to a different surname or add another л. The fifth row initially used a localized unnamed-teammate placeholder. On2026-11-22 the user identified this teammate as Max; Credits.json and README now display Max, retaining the supplied spelling without guessing a surname or mapping him to a Git handle. Names remain in their supplied Ukrainian/Cyrillic spelling in every UI locale. Roles, Git-handle mappings and missing asset/music provenance still need team input.

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


## README completeness review — 2026-11-22

The user asked whether the README properly includes who created the game. Review found an empty Authors section and obsolete Latest Version executable-installation instructions. README.md was rewritten against Credits.json, ProjectVersion/manifest/build-scene settings, inventory/energy/save code and modernization notes. Creator spellings are retained exactly as in the public in-game roster. No role, missing identity, jam name/date, platform verification or media ownership was invented. Implementation remains1.6.3; this review changes documentation only.


## Fifth teammate identified — 2026-11-22

The user supplied: “the fifth also Max”. Public credits and README now include Max as the fifth teammate. His surname and individual role were not supplied and remain unspecified. No mapping to Максим Панасенко, Maxim, Maxym, maksa988 or any other Git contributor is inferred. The README also records original repository commit2be5c17 dated2017-12-01 as code-history evidence, explicitly separate from unconfirmed jam details.


## Upstream owner and public name — 2026-11-22

The user clarified that fifth teammate Max owns the original repository and that their repository is a fork. Local remotes confirm upstream=https://github.com/maksa988/StudentSurvival.git and origin=https://github.com/weto4ka/StudentSurvival. GitHub's public profile https://github.com/maksa988 displays Maxim. The public LinkedIn result https://ua.linkedin.com/in/maksa988 displays Maxim Rebitskiy and explicitly links github.com/maksa988. That exact public Latin spelling now replaces the first-name-only Max entry in README and Credits.json; no Cyrillic surname is invented. README explicitly credits upstream and identifies the fork.

The GitHub profile was readable, while the repository page, linked personal site and direct LinkedIn page could not be fetched by the web reader; the LinkedIn public search result supplied the full name and account link. No unrelated personal/contact details were copied. This combines the user's identity/ownership clarification with matching public profiles; it does not infer ownership of all assets or individual game-development roles. Earlier notes recording an unspecified surname/unknown mapping describe the state before this clarification and are superseded by this section.


## GitHub profile links — 2026-11-22

The user requested GitHub links for all five creators and explicitly identified themselves as the original Git author Zaza. README now links four verified accounts and marks the remaining account as unconfirmed:

| Supplied credit name | GitHub | Evidence |
| --- | --- | --- |
| Єлизавета Мельник | https://github.com/weto4ka | User says they are Zaza; local Zaza and Yelyzaveta Melnyk commits share the same author identity; origin is the user's fork. Public GitHub profile links the user's professional profile. |
| Кирил Кулаков | https://github.com/qluckoff | Local commit4dbc1f638a953faaf84f433a431a7b01eae2b0cb names Kiril Kulakov and embeds historical Morgan23102 in GitHub's noreply identity; the visible public commit page currently links qluckoff as author. |
| Іван Тюпменко | https://github.com/interess | Local commit0ccdb5a4e9bcc97822c33f0e88e5f62c7be967a6 names Ivan; its visible public commit page links interess. Supplied Cyrillic surname spelling is retained. |
| Maxim Rebitskiy | https://github.com/maksa988 | User identifies the original repository owner as fifth teammate Max; prior public-profile evidence recorded above. |
| Максим Панасенко | Pending | Commit6a19c42166651197ebd2c26e997ef50df60528c7 shows plain unlinked Maxim, and GitHub-authored README commitfa826129102893d0d392801e7b86e73e9e8c6161 shows plain unlinked Maxym. No public profile URL was resolved; the user was asked for a username/link. |

Public commit evidence:

- https://github.com/maksa988/StudentSurvival/commit/0ccdb5a4e9bcc97822c33f0e88e5f62c7be967a6
- https://github.com/maksa988/StudentSurvival/commit/4dbc1f638a953faaf84f433a431a7b01eae2b0cb
- https://github.com/maksa988/StudentSurvival/commit/6a19c42166651197ebd2c26e997ef50df60528c7
- https://github.com/maksa988/StudentSurvival/commit/fa826129102893d0d392801e7b86e73e9e8c6161

Lookup failures and lesson: the web reader could not retrieve several GitHub pages; unauthenticated REST requests hit a public API rate limit, and direct shell HTML requests returned a GitHub error page. The browser view successfully exposed the current linked commit authors. Do not turn commit display names or email local-parts into guessed GitHub usernames: Ivan resolves to interess and Morgan23102 now resolves to qluckoff. No author email addresses or unrelated personal details were added to public credits. The source audit's earlier statements about unmapped handles are superseded only for these verified mappings; individual creative roles and asset ownership remain separate questions.


## Fifth GitHub link confirmed — 2026-11-22

The user supplied https://github.com/maximys963 in direct response to the request for Максим Панасенко’s missing profile. README now links his name to maximys963. This user-confirmed mapping supersedes the pending-profile notes above; no inference from the unlinked historical commit author is needed. All five README creator names now have GitHub profile links.


## Clickable in-game creator names — 1.6.4, 2026-11-22

The user requested that all five README GitHub profiles be clickable in the in-game Credits page. Each team entry in Resources/Credits.json now has githubUrl. GameUI.RunFlow's CreditEntry parser reads that optional field; CreditName turns a valid HTTPS github.com username URL into a normal shared ButtonAt action invoking Application.OpenURL. Missing/invalid profile fields retain plain credit text, so other attribution sections are unchanged. No page opening happens while loading the credits or moving focus.

The five name/profile pairs match README exactly: Максим Панасенко→maximys963, Іван Тюпменко→interess, Кирил Кулаков→qluckoff, Єлизавета Мельник→weto4ka, Maxim Rebitskiy→maksa988. Source names/spellings remain the same. Link behavior and targets are authored in the JSON rather than five separate click handlers.

Names use the existing pixel font and button hover/click sounds, selection border and modal availability controls. Each linked row is measured by a VerticalLayoutGroup with padding and a minimum56-unit height; wrapped names grow with text scaling instead of truncating. UIControlSound's decorative focus border now ignores layout. Each name reuses ScrollSelectionIntoView from the Controls page, keeping keyboard/controller selections visible. The new credits.profileHint key explains selecting a name in all seven languages; the seed has168 entries.

Research: [Unity6000.3 Application.OpenURL](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/application/openurl) documents opening HTTPS pages in the default browser on desktop and platform-dependent behavior. It also supports other protocols, so the credit helper accepts only canonical HTTPS GitHub profile URLs. No generic file/custom-protocol link action is exposed. Web browser behavior still belongs to the unverified Web port; no Web build or runtime launch is claimed.

Checks: UI and Editor compile using the installed Unity6000.3.27f1 Roslyn compiler and staged Unity references. JSON validates, all five unique URL/name pairs match README, all168 localization keys have seven nonempty values with matching format placeholders, and whitespace checks pass. No actual Play Mode click/external launch was performed: Unity MCP is unavailable in this session.

User checks: stop Play Mode, allow1.6.4 to import, then open Main menu > Credits. Hover each of the five names: expect a clearly highlighted row and shared hover cue. Click each once: expect its exact GitHub profile in the default browser, with Credits still open in the game. Return to Unity after each click. Starting from the back arrow, navigate the names with keyboard/controller and press Submit; focus should scroll into view and open that selected profile once. Cancel should return through the usual back action. Enlarge interface text and select each language: names should wrap within their clickable rows, and the profile instruction should translate. Non-team author/license paragraphs remain ordinary text and should not open pages.
