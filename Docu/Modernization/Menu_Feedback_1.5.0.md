# Menu feedback and repair — 1.5.0

Date: 2026-11-21. Status: implemented and source-verified; runtime appearance/audio approval pending.

## User feedback captured

The user reports obsolete menu camera component warnings, unclear hover/action affordances, inaudible hover feedback, a Continue button visible with no saved run, poor credits/help formatting, the loss of a pixel-menu aesthetic, How to play containing another How to play entry that actually opens controls, and duplicate Back buttons beside back arrows. They asked whether this had been checked in-game. The previous user instruction assigned gameplay testing to them; previous compiler/source checks were not an actual UI pass. Do not represent them as visual/audio runtime verification.

## Decisions and implementation

- Continue is hidden when RunSaveService.HasContinue is false; it is not left as a disabled invitation. The remaining menu actions reflow upward. Valid autosaves remain resumable; save files are not deleted or altered merely to hide the action.
- Main and pause menus expose separate How to play and Controls actions. Controls has its own localized title and one-line binding instructions; it contains no nested game guide. Settings also links directly to Controls.
- How to play is a scrollable guide with goal, supplies, lab combat, timed energy, save rules and optional flirting headings. Text flows by preferred height and wraps with fixed readable body sizes, rather than squeezing one long paragraph into a fixed rectangle. Dynamic Interact/Fire/Use labels retain remapped bindings. A new run has one Let's go action; ordinary help has one top-left arrow. The first-run guide hides the back arrow so it does not duplicate the start action.
- Credits use the same central scroll/flow helper, separated section headings, one contributor name per row and secondary detail rows for tools/audio. Team names are no longer repeated beside a redundant Repository contributor label. Long names/details wrap; content height comes from VerticalLayoutGroup and ContentSizeFitter.
- Confirmation dialogs and results no longer pair a duplicate bottom navigation button with a top-left arrow. Confirmation initial focus is the back arrow; committing remains an explicit primary action.
- Hover/controller focus use a clearly different opaque button fill plus an independent three-pixel bright border. The old tint multiplied a pale highlight by a dark base image, making changes almost invisible. Border graphics have their own renderer so the button's dark tint does not dim them. They do not intercept pointer rays. Mouse hover updates selection; keyboard/controller selection uses the same feedback. Dropdown rows, sliders and scrollbar handles share the feedback path.
- UI audio cues were measured and regenerated at higher levels without clipped samples. Priority and missing-resource diagnostics were added. User Master/UI volume and global Editor mute remain respected. Cues are short synthesized blips/clicks, not spoken narration. See [audio/camera evidence](Menu_Repair_Audio.md).
- The obsolete native GUILayer (class 92, component816257230) caused both Main Camera warnings. Only its reference/record was removed from menu.unity; Camera, AudioListener and the still-supported FlareLayer remain.

## Pixel font and communication correction

Press Start 2P is now bundled unmodified with its complete OFL notice under Assets/Resources/Fonts. UI loads this resource rather than the temporary LegacyRuntime font; menu titles/body use pixel-friendly sizes and the dynamic atlas is kept at Point filtering. The actual cmap was checked against all 148 seven-language rows, contributor names, arrows and Ukrainian special letters. Only U+2212 minus needed a display mapping to ASCII hyphen-minus; GameLocalization.Get normalizes it centrally, including existing translator-edited tables. See [font provenance/coverage](Font_Audit.md). The font has its own credits section.

The user correctly pointed out that the earlier approval was not clearly asked. A tool request reporting aborted does not establish that the user saw or cancelled it; do not attribute that decision to them. Root explicitly asked for the exact official font/license download, and the subsequent command succeeded. The menu-only Play Mode question was then dispatched explicitly and is pending a response; an unanswered question does not authorize Play Mode.

## Validation policy

No Play Mode, gameplay or audio playback was performed while the prior user testing restriction remains unresolved. An earlier runtime-check question did not dispatch. The later explicit question was accepted by the question UI; absence of an answer is not approval. Source compilation, serialized references, Unicode font coverage, wave peaks and layout ownership are checkable without running the game. The user still needs a real menu pass to confirm appearance, audio and controller navigation; source checks alone do not prove those outcomes.

## Learnings

- A dark Image.color multiplied by Selectable tint is not an additive highlight. Keep the image white and set normal/hover colors on ColorBlock; render the border separately.
- A scalar Text rectangle with best-fit is unsuitable for long multilingual help/credits. Use one central flowing layout and readable fixed body fonts.
- Separate information architecture labels from reused legacy translation keys. A controls page needs a Controls key even if a previous menu.controls value said How to play.
- Source success does not establish UI quality. Present runtime evidence separately and never describe a compiler check as an in-game check.

Final source evidence: Localization, Editor and UI assemblies compile with the installed Unity Roslyn/reference inputs after the menu changes. All 148 translation rows have seven nonempty placeholder-compatible cells. Font cmap coverage passes after minus normalization; supported menu camera components and all new metadata/license files are present. No runtime/audio claim follows from those checks.

1.5.1 handoff update (2026-11-21): the user authorized a Unity MCP Play Mode check with user testing as fallback. No Unity MCP commands are exposed in this chat, so no Play Mode check was run; latest automatic Editor reload/setup success was observed separately. Public credits now use the supplied roster and one localized unresolved teammate row.
