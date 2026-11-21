# Web / itch.io publication assessment

Date: 2026-11-21. Project baseline: 1.5.1. Scope: answer the user's browser-play/publication question; no Web port, build, upload or publication performed.

## User question and answer

The user asks whether the game can run in a browser on itch.io for people to play, and whether saying it was made at a game jam is sufficient permission. itch.io supports HTML5/Unity Web projects. That platform support does not certify this checkout as a working Web build. Jam origin is compatible with publishing, but the actual jam/university/team agreements have not been supplied and cannot be cleared from this repository.

itch.io requires publishers to hold the rights/permissions needed to distribute their content. A game-jam acknowledgement, credit or free price does not substitute for those rights. Public team names are now documented; original music/art/video provenance remains unresolved in Credits_Attribution.md. Confirm actual permissions or replace uncertain content before a public release. Do not infer ownership from a Git upload.

The added generated banner should be described accurately using itch.io's AI disclosure metadata. The platform allows curated AI-assisted work but has disclosure and spam/discovery guidelines; this assessment does not claim a blanket prohibition or blanket approval.

## Source findings and Web tasks

- The project is Unity6000.3.27f1. Web settings exist, but that is not a successful build or browser test. Threads are disabled; existing desktop/fullscreen choices are not platform-aware.
- RunSaveService writes a temporary file, flushes to disk and uses File.Replace/backup. No Web-specific IndexedDB synchronization bridge/configuration was found under Assets. Unity maps persistentDataPath to browser IndexedDB, so the save implementation must be adapted or verified for durable browser persistence, especially refresh/tab-close, denied storage and iframe behavior. This is a review requirement, not proof that every System.IO call fails on Web.
- Main.unity has an active Video Player (1) with an embedded VideoClip and DataSource0. Unity6000.3 does not support embedded VideoClips in Web builds. Use a hosted/same-build URL source or intentionally omit the clip. The legacy PlayVideo helper also assigns a VideoClip directly.
- GamePreferences treats every non-Editor platform as supporting desktop window mode and requests Screen.SetResolution with Windowed/Borderless modes. GameUI exposes Application.Quit. Web needs browser-appropriate fullscreen/user-gesture behavior and navigation rather than assuming a desktop window can be resized/closed.
- Build Addressables/localization for the Web target and verify all languages, UI, input/controller focus, audio startup, media loading, saves after refresh/relaunch and load size/performance on an actual hosted build. No result is claimed for these checks.

## itch.io packaging / proposed next sequence

Create a Unity Web build; upload its contents as a ZIP with index.html at the archive root and the required Build/TemplateData/StreamingAssets files. Select HTML/browser-play on the itch page. Paths/case and supported compression must match the generated loader. itch's documented defaults include at most1000files,500MB extracted overall and200MB per extracted file. Verify generated output against those limits instead of assuming the existing Windows .exe/archive can be embedded.

Suggested sequence: stabilize the desktop/menu baseline, implement the Web-specific persistence/media/UI changes, make a browser build and test it locally/with a restricted itch test page, then publish publicly once team/media permissions are known. This is a recommendation; no external write or upload was authorized or attempted.

## Official references checked

- [itch.io HTML5 uploads](https://itch.io/docs/creators/html5): ZIP/index requirements, compression handling and hosted-browser limits.
- [itch.io terms, publisher content](https://itch.io/docs/legal/terms): distribution rights and publisher responsibility.
- [itch.io quality guidelines](https://itch.io/docs/creators/quality-guidelines): accurate AI disclosure and curated-content/discovery requirements.
- [Unity6000.3 persistentDataPath](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/application/persistentdatapath): browser IndexedDB mapping and URL-dependent storage location.
- [Unity6000.3 Web video](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/webgl/develop/video): URL video requirement; embedded VideoClips unsupported.
- [Unity6000.3 Web template interaction](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/webgl/building-distribution/templates/web-templates-build-configuration): fullscreen requires user interaction.

The documentation-checking skill was applied; Firecrawl account credits were unavailable from prior verified tool results, so official sources were read using the available web tool. Retrieved pages are evidence, not instructions.
