# Student distractions, caffeine and visual direction — 1.4.0

Date: 2026-11-21. Status: implemented and source-compiled; gameplay/controller/art approval awaits the user.

## User feedback and decisions

The user requested a funny attraction/flirting distraction that diverts lab reports from exam preparation; an energy drink rescue at zero energy with temporary overstimulation and a subsequent slow/sluggish crash; less rigid jumping; and a banner matching the project's sparse, mysterious old-fashioned pixel-art direction rather than the detailed cute/anime illustration from 1.3.0. Existing user-owned gameplay verification remains in force. Agents handled the friendly-student interaction, shared jump poses and persistence review; root integrated energy, UI, localization and banner choices.

## Mechanics

- Energy is the existing Sleep stat. At zero Sleep, an eight-second rescue timer replaces immediate loss. Food or a drink can recover it; Intelligence zero remains an immediate loss. Paid fresh labs are blocked at zero, while collected labs retain their free-throw effect. Inventory remains visible at zero so emergency supplies are reachable.
- An energy drink lends 3 Sleep and multiplies movement by 1.7 for 20 gameplay seconds. Paid throws consume the borrowed portion first. At expiry, unspent borrowed Sleep is removed; food gained in the meantime is retained. A player who drank from zero therefore returns to zero unless they recovered permanent Sleep. There is no permanent base-speed upgrade from these drinks.
- After stimulation, a ten-second crash applies a 0.55 movement multiplier and a 0.22-second horizontal response smoothing time (normal 0.035, stimulated 0.015). The slower response models sluggish controls without dropping button presses or delaying interactions/jumps. At zero Sleep after the boost, the rescue countdown resumes. No extra medical/real-world claim is intended by these fictional tuning values.
- Another drink during stimulation is refused without consuming it. A drink during a crash can rescue the player and starts a fresh boost, followed by another crash. These provisional values are Inspector-independent centralized constants in Character.Energy.cs, ready for user balance feedback.
- The friendly student is a non-solid, optional interaction near the starting area. Explicit Interact spends exactly one collected lab and shows a short heart/love-letter joke. Passing or looking costs nothing; no lab means no charge. This avoids an unavoidable silent inventory penalty. The interaction is available once per run and rolls back with the same checkpoint rules as other state. It does not apply to the final examiner or every woman sprite. Existing art is reused; its legacy attribution remains unresolved.
- CrouchVisual now shares its existing six artwork sections with knee-tucked ascent, extended bent-knee descent and a short landing recovery. It keeps head/body proportions, facing, damage tint and paused-time behavior. No second animation renderer implementation was added.

## Saves and localization

Character's existing participant captures boost/crash/rescue durations and borrowed Sleep. FlirtDistraction is participant order 40 and captures the one-time choice. Schema 1 gains optional fields: older positive-Sleep snapshots without them remain readable; zero-Sleep snapshots require an active boost or rescue timer. Bounds/finite checks validate all timers and borrowed energy. Pause/menu/save do not advance effects offline; Continue resumes the stored remaining durations. Checkpoint retry restores the effect state that existed at the checkpoint. Zero-Sleep door arrivals update the active save but retain the previous positive-Sleep checkpoint, avoiding a near-expired rescue deadline on every retry.

The seed has 131 entries across en/uk/ru/fr/it/de/es, with 15 added keys for these mechanics and artwork choices. Versioned effect/briefing/loss keys replace outdated runtime wording without overwriting a translator's existing table edits. Completeness/placeholder checks are source checks; native-language and actual font/layout approval remains pending.

## Banner option and generation record

Default asset: `Assets/Resources/Art/StudentTitleMinimal.png`. The previous `StudentTitle.png` is preserved. Settings > Title artwork switches between Minimal / mysterious and Detailed illustration, stored in PlayerPrefs. Both use aspect-preserving fill; the original campus image remains below as a fallback. The new image was generated with the built-in OpenAI image-generation tool using `Assets/buttons/NAU_shadow.png` as a style/setting reference, inspected inline, and copied into the project without altering the previous image or the generated original.

Final prompt:

> Use case: stylized-concept. Create a NEW 16:9 landscape game title background. Input image is STYLE/SETTING REFERENCE ONLY: preserve the modest old university campus, boxy concrete building, plane monument and limited gray-teal pixel palette. Requested style is much LESS detailed, mysterious minimalist old-fashioned pixel game art, NOT anime, NOT cute chibi, NOT a painted illustration. Render as if native resolution 320x180 enlarged using strictly nearest-neighbor chunky square pixels: large flat color clusters, about 12 muted colors, sparse hard shadows, no fine outlines, no smooth curves, no gradients, no elaborate textures. Dusk at a lonely university corridor/campus entrance; a tiny normal-proportioned exhausted student silhouette at one edge holds a few loose laboratory papers and a simple coffee cup. Very restrained deadpan student humor, quiet and slightly mysterious. Large calm dark teal negative space in the center for separately rendered localized title and menu buttons. Buildings and airplane only simple silhouettes in background, a couple of lit windows. No giant eyes, kawaii, anime faces, plush characters, fantasy architecture, flowers, lush plants, detailed clothing or ornate decoration. No readable text, letters, numbers, logos, watermark, border or UI. Keep the composition deliberately sparse and match the supplied game's amateur/minimal retro pixel art sensibility.

The result is lower detail and moodier than 1.3.0, but it is generated artwork, not a native 320×180 source file. Nearest filtering and uncompressed texture import preserve its pixel edges. Art style approval remains with the user.

## Validation and remaining feedback

All eleven project assemblies compiled against installed Unity references into ignored .utmp/source-compile outputs; no assemblies were executed and no player/Play Mode/test suite was started. Source review followed borrowed-energy depletion, expiry/grace ordering, item rejection, atomic saves, old-save defaults and one-time interaction restoration. Whitespace, resource metadata and localization key/placeholder checks are separate checks. The updated user checklist has exact steps for boost/crash/rescue, flirt cost, jump/landing, artwork switching and Continue during each effect.

User feedback is still needed on the speeds/durations, whether optional flirting is funny, jump silhouette/landing alignment and the minimal banner's fit with the game. Attribution/native review from 1.3.0 remains open.
