# Pixel UI font audit — 2026-11-21

## Bundled font and provenance

The game bundles the unmodified **Press Start 2P** TrueType font from the official Google Fonts repository:

- [Official font directory](https://github.com/google/fonts/tree/main/ofl/pressstart2p)
- [Downloaded binary](https://raw.githubusercontent.com/google/fonts/main/ofl/pressstart2p/PressStart2P-Regular.ttf)
- [Downloaded license](https://raw.githubusercontent.com/google/fonts/main/ofl/pressstart2p/OFL.txt)
- [Publisher description](https://github.com/google/fonts/blob/main/ofl/pressstart2p/DESCRIPTION.en_us.html)
- [Publisher font history and acknowledgements](https://github.com/google/fonts/blob/main/ofl/pressstart2p/FONTLOG.txt)

The license copyright is **2012 The Press Start 2P Project Authors**, with Reserved Font Name **Press Start 2P**. The complete SIL Open Font License 1.1 accompanies the binary in `Assets/Resources/Fonts/PressStart2P-OFL.txt`. The publisher credits Cody Boisclair (CodeMan38) for TrueType conversion and non-ASCII glyphs, QTQ for original bitmap extraction, and Cyreal for Cyrillic Plus expansion. The exact bundled license remains the authoritative notice; no project-wide license is implied.

Binary path: `Assets/Resources/Fonts/PressStart2P-Regular.ttf`.

Resource path: `Resources.Load<Font>("Fonts/PressStart2P-Regular")`.

Binary size: **118,204 bytes**.

SHA-256: `034c77f1f05ec89421e4a63f0e3a4ca1ecf852cc6d2bf611f126f275728e017d`.

## Character coverage

A source-only audit decoded the binary's Unicode `cmap` format 4 subtables (Unicode platform and Windows Unicode BMP platform). It found **656 mapped Unicode code points**. This checks real nonzero glyph mappings rather than trusting the font's script labels.

The audit compared **177 unique non-whitespace characters** across all **146 translation seed entries in seven languages**, all credit display names in `Assets/Resources/Credits.json`, and the UI arrow characters `←`, `→`, and `↓`.

- Russian and Ukrainian text characters are covered, including the explicit Ukrainian check `ҐґЄєІіЇї`.
- Latin accents used by French, Italian, German, and Spanish are covered.
- Credit names, left/right/down arrows, multiplication sign, curly apostrophes, en/em dashes, and ellipsis used in those sources are covered.
- The only missing character in the audited seed was **U+2212 MINUS SIGN (`−`)**, used in the combat guide. The UI should normalize that display character to the supported ASCII hyphen-minus (`-`) or replace those seed occurrences. The font binary has not been modified to invent a glyph.

This character check does not certify translation quality and does not cover arbitrary future user-entered text or future localization additions.

## Import and display

The publisher describes the font as bitmap-derived and recommends **8px, 16px, and other multiples of 8**. The importer uses native size **16**, dynamic glyph generation, embedded font data, and raster rendering. Menu body size 16 and heading size 24 match the publisher's pixel grid. Keep the generated font texture's filtering at Point after texture rebuilds. Fractional Canvas scaling can still affect the final appearance; visual readability remains a user verification item.

No Unity Play Mode, player build, gameplay, or visual runtime tests were run for this audit. The binary, license, metadata, and character map were checked from source files only.

## Integration recheck — 2026-11-21

After adding font-credit headings/details, root repeated the cmap check for all 148 translation rows plus all credit names/arrows. All displayed characters are covered after GameLocalization.Get maps U+2212 to ASCII hyphen-minus. That central display normalization also handles already-existing String Table values without overwriting translator edits. The TTF binary remains unmodified.

Roster recheck (1.5.1,2026-11-21): all 149 rows plus the user-supplied Cyrillic team names and localized missing-person placeholder passed the same cmap/format checks.
